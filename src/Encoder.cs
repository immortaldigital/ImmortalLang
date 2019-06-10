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
		
        public static List<byte> uLEB128(int num)
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
            newBits.InsertRange(0, uLEB128(newBits.Count));
            return newBits;
        }
        
        // takes multiple byte arrays, concatenates them and prepends the number of arrays        
        public static List<byte> wrapLists(params List<byte>[] bits)
		{
			List<byte> list = new List<byte>();
			
			list.AddRange(uLEB128(bits.Length));
			
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
        public static readonly byte[] magicModule = { 0x00, 0x61, 0x73, 0x6d };
        public static readonly byte[] moduleVersion = { 0x01, 0x00, 0x00, 0x00 };
    }

    public static class Op
    {
    	public static readonly byte[] block = {0x02, 0x40};
    	public static readonly byte[] loop = {0x03, 0x40};
    	public static readonly byte[] br = {0x0c};
    	public static readonly byte[] br_if = {0x0d};
    	public static readonly byte[] br_table = {0x0e};
    	public static readonly byte[] end = {0x0b};
    	public static readonly byte[] _return = {0x0f}; //same as _br to outermost region
    	public static readonly byte[] unreachable = {0x00}; //code that should not be executed, no idea what it's used for
    	
    	public static readonly byte[] nop = {0x01};
    	public static readonly byte[] drop = {0x1a}; //nop, but drops a value from the stack
    	
    	public static readonly byte[] i32_const = {0x41};
        public static readonly byte[] i64_const = {0x42};
        public static readonly byte[] f32_const = {0x43};
		public static readonly byte[] f64_const = {0x44};
		
		public static readonly byte[] get_local = {0x20};
		public static readonly byte[] set_local = {0x21};
		public static readonly byte[] tee_local = {0x22}; //same as setlocal but also returns the param to the stack
		public static readonly byte[] get_global = {0x23};
		public static readonly byte[] set_global = {0x24};
		
		public static readonly byte[] _select = {0x1b}; //returns first operand if condition true, second if false
		
		public static readonly byte[] call = {0x10};
		public static readonly byte[] call_indirect = {0x11};
    	
    	public static readonly byte[] _if = {0x04, 0x40};
    	public static readonly byte[] _else = {0x05};
    	
    	public static readonly byte[] i32_add = {0x6a};
    	public static readonly byte[] i32_sub = {0x6b};
    	public static readonly byte[] i32_mul = {0x6c};
    	public static readonly byte[] i32_div_s = {0x6d};
    	public static readonly byte[] i32_rem_s = {0x6f}; //remainder
    	public static readonly byte[] i32_rem_u = {0x70};
    	public static readonly byte[] i32_and = {0x71};
    	public static readonly byte[] i32_or = {0x72};
    	public static readonly byte[] i32_xor = {0x73};
    	public static readonly byte[] i32_shl = {0x74}; //shift left
    	public static readonly byte[] i32_shr_s = {0x75}; //shift right 
    	public static readonly byte[] i32_shr_u = {0x76};
    	public static readonly byte[] i32_rotl = {0x77}; //rotate left
    	public static readonly byte[] i32_rotr = {0x78};
    	public static readonly byte[] i32_clz = {0x67}; //count leading zeroes
    	public static readonly byte[] i32_ctz = {0x68}; //count trailing zeroes
    	public static readonly byte[] i32_popcnt = {0x69}; //pop count (number of 1's in byte[]
    	public static readonly byte[] i32_eqz = {0x45}; //if equal to zero
    	
    	public static readonly byte[] i64_add = {0x7c};
    	public static readonly byte[] i64_sub = {0x7d};
    	public static readonly byte[] i64_mul = {0x7e};
    	public static readonly byte[] i64_div_s = {0x7f};
    	public static readonly byte[] i64_div_u = {0x80};
    	public static readonly byte[] i64_rem_s = {0x81};
    	public static readonly byte[] i64_rem_u = {0x82};
    	public static readonly byte[] i64_and = {0x83};
    	public static readonly byte[] i64_or = {0x84};
    	public static readonly byte[] i64_xor = {0x85};
    	public static readonly byte[] i64_shl = {0x86};
    	public static readonly byte[] i64_shr_s = {0x87};
    	public static readonly byte[] i64_shr_u = {0x88};
    	public static readonly byte[] i64_rotl = {0x89};
    	public static readonly byte[] i64_rotr = {0x8a};
    	public static readonly byte[] i64_clz = {0x79};
    	public static readonly byte[] i64_ctz = {0x7a};
    	public static readonly byte[] i64_popcnt = {0x7b};
    	public static readonly byte[] i64_eqz = {0x50};
    	
    	
    	//following were copy/pasted/modified hence the formatting weirdness
    	public static readonly byte[]  f32_add = {0x92}; // (f32, f32) : (f32) F
		public static readonly byte[]  f64_add = {0xa0}; // (f64, f64) : (f64) F
		
		public static readonly byte[]  f32_sub = {0x93}; // (f32, f32) : (f32) F
		public static readonly byte[]  f64_sub = {0xa1}; // (f64, f64) : (f64) F
		
		public static readonly byte[]  f32_mul = {0x94}; // (f32, f32) : (f32) F
		public static readonly byte[]  f64_mul = {0xa2}; // (f64, f64) : (f64) F
		
		public static readonly byte[]  f32_div = {0x95}; // (f32, f32) : (f32) F
		public static readonly byte[]  f64_div = {0xa3}; // (f64, f64) : (f64) F
		
		public static readonly byte[]  f32_sqrt = {0x91}; // (f32) : (f32) F
		public static readonly byte[]  f64_sqrt = {0x9f}; // (f64) : (f64) F
		
		public static readonly byte[]  f32_min = {0x96}; // (f32, f32) : (f32) F
		public static readonly byte[]  f64_min = {0xa4}; // (f64, f64) : (f64) F
		    
		public static readonly byte[]  f32_max = {0x97}; // (f32, f32) : (f32) F
		public static readonly byte[]  f64_max = {0xa5}; // (f64, f64) : (f64) F
		    
		public static readonly byte[]  f32_ceil = {0x8d}; // (f32) : (f32) F
		public static readonly byte[]  f64_ceil = {0x9b}; // (f64) : (f64) F
		    
		public static readonly byte[]  f32_floor = {0x8e}; // (f32) : (f32) F
		public static readonly byte[]  f64_floor = {0x9c}; // (f64) : (f64) F
		    
		public static readonly byte[]  f32_trunc = {0x8f}; // (f32) : (f32) F
		public static readonly byte[]  f64_trunc = {0x9d}; // (f64) : (f64) F
		
		public static readonly byte[]  f32_nearest = {0x90}; // (f32) : (f32) F
		public static readonly byte[]  f64_nearest = {0x9e}; // (f64) : (f64) F
		    
		public static readonly byte[]  f32_abs = {0x8b}; // (f32) : (f32) E
		public static readonly byte[]  f64_abs = {0x99}; // (f64) : (f64) E
		    
		public static readonly byte[]  f32_neg = {0x8c}; // (f32) : (f32) E
		public static readonly byte[]  f64_neg = {0x9a}; // (f64) : (f64) E
		
		public static readonly byte[]  f32_copysign = {0x98}; // (f32, f32) : (f32) E
		public static readonly byte[]  f64_copysign = {0xa6}; // (f64, f64) : (f64) E
		
		public static readonly byte[]  i32_eq = {0x46}; // (i32, i32) : (i32) C, G
		public static readonly byte[]  i64_eq = {0x51}; // (i64, i64) : (i32) C, G
		
		public static readonly byte[]  i32_ne = {0x47}; // (i32, i32) : (i32) C, G
		public static readonly byte[]  i64_ne = {0x52}; // (i64, i64) : (i32) C, G
		
		public static readonly byte[]  i32_lt_s = {0x48}; // (i32, i32) : (i32) C, S
		public static readonly byte[]  i64_lt_s = {0x53}; // (i64, i64) : (i32) C, S
		
		public static readonly byte[]  i32_lt_u = {0x49}; // (i32, i32) : (i32) C, U
		public static readonly byte[]  i64_lt_u = {0x54}; // (i64, i64) : (i32) C, U
		
		public static readonly byte[]  i32_le_s = {0x4c}; // (i32, i32) : (i32) C, S
		public static readonly byte[]  i64_le_s = {0x57}; // (i64, i64) : (i32) C, S
		    
		public static readonly byte[]  i32_le_u = {0x4d}; // (i32, i32) : (i32) C, U
		public static readonly byte[]  i64_le_u = {0x58}; // (i64, i64) : (i32) C, U
		    
		public static readonly byte[]  i32_gt_s = {0x4a}; // (i32, i32) : (i32) C, S
		public static readonly byte[]  i64_gt_s = {0x55}; // (i64, i64) : (i32) C, S
		
		public static readonly byte[]  i32_gt_u = {0x4b}; // (i32, i32) : (i32) C, U
		public static readonly byte[]  i64_gt_u = {0x56}; // (i64, i64) : (i32) C, U
		
		public static readonly byte[]  i32_ge_s = {0x4e}; // (i32, i32) : (i32) C, S
		public static readonly byte[]  i64_ge_s = {0x59}; // (i64, i64) : (i32) C, S
		    
		public static readonly byte[]  i32_ge_u = {0x4f}; // (i32, i32) : (i32) C, U
		public static readonly byte[]  i64_ge_u = {0x5a}; // (i64, i64) : (i32) C, U
		
		public static readonly byte[]  f32_eq = {0x5b}; // (f32, f32) : (i32) C, F
		public static readonly byte[]  f64_eq = {0x61}; // (f64, f64) : (i32) C, F
		    
		public static readonly byte[]  f32_ne = {0x5c}; // (f32, f32) : (i32) C, F
		public static readonly byte[]  f64_ne = {0x62}; // (f64, f64) : (i32) C, F
		    
		public static readonly byte[]  f32_lt = {0x5d}; // (f32, f32) : (i32) C, F
		public static readonly byte[]  f64_lt = {0x63}; // (f64, f64) : (i32) C, F
		
		public static readonly byte[]  f32_le = {0x5f}; // (f32, f32) : (i32) C, F
		public static readonly byte[]  f64_le = {0x65}; // (f64, f64) : (i32) C, F
		    
		public static readonly byte[]  f32_gt = {0x5e}; // (f32, f32) : (i32) C, F
		public static readonly byte[]  f64_gt = {0x64}; // (f64, f64) : (i32) C, F
		
		public static readonly byte[]  f32_ge = {0x60}; // (f32, f32) : (i32) C, F
		public static readonly byte[]  f64_ge = {0x66}; // (f64, f64) : (i32) C, F
		
		public static readonly byte[]  i32_wrap_i64 = {0xa7}; // (i64) : (i32) G
		
		public static readonly byte[]  i64_extend_s_i32 = {0xac}; // (i32) : (i64) S
		
		public static readonly byte[]  i64_extend_u_i32 = {0xad}; // (i32) : (i64) U
		
		public static readonly byte[]  i32_trunc_s_f32 = {0xa8}; // (f32) : (i32) F, S
		public static readonly byte[]  i32_trunc_s_f64 = {0xaa}; // (f64) : (i32) F, S
		public static readonly byte[]  i64_trunc_s_f32 = {0xae}; // (f32) : (i64) F, S
		public static readonly byte[]  i64_trunc_s_f64 = {0xb0}; // (f64) : (i64) F, S
		    
		public static readonly byte[]  i32_trunc_u_f32 = {0xa9}; // (f32) : (i32) F, U
		public static readonly byte[]  i32_trunc_u_f64 = {0xab}; // (f64) : (i32) F, U
		public static readonly byte[]  i64_trunc_u_f32 = {0xaf}; // (f32) : (i64) F, U
		public static readonly byte[]  i64_trunc_u_f64 = {0xb1}; // (f64) : (i64) F, U
		
		public static readonly byte[]  f32_demote_f64 = {0xb6}; // (f64) : (f32) F
		
		public static readonly byte[]  f64_promote_f32 = {0xbb}; // (f32) : (f64) F
		
		public static readonly byte[]  f32_convert_s_i32 = {0xb2}; // (i32) : (f32) F, S
		public static readonly byte[]  f32_convert_s_i64 = {0xb4}; // (i64) : (f32) F, S
		public static readonly byte[]  f64_convert_s_i32 = {0xb7}; // (i32) : (f64) F, S
		public static readonly byte[]  f64_convert_s_i64 = {0xb9}; // (i64) : (f64) F, S
		
		public static readonly byte[]  i32_reinterpret_f32 = {0xbc}; // (f32) : (i32) 
		public static readonly byte[]  i64_reinterpret_f64 = {0xbd}; // (f64) : (i64) 
		public static readonly byte[]  f32_reinterpret_i32 = {0xbe}; // (i32) : (f32) 
		public static readonly byte[]  f64_reinterpret_i64 = {0xbf}; // (i64) : (f64) 
		
		public static readonly byte[]  i32_load = {0x28, 0x02, 0x00}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i32) M, G
		public static readonly byte[]  i64_load = {0x29, 0x02, 0x00}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i64) M, G
		public static readonly byte[]  f32_load = {0x2a, 0x02, 0x00}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (f32) M, E
		public static readonly byte[]  f64_load = {0x2b, 0x02, 0x00}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (f64) M, E
		
		public static readonly byte[]  i32_store = {0x36, 0x02, 0x00}; // $flags: memflags, $offset: varuPTR ($base: iPTR, $value: i32) : () M, G
		public static readonly byte[]  i64_store = {0x37, 0x02, 0x00}; // $flags: memflags, $offset: varuPTR ($base: iPTR, $value: i64) : () M, G
		public static readonly byte[]  f32_store = {0x38, 0x02, 0x00}; // $flags: memflags, $offset: varuPTR ($base: iPTR, $value: f32) : () M, F
		public static readonly byte[]  f64_store = {0x39, 0x02, 0x00}; // $flags: memflags, $offset: varuPTR ($base: iPTR, $value: f64) : () M, F
		
		public static readonly byte[]  i32_load8_s = {0x2c}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i32) M, S
		public static readonly byte[]  i32_load16_s = {0x2e}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i32) M, S
						
		public static readonly byte[]  i64_load8_s = {0x30}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i64) M, S
		public static readonly byte[]  i64_load16_s = {0x32}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i64) M, S
		public static readonly byte[]  i64_load32_s = {0x34}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i64) M, S
		
		public static readonly byte[]  i32_load8_u = {0x2d}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i32) M, U
		public static readonly byte[]  i32_load16_u = {0x2f}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i32) M, U
						
		public static readonly byte[]  i64_load8_u = {0x31}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i64) M, U
		public static readonly byte[]  i64_load16_u = {0x33}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i64) M, U
		public static readonly byte[]  i64_load32_u = {0x35}; // $flags: memflags, $offset: varuPTR ($base: iPTR) : (i64) M, U
		
		public static readonly byte[]  i32_store8 = {0x3a}; // $flags: memflags, $offset: varuPTR ($base: iPTR, $value: i32) : () M, G
		public static readonly byte[]  i32_store16 = {0x3b}; // $flags: memflags, $offset: varuPTR ($base: iPTR, $value: i32) : () M, G
						
		public static readonly byte[]  i64_store8 = {0x3c}; // $flags: memflags, $offset: varuPTR ($base: iPTR, $value: i64) : () M, G
		public static readonly byte[]  i64_store16 = {0x3d}; // $flags: memflags, $offset: varuPTR ($base: iPTR, $value: i64) : () M, G
		public static readonly byte[]  i64_store32 = {0x3e}; // $flags: memflags, $offset: varuPTR ($base: iPTR, $value: i64) : () M, G
		
		public static readonly byte[]  memory_grow = {0x40}; // $reserved: varuint1 ($delta: iPTR) : (iPTR) Z
		
		public static readonly byte[]  memory_size = {0x3f}; // $reserved: varuint1 () : (iPTR) Z
		
		//public static readonly byte[] function_end = {0x0b};
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
