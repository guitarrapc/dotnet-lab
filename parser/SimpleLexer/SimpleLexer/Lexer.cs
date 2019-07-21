using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLexer
{
    public class Token
    {
        public string Kind { get; set; }
        public string Value { get; set; }
        public Token Left { get; set; }
        public Token Right { get; set; }

        public Token Ident { get; set; }
        public List<Token> Params { get; set; }
        public List<Token> Block { get; set; }

        public override string ToString()
            => $"{Kind}, {Value}";

        public string ToParenthesis()
        {
            if (Left == null && Right == null && Params == null)
                return Value;
            if (Left != null && Right == null && Params == null)
                return Value + Left.ToParenthesis();
            var builder = new StringBuilder();
            builder.Append("(");
            if (Left != null)
                builder.Append(Left.ToParenthesis()).Append(" ");
            builder.Append(Value);
            if (Right != null)
                builder.Append(" ").Append(Right.ToParenthesis());
            if (Left == null && Right == null && Ident != null && Value == "function")
            {
                // function param
                builder.Append(" ");
            }
            if (Left == null && Right == null && Ident != null)
            {
                builder.Append(Ident.ToParenthesis());
                builder.Append("(");
            }
            // function param / method param
            if (Params != null)
            {
                if (Params.Count > 0)
                {
                    builder.Append(Params[0].ToParenthesis());
                    for (var i = 1; i < Params.Count(); ++i)
                    {
                        builder.Append(", ").Append(Params[i].ToParenthesis());
                    }
                }
                builder.Append(")");
            }
            // inside function block
            if (Left == null && Right == null && Ident != null && Value == "function")
            {
                builder.Append("{");
                builder.Append(" ");
                if (Block.Count() > 0)
                {
                    for (var i = 0; i < Block.Count(); ++i)
                    {
                        builder.Append(Block[i].ToParenthesis());
                    }
                }
                builder.Append("}");
            }
            builder.Append(")");
            return builder.ToString();
        }
    }

    /// <summary>
    /// 3 kinds lexer implementation.
    /// 1. variable (must start with ascii char)
    /// 2. digit
    /// 3. sign(=,+,-,*,/)
    /// </summary>
    /// <remarks>
    /// can lexer following.
    /// ` ans1 = 10 + 20 `
    /// ans1: variable
    /// </remarks>
    public class Lexer
    {
        private string text;
        private int index;

        public Lexer Set(string text)
        {
            this.text = text;
            this.index = 0;
            return this;
        }

        // tokenize
        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            var token = NextToken();
            while (token != null)
            {
                tokens.Add(token);
                token = NextToken();
            }
            return tokens;
        }
        private Token NextToken()
        {
            SkipSpace();
            if (IsEndOfToken()) return null;
            if (IsSigneStart(Current())) return Sign();
            if (IsDigitStart(Current())) return Digit();
            if (IsIdentStart(Current())) return Ident();
            if (IsParenthisStart(Current())) return Parenthesis();
            if (IsCurlyBracketStart(Current())) return CurlyBracket();
            if (IsSymbolStart(Current())) return Symbol();
            throw new ArgumentOutOfRangeException("Not a character for tokens");
        }

        // move index
        private bool IsEndOfToken() => text.Length <= index;
        private char Current()
        {
            if (IsEndOfToken()) throw new ArgumentOutOfRangeException("no more character");
            return text[index];
        }
        private char Next()
        {
            var c = Current();
            ++index;
            return c;
        }
        private void SkipSpace()
        {
            while (!IsEndOfToken() && char.IsWhiteSpace(Current()))
            {
                Next();
            }
        }

        // detect token
        private bool IsSigneStart(char c)
        {
            return c == '=' || c == '+' || c == '-' || c == 'c' || c == '*' || c == '/';
        }
        private Token Sign()
        {
            var token = new Token()
            {
                Kind = "sign",
                Value = Next().ToString(),
            };
            return token;
        }

        private bool IsDigitStart(char c)
        {
            return char.IsDigit(c);
        }
        private Token Digit()
        {
            var builder = new StringBuilder();
            builder.Append(Next());
            while (!IsEndOfToken() && char.IsDigit(Current()))
            {
                builder.Append(Next());
            }
            var token = new Token()
            {
                Kind = "digit",
                Value = builder.ToString(),
            };
            return token;
        }

        private bool IsIdentStart(char c)
        {
            return char.IsLetter(c);
        }
        private Token Ident()
        {
            var builder = new StringBuilder();
            builder.Append(Next());
            while (!IsEndOfToken() && (char.IsLetter(Current())) || char.IsDigit(Current()))
            {
                builder.Append(Next());
            }
            var token = new Token()
            {
                Kind = "ident",
                Value = builder.ToString(),
            };
            return token;
        }

        private bool IsParenthisStart(char c)
        {
            return c == '(' || c == ')';
        }
        private Token Parenthesis()
        {
            var token = new Token()
            {
                Kind = "parenthesis",
                Value = Next().ToString(),
            };
            return token;
        }

        private bool IsCurlyBracketStart(char c)
        {
            return c == '{' || c == '}';
        }
        private Token CurlyBracket()
        {
            var token = new Token();
            if (Current() == '{')
            {
                token.Kind = "curlybracket";
            }
            else
            {
                token.Kind = "endofblock";
            }
            token.Value = Next().ToString();
            return token;
        }

        private bool IsSymbolStart(char c)
        {
            return c == ',';
        }
        private Token Symbol()
        {
            var token = new Token()
            {
                Kind = "symbol",
                Value = Next().ToString(),
            };
            return token;
        }
    }
}
