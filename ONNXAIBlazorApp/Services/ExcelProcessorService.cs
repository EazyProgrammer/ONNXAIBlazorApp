using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using FaissSharp;
using LangChain.DocumentLoaders;
using ClosedXML.Excel;
using ONNXAIBlazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ONNXAIBlazorApp.Data
{
    public class ExcelProcessorService : IExcelProcessorService
    {
        // Method to load data from an Excel file
        public Dictionary<string, List<List<string>>> LoadExcelData(string filePath)
        {
            var excelData = new Dictionary<string, List<List<string>>>();

            // Load the Excel file
            using (var workbook = new XLWorkbook(filePath))
            {
                foreach (var worksheet in workbook.Worksheets)
                {
                    var sheetData = new List<List<string>>();

                    // Load all rows and columns from the worksheet
                    foreach (var row in worksheet.Rows())
                    {
                        var rowData = new List<string>();
                        foreach (var cell in row.Cells())
                        {
                            rowData.Add(cell.Value.ToString());
                        }
                        sheetData.Add(rowData);
                    }
                    excelData[worksheet.Name] = sheetData;
                }
            }

            return excelData;
        }

        // Method to clean the Excel data (optional)
        public async Task<List<string>> CleanData(string fileName, Dictionary<string, List<List<string>>> excelData)
        {
            var cleanedData = new List<string>();

            foreach (var sheet in excelData)
            {
                var sheetText = string.Join("\n", sheet.Value.Select(row => string.Join(", ", row)));
                cleanedData.Add(sheetText);
                await StoreCleanedDataInDb(string.Concat(fileName, " - ", sheet.Key), sheetText);
            }

            return cleanedData;
        }

        public async Task StoreCleanedDataInDb(string sheetName, string cleanedContent)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "excel_data.db");

            using (var dbContext = new AppDbContext(dbPath))
            {
                var excelData = new ExcelDataDto
                {
                    SheetName = sheetName,
                    CleanedContent = cleanedContent
                };

                var existing = await dbContext.ExcelData
                    .Where(e => e.SheetName == sheetName)
                    .FirstOrDefaultAsync();

                if (existing == null || existing.Id <= 0)
                {
                    dbContext.ExcelData.Add(excelData);
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new Exception("Excel file sheet already exisist");
                }
            }
        }

        public List<ExcelDataDto> GetCleanedDataFromDb()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "excel_data.db");

            using (var dbContext = new AppDbContext(dbPath))
            {
                return dbContext.ExcelData.ToList(); // Retrieve all cleaned data from the database
            }
        }
    }
}
