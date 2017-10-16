using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace sgq
{
    public class Excel
    {
        private Application app;
        private Workbook workbook;
        private Worksheet previousWorksheet;

        public Excel()
        {
            this.app = null;
            this.workbook = null;
            this.previousWorksheet = null;
            createDoc();
        }

        private void createDoc()
        {
            try
            {
                app = new Application();

                app.Visible = false;
                workbook = app.Workbooks.Add(1);
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            finally
            {
            }
        }

        public void shutDown()
        {
            try
            {
                workbook = null;

                app.Quit();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

            finally
            {
            }
        }

        public void Create_Worksheet(SqlDataReader oReader, string sheetName)
        {
            if (oReader != null && !oReader.IsClosed)
            {
                try
                {
                    Worksheet worksheet = (Worksheet)workbook.Sheets.Add(Missing.Value, Missing.Value, 1, XlSheetType.xlWorksheet);
                    worksheet.Name = sheetName;
                    previousWorksheet = worksheet;

                    int columnCount = oReader.FieldCount;

                    for (int n = 0; n < columnCount; n++)
                    {
                        createHeaders(worksheet, 1, n + 1, oReader.GetName(n));
                    }

                    int rowCounter = 2;
                    while (oReader.Read())
                    {
                        for (int n = 0; n < columnCount; n++)
                        {
                            addData(worksheet, rowCounter, n + 1, oReader[oReader.GetName(n)].ToString());
                        }
                        rowCounter++;
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                }
            }
        }

        public void createHeaders(Worksheet worksheet, int row, int col, string htext)
        {
            worksheet.Cells[row, col] = htext;
        }

        public void addData(Worksheet worksheet, int row, int col, string data)
        {
            worksheet.Cells[row, col] = data;
        }

        public void Save_Workbook(String File_Path)
        {
            try
            {
                if (File.Exists(File_Path))
                    File.Delete(File_Path);

                workbook.SaveAs(File_Path, XlFileFormat.xlWorkbookDefault, Missing.Value, Missing.Value, Missing.Value, Missing.Value, XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}