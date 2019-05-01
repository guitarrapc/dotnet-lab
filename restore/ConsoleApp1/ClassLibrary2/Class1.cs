using Newtonsoft.Json;
using System;

namespace ClassLibrary2
{
    public class Class2
    {
        public string Hoge()
        {
            var fuga = new Fuga()
            {
                Hogemoge = "hogmoge",
            };
            var json = JsonConvert.SerializeObject(fuga);
            return json;
        }
    }

    public class Fuga
    {
        public string Hogemoge { get; set; }
    }
}
