using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace ONNXAIBlazorApp.Platforms.Windows
{
    public class FilePickerService : IFilePickerService
    {
        public async Task<(string, string)> PickFileAsync(string[] fileTypes)
        {
            try
            {
                var picker = new FileOpenPicker
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                };

                foreach (var fileType in fileTypes)
                {
                    picker.FileTypeFilter.Add(fileType);
                }

                var hwnd = ((MauiWinUIWindow)Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                var file = await picker.PickSingleFileAsync();
                return (file.Name, file.Path);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
