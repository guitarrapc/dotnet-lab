using SimpleLexer;
using System.Collections.Generic;
using Xunit;

namespace SimpleLext.Tests
{
    public class InterpreterUnitTest
    {
        public class Data
        {
            public string Input { get; set; }
            public (string key, int value) Expected { get; set; }
        }

        public static IEnumerable<object[]> GetTokens()
        {
            yield return new []
            {
                new Data
                {
                    Input = " ans1 = 10 + 20 ",
                    Expected = ("ans1", 30),
                },
            };
            yield return new[]
            {
                new Data
                {
                    Input = " a = 3 + 4 * 5 ",
                    Expected = ("a", 23),
                },
            };
            yield return new[]
            {
                new Data
                {
                    Input = " a = 3 + 4 * 5 println(a)",
                    Expected = ("a", 23),
                },
            };
            yield return new[]
            {
                new Data
                {
                    Input = " a = (3 + 4) * 5 println(a)",
                    Expected = ("a", 35),
                },
            };
        }

        [Theory, MemberData(nameof(GetTokens))]
        public void TokenizeTest(Data data)
        {
            var lexeredTokens = new Lexer().Set(data.Input).Tokenize();
            var parsedTokens = new Parser().Set(lexeredTokens).Block();
            var interpreterResult = new Interpreter().Set(parsedTokens).Execute();
            foreach (var kv in interpreterResult)
            {
                Assert.Equal(kv.Key, data.Expected.key);
                Assert.Equal(kv.Value, data.Expected.value);
            }
        }
    }
}
