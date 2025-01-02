using ONNXAIBlazorApp.Data;
using System.Runtime.InteropServices;

namespace ONNXAIBlazorApp;

public partial class MainPage : ContentPage
{
    private readonly OllamaService _ollamaService;
    private string? _excelData;
    private readonly IExcelProcessorService excelProcessorService;
    private readonly ONNXAIBlazorApp.Platforms.Windows.IFilePickerService picker;

    public MainPage()
    {
        InitializeComponent();

        _ollamaService = new OllamaService("http://localhost:11434");
        excelProcessorService = IPlatformApplication.Current.Services.GetService<IExcelProcessorService>();
        picker = IPlatformApplication.Current.Services.GetService<ONNXAIBlazorApp.Platforms.Windows.IFilePickerService>();

        if (picker == null || excelProcessorService == null)
        {
            DisplayAlert("Error", "File picker or Excel processor service not found", "OK").Wait();
            return;
        };
    }

    // Allow user to pick an Excel file
    private async void OnUploadButtonClicked(object sender, EventArgs e)
    {
        try
        {

            string[] fileTypes = { ".xls", ".xlsx" };

            var file = await picker.PickFileAsync(fileTypes);

            if (!string.IsNullOrEmpty(file.Item1) & !string.IsNullOrEmpty(file.Item2))
            {
                // Process the selected Excel file
                var excelData = excelProcessorService.LoadExcelData(file.Item2);
                var cleanedData = await excelProcessorService.CleanData(file.Item1, excelData);

                _excelData = string.Join("\n\n", cleanedData); // Store data for queries

                await DisplayAlert("Success", "Excel file loaded successfully!", "OK");
            }
            else
            {
                await DisplayAlert("Cancelled", "No file selected", "OK");
            }
        }
        catch (COMException comEx)
        {
            Console.WriteLine($"COMException: {comEx}");
            await DisplayAlert("Error", $"File picker error: {comEx.Message}", "OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex}");
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    // Handle the query button click event
    private async void OnQueryButtonClicked(object sender, EventArgs e)
    {
        QueryResultLabel.Text = string.Empty;


        string query = QueryInput.Text; // Get the query from the user input

        if (string.IsNullOrWhiteSpace(query))
        {
            await DisplayAlert("Error", "Please enter a query.", "OK");
            return;
        }

        var excelProcessorService = IPlatformApplication.Current.Services.GetService<IExcelProcessorService>();

        if (excelProcessorService == null)
        {
            await DisplayAlert("Error", "File picker service not found", "OK");
            return;
        }

        var cleanedData = excelProcessorService.GetCleanedDataFromDb();
        string contextData = string.Empty;

        foreach(var d in cleanedData)
        {
            contextData += d.CleanedContent;
        }

        try
        {
            // Combine the cleaned Excel data with the user's query
            var fullQuery = $"{contextData}\n\n{query}";
            var result = await _ollamaService.QueryOllamaModelAsync(fullQuery);
            QueryResultLabel.Text = $"Answer: {result}";
        }
        catch (Exception ex)
        {
            QueryResultLabel.Text = $"Error: {ex.Message}";
        }
    }
}
