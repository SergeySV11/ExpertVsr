using OfficeOpenXml;
using System.Collections.Generic;


namespace Expert_VSR
{
    class ExpToExcel
    {
        public ExpToExcel(List<object> data, ExcelPackage pck)          //конструктор класса
        {
            int SelectNum = 0;
            foreach (List<Main> select in data)                         //Данные
            {
                SelectNum++;    
                ExcelRangeBase tag = null;
                ExcelWorksheet sheet = null;

                foreach (var _list in pck.Workbook.Worksheets)      //Листы шаблона
                {
                    var activeSheet = _list;        //Лист

                    foreach (var cell in activeSheet.Cells) //Сканируем по ячейкам
                    {
                        if (cell.Value != null && cell.Value.ToString() == @"<val" + SelectNum.ToString() + ">")    //Если нашли тег в Excel таблице <val1>
                        {
                            tag = cell;         //tag - Ячейка куда будет идти вставка
                            sheet = activeSheet;
                            break;
                        }
                    }
                    if (tag != null)
                    {
                        break;
                    }
                }
                if (tag != null)
                {
                    int adrD = tag.Start.Row;           //строка
                    
                    foreach (Main _value in select)
                    {
                        if (adrD != tag.Start.Row)
                        {
                            sheet.InsertRow(adrD, 1);    //добавляю пустую строку перед вставкой
                        }
                        int adrL = tag.Start.Column;     //столбец
                        foreach (var _subval in _value)
                        {
                            sheet.Cells[adrD, adrL].Value = _subval;    //Column
                            if (adrD > 1)
                                sheet.Cells[adrD, adrL].StyleID = sheet.Cells[adrD - 1, adrL].StyleID;  //Column Style (стиль по верхней ячейке)
                            adrL++;
                        }
                        adrD++;
                    }
                }
            }
            return;
        }
    }
}
