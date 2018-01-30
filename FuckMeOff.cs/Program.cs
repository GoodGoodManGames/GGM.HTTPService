using System;
using System.Net;
using System.Text;
using TestObject;
using Google.Protobuf;

namespace TestSerializer
{
    class Program
    {
        static void Main(string[] args)
        {
            Serializer serializer = new Serializer();
            string uriString = "http://localhost:8002/bodytest3";
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            Console.WriteLine("\nPlease enter the data to be posted to the URI {0}:", uriString);

            serializer.tester.Name = "HeoJunMoo";
            byte[] postArray = serializer.Serialize(serializer.tester);
            myWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //UploadData implicitly sets HTTP POST as the request method.
            byte[] request = myWebClient.UploadData(uriString, postArray);

            // Decode and display the response.
            Console.WriteLine("\nResponse received was :{0}", Encoding.ASCII.GetString(request));
            while (true) { }
        }
    }



    public class Serializer
    {
        public TestObj tester { get; set; } = new TestObj();
        
        public byte[] Serialize(TestObj model)
        {
            return model.ToByteArray();
        }
    }
}
