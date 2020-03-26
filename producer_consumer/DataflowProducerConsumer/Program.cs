using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataflowProducerConsumer
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
        private readonly BufferBlock<IData> bufferBlock = new BufferBlock<IData>();

        public async Task WriteThenRead()
        {
            for (var i = 0; i < 1000; i++)
            {
                bufferBlock.Post(new Data { Id = i });
                var item = await bufferBlock.ReceiveAsync();
                Console.WriteLine($"{DateTime.Now}: {item.Id}");
            }
        }

        public async Task ReadThenWrite()
        {
            for (var i = 0; i < 1000; i++)
            {
                var vt = bufferBlock.ReceiveAsync();
                bufferBlock.Post(new Data { Id = i });
                var item = await vt;
                Console.WriteLine($"{DateTime.Now}: {item.Id}");
            }
        }
    }
}
