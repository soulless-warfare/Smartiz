using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Smartiz
{
    internal static class ValidText
    {
        private static readonly Regex TextPattern = new Regex("^[a-zA-Z0-9@._\\-+]*$", RegexOptions.Compiled);

        /// <summary>Validates if input was only all of them was correct</summary>
        public static bool AllCorrect(string[] texts, string[] patterns, string[] numbers)
        {
            foreach (var text in texts) if(IsText(text) == false) return false;
            foreach (var pattern in patterns) if (IsPatterned(pattern) == false) return false;
            foreach (var number in numbers) if (IsNumber(number) == false) return false;

            return true;
        }

        /// <summary>Validates if input was only text</summary>
        public static bool IsText(string input)
        {
            return !input.Any(char.IsDigit);
        }

        /// <summary>Validates if input was only text</summary>
        public static bool IsText(string[] inputs)
        {
            foreach (var text in inputs) if (IsText(text) == false) return false;

            return true;
        }

        /// <summary>Validates if input matches allowed pattern</summary>
        public static bool IsPatterned(string input)
        {
            return TextPattern.IsMatch(input);
        }

        /// <summary>Validates if input matches allowed pattern</summary>
        public static bool IsPatterned(string[] inputs)
        {
            foreach (var text in inputs) if (IsPatterned(text) == false) return false;

            return true;
        }

        /// <summary>Validates if input was only number</summary>
        public static bool IsNumber(string input)
        {
            return input.All(char.IsDigit);
        }

        /// <summary>Validates if input was only number</summary>
        public static bool IsNumber(string[] inputs)
        {
            foreach (var text in inputs) if (IsNumber(text) == false) return false;

            return true;
        }
    }
}
