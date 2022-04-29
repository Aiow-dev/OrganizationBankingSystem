using DocumentFormat.OpenXml.Spreadsheet;
using System;

namespace OrganizationBankingSystem.Core.Helpers
{
    public static class DocumentHelpers
    {
        public static void AppendCellToRow(Row row, string cellItem, CellValues dataType)
        {
            if (dataType == CellValues.Number)
            {
                row.Append(new Cell()
                {
                    CellValue = new CellValue(Convert.ToDouble(cellItem)),
                    DataType = dataType
                });
            }
            else
            {
                row.Append(new Cell()
                {
                    CellValue = new CellValue(cellItem),
                    DataType = dataType
                });
            }
        }

        public static void AppendCellsToRow(Row row, string[] cellItems, CellValues dataType)
        {
            foreach (string cellItem in cellItems)
            {
                AppendCellToRow(row, cellItem, dataType);
            }
        }
    }
}
