using ClassLibrary1;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var dep = new Class1();
            var json = dep.Hoge();
            Console.WriteLine(json);
        }
    }
}
