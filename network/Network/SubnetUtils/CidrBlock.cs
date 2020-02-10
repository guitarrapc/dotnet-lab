using System;
using System.Buffers;
using System.Collections;
using System.Linq;
using System.Text;

namespace SubnetUtils
{
    public struct CidrBlock
    {
        public byte[] AddressBytes;

        public byte VpcCidr1;
        public byte VpcCidr2;
        public byte VpcCidr3;
        public byte VpcCidr4;

        public byte[] VpcCidrBytes1;
        public byte[] VpcCidrBytes2;
        public byte[] VpcCidrBytes3;
        public byte[] VpcCidrBytes4;

        public BitArray VpcCidrBitArray1;
        public BitArray VpcCidrBitArray2;
        public BitArray VpcCidrBitArray3;
        public BitArray VpcCidrBitArray4;

        // 24
        // 192.168.1.10
        public CidrBlock(string cidr)
        {
            AddressBytes = new byte[4];
            (VpcCidr1, VpcCidr2, VpcCidr3, VpcCidr4) = (AddressBytes[0], AddressBytes[1], AddressBytes[2], AddressBytes[3]);
            (VpcCidrBitArray1, VpcCidrBitArray2, VpcCidrBitArray3, VpcCidrBitArray4) = (ToBitArray(VpcCidr1), ToBitArray(VpcCidr2), ToBitArray(VpcCidr3), ToBitArray(VpcCidr4));
            (VpcCidrBytes1, VpcCidrBytes2, VpcCidrBytes3, VpcCidrBytes4) = (ToByteArray(VpcCidrBitArray1), ToByteArray(VpcCidrBitArray2), ToByteArray(VpcCidrBitArray3), ToByteArray(VpcCidrBitArray4));

            var split = cidr.Trim().Split(".");
            if (split.Length == 1 && byte.TryParse(split[0], out var mask))
            {
                var maskBytes = GetMaskBytes(mask);
                AddressBytes = maskBytes;
                (VpcCidr1, VpcCidr2, VpcCidr3, VpcCidr4) = (AddressBytes[0], AddressBytes[1], AddressBytes[2], AddressBytes[3]);
                (VpcCidrBitArray1, VpcCidrBitArray2, VpcCidrBitArray3, VpcCidrBitArray4) = (ToBitArray(VpcCidr1), ToBitArray(VpcCidr2), ToBitArray(VpcCidr3), ToBitArray(VpcCidr4));
                (VpcCidrBytes1, VpcCidrBytes2, VpcCidrBytes3, VpcCidrBytes4) = (ToByteArray(VpcCidrBitArray1), ToByteArray(VpcCidrBitArray2), ToByteArray(VpcCidrBitArray3), ToByteArray(VpcCidrBitArray4));
            }
            else if (split.Length == 4)
            {
                var maskBytes = new byte[4];
                for (var i = 0; i < split.Length; i++)
                {
                    if (!byte.TryParse(split[i], out var address))
                    {
                        throw new ArgumentOutOfRangeException($"{split[i]} could not parse. Make sure value is byte.");
                    }
                    else
                    {
                        maskBytes[i] = address;
                    }
                }
                AddressBytes = maskBytes;
                (VpcCidr1, VpcCidr2, VpcCidr3, VpcCidr4) = (AddressBytes[0], AddressBytes[1], AddressBytes[2], AddressBytes[3]);
                (VpcCidrBitArray1, VpcCidrBitArray2, VpcCidrBitArray3, VpcCidrBitArray4) = (ToBitArray(VpcCidr1), ToBitArray(VpcCidr2), ToBitArray(VpcCidr3), ToBitArray(VpcCidr4));
                (VpcCidrBytes1, VpcCidrBytes2, VpcCidrBytes3, VpcCidrBytes4) = (ToByteArray(VpcCidrBitArray1), ToByteArray(VpcCidrBitArray2), ToByteArray(VpcCidrBitArray3), ToByteArray(VpcCidrBitArray4));

            }
            else
            {
                throw new ArgumentOutOfRangeException($"{cidr} was not single-byte or 4 octed byte.");
            }
        }
        // 192, 168, 100, 1
        public CidrBlock(byte cidr1, byte cidr2, byte cidr3, byte cidr4)
        {
            AddressBytes = new byte[4] { cidr1, cidr2, cidr3, cidr4 };
            (VpcCidr1, VpcCidr2, VpcCidr3, VpcCidr4) = (cidr1, cidr2, cidr3, cidr4);
            (VpcCidrBitArray1, VpcCidrBitArray2, VpcCidrBitArray3, VpcCidrBitArray4) = (ToBitArray(VpcCidr1), ToBitArray(VpcCidr2), ToBitArray(VpcCidr3), ToBitArray(VpcCidr4));
            (VpcCidrBytes1, VpcCidrBytes2, VpcCidrBytes3, VpcCidrBytes4) = (ToByteArray(VpcCidrBitArray1), ToByteArray(VpcCidrBitArray2), ToByteArray(VpcCidrBitArray3), ToByteArray(VpcCidrBitArray4));
        }

        public byte[] ToBit()
        {
            var result = ArrayPool<byte>.Shared.Rent(32);
            try
            {
                var resultIndex = 0;
                for (var i = 0; i < VpcCidrBytes1.Length; i++)
                {
                    result[resultIndex] = VpcCidrBytes1[i];
                    resultIndex++;
                }
                for (var i = 0; i < VpcCidrBytes2.Length; i++)
                {
                    result[resultIndex] = VpcCidrBytes2[i];
                    resultIndex++;
                }
                for (var i = 0; i < VpcCidrBytes3.Length; i++)
                {
                    result[resultIndex] = VpcCidrBytes3[i];
                    resultIndex++;
                }
                for (var i = 0; i < VpcCidrBytes4.Length; i++)
                {
                    result[resultIndex] = VpcCidrBytes4[i];
                    resultIndex++;
                }
                return result;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(result);
            }
        }

        public CidrBlock Add(CidrBlock add)
        {
            return new CidrBlock((byte)(this.VpcCidr1 + add.VpcCidr1), (byte)(this.VpcCidr2 + add.VpcCidr2), (byte)(this.VpcCidr3 + add.VpcCidr3), (byte)(this.VpcCidr4 + add.VpcCidr4));
        }

        public override string ToString()
        {
            return $"{VpcCidr1}.{VpcCidr2}.{VpcCidr3}.{VpcCidr4}";
        }
        public UInt32 ToUnit32()
        {
            return BitConverter.ToUInt32(AddressBytes.Reverse().ToArray());
        }

        public string ToBitString()
        {
            var builder = new StringBuilder(32);
            var resultIndex = 0;
            for (var i = 0; i < VpcCidrBytes1.Length; i++)
            {
                builder.Append(VpcCidrBytes1[i]);
                resultIndex++;
            }
            for (var i = 0; i < VpcCidrBytes2.Length; i++)
            {
                builder.Append(VpcCidrBytes2[i]);
                resultIndex++;
            }
            for (var i = 0; i < VpcCidrBytes3.Length; i++)
            {
                builder.Append(VpcCidrBytes3[i]);
                resultIndex++;
            }
            for (var i = 0; i < VpcCidrBytes4.Length; i++)
            {
                builder.Append(VpcCidrBytes4[i]);
                resultIndex++;
            }
            return builder.ToString();
        }

        private static BitArray ToBitArray(byte @byte)
        {
            var b = new[] { @byte };
            var bitArray = new BitArray(b);
            return bitArray;
        }
        private static byte[] ToByteArray(BitArray bitArray)
        {
            var result = new byte[8];
            var index = bitArray.Length;
            var j = index - 1;
            for (var i = 0; i < bitArray.Length; i++)
            {
                result[j] = bitArray[i] ? (byte)1 : (byte)0;
                j--;
            }
            return result;
        }

        // var sub = 16.Dump("sub");
        // var mask = (~(uint.MaxValue >> sub).Dump("shift")).Dump("mask");
        // var maskBytes = BitConverter.GetBytes(mask).Reverse().ToArray().Dump("maskbytes");
        // var revMask = BitConverter.ToUInt32(maskBytes.Reverse().ToArray()).Dump("revmask");
        // var Shift = (revMask ^ uint.MaxValue).Dump("revshift");
        // var revSub = (32 - (byte)Math.Log(Shift + 1, 2)).Dump("revSub");
        public static byte[] GetMaskBytes(byte maskInt)
        {
            var shift = (uint.MaxValue >> maskInt);
            var sub = ~shift;
            var maskBytes = BitConverter.GetBytes(sub).Reverse().ToArray();
            return maskBytes;
        }
        public static byte FromMaskBytes(byte[] maskBytes)
        {
            var sub = BitConverter.ToUInt32(maskBytes.Reverse().ToArray());
            var xor = (sub ^ uint.MaxValue);
            var mask = (byte)(32 - (byte)Math.Log(xor + 1, 2));
            return mask;
        }
    }
}
