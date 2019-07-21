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
                        new Token(){ Kind = "ident", Value = "ans1"},
                        new Token(){ Kind = "sign", Value = "="},
                        new Token(){ Kind = "digit", Value = "10"},
                        new Token(){ Kind = "sign", Value = "+"},
                        new Token(){ Kind = "digit", Value = "20"}
                    },
                },
            };
            yield return new[]
            {
                new Data
                {
                    Input = " a = 3 + 4 * 5 ",
                    Expected = new[] {
                        new Token(){ Kind = "ident", Value = "a"},
                        new Token(){ Kind = "sign", Value = "="},
                        new Token(){ Kind = "digit", Value = "3"},
                        new Token(){ Kind = "sign", Value = "+"},
                        new Token(){ Kind = "digit", Value = "4"},
                        new Token(){ Kind = "sign", Value = "*"},
                        new Token(){ Kind = "digit", Value = "5"}
                    },
                },
            };
            yield return new[]
            {
                new Data
                {
                    Input = " a = 3 + 4 * 5 println(a)",
                    Expected = new[] {
                        new Token(){ Kind = "ident", Value = "a"},
                        new Token(){ Kind = "sign", Value = "="},
                        new Token(){ Kind = "digit", Value = "3"},
                        new Token(){ Kind = "sign", Value = "+"},
                        new Token(){ Kind = "digit", Value = "4"},
                        new Token(){ Kind = "sign", Value = "*"},
                        new Token(){ Kind = "digit", Value = "5"},
                        new Token(){ Kind = "ident", Value = "println"},
                        new Token(){ Kind = "parenthesis", Value = "("},
                        new Token(){ Kind = "ident", Value = "a"},
                        new Token(){ Kind = "parenthesis", Value = ")"},
                    },
                },
            };
            yield return new[]
            {
                new Data
                {
                    Input = " a = (3 + 4) * 5 println(a)",
                    Expected = new[] {
                        new Token(){ Kind = "ident", Value = "a"},
                        new Token(){ Kind = "sign", Value = "="},
                        new Token(){ Kind = "parenthesis", Value = "("},
                        new Token(){ Kind = "digit", Value = "3"},
                        new Token(){ Kind = "sign", Value = "+"},
                        new Token(){ Kind = "digit", Value = "4"},
                        new Token(){ Kind = "parenthesis", Value = ")"},
                        new Token(){ Kind = "sign", Value = "*"},
                        new Token(){ Kind = "digit", Value = "5"},
                        new Token(){ Kind = "ident", Value = "println"},
                        new Token(){ Kind = "parenthesis", Value = "("},
                        new Token(){ Kind = "ident", Value = "a"},
                        new Token(){ Kind = "parenthesis", Value = ")"},
                    },
                },
            };
        }

        [Theory, MemberData(nameof(GetTokens))]
        public void TokenizeTest(Data data)
        {
            var tokens = new Lexer().Set(data.Input).Tokenize();
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
