using SimpleLexer;
using System.Collections.Generic;
using Xunit;

namespace SimpleLext.Tests
{
    public class LexerUnitTest
    {
        public class Data
        {
            public string Input { get; set; }
            public Token[] Expected { get; set; }
        }

        public static IEnumerable<object[]> GetTokens()
        {
            yield return new []
            {
                new Data
                {
                    Input = " ans1 = 10 + 20 ",
                    Expected = new[] {
                        new Token(){ Kind = "variable", Value = "ans1"},
                        new Token(){ Kind = "sign", Value = "="},
                        new Token(){ Kind = "digit", Value = "10"},
                        new Token(){ Kind = "sign", Value = "+"},
                        new Token(){ Kind = "digit", Value = "20"}
                    },
                },
            };
        }

        [Theory, MemberData(nameof(GetTokens))]
        public void TokenizeTest(Data data)
        {
            var tokens = new Lexer(data.Input).Tokenize();
            var i = 0;
            foreach (var token in tokens)
            {
                Assert.Equal(token.Kind, data.Expected[i].Kind);
                Assert.Equal(token.Value, data.Expected[i].Value);
                i++;
            }
        }
    }
}
