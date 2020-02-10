using System;
using System.Linq;

namespace SubnetUtils
{
    public struct Subnet
    {
        public CidrBlock Address { get; set; }
        public byte Mask { get; set; }
        public CidrBlock MaskCidr { get; set; }
        public CidrBlock NetworkBeginAddress { get; set; }
        public CidrBlock NetworkEndAddress { get; set; }

        // 10.1.100.1/24
        // 10.1.100.1/255.255.255.0
        public Subnet(string subnetWithMask)
        {
            var split = subnetWithMask.Split("/");
            var address = new CidrBlock(split[0]);
            var mask = new CidrBlock(split[1]);

            var maskByte = CidrBlock.FromMaskBytes(mask.AddressBytes);
            (Address, Mask) = (address, maskByte);
            MaskCidr = mask;
            var networkBeginAddress = GetNetworkBeginAddress(Address.AddressBytes, MaskCidr.AddressBytes);
            NetworkBeginAddress = new CidrBlock(networkBeginAddress[0], networkBeginAddress[1], networkBeginAddress[2], networkBeginAddress[3]);
            var networkEndAddress = GetNetworkEndAddress(Address.AddressBytes, MaskCidr.AddressBytes);
            NetworkEndAddress = new CidrBlock(networkEndAddress[0], networkEndAddress[1], networkEndAddress[2], networkEndAddress[3]);
        }

        // 10.1.100.1/24
        public Subnet(CidrBlock address, byte mask)
        {
            (Address, Mask) = (address, mask);
            var maskBytes = CidrBlock.GetMaskBytes(Mask);
            MaskCidr = new CidrBlock(maskBytes[0], maskBytes[1], maskBytes[2], maskBytes[3]);
            var networkBeginAddress = GetNetworkBeginAddress(Address.AddressBytes, MaskCidr.AddressBytes);
            NetworkBeginAddress = new CidrBlock(networkBeginAddress[0], networkBeginAddress[1], networkBeginAddress[2], networkBeginAddress[3]);
            var networkEndAddress = GetNetworkEndAddress(Address.AddressBytes, MaskCidr.AddressBytes);
            NetworkEndAddress = new CidrBlock(networkEndAddress[0], networkEndAddress[1], networkEndAddress[2], networkEndAddress[3]);
        }
        // 10.1.100.1/255.255.255.0
        public Subnet(CidrBlock address, CidrBlock mask)
        {
            var maskByte = CidrBlock.FromMaskBytes(mask.AddressBytes);
            (Address, Mask) = (address, maskByte);
            MaskCidr = mask;
            var networkBeginAddress = GetNetworkBeginAddress(Address.AddressBytes, MaskCidr.AddressBytes);
            NetworkBeginAddress = new CidrBlock(networkBeginAddress[0], networkBeginAddress[1], networkBeginAddress[2], networkBeginAddress[3]);
            var networkEndAddress = GetNetworkEndAddress(Address.AddressBytes, MaskCidr.AddressBytes);
            NetworkEndAddress = new CidrBlock(networkEndAddress[0], networkEndAddress[1], networkEndAddress[2], networkEndAddress[3]);
        }

        public override string ToString()
        {
            return $"{Address}/{Mask}";
        }

        public static byte[] GetNetworkBeginAddress(byte[] ipBytes, byte[] maskBytes)
        {
            var result = new byte[4];
            for (int i = 0; i < ipBytes.Length; i++)
            {
                result[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }
            return result;
        }
        public static byte[] GetNetworkEndAddress(byte[] ipBytes, byte[] maskBytes)
        {
            var result = new byte[4];
            for (int i = 0; i < ipBytes.Length; i++)
            {
                result[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
            }
            return result;
        }
    }
}
