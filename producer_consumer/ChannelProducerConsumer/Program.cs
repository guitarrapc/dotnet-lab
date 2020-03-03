using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelProducerConsumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var pc = new ProducerConsumer();
            await pc.ReadThenWrite();
            await pc.WriteThenRead();
        }
    }

    public interface IData
    {
        int Id { get; set; }
    }
    public struct Data : IData
    {
        public int Id { get; set; }
    }

    public class ProducerConsumer
    {
        private readonly Channel<IData> channel = Channel.CreateUnbounded<IData>();

        public async Task WriteThenRead()
        {
            var writer = channel.Writer;
            var reader = channel.Reader;
            for (var i = 0; i < 1000; i++)
            {
                writer.TryWrite(new Data { Id = i });
                var item = await reader.ReadAsync();
                Console.WriteLine($"{DateTime.Now}: {item.Id}");
            }
        }

        public async Task ReadThenWrite()
        {
            var writer = channel.Writer;
            var reader = channel.Reader;
            for (var i = 0; i < 1000; i++)
            {
                var vt = reader.ReadAsync();
                writer.TryWrite(new Data { Id = i });
                var item = await vt;
                Console.WriteLine($"{DateTime.Now}: {item.Id}");
            }
        }
    }
}
