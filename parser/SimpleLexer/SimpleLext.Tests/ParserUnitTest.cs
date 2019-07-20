using SimpleLexer;
using System.Collections.Generic;
using Xunit;

namespace SimpleLext.Tests
{
    public class ParserUnitTest
    {
        public class Data
        {
            public string Input { get; set; }
            public string Parenthis { get; set; }
            public Token[] Expected { get; set; }
        }

        public static IEnumerable<object[]> GetTokens()
        {
            yield return new[]
            {
                new Data
                {
                    Input = " ans1 = 10 + 20 ",
                    Parenthis = "(ans1 = (10 + 20))",
                    Expected = new[] {
                        new Token()
                        {
                            Kind = "sign",
                            Value = "=",
                            Left = new Token(){ Kind = "ident", Value = "ans1"},
                            Right = new Token()
                            {
                                Kind = "sign",
                                Value = "+",
                                Left = new Token(){ Kind = "digit", Value = "10"},
                                Right =  new Token(){ Kind = "digit", Value = "20"},
                            }
                        },
                    },
                },
            };
            yield return new[]
            {
                new Data
                {
                    Input = "4",
                    Parenthis = "4",
                    Expected = new[] {
                        new Token(){Kind = "digit",Value = "4"},
                    },
                },
            };
            yield return new[]
            {
                new Data
                {
                    Input = "4 + 5",
                    Parenthis = "(4 + 5)",
                    Expected = new[] {
                        new Token()
                        {
                            Kind = "sign",
                            Value = "+",
                            Left = new Token(){Kind = "digit",Value = "4"},
                            Right = new Token(){Kind = "digit",Value = "5"},
                        },
                    },
                },
            };
            yield return new[]
            {
                new Data
                {
                    Input = "3 + 4 * 5",
                    Parenthis = "(3 + (4 * 5))",
                    Expected = new[] {
                        new Token()
                        {
                            Kind = "sign",
                            Value = "+",
                            Left = new Token(){Kind = "digit",Value = "3"},
                            Right = new Token()
                            {
                                Kind = "sign",
                                Value = "*",
                                Left = new Token(){Kind = "digit",Value = "4",},
                                Right = new Token(){Kind = "digit",Value = "5",},
                            },
                        },
                    },
                },
            };
            yield return new[]
            {
                new Data
                {
                    Input = "a = 3 + 4 * 5",
                    Parenthis = "(a = (3 + (4 * 5)))",
                    Expected = new[] {
                        new Token()
                        {
                            Kind = "sign",
                            Value = "=",
                            Left = new Token(){Kind = "ident",Value = "a"},
                            Right = new Token()
                            {
                                Kind = "sign",
                                Value = "+",
                                Left = new Token(){Kind = "digit",Value = "3"},
                                Right = new Token()
                                {
                                    Kind = "sign",
                                    Value = "*",
                                    Left = new Token(){Kind = "digit",Value = "4"},
                                    Right = new Token(){Kind = "digit",Value = "5"},
                                },
                            },
                        },
                    },
                },
            };
        }

        [Theory, MemberData(nameof(GetTokens))]
        public void TokenizeTest(Data data)
        {
            var lexeredTokens = new Lexer().Set(data.Input).Tokenize();
            var parsedTokens = new Parser().Set(lexeredTokens).Block();
            var i = 0;
            foreach (var token in parsedTokens)
            {
                Assert.Equal(token.Kind, data.Expected[i].Kind);
                Assert.Equal(token.Value, data.Expected[i].Value);
                RecurseLeft(token.Left, data.Expected[i].Left);
                RecurseRight(token.Right, data.Expected[i].Right);
                i++;
            }
        }

        private void RecurseLeft(Token token, Token expected)
        {
            if (token == null) return;
            Assert.Equal(token.Kind, expected.Kind);
            Assert.Equal(token.Value, expected.Value);
            RecurseLeft(token.Left, expected.Left);
            RecurseRight(token.Right, expected.Right);
        }

        private void RecurseRight(Token token, Token expected)
        {
            if (token == null) return;
            Assert.Equal(token.Kind, expected.Kind);
            Assert.Equal(token.Value, expected.Value);
            RecurseLeft(token.Left, expected.Left);
            RecurseRight(token.Right, expected.Right);
        }
    }
}
