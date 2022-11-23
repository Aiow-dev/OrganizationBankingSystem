using DocumentFormat.OpenXml.Spreadsheet;

namespace OrganizationBankingSystem.Data
{
    public class DocumentItem
    {
        public string Value { get; set; }

        public CellValues CellType { get; set; }

        public DocumentItem(string value)
        {
            Value = value;
            CellType = CellValues.String;
        }

        public DocumentItem(string value, CellValues cellType)
        {
            Value = value;
            CellType = cellType;
        }
    }
}
