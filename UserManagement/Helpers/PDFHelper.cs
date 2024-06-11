using Microsoft.JSInterop;

namespace UserManagement.Helpers
{
    public class PDFHelper
    {
        private readonly IWebHostEnvironment _environment;
        public PDFHelper(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public void OpenNewTab(IJSRuntime js, string filename)
        {
            string Url = $"{_environment.ContentRootPath}\\FinancialFiles\\{filename}";
            System.Net.WebClient webClient = new System.Net.WebClient();
            byte[] byteArray = webClient.DownloadData(Url);
            js.InvokeVoidAsync("jsOpenIntoNewTab",
                filename,
                Convert.ToBase64String(byteArray)
            );
        }
    }
}
