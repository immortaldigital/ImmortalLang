using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ImmortalLang
{
	public class Compiler
    {
        private string source;
        private string outFilename;

        public void BuildWASM()
        {
        	Func funcAdd = new Func("add");
        	Func funcMagic = new Func("magic");
        	Func func42 = new Func("f42");
        	Binary bin = new Binary();
        	
            funcAdd.initAddInt();
            
            funcMagic.setInputParameters();
            funcMagic.setOutputParameters(Types.i32);
            funcMagic.pushCode(Opcodes.i32_const, 0x20); //to pass a value greater than 255, use Func.pushCodeList(unsignedLEB128());
            funcMagic.pushCode(Opcodes.i32_const, 0x0a);
            funcMagic.pushCode(Opcodes.call, 0x00);
            
            func42.setInputParameters();
            func42.setOutputParameters(Types.i32);
            func42.pushCode(Opcodes.i32_const, 0x2a); //WARNING. second parameter should be List<byte> LEB128. Only works here cause single byte

		    funcAdd.setExport(false);
		    
		    // Binary class will automatically sort these based on index to ensure the export definition index matches the function code index
		    // This means they can be added in whatever order desired    
		    bin.addFunction(funcAdd);
		    bin.addFunction(funcMagic);
		    bin.addFunction(func42);
		    
		    
		    byte[] bits = bin.getBinary();

            File.Delete(outFilename);
            FileStream fs = File.OpenWrite(outFilename);
            fs.Write(bits, 0, bits.Length);
            fs.Close();
        }

        public Compiler(string src, string outF)
        {
            source = src;
            //output = new List<byte>();
            //functionCode = new List<byte>();
            //local= new List<byte>();
            outFilename = outF;
        }

        public void LoadSource(string src)
        {
        }

        public void GenerateAST()
        {
        }

        public string Compile(string file)
        {
            LoadSource(file);
            GenerateAST();
            BuildWASM();

            return "";
        }
    }
}
