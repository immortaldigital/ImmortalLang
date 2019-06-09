using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ImmortalLang
{
    class Program
    {
        public static void Main(string[] args)
        {
            byte[] bits = Encoder.unsignedLEB128(42).ToArray();

            foreach (byte b in bits)
            {
                Console.WriteLine(b);
            }

            string filename = "imlang.wasm";
            Compiler c = new Compiler("", filename);

            c.BuildWASM();

            Console.WriteLine("Compiling...");
            Console.Write("Built: " + filename);
            Console.ReadKey(true);
        }
    }
}