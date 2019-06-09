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
            string filename = "imlang.wasm";
            Compiler c = new Compiler("", filename);

            c.BuildWASM();
            

            Console.WriteLine("Compiling...");
            Console.Write("Built: " + filename);
            Console.ReadKey(true);
        }
    }
}