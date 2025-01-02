
using ONNXAIBlazorApp.Models;

namespace ONNXAIBlazorApp.Data
{
    public interface IExcelProcessorService
    {
        public Dictionary<string, List<List<string>>> LoadExcelData(string filePath);
        Task<List<string>> CleanData(string fileName, Dictionary<string, List<List<string>>> excelData);
        Task StoreCleanedDataInDb(string sheetName, string cleanedContent);
        public List<ExcelDataDto> GetCleanedDataFromDb();
    }
}
