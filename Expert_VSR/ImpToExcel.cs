using System;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Windows;
using System.Data;

namespace Expert_VSR
{
    public class ImpToExcel: Spr
    {
        public DataTable Imp_Table = new DataTable("Excel");
        DataColumn T1 { get; } = new DataColumn("t1", typeof(string));
        DataColumn T2 { get; } = new DataColumn("t2", typeof(string));
        DataColumn T3 { get; } = new DataColumn("t3", typeof(string));
        DataColumn T4 { get; } = new DataColumn("t4", typeof(string));
        DataColumn T5 { get; } = new DataColumn("t5", typeof(string));
        DataColumn T6 { get; } = new DataColumn("t6", typeof(string));
        
        public ImpToExcel(string pck)
        {            
            try
            {               
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(pck, false))                              //Чтение файла
                {
                    SharedStringTable sharedStringTable = doc.WorkbookPart.SharedStringTablePart.SharedStringTable;
                    int sheetIndex = 0;
                    foreach (WorksheetPart worksheetPart in doc.WorkbookPart.WorksheetParts)
                    {
                        WorkSheetProcess(sharedStringTable, worksheetPart, doc, sheetIndex);
                        sheetIndex++;
                    }
                }
            }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Возможно необходимо закрыть файл Exsel");
            }
        }

        private void WorkSheetProcess(SharedStringTable sharedStringTable, WorksheetPart worksheetPart, SpreadsheetDocument doc, int sheetIndex)
        {
            
            string sheetName = doc.WorkbookPart.Workbook.Descendants<Sheet>().ElementAt(sheetIndex).Name.ToString();  //Возвращает имя листа
            foreach (SheetData sheetData in worksheetPart.Worksheet.Elements<SheetData>())                            //Данные листа
            {
                Imp_Table.Columns.AddRange(new DataColumn[] {T1, T2, T3, T4, T5, T6});
                foreach (var row in sheetData.Elements<Row>())                                                        
                {
                    int i = 0;
                    string[] arr = new string[6];
                    foreach (var cell in row.Elements<Cell>())                                                        
                    {
                        string cellValue;
                        if (cell.CellFormula != null)
                        {
                            cellValue = cell.CellValue.InnerText;
                            continue;
                        }
                        cellValue = cell.InnerText;

                        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                        {
                            arr[i] = sharedStringTable.ElementAt(Int32.Parse(cellValue)).InnerText;
                        }
                        else
                        {
                            arr[i] = cellValue;
                        }
                        i++;
                    }
                    Imp_Table.Rows.Add(arr);
                }
            }
        }
    }
}
