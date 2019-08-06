using System;
using System.Runtime.InteropServices;

namespace ConsoleApp4
{
    class Program
    {
        // windows. you can try `icuuc` without file extension.
        [DllImport("icuuc.dll")]
        // linux -> entrypoint not found..... (try some method that is `extern "C"`.....)
        //[DllImport("libicuuc.so.57.1", CallingConvention = CallingConvention.Cdecl)]
        public static extern double u_getNumericValue(int c);

        static void Main()
        {
            Console.WriteLine(u_getNumericValue('Ⅱ'));
            Console.WriteLine(u_getNumericValue('ⅱ'));
            Console.WriteLine(u_getNumericValue('٢'));
            Console.WriteLine(u_getNumericValue('二'));
        }
    }
}
