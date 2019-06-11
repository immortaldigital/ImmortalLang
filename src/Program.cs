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

            //c.BuildWASM();
            
            List<Token> tokens = Tokeniser.getTokenArray(@"123 12.3");
            foreach ( Token t in tokens )
            {
            	
            	if(t.Group != TokenCodes.WHITESPACE)
            	{
            		Console.WriteLine("Group: {0}, {2}, Value: |{1}|", t.GroupName, t.Value, t.Group);
            		//Console.Write(t.Value);
            	}
            }
            Console.WriteLine("\nprint123;end(hello);foobar123;varasdf;1+1=3");
            

            Console.WriteLine("Compiling...");
            Console.Write("Built: " + filename);
            Console.ReadKey(true);
        }
    }
}