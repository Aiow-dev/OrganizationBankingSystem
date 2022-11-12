using OrganizationBankingSystem.Core.Notifications;
using System.Windows.Controls;

namespace OrganizationBankingSystem.Core.Helpers
{
    public static class ComboBoxHelper
    {
        public static void SwapValuesComboBox(ComboBox comboBox1, ComboBox comboBox2, string errorMessage)
        {
            object comboBoxValue1 = comboBox1.SelectedItem;
            object comboBoxValue2 = comboBox2.SelectedItem;

            if (ValidatorObject.AllNotNull(comboBoxValue1, comboBoxValue2))
            {
                comboBox1.SelectedItem = comboBoxValue2;
                comboBox2.SelectedItem = comboBoxValue1;
            }
            else
            {
                NotificationManager.notifier.ShowErrorPropertyMessage(errorMessage);
            }
        }

        public static void SortValuesComboBox(ComboBox comboBox, string sortDescription)
        {
            comboBox.Items.SortDescriptions.Clear();

            comboBox.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription(sortDescription, System.ComponentModel.ListSortDirection.Ascending));
        }
    }
}