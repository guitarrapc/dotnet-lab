using icusample.Interop;
using System;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
            //var length = TextLengthInGrapheme("hogemoge");
            GetIcuText("👪");
            //Console.WriteLine(length);
        }

        public static int TextLengthInGrapheme(string text)
        {
            //TODO: won't work....
            var iterator = Icuuc.ubrk_open(UBreakIteratorType.UBRK_CHARACTER, Icuuc.uloc_getDefault(), null, 0, out _);
            var utext = Icuuc.utext_openUChars(IntPtr.Zero, text, text.Length, out _);
            Icuuc.ubrk_setUText(iterator, utext, out _);
            var n = 0;
            while (Icuuc.ubrk_next(iterator) != Icuuc.UBRK_DONE) n++;
            Icuuc.ubrk_close(iterator);
            Icuuc.utext_close(utext);
            return n;
        }

        public static void GetIcuText(string text)
        {
            //TODO: won't work....
            var iterator = Icuuc.ubrk_open(UBreakIteratorType.UBRK_CHARACTER, Icuuc.uloc_getDefault(), null, 0, out _);
            var utext = Icuuc.utext_openUChars(IntPtr.Zero, text, text.Length, out _);
            Icuuc.ubrk_setUText(iterator, utext, out _);
            int current = Icuuc.ubrk_current(iterator);
            while (current != Icuuc.UBRK_DONE)
            {
                int next = Icuuc.ubrk_next(iterator);
                if (next == Icuuc.UBRK_DONE) break;
                int size = next - current;
                Console.WriteLine($"{current}-{next}: {text.Substring(current, size)}");
                current = next;
            }
            Icuuc.ubrk_close(iterator);
            Icuuc.utext_close(utext);
        }
    }
}
