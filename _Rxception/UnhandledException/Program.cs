using System;

namespace UnhandledException
{
    class Program
    {
        static void Main(string[] args)
        {
            Do();
            DoNot();            
        }
        
        private static void Do()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                // You can catch unhandled exception without try/catch root execution point.
                var ex = (Exception)e.ExceptionObject;
                Console.WriteLine($"UnhandledException occured. {ex.GetType().FullName}, {ex.Message}, {ex.StackTrace}");
                
                // if you want handle exception as normal, you may able to use Exit(int);
                //Environment.Exit(1);
            };
            Hoge();
        }

        private static void DoNot()
        {
            try
            {
                // It may better avoid try/catch root execution for global escape.
                Hoge()
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UnhandledException occured. {ex.GetType().FullName}, {ex.Message}, {ex.StackTrace}");
            }
        }

        private static void Hoge()
        {
            throw new Exception("");
        }
    }
}
