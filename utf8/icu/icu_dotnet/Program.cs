using System;
using System.Globalization;
using System.Linq;
using Icu.BreakIterators;

namespace icu_dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("ICU\n");
            RunIcu();

            Console.Write("StringInfo\n");
            RunStringInfo();
        }

        private static void RunIcu()
        {
            // icu
            Icu.Wrapper.Init();
            var locale = Icu.Locale.GetLocaleForLCID(0);

            var family = Icu.BreakIterator.GetBoundaries(Icu.BreakIterator.UBreakIteratorType.CHARACTER, locale, "👪");
            var families = Icu.BreakIterator.GetBoundaries(Icu.BreakIterator.UBreakIteratorType.CHARACTER, locale, "👨‍👩‍👧‍👧");
            var wordBoundaries = Icu.BreakIterator.GetWordBoundaries(locale, "👨‍👩‍👧‍👧", true);
            Console.WriteLine($"👪: {family.First()} 👨‍👩‍👧‍👧: {families.First()} 👨‍👩‍👧‍👧:{wordBoundaries.First()}");
            Console.WriteLine($"{Icu.Normalizer.Normalize("👪", Icu.Normalizer.UNormalizationMode.UNORM_NFC)}");
            Icu.Wrapper.Cleanup();
        }

        private static void RunStringInfo()
        {
            // string info
            var stringInfoItelator = StringInfo.GetTextElementEnumerator("👨‍👩‍👧‍👧");
            while (stringInfoItelator.MoveNext())
            {
                Console.WriteLine(stringInfoItelator.Current);
            }
            var stringInfo = new StringInfo("👨‍👩‍👧‍👧");
            Console.WriteLine(stringInfo.LengthInTextElements);
            // same as Enumerator
            for (var i = 0; i < stringInfo.LengthInTextElements; i++)
            {
                Console.WriteLine(stringInfo.SubstringByTextElements(i, 1));
            }
        }
    }
}

