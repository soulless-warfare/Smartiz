using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Smartiz
{
    public class FaceRecognition
    {
        public enum SimilarityMode
        {
            EuclideanWeighted,
            CosineWeighted
        }

        private readonly double _tolerance;
        private readonly SimilarityMode _mode;
        private readonly Guna2CircleProgressBar _progressBar;

        public FaceRecognition(
            SimilarityMode mode = SimilarityMode.CosineWeighted,
            double tolerance = 0.6,
            Guna2CircleProgressBar progressBar = null)
        {
            _mode = mode;
            _tolerance = tolerance;
            _progressBar = progressBar;
        }

        /// <summary>Identify the most similar user using Specifics and Embeddeds tables.</summary>
        public async Task<(string UserID, float SimilarityPercent)> IdentifyUserAsync(
            DataTable specificsTable,
            DataTable embeddedsTable,
            float[] queryEmbedding)
        {
            if (specificsTable == null || embeddedsTable == null || queryEmbedding == null)
                throw new ArgumentNullException("Tables or query embedding cannot be null.");

            if (!specificsTable.Columns.Contains("UserID") ||
                !specificsTable.Columns.Contains("EmbeddedID") ||
                !embeddedsTable.Columns.Contains("EmbeddedID") ||
                !embeddedsTable.Columns.Contains("Embedded"))
                throw new ArgumentException("Invalid table schema for Specifics or Embeddeds.");

            var embeddedVectors = new Dictionary<string, float[]>();

            bool hasIndexColumn = embeddedsTable.Columns.Contains("EmbeddedIndex");

            var groupedByEmbeddedId = embeddedsTable.AsEnumerable()
                .GroupBy(r => r.Field<string>("EmbeddedID"));

            foreach (var group in groupedByEmbeddedId)
            {
                string embeddedId = group.Key;

                var singleRow = group.FirstOrDefault(r => r["Embedded"] != null && r["Embedded"] is string s && s.Contains(","));
                if (singleRow != null)
                {
                    var text = singleRow["Embedded"].ToString();
                    var parts = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var vec = new List<float>(parts.Length);
                    foreach (var p in parts)
                    {
                        if (float.TryParse(p.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var ff))
                            vec.Add(ff);
                        else if (float.TryParse(p.Trim(), out ff))
                            vec.Add(ff);
                        else
                            vec.Add(0f);
                    }
                    embeddedVectors[embeddedId] = vec.ToArray();
                    continue;
                }

                if (hasIndexColumn)
                {
                    var ordered = group
                        .Where(r => r["Embedded"] != null)
                        .OrderBy(r => Convert.ToInt32(r["EmbeddedIndex"]))
                        .ToArray();

                    var vecList = new List<float>(ordered.Length);
                    foreach (var r in ordered)
                    {
                        var val = r["Embedded"];
                        if (val is float f) vecList.Add(f);
                        else if (val is double d) vecList.Add((float)d);
                        else if (val is decimal dec) vecList.Add((float)dec);
                        else
                        {
                            var s = val.ToString();
                            if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var ff))
                                vecList.Add(ff);
                            else if (float.TryParse(s, out ff))
                                vecList.Add(ff);
                            else
                                vecList.Add(0f);
                        }
                    }

                    if (vecList.Count > 0)
                    {
                        embeddedVectors[embeddedId] = vecList.ToArray();
                        continue;
                    }
                }

                var fallbackList = new List<float>();
                foreach (var r in group)
                {
                    var val = r["Embedded"];
                    if (val is float f) fallbackList.Add(f);
                    else if (val is double d) fallbackList.Add((float)d);
                    else if (val is decimal dec) fallbackList.Add((float)dec);
                    else
                    {
                        var s = val?.ToString() ?? string.Empty;
                        if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var ff))
                            fallbackList.Add(ff);
                        else if (float.TryParse(s, out ff))
                            fallbackList.Add(ff);
                        else
                            fallbackList.Add(0f);
                    }
                }
                if (fallbackList.Count > 0)
                    embeddedVectors[embeddedId] = fallbackList.ToArray();
            }

            var userVectors = new Dictionary<string, List<float[]>>();
            foreach (DataRow spec in specificsTable.Rows)
            {
                var userId = spec["UserID"]?.ToString();
                var embeddedId = spec["EmbeddedID"]?.ToString();
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(embeddedId)) continue;
                if (!embeddedVectors.ContainsKey(embeddedId)) continue;

                if (!userVectors.TryGetValue(userId, out var list))
                {
                    list = new List<float[]>();
                    userVectors[userId] = list;
                }
                list.Add(embeddedVectors[embeddedId]);
            }

            if (userVectors.Count == 0)
                return (null, 0f);

            if (_progressBar != null)
            {
                _progressBar.Minimum = 0;
                _progressBar.Maximum = userVectors.Count;
                _progressBar.Value = 0;
            }

            string bestUser = null;
            double bestScore = double.MinValue;

            await Task.Run(() =>
            {
                foreach (var kv in userVectors)
                {
                    var userId = kv.Key;
                    var vectors = kv.Value;

                    var valid = vectors.Where(v => v != null && v.Length == queryEmbedding.Length).ToList();
                    if (valid.Count == 0)
                    {
                        if (_progressBar != null) _progressBar.Invoke((Action)(() => _progressBar.Value++));
                        continue;
                    }

                    double score = _mode switch
                    {
                        SimilarityMode.EuclideanWeighted => WeightedEuclideanSimilarity(valid, queryEmbedding),
                        SimilarityMode.CosineWeighted => WeightedCosineSimilarity(valid, queryEmbedding),
                        _ => 0.0
                    };

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestUser = userId;
                    }

                    if (_progressBar != null) _progressBar.Invoke((Action)(() => _progressBar.Value++));
                }
            });

            if (bestUser == null)
                return (null, 0f);

            var percent = (float)(Math.Clamp(bestScore, 0.0, 1.0) * 100.0);
            return (bestUser, percent);
        }

        /// <summary>Weighted similarity using inverse-distance weighting (Euclidean).</summary>
        private double WeightedEuclideanSimilarity(List<float[]> vectors, float[] query)
        {
            var distances = vectors.Select(v => EuclideanDistance(v, query)).ToList();
            var sims = distances.Select(d => 1.0 / (1.0 + d)).ToList();
            double weightSum = sims.Sum() + 1e-12;
            double weighted = sims.Zip(sims, (s, w) => s * (w / weightSum)).Sum();
            return weighted;
        }

        /// <summary>Weighted similarity using cosine similarity and self-weights.</summary>
        private double WeightedCosineSimilarity(List<float[]> vectors, float[] query)
        {
            var sims = vectors.Select(v => CosineSimilarityNormalized(v, query)).ToList();
            double weightSum = sims.Sum() + 1e-12;
            double weighted = sims.Zip(sims, (s, w) => s * (w / weightSum)).Sum();
            return weighted;
        }

        /// <summary>Euclidean distance between two vectors.</summary>
        private double EuclideanDistance(float[] a, float[] b)
        {
            double sum = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                var diff = a[i] - b[i];
                sum += diff * diff;
            }
            return Math.Sqrt(sum);
        }

        /// <summary>Cosine similarity normalized to [0,1].</summary>
        private double CosineSimilarityNormalized(float[] a, float[] b)
        {
            double dot = 0.0, na = 0.0, nb = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                na += a[i] * a[i];
                nb += b[i] * b[i];
            }
            if (na == 0 || nb == 0) return 0.0;
            double cos = dot / (Math.Sqrt(na) * Math.Sqrt(nb));
            return (cos + 1.0) / 2.0;
        }
    }
}
