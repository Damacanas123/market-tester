using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using ExcelN = Microsoft.Office.Interop.Excel;
using System.Linq;
using MarketTester.Model.OrderHistoryFix;
using System.IO;
using System.Windows;
using System.Runtime.InteropServices;

namespace MarketTester.Worksheet
{
    
    public class Excel
    {
        private static List<Color> outboundColors = new List<Color>()
        {
            Color.FromArgb(249, 233, 203),
            Color.FromArgb(249, 244, 235)
        };
        private static List<Color> inboundColors = new List<Color>()
        {
            Color.FromArgb(247, 248, 254),
            Color.FromArgb(234, 242, 255)
        };

        private static Color headerColor = Color.FromArgb(92, 115, 171);



        //creates a new file and saves the content as first sheet
        //returns success status and error message if any
        public static (bool,string) SaveXLSXFromDataTable(List<HistoryItem> historyItems, string filePath, string fixHistory, string sheetName)
        {
            ExcelN.Application excelApp = new ExcelN.Application();
            try
            {
                excelApp.SheetsInNewWorkbook = 1;
                ExcelN.Workbook excelBook = excelApp.Workbooks.Add();
                ExcelN.Worksheet excelSheet = excelBook.Worksheets[1];
                excelSheet.Name = sheetName;
                string[] arrayColumns = {"Message Received","Message Sent","OrderId","Exec Type","ExecId","Ord Status","OrderQty",
            "Price","CumQty","AvgPx","LeavesQty","Last Shares","LastPx","Fix Raw" };
                for (int i = 0; i < arrayColumns.Length; i++)
                {
                    excelSheet.Cells[1, i + 1] = arrayColumns[i].ToString();
                    excelSheet.Cells[1, i + 1].Interior.Color = headerColor;
                    excelSheet.Cells[1, i + 1].Borders.Color = Color.White;
                }

                for (int i = 0; i < historyItems.Count; i++)
                {
                    string origclOrdId = historyItems[i].OrigClOrdID.ToString();
                    if (historyItems[i].Origin.ToString() == ExcelConstants.MESSAGE_ORIGIN_INBOUND)
                    {
                        excelSheet.Cells[i + 2, 2] = historyItems[i].MessageType + "(" +
                            historyItems[i].ClOrdID + (!String.IsNullOrEmpty(origclOrdId) ? "," + historyItems[i].OrigClOrdID + ")" : ")");
                        excelSheet.Cells[i + 2, 1].Interior.Color = inboundColors[i % 2];
                        excelSheet.Cells[i + 2, 2].Interior.Color = inboundColors[i % 2];
                    }
                    else
                    {
                        excelSheet.Cells[i + 2, 1] = historyItems[i].MessageType + "(" +
                            historyItems[i].ClOrdID + (!String.IsNullOrEmpty(origclOrdId) ? "," + historyItems[i].OrigClOrdID + ")" : ")");
                        excelSheet.Cells[i + 2, 1].Interior.Color = outboundColors[i % 2];
                        excelSheet.Cells[i + 2, 2].Interior.Color = outboundColors[i % 2];
                    }

                    List<string> columnValues = historyItems[i].Columns;
                    for (int j = 3; j < HistoryItem.ColumnNames.Count; j++)
                    {
                        if (historyItems[i].Origin.Equals(ExcelConstants.MESSAGE_ORIGIN_INBOUND))
                        {
                            excelSheet.Cells[i + 2, j].Interior.Color = inboundColors[i % 2];
                        }
                        else
                        {
                            excelSheet.Cells[i + 2, j].Interior.Color = outboundColors[i % 2];
                        }
                        excelSheet.Cells[i + 2, j] = columnValues[j];
                        excelSheet.Cells[i + 2, j].Borders.LineStyle = ExcelN.XlLineStyle.xlContinuous;
                        excelSheet.Cells[i + 2, j].Borders.Color = Color.White;
                    }
                }

                int fixHistoryLineCount = fixHistory.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length;
                excelSheet.Cells[historyItems.Count + 2, 1] = "FIX Logs";
                excelSheet.Cells[historyItems.Count + 2, 1].Interior.Color = headerColor;
                excelSheet.Range[excelSheet.Cells[historyItems.Count + 2, 1], excelSheet.Cells[historyItems.Count + 2, HistoryItem.ColumnNames.Count]].Merge();
                ExcelN.Range fixRange = excelSheet.Range[excelSheet.Cells[historyItems.Count + 3, 1], excelSheet.Cells[historyItems.Count + 3, HistoryItem.ColumnNames.Count]];
                fixRange.Merge();
                excelSheet.Cells[historyItems.Count + 3, 1] = fixHistory;
                excelSheet.Columns.VerticalAlignment = ExcelN.XlVAlign.xlVAlignCenter;


                //excelSheet.Cells[dt.Rows.Count + 3, 1].WrapText = false;
                excelSheet.Rows.RowHeight = 50;
                double fixHistoryRowHeight = fixHistoryLineCount * 20;
                excelSheet.Cells[historyItems.Count + 3, 1].RowHeight = fixHistoryRowHeight < ExcelConstants.EXCEL_MAXROWHEIGHT ? fixHistoryRowHeight : ExcelConstants.EXCEL_MAXROWHEIGHT;
                fixRange.VerticalAlignment = ExcelN.XlVAlign.xlVAlignTop;
                excelSheet.Columns.AutoFit();
                excelSheet.Columns[1].ColumnWidth = 20;

                excelBook.SaveAs(filePath);
                excelBook.Close();
                excelApp.Quit();
                return (true, ResourceKeys.StringFinishedExporting);
            }
            catch(COMException ex)
            {
                excelApp.Quit();
                return (false, ResourceKeys.StringCOMExplanation);
            }
            catch(Exception ex)
            {
                ex.GetType();
                excelApp.Quit();
                return (false, ResourceKeys.StringCouldntExportXlsx);
            }
            
        }

        public static (bool,string) AppendSheetSaveXLSXFromDataTable(List<HistoryItem> historyItems, string filePath, string fixHistory, string sheetName)
        {
            ExcelN.Application excelApp = new ExcelN.Application();
            try
            {
                excelApp.DisplayAlerts = false;
                ExcelN.Workbook excelBook = excelApp.Workbooks.Open(filePath, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                ExcelN.Sheets worksheets = excelBook.Worksheets;
                var excelSheet = (ExcelN.Worksheet)worksheets.Add(Type.Missing, worksheets[excelBook.Sheets.Count], Type.Missing, Type.Missing);
                excelSheet.Name = sheetName;


                string[] arrayColumns = {"Message Received","Message Sent","OrderId","Exec Type","ExecId","Ord Status","OrderQty",
            "Price","CumQty","AvgPx","LeavesQty","Last Shares","LastPx","Fix Raw" };
                for (int i = 0; i < arrayColumns.Length; i++)
                {
                    excelSheet.Cells[1, i + 1] = arrayColumns[i].ToString();
                    excelSheet.Cells[1, i + 1].Interior.Color = headerColor;
                    excelSheet.Cells[1, i + 1].Borders.Color = Color.White;
                }

                for (int i = 0; i < historyItems.Count; i++)
                {
                    string origclOrdId = historyItems[i].OrigClOrdID.ToString();
                    if (historyItems[i].Origin.ToString() == ExcelConstants.MESSAGE_ORIGIN_INBOUND)
                    {
                        excelSheet.Cells[i + 2, 2] = historyItems[i].MessageType + "(" +
                            historyItems[i].ClOrdID + (!String.IsNullOrEmpty(origclOrdId) ? "," + historyItems[i].OrigClOrdID + ")" : ")");
                        excelSheet.Cells[i + 2, 1].Interior.Color = inboundColors[i % 2];
                        excelSheet.Cells[i + 2, 2].Interior.Color = inboundColors[i % 2];
                    }
                    else
                    {
                        excelSheet.Cells[i + 2, 1] = historyItems[i].MessageType + "(" +
                            historyItems[i].ClOrdID + (!String.IsNullOrEmpty(origclOrdId) ? "," + historyItems[i].OrigClOrdID + ")" : ")");
                        excelSheet.Cells[i + 2, 1].Interior.Color = outboundColors[i % 2];
                        excelSheet.Cells[i + 2, 2].Interior.Color = outboundColors[i % 2];
                    }

                    for (int j = 3; j < HistoryItem.ColumnNames.Count; j++)
                    {
                        if (historyItems[i].Origin.Equals(ExcelConstants.MESSAGE_ORIGIN_INBOUND))
                        {
                            excelSheet.Cells[i + 2, j].Interior.Color = inboundColors[i % 2];
                        }
                        else
                        {
                            excelSheet.Cells[i + 2, j].Interior.Color = outboundColors[i % 2];
                        }
                        excelSheet.Cells[i + 2, j] = historyItems[i].Columns[j];
                        excelSheet.Cells[i + 2, j].Borders.LineStyle = ExcelN.XlLineStyle.xlContinuous;
                        excelSheet.Cells[i + 2, j].Borders.Color = Color.White;
                    }
                }
                int fixHistoryLineCount = fixHistory.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length;
                excelSheet.Cells[historyItems.Count + 2, 1] = "FIX Logs";
                excelSheet.Cells[historyItems.Count + 2, 1].Interior.Color = headerColor;
                excelSheet.Range[excelSheet.Cells[historyItems.Count + 2, 1], excelSheet.Cells[historyItems.Count + 2, HistoryItem.ColumnNames.Count]].Merge();
                ExcelN.Range fixRange = excelSheet.Range[excelSheet.Cells[historyItems.Count + 3, 1], excelSheet.Cells[historyItems.Count + 3, HistoryItem.ColumnNames.Count]];
                fixRange.Merge();
                excelSheet.Cells[historyItems.Count + 3, 1] = fixHistory;
                excelSheet.Columns.VerticalAlignment = ExcelN.XlVAlign.xlVAlignCenter;


                //excelSheet.Cells[dt.Rows.Count + 3, 1].WrapText = false;
                excelSheet.Rows.RowHeight = 50;
                double fixHistoryRowHeight = fixHistoryLineCount * 20;
                excelSheet.Cells[historyItems.Count + 3, 1].RowHeight = fixHistoryRowHeight < ExcelConstants.EXCEL_MAXROWHEIGHT ? fixHistoryRowHeight : ExcelConstants.EXCEL_MAXROWHEIGHT;
                fixRange.VerticalAlignment = ExcelN.XlVAlign.xlVAlignTop;
                excelSheet.Columns.AutoFit();
                excelSheet.Columns[1].ColumnWidth = 20;

                excelBook.SaveAs(filePath);
                excelBook.Close();
                excelApp.Quit();
                return (true, ResourceKeys.StringFinishedExporting);
            }
            catch (COMException ex)
            {
                excelApp.Quit();
                return (false, ResourceKeys.StringCOMExplanation);
            }
            catch (Exception ex)
            {
                excelApp.Quit();
                return (false, ResourceKeys.StringCouldntExportXlsx);
            }
        }

        
    }
}
