using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONNXAIBlazorApp.Models
{
    public class ExcelDataDto
    {
        public int Id { get; set; }
        public string SheetName { get; set; }
        public string CleanedContent { get; set; }
    }
}
