using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adapter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ExecAsync<OutputItemFoo>();
            // OUTPUT: Id 2, Name Hooked foo!!

            await ExecAsync<OutputItemBar>();
            // OUTPUT: Id 0, Name default

            await ExecAsync<OutputItemHoge>();
            // OUTPUT: Id 101, Name Hooked hoge!!(aaa)
        }

        static async Task ExecAsync<T>() where T : IOutput, new()
        {
            var defaultOutput = new T()
            {
                Id = 0,
                Name = "default",
            };
            Func<Input, Task<Output>> willDo = input => Task.FromResult(new Output<T>(defaultOutput) as Output);
            var result = await InvokeAsync<Input, T>(willDo);
            Console.WriteLine($"Id {result.Id}, Name {result.Name}");
        }

        static async Task<TOutput> InvokeAsync<TInput, TOutput>(Func<Input, Task<Output>> willDo)
        {
            // input
            var input = new Input("a", 1, typeof(TOutput));

            //TODO: auto adapt via Assebly
            // Register adapters for injected OutputType
            var trigger = new Trigger(new IAdapter[] { new FooAdapter(), new HogeAdapter() });

            // get output
            var output = await trigger.Invoke(input, willDo) as Output<TOutput>;
            if (output == null)
            {
                throw new ArgumentNullException($"could not get output");
            }
            return await output.GetValue();
        }
    }

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

        public Task<Output> Invoke(Input item, Func<Input, Task<Output>> continuation)
        {
            if (dictionary.TryGetValue(item.OutputType, out var value))
            {
                var output = value.OnGet(item);
                return Task.FromResult(output);
            }
            else
            {
                return continuation(item);
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
        public Type OutputType { get; }

        public Input(string name, int referenceId, Type Outputtype)
        {
            Name = name;
            ReferenceId = referenceId;
            OutputType = Outputtype;
        }
    }

    // Output
    public abstract class Output
    {
        public abstract Task WaitAsync();
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

        public override async Task WaitAsync() => await Value;
    }

    public interface IOutput
    {
        string Name { get; set; }
        int Id { get; set; }
    }

    public class OutputItemFoo : IOutput
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class OutputItemBar : IOutput
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class OutputItemHoge : IOutput
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
