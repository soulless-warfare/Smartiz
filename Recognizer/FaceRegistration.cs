using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FaceRecognitionDotNet;
using Guna.UI2.WinForms;
using System.Drawing.Imaging;
using System.Collections.Concurrent;
using Smartiz;


namespace Samrtiz
{
    /// <summary>Register images and produce DataTables for Specifics and Embeddeds</summary>
    public class FaceRegistration : IDisposable
    {
        /// <summary>Accuracy levels for face registration augmentation</summary>
        public enum Accuracy
        {
            /// <summary>Low augmentation and processing</summary>
            Low,
            /// <summary>Moderate augmentation for better accuracy</summary>
            Intermediate,
            /// <summary>High augmentation with multiple angles and brightness</summary>
            High,
            /// <summary>Extensive augmentation including flips and many variations</summary>
            Intricate
        }

        private readonly FaceRecognitionDotNet.FaceRecognition _faceRecognition;
        private Guna2CircleProgressBar ProgressBar { get; set; }

        /// <summary>Initialize FaceRegistration with optional progress bar</summary>
        public FaceRegistration(Guna2CircleProgressBar progressBar = null)
        {
            ProgressBar = progressBar;
            string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models");

            if (!File.Exists(Path.Combine(modelPath, "shape_predictor_68_face_landmarks.dat")) ||
                !File.Exists(Path.Combine(modelPath, "dlib_face_recognition_resnet_model_v1.dat")))
                throw new FileNotFoundException("Required model files not found in the Models folder.");

            _faceRecognition = FaceRecognitionDotNet.FaceRecognition.Create(modelPath);
        }

        /// <summary>Register one image with augmentations and return averaged embedding</summary>
        public async Task<float[]> RegistrationAsync(byte[] imagesbyte, Accuracy accuracy)
        {
            try
            {
                if (imagesbyte == null)
                    throw new ArgumentNullException(nameof(imagesbyte));

                // choose augmentation parameters based on accuracy
                var (angles, brightnessFactors, doFlip) = GetAugmentationParameters(accuracy);

                int totalVariants = angles.Length * brightnessFactors.Length * (doFlip ? 3 : 1);

                if (ProgressBar != null)
                {
                    ProgressBar.Minimum = 0;
                    ProgressBar.Maximum = totalVariants;
                    ProgressBar.Value = 0;
                }

                System.Drawing.Image original = ByteToImage(imagesbyte);

                using (var originalClone = (System.Drawing.Image)original.Clone())
                {
                    var tempEmbeddings = new List<float[]>();

                    foreach (var angle in angles)
                    {
                        using (var rotated = RotateImage(originalClone, angle))
                        {
                            foreach (var bright in brightnessFactors)
                            {
                                using (var adjusted = AdjustBrightness(rotated, bright))
                                {
                                    var variants = new List<System.Drawing.Image> { (System.Drawing.Image)adjusted.Clone() };
                                    if (doFlip)
                                    {
                                        variants.Add(FlipImage((System.Drawing.Image)adjusted.Clone(), true));  // horizontal flip
                                        variants.Add(FlipImage((System.Drawing.Image)adjusted.Clone(), false)); // vertical flip
                                    }

                                    foreach (System.Drawing.Image variant in variants)
                                    {
                                        var emb = await Task.Run(() => ExtractEmbeddingFromImage(variant));
                                        variant.Dispose();

                                        if (emb != null)
                                            tempEmbeddings.Add(emb);

                                        if (ProgressBar != null)
                                        {
                                            ProgressBar.Value = Math.Min(ProgressBar.Value + 1, ProgressBar.Maximum);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (tempEmbeddings.Count == 0)
                        return null;

                    return AverageEmbeddings(tempEmbeddings);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Register user with image and set in database</summary>
        public async Task<bool> RegistrationAsync(List<byte[]> imagesbyte, string userID,Accuracy accuracy)
        {
            try
            {
                if (imagesbyte == null) throw new ArgumentNullException(nameof(imagesbyte));
                if (Program.Specifics == null) throw new ArgumentNullException(nameof(Program.Specifics));
                if (Program.Embeddeds == null) throw new ArgumentNullException(nameof(Program.Embeddeds));

                // choose augmentation parameters based on accuracy
                var (angles, brightnessFactors, doFlip) = GetAugmentationParameters(accuracy);

                int totalVectors = imagesbyte.Count * angles.Length * brightnessFactors.Length * (doFlip ? 3 : 1);

                if (ProgressBar != null)
                {
                    ProgressBar.Minimum = 0;
                    ProgressBar.Maximum = totalVectors;
                    ProgressBar.Value = 0;
                }

                for (int i = 0; i < imagesbyte.Count; i++)
                {
                    System.Drawing.Image original = ByteToImage(imagesbyte[i]);
                    string currentSpecificID = Program.Specifics.CreateID("SpecificID", 10);
                    string currentEmbeddedID = Program.Embeddeds.CreateID("EmbeddedID", 10);

                    float[] finalEmbedding = null;

                    // clone original to avoid modifying caller object
                    using (var originalClone = (System.Drawing.Image)original.Clone())
                    {
                        var tempEmbeddings = new List<float[]>();

                        // for each augmentation: rotate, adjust brightness, optional flip
                        foreach (var angle in angles)
                        {
                            using (var rotated = RotateImage(originalClone, angle))
                            {
                                foreach (var bright in brightnessFactors)
                                {
                                    using (var adjusted = AdjustBrightness(rotated, bright))
                                    {
                                        // optional flipped variants
                                        var variants = new List<System.Drawing.Image> { (System.Drawing.Image)adjusted.Clone() };
                                        if (doFlip)
                                        {
                                            variants.Add(FlipImage((System.Drawing.Image)adjusted.Clone(), true));  // horizontal flip
                                            variants.Add(FlipImage((System.Drawing.Image)adjusted.Clone(), false)); // vertical flip
                                        }

                                        foreach (System.Drawing.Image variant in variants)
                                        {
                                            // extract embedding from variant (save to temp file for FaceRecognition)
                                            var emb = await Task.Run(() => ExtractEmbeddingFromImage(variant));
                                            variant.Dispose();

                                            if (emb != null)
                                                tempEmbeddings.Add(emb);

                                            if (ProgressBar != null)ProgressBar.Value++;
                                        }
                                    }
                                }
                            }
                        }

                        // if no embeddings found for this image, skip it
                        if (tempEmbeddings.Count == 0)
                        {
                            if (ProgressBar != null) ProgressBar.Value++;
                            continue;
                        }

                        // average embeddings
                        finalEmbedding = AverageEmbeddings(tempEmbeddings);
                    }

                    // convert image to bytes for Specifics table (use original image)
                    byte[] imageBytes;
                    using (var originalimg = ByteToImage(imagesbyte[i]))
                    using (var bmp = new Bitmap(originalimg.Width, originalimg.Height))
                    {
                        using (var g = Graphics.FromImage(bmp))
                            g.DrawImage(originalimg, 0, 0, originalimg.Width, originalimg.Height);

                        using (var ms = new MemoryStream())
                        {
                            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            imageBytes = ms.ToArray();
                        }
                    }

                    // add Specifics row
                    var specRow = Program.Specifics.DataTable.NewRow();
                    specRow["SpecificID"] = currentSpecificID;
                    specRow["UserID"] = userID;
                    specRow["EmbeddedID"] = currentEmbeddedID;
                    specRow["Image"] = imageBytes;
                    Program.Specifics.Add(specRow);

                    // add Embeddeds rows (include EmbeddedIndex column)
                    for (int idx = 0; idx < finalEmbedding.Length; idx++)
                    {
                        var embRow = Program.Embeddeds.DataTable.NewRow();
                        embRow["EMID"] = Program.Embeddeds.CreateID("EMID", 10);
                        embRow["EmbeddedID"] = currentEmbeddedID;
                        embRow["EmbeddedIndex"] = idx;
                        embRow["Embedded"] = finalEmbedding[idx];
                        Program.Embeddeds.Add(embRow);
                    }

                    if (ProgressBar != null) ProgressBar.Value++;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Get augmentation parameters for a given accuracy</summary>
        private static (float[] angles, float[] brightnessFactors, bool doFlip) GetAugmentationParameters(Accuracy accuracy)
        {
            return accuracy switch
            {
                Accuracy.Low => (new float[] { 0f }, new float[] { 1.0f }, false),
                Accuracy.Intermediate => (new float[] { -10f, 0f, 10f }, new float[] { 1.0f, 1.1f }, false),
                Accuracy.High => (new float[] { -15f, -5f, 0f, 5f, 15f }, new float[] { 0.95f, 1.0f, 1.05f }, false),
                Accuracy.Intricate => (new float[] { -20f, -15f, -10f, -5f, 0f, 5f, 10f, 15f, 20f }, new float[] { 0.9f, 0.97f, 1.0f, 1.05f, 1.12f }, true),
                _ => (new float[] { 0f }, new float[] { 1.0f }, false),
            };
        }

        /// <summary>Rotate image by angle degrees</summary>
        private static Bitmap RotateImage(System.Drawing.Image img, float angle)
        {
            var bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.TranslateTransform(img.Width / 2f, img.Height / 2f);
                g.RotateTransform(angle);
                g.TranslateTransform(-img.Width / 2f, -img.Height / 2f);
                g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            }
            return bmp;
        }

        /// <summary>Adjust brightness of an image</summary>
        private static Bitmap AdjustBrightness(System.Drawing.Image image, float factor)
        {
            var bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bmp))
            {
                var cm = new System.Drawing.Imaging.ColorMatrix(new float[][]
                {
                    new float[] { factor, 0, 0, 0, 0 },
                    new float[] { 0, factor, 0, 0, 0 },
                    new float[] { 0, 0, factor, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });
                var ia = new System.Drawing.Imaging.ImageAttributes();
                ia.SetColorMatrix(cm);
                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
            }
            return bmp;
        }

        /// <summary>Flip image horizontally or vertically</summary>
        private static Bitmap FlipImage(System.Drawing.Image img, bool horizontal)
        {
            var bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bmp))
            {
                if (horizontal)
                    g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), img.Width, 0, -img.Width, img.Height, GraphicsUnit.Pixel);
                else
                    g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, img.Height, img.Width, -img.Height, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        /// <summary>Extract embedding from an Image by saving to temp file and using FaceRecognition</summary>
        private float[] ExtractEmbeddingFromImage(System.Drawing.Image image)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".bmp");
            try
            {
                image.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

                using (var frImg = FaceRecognitionDotNet.FaceRecognition.LoadImageFile(tempPath))
                {
                    var faceLocations = _faceRecognition.FaceLocations(frImg).ToArray();
                    if (faceLocations.Length == 0) return null;

                    var encodings = _faceRecognition.FaceEncodings(frImg, faceLocations).ToArray();
                    if (encodings.Length == 0) return null;

                    var raw = encodings.First().GetRawEncoding();
                    return Array.ConvertAll(raw, x => (float)x);
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { /* ignore cleanup errors */
            }
        }
        }

        /// <summary>Average a list of embeddings element-wise</summary>
        private static float[] AverageEmbeddings(List<float[]> embeddings)
        {
            if (embeddings == null || embeddings.Count == 0) return null;
            int len = embeddings[0].Length;
            var avg = new float[len];
            foreach (var vec in embeddings)
            {
                for (int i = 0; i < len; i++)
                    avg[i] += vec[i];
            }
            for (int i = 0; i < len; i++)
                avg[i] /= embeddings.Count;
            return avg;
        }

        /// <summary>Convert byte to image</summary>
        private System.Drawing.Image ByteToImage(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException("Error: ", nameof(data));

            using (var ms = new MemoryStream(data))
            {
                return System.Drawing.Image.FromStream(ms);
            }
        }

        /// <summary>Dispose FaceRecognition resources</summary>
        public void Dispose()
        {
            _faceRecognition?.Dispose();
        }
    }
}
