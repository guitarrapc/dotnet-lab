using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLexer
{
    public class Parser
    {
        private Dictionary<string, int> degrees;
        private readonly List<string> factorKinds;
        private readonly List<string> binaryKinds;
        private readonly List<string> rightAssociates;
        private readonly List<string> unaryOperators;
        private readonly List<string> reserved;

        private readonly Token endOfToken = new Token()
        {
            Kind = "endofblock",
            Value = "(__endofblock__)"
        };

        private List<Token> tokens;
        private int index;

        public Parser()
        {
            degrees = new Dictionary<string, int>()
            {
              {"(", 80},
              // unaryoperators: 70
              {"*", 60},
              {"/", 60},
              {"+", 50},
              {"-", 50},
              {"=", 10},
            };

            factorKinds = new List<string>() { "digit", "ident" };
            binaryKinds = new List<string>() { "sign" };
            rightAssociates = new List<string>() { "=" };
            unaryOperators = new List<string>() { "+", "-" };
            reserved = new List<string>() { "function" };
        }

        public Parser Set(List<Token> tokens)
        {
            index = 0;
            this.tokens = tokens;
            // add last letter symbol token
            this.tokens.Add(endOfToken);
            return this;
        }

        private Token Current()
        {
            if (tokens.Count() <= index)
                throw new ArgumentOutOfRangeException("no more token");
            return tokens[index];
        }
        private Token Next()
        {
            var token = Current();
            ++index;
            return token;
        }
        private Token Consume(string expectedValue)
        {
            if (!expectedValue.Equals(Current().Value))
                throw new Exception($"Not expected value. {expectedValue}");
            return Next();
        }
        /// <summary>
        /// determine is token can place left or right edge of expression.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private Token Lead(Token token)
        {
            if (token.Kind.Equals("ident") && token.Value.Equals("function"))
            {
                return Function(token);
            }
            if (factorKinds.Contains(token.Kind))
            {
                return token;
            }
            if (unaryOperators.Contains(token.Value))
            {
                // override
                token.Kind = "unary";
                token.Left = Express(70);
                return token;
            }
            if (token.Kind.Equals("parenthesis") && token.Value.Equals("("))
            {
                var expr = Express(0);
                Consume(")");
                return expr;
            }
            throw new Exception($"the token cannot place here. {token.Kind}: {token.Value}");
        }
        private Token Function(Token token)
        {
            // function name(param){}
            token.Kind = "function";
            token.Ident = Ident();
            Consume("(");
            token.Params = new List<Token>();
            if (!Current().Value.Equals(")"))
            {
                token.Params.Add(Ident());
                while (!Current().Value.Equals(")"))
                {
                    Consume(",");
                    token.Params.Add(Ident());
                }
            }
            Consume(")");
            Consume("{");
            token.Block = Block();
            Consume("}");
            return token;
        }
        private Token Ident()
        {
            var id = Next();
            if (!id.Kind.Equals("ident"))
                throw new Exception($"not a identical token, {id.Kind}");
            if (reserved.Contains(id.Value))
                throw new ArgumentOutOfRangeException($"The token was reserved. {id.Value}");
            return id;
        }
        /// <summary>
        /// token priority via op degree
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private int Degree(Token token)
        {
            if (degrees.ContainsKey(token.Value))
                return degrees[token.Value];
            return 0;
        }
        /// <summary>
        /// Tokens comes to left/right of op token
        /// </summary>
        /// <remarks>
        /// in case of 4 + 3, this method will asop 4 for left, and 3 for right.
        /// </remarks>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        private Token Bind(Token left, Token op)
        {
            if (binaryKinds.Contains(op.Kind))
            {
                op.Left = left;
                var leftDegree = Degree(op);
                if (rightAssociates.Contains(op.Value))
                {
                    // minus degree of left op
                    leftDegree -= 1;
                }
                op.Right = Express(leftDegree);
                return op;
            }
            else if (op.Kind.Equals("parenthesis") && op.Value.Equals("("))
            {
                op.Left = left;
                op.Params = new List<Token>();
                if (!Current().Value.Equals(")"))
                {
                    op.Params.Add(Express(0));
                    while (!Current().Value.Equals(")"))
                    {
                        Consume(",");
                        op.Params.Add(Express(0));
                    }
                }
                Consume(")");
                return op;
            }
            throw new Exception("The token cannot place here.");
        }
        /// <summary>
        /// Relate each Token via op degree
        /// </summary>
        /// <param name="leftDegree"></param>
        /// <returns></returns>
        private Token Express(int leftDegree)
        {
            var left = Lead(Next());
            var rightDegree = Degree(Current());
            while (leftDegree < rightDegree)
            {
                var op = Next();
                left = Bind(left, op);
                rightDegree = Degree(Current());
            }
            return left;
        }

        /// <summary>
        /// Express until last letter (eot)
        /// </summary>
        /// <returns></returns>
        public List<Token> Block()
        {
            var block = new List<Token>();
            while (!Current().Kind.Equals("endofblock"))
            {
                block.Add(Express(0));
            }
            return block;
        }
    }
}
