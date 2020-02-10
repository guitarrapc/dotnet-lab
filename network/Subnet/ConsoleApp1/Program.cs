using System;
using SubnetUtils;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            StringInput("10.1.100.1/24");
            Console.WriteLine();
            StringInput("10.1.100.1/255.255.255.0");
            Console.WriteLine();
            CidrAndByteMaskInput(new CidrBlock(10, 1, 100, 1), 24);
            Console.WriteLine();
            CidrAndByteMaskInput(new CidrBlock(10, 1, 100, 1), new CidrBlock(255, 255, 255, 0));
        }

        static void StringInput(string input)
        {
            Console.WriteLine($"Input: {input}");
            var subnet = new Subnet(input);
            Execute(subnet);
        }
        static void CidrAndByteMaskInput(CidrBlock address, byte mask)
        {
            Console.WriteLine($"Input: {address} {mask}");
            var subnet = new Subnet(address, mask);
            Execute(subnet);
        }
        static void CidrAndByteMaskInput(CidrBlock address, CidrBlock mask)
        {
            Console.WriteLine($"Input: {address} {mask}");
            var subnet = new Subnet(address, mask);
            Execute(subnet);
        }

        static void Execute(Subnet subnet)
        {
            Console.WriteLine($"Subnet: {subnet}");
            Console.WriteLine($"Address: {subnet.Address}");
            Console.WriteLine($"MaskCidr: {subnet.MaskCidr}");
            Console.Write("Address Bit : ");
            foreach (var x in subnet.Address.ToBit())
                Console.Write(x);
            Console.WriteLine();

            Console.WriteLine($"Address ToUint32: {subnet.Address.ToUnit32()}");
            Console.WriteLine($"Subnet  ToUint32: {subnet.MaskCidr.ToUnit32()}");

            var maskBytes = CidrBlock.GetMaskBytes(24);
            Console.WriteLine($"MaskBytes: {maskBytes[0]}.{maskBytes[1]}.{maskBytes[2]}.{maskBytes[3]}");
            Console.WriteLine($"Mask: {CidrBlock.FromMaskBytes(maskBytes)}");
        }
    }
}
