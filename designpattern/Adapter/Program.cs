using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adapter
{
    class Program
    {
        static ITrigger[] triggers;

        // static constructor
        static Program()
        {
            //TODO: auto adapt via Assebly
            // Register adapters for injected OutputType
            triggers = new ITrigger[] {
                // 1st trigger
                new Trigger(new IAdapter[] { new FooAdapter(), new HogeAdapter() }),
                // 2nd trigger
                new Trigger(new IAdapter[] { new BarAdapter() }),
            };
        }

        static async Task Main(string[] args)
        {
            await ExecAsync<OutputItemDefault>();
            // OUTPUT: Id 0, Name default

            await ExecAsync<OutputItemFoo>();
            // OUTPUT: Id 2, Name Hooked foo!!

            await ExecAsync<OutputItemBar>();
            // OUTPUT: Id 51, Name Hooked bar!!

            await ExecAsync<OutputItemHoge>();
            // OUTPUT: Id 101, Name Hooked hoge!!(aaa)
        }

        static async Task ExecAsync<T>() where T : IOutputItem, new()
        {
            var defaultOutput = new T()
            {
                Id = 0,
                Name = "default",
            };
            Func<Input, Output> willDo = input => {
                Console.Write("No override, default willdo is running.\t");
                return new Output<T>(defaultOutput) as Output;
            };
            var result = await InvokeAsync<Input, T>(willDo);
            Console.WriteLine($"Id {result.Id}, Name {result.Name}");
        }

        static async Task<TOutput> InvokeAsync<TInput, TOutput>(Func<Input, Output> willDo)
        {
            // input
            var input = new Input("a", 1, triggers, willDo, typeof(TOutput));

            // get output
            var output = await InvokeTriggers(input) as Output<TOutput>;

            if (output == null)
                throw new ArgumentNullException($"could not get output");

            return await output.GetValue();
        }

        // nest multiple triggers
        static Task<Output> InvokeTriggers(Input input)
        {
            switch (input.Triggers.Length)
            {
                case 0:
                    return Task.FromResult(input.Continuation(input));
                case 1:
                    return InvokeTrigger1(input);
                case 2:
                    return InvokeTrigger2(input);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static Task<Output> InvokeTrigger1(Input input)
            => input.Triggers[0].Invoke(input,
                i => input.Continuation(i).WaitAsync());
        static Task<Output> InvokeTrigger2(Input input)
            => input.Triggers[0].Invoke(input,
                x1 => x1.Triggers[1].Invoke(x1,
                    i => input.Continuation(i).WaitAsync()));

        // Trigger
        public interface ITrigger
        {
            Task<Output> Invoke(Input item, Func<Input, Task<Output>> continuation);
        }

        public class Trigger : ITrigger
        {
            readonly Dictionary<Type, IAdapter> dictionary;
            public Trigger(IAdapter[] adapters)
            {
                // register adapters for OutputType
                dictionary = new Dictionary<Type, IAdapter>(adapters.Length);
                foreach (var adapter in adapters)
                {
                    dictionary.Add(adapter.OutputType, adapter);
                }
            }

            public async Task<Output> Invoke(Input item, Func<Input, Task<Output>> continuation)
            {
                if (dictionary.TryGetValue(item.OutputType, out var value))
                {
                    var output = value.OnGet(item);
                    return output;
                }
                else
                {
                    return await continuation(item);
                }
            }
        }

        // Adapter
        public interface IAdapter
        {
            Type OutputType { get; }
            Output OnGet(Input item);
        }

        public abstract class AdapterBase<T> : IAdapter
        {
            public Type OutputType => typeof(T);

            public Output OnGet(Input item)
            {
                return new Output<T>(CreateContent(item));
            }

            public abstract T CreateContent(Input item);
        }

        public class FooAdapter : AdapterBase<OutputItemFoo>
        {
            public override OutputItemFoo CreateContent(Input item)
            {
                return new OutputItemFoo()
                {
                    Name = "Hooked foo!!",
                    Id = item.ReferenceId + 1,
                };
            }
        }

        public class BarAdapter : AdapterBase<OutputItemBar>
        {
            public override OutputItemBar CreateContent(Input item)
            {
                return new OutputItemBar()
                {
                    Name = "Hooked bar!!",
                    Id = item.ReferenceId + 50,
                };
            }
        }

        public class HogeAdapter : AdapterBase<OutputItemHoge>
        {
            public override OutputItemHoge CreateContent(Input item)
            {
                return new OutputItemHoge()
                {
                    Name = "Hooked hoge!!" + $"({item.Name}{item.Name}{item.Name})",
                    Id = item.ReferenceId + 100,
                };
            }
        }

        // Input
        public class Input
        {
            public string Name { get; }
            public int ReferenceId { get; }
            public ITrigger[] Triggers { get; }
            public Type OutputType { get; }

            internal Func<Input, Output> Continuation { get; }

            public Input(string name, int referenceId, ITrigger[] triggers, Func<Input, Output> continuation, Type Outputtype)
            {
                Name = name;
                ReferenceId = referenceId;
                Triggers = triggers;
                Continuation = continuation;
                OutputType = Outputtype;
            }
        }

        // Output
        public abstract class Output
        {
            public abstract Task<Output> WaitAsync();
        }
        public class Output<T> : Output
        {
            readonly T item;
            public Output(T item)
            {
                this.item = item;
            }

            Task<T> Value => Task.FromResult(item);
            public Task<T> GetValue() => Value;

            public override async Task<Output> WaitAsync()
            {
                await Value;
                return this;
            }
        }

        public interface IOutputItem
        {
            string Name { get; set; }
            int Id { get; set; }
        }

        public class OutputItemDefault : IOutputItem
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public class OutputItemFoo : IOutputItem
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public class OutputItemBar : IOutputItem
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public class OutputItemHoge : IOutputItem
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }
    }
}
