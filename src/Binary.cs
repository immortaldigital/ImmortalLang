using System;
using System.Collections.Generic;

namespace ImmortalLang
{
	public class Binary
	{
		List<byte> codeTotal = new List<byte>();

        List<byte> codeSection = new List<byte>();
        List<byte> typeSection = new List<byte>();
        List<byte> funcSection = new List<byte>();
        List<byte> memSection = new List<byte>();
        List<byte> exportSection = new List<byte>();
        
        List<byte> codeDef = new List<byte>();
        List<byte> typeDef = new List<byte>();
        List<byte> memDef = new List<byte>();
        List<byte> exportDef = new List<byte>();
        
        List<Func> functions = new List<Func>();
            
		public Binary()
		{
		}
		
		public void addFunction(Func f)
		{
			functions.Add(f);
		}
		
		public byte[] getBinary()
		{
			//sort functions by index to make sure they are added in the correct order
			functions.Sort();           
			
			//first add memory definition to export
			int exportCount = 1;
			
			exportDef.AddRange(Encoder.encodeString("mem"));
			exportDef.Add(ExportType.MEM);
			exportDef.Add(0x00); //memory id 0
			
			for(int i=0; i<functions.Count; i++)
			{
				funcSection.Add(functions[i].getIndex()); //encode indicies of each function
				
				Console.WriteLine("{0}({4}): {5}\n\tIndex: {1}\n\tExport: {2}\n\tCode: {3}\n\t",
				                  functions[i].getLabel(),
				                  functions[i].getIndex(),
				                  functions[i].getExport(),
				                  Encoder.hexString(functions[i].getCode()),
				                  Encoder.hexString(functions[i].getInputParameters()),
				                  Encoder.hexString(functions[i].getOutputParameters()));
				
				//then add each sucessive definition
				codeDef.AddRange(functions[i].getEncodedBody());
				typeDef.AddRange(functions[i].getTypeDefinition());
				if(functions[i].getExport())
				{
					exportDef.AddRange(functions[i].getExportDefinition());
					exportCount++;
				}
			}
			
			//each of the definitions is the Encoder.wrapList() called on all Func.def()
			//which means the first byte will be the function count
			codeDef.InsertRange(0, Encoder.uLEB128(functions.Count));
			typeDef.InsertRange(0, Encoder.uLEB128(functions.Count)); //+1 for memory type definition
			exportDef.InsertRange(0, Encoder.uLEB128(exportCount));
			
			memDef.Add(0x00);memDef.Add(0x01); //flags, minimum size
			memDef.InsertRange(0, Encoder.uLEB128(1)); //1 memory object always
			
			typeSection = Encoder.createSection(Section.TYPE, typeDef);
	
            funcSection = Encoder.wrap(funcSection);
            funcSection = Encoder.createSection(Section.FUNC, funcSection);
            
            memSection = Encoder.createSection(Section.MEMORY, memDef);
            exportSection = Encoder.createSection(Section.EXPORT, exportDef);
            codeSection = Encoder.createSection(Section.CODE, codeDef);
            
            Console.WriteLine("Type Section: {0}", Encoder.hexString(typeSection));
            Console.WriteLine("Func Section: {0}", Encoder.hexString(funcSection));
            Console.WriteLine("Mem Section: {0}", Encoder.hexString(memSection));
            Console.WriteLine("Expt Section: {0}", Encoder.hexString(exportSection));
            Console.WriteLine("Code Section: {0}", Encoder.hexString(codeSection));
            
            codeTotal = Encoder.concatentate(Headers.magicModule, Headers.moduleVersion, typeSection, funcSection, memSection, exportSection, codeSection);
            	
            return codeTotal.ToArray();
		}
	}
}
