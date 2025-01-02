using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONNXAIBlazorApp.Platforms.Windows
{
    public interface IFilePickerService
    {
        Task<(string, string)> PickFileAsync(string[] fileTypes);
    }
}
