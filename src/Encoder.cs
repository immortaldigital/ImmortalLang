using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ImmortalLang
{
	public static class Encoder
    {
		//uLEB128 is a way of encoding an arbitrary length of bytes in a minimal space
		/* You take the set of bits, extend it to a multiple of 7, then break it into 7-bit lengths  
		 * Turn each of those 7-bit lengths into a byte by adding a 1 to all and a 0 to the final byte
		*/
		
		public static string hexString(List<byte> bits)
		{
			StringBuilder hex = new StringBuilder(bits.Count * 3);
			
			foreach (byte b in bits)
			{
				hex.AppendFormat("{0:x2} ", b);
			}
			
			if(hex.Length>0)
			{
				hex.Remove(hex.Length - 1, 1); //remove space at end
			}
			
			return hex.ToString();
		}
		
        public static List<byte> unsignedLEB128(int num)
        {
            List<byte> buffer = new List<byte>();

            do
            {
                byte b = (byte)(num & 0x7f);
                num >>= 7;
                if (num != 0)
                {
                    b |= 0x80;
                }
                buffer.Add(b);
            } while (num != 0);

            return buffer;
        }

        //encodeString simply turns the string into a byte array with the first byte being the length of the array
        //I suspect the byte length should be LEB encoded incase the string is longer than 256 characters
        //TODO check the Webassembly specification and verify if it should be LEB128 or if the maximum string length is just 256
        public static List<byte> encodeString(string s)
        {
            List<byte> bits = new List<byte>();
            bits.AddRange(Encoding.ASCII.GetBytes(s));
            bits.Insert(0, (byte)s.Length);
            return bits;
        }

        // takes a byte array and prepends the length of the array (LEB128 encoded)
        public static List<byte> wrap(List<byte> bits)
        {
            var newBits = new List<byte>(bits.Select(x => x));
            newBits.InsertRange(0, unsignedLEB128(newBits.Count));
            return newBits;
        }
        
        // takes multiple byte arrays, concatenates them and prepends the number of arrays        
        public static List<byte> wrapLists(params List<byte>[] bits)
		{
			List<byte> list = new List<byte>();
			
			list.AddRange(unsignedLEB128(bits.Length));
			
			for(int i=0; i<bits.Length; i++)
			{
				var newBits = new List<byte>(bits[i].Select(x => x));
				list.AddRange(newBits);
			}
			
			return list;
		}
        
        public static List<byte> concatentate(params IEnumerable<byte>[] bits)
        {
        	List<byte> list = new List<byte>();
			
			for(int i=0; i<bits.Length; i++)
			{
				var newBits = new List<byte>(bits[i].Select(x => x));
				list.AddRange(newBits);
			}
			
			return list;
        }

        public static List<byte> createSection(byte section, List<byte> bits)
        {
            bits = wrap(bits);
            bits.Insert(0, section);
            return bits;
        }
    }
	
	// below is the incomplete set of opcodes/headers/types etc

    public static class Headers
    {
        public static readonly byte[] magicModule = new byte[] { 0x00, 0x61, 0x73, 0x6d };
        public static readonly byte[] moduleVersion = new byte[] { 0x01, 0x00, 0x00, 0x00 };
    }

    public static class Opcodes
    {
        public static readonly byte get_local = 0x20;
        public static readonly byte f32_add = 0x92;
        public static readonly byte f32_sub = 0x93;	
        public static readonly byte i32_add = 0x6a;
        
        public static readonly byte i32_const = 0x41;
        public static readonly byte i64_const = 0x42;
        public static readonly byte f32_const = 0x43;
		public static readonly byte f64_const = 0x44;
		
		public static readonly byte call = 0x10;
		public static readonly byte function_end = 0x0b;
    }

    public static class Types
    {
        public static readonly byte f32 = 0x7d;
        public static readonly byte i32 = 0x7f;
        public static readonly byte FUNC = 0x60;
        public static readonly byte EMPTY = 0x00;
    }

    public static class ExportType
    {
        public static readonly byte FUNC = 0x00;
        public static readonly byte TABLE = 0x01;
        public static readonly byte MEM = 0x02;
        public static readonly byte GLOBAL = 0x03;
    }

    public static class Section
    {
        public static readonly byte CUSTOM = 0x00;
        public static readonly byte TYPE = 0x01;
        public static readonly byte IMPORT = 0x02;
        public static readonly byte FUNC = 0x03;
        public static readonly byte TABLE = 0x04;
        public static readonly byte MEMORY = 0x05;
        public static readonly byte GLOBAL = 0x06;
        public static readonly byte EXPORT = 0x07;
        public static readonly byte START = 0x08;
        public static readonly byte ELEMENT = 0x09;
        public static readonly byte CODE = 0x0a;
        public static readonly byte DATA = 0x0b;
    }
}
