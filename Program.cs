
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace BlobTransferMiddleware
{
    public class Program
    {
        private static IConfiguration _configuration;
        static async Task Main(string[] args)
        {
            GetAppSettingsFile();

            await UploadFiles();

            Console.WriteLine("Upload File Completed...");
        }

        static void GetAppSettingsFile()
        {
            Console.WriteLine($"appsetting directory: {Directory.GetCurrentDirectory()}");

            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();
        }

        static async Task UploadFiles()
        {
            var filesList = new FilesDatabase(_configuration);
            var listModel = filesList.GetFilesToUpload();
            //get a list of files to upload
            foreach (var item in listModel)
            {
                Console.WriteLine("uploading..." + item.FileName);
                await BlobTransfer.UploadFilesAsync(_configuration, item.PhysicalPath);

                filesList.UpdateFile(item.Id);

            }
           
        }

    }

    
}
