using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace StorageOAuthToken
{
    class Program
    {
        static  void Main(string[] args)
        {
            //Console.WriteLine("entered main");
            FooBar();
        }
        static void FooBar()
        {
            try
            {
              //  Console.Write("enteredFooBar");
                //get token
                string accessToken = GetMSIToken("https://storage.azure.com/");

                //create token credential
                TokenCredential tokenCredential = new TokenCredential(accessToken);

                //create storage credentials
                StorageCredentials storageCredentials = new StorageCredentials(tokenCredential);

                Uri blobAddress = new Uri("https://aksstoragedemo1.blob.core.windows.net/democontainer");

                //create block blob using storage credentials
                var container = new CloudBlobContainer(blobAddress, storageCredentials);
                // var x = await blob..DownloadTextAsync();
                //retrieve blob contents
                //Console.WriteLine(container.Name);
                CloudBlob b = container.GetBlobReference("demoFile.txt");
                //Console.WriteLine(b.Name);

                using (var stream = b.OpenReadAsync().Result)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                            Console.WriteLine(reader.ReadLine());
                    }
                }
                // Console.WriteLine(x); //blob.DownloadTextAsync());// ;
               //Console.Write("Finished");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static string GetMSIToken(string resourceID)
        {
            //Console.WriteLine("AccessTokenGetCallInit");
            string accessToken = string.Empty;
            // Build request to acquire MSI token
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=" + resourceID);
            request.Headers["Metadata"] = "true";
            request.Method = "GET";

            try
            {
                // Call /token endpoint
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // Pipe response Stream to a StreamReader, and extract access token
                StreamReader streamResponse = new StreamReader(response.GetResponseStream());
                string stringResponse = streamResponse.ReadToEnd();
                //Console.WriteLine(stringResponse);
                JsonSerializer j = new JsonSerializer();
                var a = JsonConvert.DeserializeObject<Dictionary<string, string>>(stringResponse);
               // Dictionary<string, string> list = j.Deserializ<Dictionary<string, string>>(stringResponse);
                //Console.WriteLine(a);
                accessToken = a["access_token"];
                Console.WriteLine("Access TOken:");
                Console.WriteLine(accessToken);
                return accessToken;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                string errorText = String.Format("{0} \n\n{1}", e.Message, e.InnerException != null ? e.InnerException.Message : "Acquire token failed");
                return accessToken;
            }
        }
    }
}