using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace BlobTransferMiddleware
{
    public static class BlobTransfer
    {
       
        public static async Task UploadFilesAsync(IConfiguration configuration, string path)
        {
            string containerName = configuration.GetConnectionString("MainTransfer");
            string connectionString = configuration.GetConnectionString("AzureBlobStorageConnectionString"); 

            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient _blobServiceClient = new BlobServiceClient(connectionString);

            string fileAndPath = Path.Combine(path);

            string currentDirectory = Path.GetDirectoryName(fileAndPath);

            string fullPathOnly = Path.GetFullPath(currentDirectory);
            // Path to the directory to upload
            //string uploadPath = Directory.GetCurrentDirectory() + "C:\\aspnetcore_projects\\BlobTransferMiddleware\\uploads";
            string uploadPath = Path.Combine(currentDirectory);

            if (!Directory.Exists(currentDirectory))
            {
                Console.WriteLine($"Path does not exists..{uploadPath}");
            }
            else
            {
                Console.WriteLine($"Path exisits: {uploadPath}");
               

            }

            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(containerName);
          

            // Start a timer to measure how long it takes to upload all the files.
            Stopwatch timer = Stopwatch.StartNew();

            try
            {
                Console.WriteLine($"Iterating in directory: {uploadPath}");
                int count = 0;

                Console.WriteLine($"Found {Directory.GetFiles(uploadPath).Length} file(s)");

                // Specify the StorageTransferOptions
                BlobUploadOptions options = new BlobUploadOptions
                {
                    TransferOptions = new StorageTransferOptions
                    {
                        // Set the maximum number of workers that 
                        // may be used in a parallel transfer.
                        MaximumConcurrency = 8,

                        // Set the maximum length of a transfer to 50MB.
                        MaximumTransferSize = 50 * 1024 * 1024
                    }
                };

                // Create a queue of tasks that will each upload one file.
                var tasks = new Queue<Task<Response<BlobContentInfo>>>();

              
                    string filePath = Path.GetFullPath(fileAndPath);
                
                    //BlobContainerClient container = containers[count % 5];
                    string fileName = Path.GetFileName(fileAndPath);

                    Console.WriteLine($"Uploading {fileName} to container {containerName}");
                    BlobClient blob = container.GetBlobClient(fileName);

                    // Add the upload task to the queue
                    tasks.Enqueue(blob.UploadAsync(filePath, options));
                    count++;
                

                // Run all the tasks asynchronously.
                await Task.WhenAll(tasks);

                timer.Stop();
                Console.WriteLine($"Uploaded {count} files in {timer.Elapsed.TotalSeconds} seconds");
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Azure request failed: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Error parsing files in the directory: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}