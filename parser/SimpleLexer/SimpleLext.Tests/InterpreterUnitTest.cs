using SimpleLexer;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleLext.Tests
{
    public class InterpreterUnitTest
    {
        public class Data
        {
            public string Input { get; set; }
            public (string key, Interpreter.Variable value)[] Expected { get; set; }
        }

        public static IEnumerable<object[]> SimpleTokens()
        {
            yield return new[]
            {
                new Data
                {
                    Input = " ans1 = 10 + 20 ",
                    Expected = new [] {("ans1", new Interpreter.Variable { Name = "ans1", Value = 30 }) },
                },
            };
            yield return new[]
            {
                new Data
                {
                    Input = " 4 ",
                    Expected = Array.Empty<(string, Interpreter.Variable)>(), // skip
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
                    Expected = Array.Empty<(string, Interpreter.Variable)>(), // skip
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
                    Expected = new [] {("a", new Interpreter.Variable { Name = "a", Value = 23 }) },
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
                    Expected = new [] {("a", new Interpreter.Variable { Name = "a", Value = 23 }) },
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
                    Expected = new [] {("a", new Interpreter.Variable { Name = "a", Value = 35 }) },
                },
            };
        }
        public static IEnumerable<object[]> UnaryOperatorTokens()
        {
            yield return new[]
            {
                new Data
                {
                    Input = " a = -1",
                    Expected = new [] {("a", new Interpreter.Variable { Name = "a", Value = -1 }) },
                },
            };
        }
        public static IEnumerable<object[]> FunctionTokens()
        {
            yield return new[]
            {
                new Data
                {
                    Input = "v = 0"
                        + "function add(num) {"
                        + "  v = v + num"
                        + "}"
                        + "add(3)"
                        + "println(v)",
                    Expected = new []
                    {
                        ("v", new Interpreter.Variable { Name = "v", Value = 3 }),
                        ("num", new Interpreter.Variable { Name = "num", Value = 3 }),
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
        [Theory, MemberData(nameof(FunctionTokens))]
        public void FunctionTokenizeTest(Data data)
            => TestCore(data);

        public void TestCore(Data data)
        {
            var lexeredTokens = new Lexer().Set(data.Input).Tokenize();
            var parsedTokens = new Parser().Set(lexeredTokens).Block();
            var interpreterResult = new Interpreter().Set(parsedTokens).Execute();
            foreach (var kv in data.Expected)
            {
                Assert.True(interpreterResult.ContainsKey(kv.value.Name));
                Assert.True(interpreterResult[kv.value.Name].Value.Equals(kv.value.Value));
            }
        }
    }
}
