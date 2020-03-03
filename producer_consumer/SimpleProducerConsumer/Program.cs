using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleProducerConsumer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            await Sample();
        }

        private readonly ProducerConsumer<IData> pc = new ProducerConsumer<IData>();

        public async Task WritehenRead()
        {
            for (int i = 0; i < 10_000_000; i++)
            {
                pc.Write(new Data { Id = i });
                await pc.ReadAsync();
            }
        }
        public async Task ReadThenWrite()
        {
            for (int i = 0; i < 1000; i++)
            {
                var t = pc.ReadAsync();
                pc.Write(new Data { Id = i });
                await t;
            }
        }

        static async Task Sample()
        {
            var pc = new ProducerConsumer<Data>();
            // consume background
            var t = Task.Run(async () =>
            {
                while (true) { await pc.ReadAsync(); }
            });

            // produce
            pc.Write(new Data { Id = 1 });
            await Task.Delay(TimeSpan.FromSeconds(1));
            pc.Write(new Data { Id = 2 });
            await Task.Delay(TimeSpan.FromSeconds(1));
            pc.Write(new Data { Id = 5 });
            await Task.Delay(TimeSpan.FromSeconds(1));
            pc.Write(new Data { Id = 4 });
            await Task.Delay(TimeSpan.FromSeconds(1));
            pc.Write(new Data { Id = 10 });
            var ts = Enumerable.Range(0, 10).Select(x => Task.Run(() => pc.Write(new Data { Id = x })));
            await Task.WhenAll(ts);
        }
    }

    public interface IData
    {
        int Id { get;set; }
    }
    public struct Data : IData
    {
        public int Id { get; set; }
    }

    public class ProducerConsumer<T> where T : IData
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        /// <summary>
        /// producer
        /// </summary>
        /// <param name="value"></param>
        public void Write(T value)
        {
            _queue.Enqueue(value); // store the data
            _semaphore.Release(); // notify any consumers that more data is available
        }

        // consumer
        public async ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false); // wait
            var gotOne = _queue.TryDequeue(out T item); // recieve the data
            Console.WriteLine($"{DateTime.Now} ({gotOne}): {item.Id}");
            return item;
        }
    }
}
