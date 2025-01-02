using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ONNXAIBlazorApp.Data;

namespace ONNXAIBlazorApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddScoped<ONNXAIBlazorApp.Platforms.Windows.IFilePickerService, ONNXAIBlazorApp.Platforms.Windows.FilePickerService>();
		builder.Services.AddScoped<IExcelProcessorService, ExcelProcessorService>();

		InitializeDatabase().Wait();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

    public static async Task InitializeDatabase()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "excel_data.db");
        using (var dbContext = new AppDbContext(dbPath))
        {
            await dbContext.Database.MigrateAsync(); // Apply migrations if needed
			await dbContext.SaveChangesAsync();
        }
    }
}
