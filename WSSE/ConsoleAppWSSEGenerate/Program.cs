using System;
using System.Net.Http;

namespace ConsoleAppWSSEGenerate
{
    class Program
    {
        static void Main(string[] args)
        {
            //WSSEHeader wSSEHeader = new WSSEHeader("admin", "admin");

            //Console.WriteLine("Hello World!{0}",wSSEHeader.GenerateHeader());
            callHttp();
            Console.ReadLine();
        }

        static async void callHttp()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/vnd.api+json");
            httpClient.AddWsseHeader("admin", "8aad71d26acf82023389a79b3bbb9833dfd9f949");
            //httpClient.BaseAddress = new Uri("http://oro.templatebar.com/project1/web/admin/api");
            using (var r = await httpClient.GetAsync(new Uri("http://oro.templatebar.com/project1/web/admin/api/accounts")))
            {
                Console.WriteLine("in");
                string result = await r.Content.ReadAsStringAsync();
                Console.WriteLine(result);
                Console.WriteLine("Out");
            }
        }
    }
}
