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

        public static IEnumerable<object[]> SimpleTokens()
        {
            yield return new[]
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
            yield return new []
            {
                new Data
                {
                    Input = " 4 ",
                    Expected = new[] {
                        new Token(){ Kind = "digit", Value = "4"},
                    },
                },
            };
        }
        public static IEnumerable<object[]> MultisignTokens()
        {
            yield return new[]
            {
                new Data
                {
                    Input = " 3 + 4 * 5 ",
                    Expected = new[] {
                        new Token(){ Kind = "digit", Value = "3"},
                        new Token(){ Kind = "sign", Value = "+"},
                        new Token(){ Kind = "digit", Value = "4"},
                        new Token(){ Kind = "sign", Value = "*"},
                        new Token(){ Kind = "digit", Value = "5"}
                    },
                },
            };
        }
        public static IEnumerable<object[]> AssignTokens()
        {
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
        }
        public static IEnumerable<object[]> PrintVariableTokens()
        {
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
        }
        public static IEnumerable<object[]> ParenthesisTokens()
        {
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
        public static IEnumerable<object[]> UnaryOperatorTokens()
        {
            yield return new[]
            {
                new Data
                {
                    Input = " a = -1 ",
                    Expected = new[] {
                        new Token(){ Kind = "ident", Value = "a"},
                        new Token(){ Kind = "sign", Value = "="},
                        new Token(){ Kind = "sign", Value = "-"},
                        new Token(){ Kind = "digit", Value = "1"},
                    },
                },
            };
        }

        [Theory, MemberData(nameof(SimpleTokens))]
        public void SimpleTokenizeTest(Data data)
            => TestCore(data);
        [Theory, MemberData(nameof(MultisignTokens))]
        public void MultisignTokenizeTest(Data data)
            => TestCore(data);
        [Theory, MemberData(nameof(AssignTokens))]
        public void AssignTokenizeTest(Data data)
            => TestCore(data);
        [Theory, MemberData(nameof(PrintVariableTokens))]
        public void PrintVaribleTokenizeTest(Data data)
            => TestCore(data);
        [Theory, MemberData(nameof(ParenthesisTokens))]
        public void ParenthesisTokenizeTest(Data data)
            => TestCore(data);
        [Theory, MemberData(nameof(UnaryOperatorTokens))]
        public void UnaryOperatorTokenizeTest(Data data)
            => TestCore(data);

        private void TestCore(Data data)
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
