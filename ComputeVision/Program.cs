using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;
using ClientLibrary.Contract;

namespace ComputeVision
{
    class Program
    {
        private static readonly string apiKey = "d8c66574e7cb4612940cd2e819d53854";

        static void Main(string[] args)
        {
            VisionServiceClient client = new VisionServiceClient(apiKey);

            string path = @"C:\Users\programmer1\Pictures\Capture.PNG";

            var t = GetTextAsync(client,
                                 path);
            t.Wait(5000);

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static async Task GetTextAsync(VisionServiceClient client,
                                               string path)
        {
            using (Stream imageStream = File.OpenRead(path))
            {
                var t = client.CreateTextRecognitionOperationAsync(imageStream,
                                                                   TextRecognitionMode.Printed);
                if(t.Result != null)
                {
                    var result = await client.GetTextRecognitionOperationResultAsync(t.Result);

                    int i = 0;
                    int maxRetry = 60;

                    while ((result.Status == TextRecognitionOperationStatus.Running || result.Status == TextRecognitionOperationStatus.NotStarted)
                           && i++ < maxRetry)
                    {
                        Console.WriteLine("Server status: {0}, waiting {1} seconds...",
                                          result.Status,
                                          i);
                        await Task.Delay(1000);

                        result = await client.GetTextRecognitionOperationResultAsync(t.Result);
                    }

                    StringBuilder builder = new StringBuilder();
                    foreach (var line in result.RecognitionResult.Lines)
                    {
                        Console.WriteLine(line.Text);
                    }
                }
            }
        }
    }
}
