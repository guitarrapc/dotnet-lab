using Newtonsoft.Json;
using System;

namespace SourceLinktest
{
    class Program
    {
        static void Main(string[] args)
        {
            var h = new Hoge { Id = 1, Text = "hogemoge" };
            var json = JsonConvert.SerializeObject(h);
            Console.WriteLine(json);
        }
    }

    public class Hoge
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}
