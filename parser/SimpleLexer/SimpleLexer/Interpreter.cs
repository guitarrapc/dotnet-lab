using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLexer
{
    public class Interpreter
    {
        public Dictionary<string, FunctionBase> functions { get; set; }
        public Dictionary<string, Variable> variables { get; set; }
        private List<Token> body;

        public Interpreter Set(List<Token> body)
        {
            functions = new Dictionary<string, FunctionBase>();
            FunctionBase f = new Println();
            functions.Add(f.Name, f);
            variables = new Dictionary<string, Variable>();
            this.body = body;
            return this;
        }

        public Dictionary<string, Variable> Execute()
        {
            Body(body);
            return variables;
        }
        private void Body(List<Token> body)
        {
            foreach (var expr in body)
            {
                Express(expr);
            }
        }
        private object Express(Token expr)
        {
            switch (expr.Kind)
            {
                case "digit":
                    return Digit(expr);
                case "ident":
                    return Ident(expr);
                case "function":
                    return Function(expr);
                case "parenthesis":
                    return Invoke(expr);
                case "sign" when expr.Value.Equals("="):
                    return Assign(expr);
                case "unary":
                    return UnaryCalculate(expr);
                case "sign":
                    return Calculate(expr);
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(expr.Kind)}, {expr.Kind} is out of scope.");
            }
        }
        private int Digit(Token token)
        {
            return int.Parse(token.Value);
        }
        // set variable to dictionary and get name
        private object Ident(Token token)
        {
            var name = token.Value;
            if (functions.ContainsKey(name))
            {
                return functions[name];
            }
            if (variables.ContainsKey(name))
            {
                return variables[name];
            }
            var v = new Variable()
            {
                Name = name,
                Value = 0,
            };
            variables[name] = v;
            return v;
        }
        // assign to variable
        private Variable Assign(Token expr)
        {
            var variable = GetVariable(Express(expr.Left));
            var value = GetValue(Express(expr.Right));
            variable.Value = value;
            variables[variable.Name] = variable;
            return variable;
        }
        // get variable
        private Variable GetVariable(object value)
        {
            if (value is Variable v)
                return v;
            throw new Exception($"Left value error. {value}");
        }
        // get value
        private int GetValue(object value)
        {
            if (value is int i)
                return i;
            if (value is Variable v)
                return v.Value;
            throw new Exception("Right value error");
        }
        private object Invoke(Token expr)
        {
            var f = Function(Express(expr.Left));
            var values = new List<object>();
            foreach (var param in expr.Params)
            {
                values.Add(GetValue(Express(param)));
            }
            return f.Invoke(values);
        }
        private FunctionBase Function(object value)
        {
            if (value is FunctionBase f)
                return f;
            throw new Exception("not a function");
        }
        private object Function(Token token)
        {
            var name = token.Ident.Value;
            if (functions.ContainsKey(name))
                throw new ArgumentException("Name was used");
            if (variables.ContainsKey(name))
                throw new ArgumentException("Name was used");
            var paramCheckList = new List<string>();
            foreach (var item in token.Params)
            {
                var param = item.Value;
                if (paramCheckList.Contains(param))
                    throw new Exception("Parameter name was already used");
                paramCheckList.Add(param);
            }
            var func = new DynamicFunction()
            {
                Context = this,
                Name = name,
                Params = token.Params,
                Block = token.Block,
            };
            functions[name] = func;
            return null;
        }

        private int Calculate(Token expr)
        {
            var left = GetValue(Express(expr.Left));
            var right = GetValue(Express(expr.Right));
            switch (expr.Value)
            {
                case "+":
                    return left + right;
                case "-":
                    return left - right;
                case "*":
                    return left * right;
                case "/":
                    return left / right;
                default:
                    throw new Exception("Unknown sign for calulate");
            }
        }
        private int UnaryCalculate(Token expr)
        {
            var left = GetValue(Express(expr.Left));
            switch (expr.Value)
            {
                case "+":
                    return left;
                case "-":
                    return -left;
                default:
                    throw new Exception("Unknown sign for unary calculation");
            }
        }

        public class Variable
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public override string ToString()
            {
                return Name + " " + Value.ToString();
            }
        }

        public abstract class FunctionBase
        {
            public string Name { get; set; }
            public abstract object Invoke(List<object> args);
        }
        public class Println : FunctionBase
        {
            public Println()
            {
                Name = "println";
            }
            public override object Invoke(List<object> args)
            {
                foreach (var arg in args)
                {
                    Console.WriteLine(arg);
                }
                return null;
            }
        }
        public class DynamicFunction : FunctionBase
        {
            public Interpreter Context { get; set; }
            public List<Token> Params { get; set; }
            public List<Token> Block { get; set; }

            public override object Invoke(List<object> args)
            {
                for (var i = 0; i < Params.Count(); i++)
                {
                    var param = Params[i];
                    var variable = Context.GetVariable(Context.Ident(param));
                    if (i < args.Count())
                    {
                        variable.Value = Context.GetValue(args[i]);
                    }
                    else
                    {
                        variable.Value = 0;
                    }
                }
                Context.Body(Block);
                return null;
            }
        }
    }
}
