using System;
using System.Linq;

namespace OrganizationBankingSystem.Core.Helpers
{

    public static class ValidatorObject
    {
        public static bool AllNotNull(params object[] objects)
        {
            return objects.All(s => s != null);
        }
    }

    public static class ValidatorNumber
    {
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
    }
}
