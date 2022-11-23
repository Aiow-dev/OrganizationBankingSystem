using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OrganizationBankingSystem.Data;
using System;
using System.Collections.Generic;
using System.IO;

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

        public static void Export(string fileName, string[] headers, List<List<DocumentItem>> values)
        {
            try
            {
                using SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook);

                WorkbookPart workBookPart = spreadSheetDocument.AddWorkbookPart();
                workBookPart.Workbook = new Workbook();

                WorksheetPart workSheetPart = workBookPart.AddNewPart<WorksheetPart>();
                workSheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = spreadSheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

                Sheet sheet = new()
                {
                    Id = spreadSheetDocument.WorkbookPart.GetIdOfPart(workSheetPart),
                    SheetId = 1,
                    Name = fileName
                };

                sheets.Append(sheet);

                Worksheet workSheet = workSheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();

                Row rowHeader = new();

                AppendCellsToRow(rowHeader, headers, CellValues.String);

                sheetData.AppendChild(rowHeader);

                foreach (List<DocumentItem> rowValues in values)
                {
                    Row row = new();

                    foreach (DocumentItem cell in rowValues)
                    {
                        AppendCellToRow(row, cell.Value, cell.CellType);
                    }

                    sheetData.AppendChild(row);
                }

                workSheetPart.Worksheet.Save();
                spreadSheetDocument.Save();

                spreadSheetDocument.Close();
            }
            catch (IOException)
            {
                throw new IOException();
            }
        }
    }
}