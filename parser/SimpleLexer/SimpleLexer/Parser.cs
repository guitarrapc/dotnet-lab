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

        private readonly Token endOfToken = new Token()
        {
            Kind = "eot",
            Value = "(__eot__)"
        };

        private readonly List<Token> tokens;
        private int index;

        public Parser(List<Token> tokens)
        {
            degrees = new Dictionary<string, int>()
            {
              {"*", 60},
              {"/", 60},
              {"+", 50},
              {"-", 50},
              {"=", 10},
            };

            factorKinds = new List<string>() { "digit", "variable" };
            binaryKinds = new List<string>() { "sign" };
            rightAssociates = new List<string>() { "=" };

            index = 0;
            this.tokens = tokens;
            // add last letter symbol token
            this.tokens.Add(endOfToken);
        }

        private Token Current()
        {
            if (!tokens.Any())
                throw new ArgumentOutOfRangeException("no more token");
            return tokens[index];
        }
        private Token Next()
        {
            var token = Current();
            ++index;
            return token;
        }
        /// <summary>
        /// determine is token can place left or right edge of expression.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private Token Lead(Token token)
        {
            if (!factorKinds.Contains(token.Kind))
                throw new Exception("the token cannot place here.");
            return token;
        }
        /// <summary>
        /// asop token priority via op degree
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
        /// Asop Tokens comes to left/right of op token
        /// </summary>
        /// <remarks>
        /// in case of 4 + 3, this method will asop 4 for left, and 3 for right.
        /// </remarks>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        private Token Bind(Token left, Token op)
        {
            if (!binaryKinds.Contains(op.Kind))
                throw new Exception("The token cannot place here.");
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
            while (!Current().Kind.Equals("eot"))
            {
                block.Add(Express(0));
            }
            return block;
        }
    }
}
