using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace FaceValidity
{
    class Program
    {
        private static HttpClient client;
        private static string loc = "";
        private static byte[] byteData;

        static void Main()
        {
            Console.WriteLine("Welcome to face Verification Portal");
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine();
            Console.WriteLine("Choose one source for image location :\n");
            Console.WriteLine("1. Local Address (Computer)");
            Console.WriteLine("2. Remote Address (Internet)");
            Console.WriteLine();
            Console.Write("Enter your choice : ");
            int c = Convert.ToInt32(Console.ReadLine());
            switch(c)
            {
                case 1: loc = "octet-stream";
                        break;
                case 2: loc = "json";
                        break;
                default: Console.WriteLine("Wrong choice");
                    Console.ReadKey();
                         Console.Clear();
                         Main();
                         break;
            }
            Console.Write("Enter the first url : ");
            string url1= Console.ReadLine();
            string faceId1 = Detect(url1);
            Console.WriteLine();
            Console.Write("Enter the second url : ");
            string url2 = Console.ReadLine();
            string faceId2 = Detect(url2);
            FaceVerify verstat = Verify(faceId1,faceId2);
            Console.WriteLine();
            Console.WriteLine();
            if (verstat.isIdentical==true)
            {
                Console.WriteLine("The two pictures are of the same person");
            }
            else Console.WriteLine("The two pictures are of two different persons");
            Console.WriteLine();
            Console.WriteLine("Match Confidence : "+(verstat.confidence*100)+" %");
            Console.ReadKey();
        }

        static string Detect(string url)
        {
            client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["returnFaceId"] = "true";
            queryString["returnFaceLandmarks"] = "false";
            

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2cfb8bd6639f47a5adc1551892f033ad");
            //client.DefaultRequestHeaders.
            var uri = "https://eastus2.api.cognitive.microsoft.com/face/v1.0/detect?" + queryString;
            byte[] byteData = null;
            if (loc=="json")
            {
                string json = "{\"url\":\"" + url + "\"}";
                byteData = Encoding.UTF8.GetBytes(json);
            }
            else
            {
                Image img = Image.FromFile(url);
                
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byteData = ms.ToArray();
                }
            }
            
            HttpResponseMessage response;
            string rcontent;
            // Request body
            string faceId;
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/"+loc);
                response = client.PostAsync(uri, content).Result;
                rcontent = response.Content.ReadAsStringAsync().Result;
                try
                {
                    List<FaceDetect> face = new List<FaceDetect>();
                    face = JsonConvert.DeserializeObject<List<FaceDetect>>(rcontent);
                    faceId = face[0].faceId;
                }
                catch (Exception)
                {

                    FaceDetect face = new FaceDetect();
                    face = JsonConvert.DeserializeObject<FaceDetect>(rcontent);
                    faceId = face.faceId;
                }
                
            }
            return faceId;

        }
        static FaceVerify Verify(string faceId1,string faceId2)
        {
            client = new HttpClient();
            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2cfb8bd6639f47a5adc1551892f033ad");

            var uri = "https://eastus2.api.cognitive.microsoft.com/face/v1.0/verify?";
            HttpResponseMessage response;
            string rcontent;
            string json = "{\"faceId1\":\"" + faceId1 + "\",\"faceId2\":\"" + faceId2 + "\"}";
            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(json);
            FaceVerify verify = new FaceVerify();
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = client.PostAsync(uri, content).Result;
                rcontent = response.Content.ReadAsStringAsync().Result;
                
                verify = JsonConvert.DeserializeObject<FaceVerify>(rcontent);
            }
            return verify;
        }

    }

}

