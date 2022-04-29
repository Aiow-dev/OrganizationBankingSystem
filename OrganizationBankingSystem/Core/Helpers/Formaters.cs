using System;
using System.Text.RegularExpressions;

namespace OrganizationBankingSystem.Core.Helpers
{
    public static class Formaters
    {
        public static string ReplaceSpaces(string textSpaces)
        {
            return Regex.Replace(textSpaces, @"\s+", string.Empty);
        }

        public static string FormatNumbers(string textFormatNumbers)
        {
            return Regex.Replace(ReplaceSpaces(textFormatNumbers), @"[,.]+", string.Empty);
        }

        public static double FormatTextToDouble(string formatText)
        {
            try
            {
                string replaceFormatText = ReplaceSpaces(formatText.Replace(".", ","));

                if (replaceFormatText.Length > 0)
                {
                    int positionFirstComma = 1 + replaceFormatText.IndexOf(',');
                    string replaceSubsequentCommasText = string.Concat(replaceFormatText.AsSpan(0, positionFirstComma),
                        replaceFormatText[positionFirstComma..].Replace(",", string.Empty));

                    return Convert.ToDouble(replaceSubsequentCommasText);
                }
                else
                {
                    return 1.0;
                }
            }
            catch (FormatException)
            {
                return 1.0;
            }
        }
    }
}
