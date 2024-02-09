using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task ExampleMain(string[] args)
    {
        await DownloadFilesAsync(1000, 10); // Download 1000 files with a rate limit of 10 requests per minute
    }

    static async Task DownloadFilesAsync(int totalFiles, int rateLimitPerMinute)
    {
        var downloadTasks = new List<Task>();
        var throttler = new SemaphoreSlim(rateLimitPerMinute, rateLimitPerMinute);

        for (int i = 0; i < totalFiles; i++)
        {
            await throttler.WaitAsync(); // Enforce rate limit

            var taskId = i; // Local copy for closure
            var task = Task.Run(async () =>
            {
                try
                {
                    await DownloadFileAsync(taskId); // Simulate file download
                    Console.WriteLine($"File {taskId} downloaded successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading file {taskId}: {ex.Message}");
                    // Handle individual task exception here
                }
                finally
                {
                    throttler.Release(); // Release semaphore, allowing another task to start
                }
            });

            downloadTasks.Add(task);

            // Check if we've reached the rate limit, then delay for the remainder of the minute
            if ((i + 1) % rateLimitPerMinute == 0)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        await Task.WhenAll(downloadTasks); // Wait for all downloads to complete
        Console.WriteLine("All files processed.");
    }

    static async Task DownloadFileAsync(int fileId)
    {
        // Simulate a file download with a random success or failure
        await Task.Delay(100); // Simulate delay

        if (new Random().Next(0, 5) == 0) // 1 in 5 chance of failure
        {
            throw new Exception($"File {fileId} failed to download due to a simulated error.");
        }
    }
}