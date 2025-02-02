﻿using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OrganizationBankingSystem.Core.Helpers
{
    public static class ValidatorObject
    {
        public static bool AllNotNull(params object[] objects)
        {
            return objects.All(s => s != null);
        }
    }

    public static class ValidatorText
    {
        public static bool NotEmpty(string text)
        {
            text = Regex.Replace(text, @"\s+", "");
            return text.Length != 0;
        }
        public static bool AllNotEmpty(params string[] text_array)
        {
            return text_array.All(s => NotEmpty(s));
        }
    }

    public static class ValidatorNumber
    {
        private static readonly Regex _regexNumber = new(@"[^0-9]+");
        private static readonly Regex _regexDouble = new(@"[^0-9.,]+");

        public static int ValidateNumberTextInput(int defaultTextInputValue, int maxTextInputValue, string textFromInput)
        {
            try
            {
                int textFromInputValue = Convert.ToInt32(textFromInput);

                if (textFromInputValue < maxTextInputValue)
                {
                    return textFromInputValue;
                }
                else
                {
                    return defaultTextInputValue;
                }
            }
            catch (FormatException)
            {
                return defaultTextInputValue;
            }
            catch (OverflowException)
            {
                return defaultTextInputValue;
            }
        }

        public static bool IsNumberText(string text, int? max=null)
        {
            if (max == null)
            {
                return _regexNumber.IsMatch(text);
            }

            try
            {
                return Convert.ToInt32(text) <= max;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool IsDoubleNumberText(string text)
        {
            return _regexDouble.IsMatch(text);
        }
    }
}