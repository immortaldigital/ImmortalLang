using System;
using System.Collections.Generic;
using System.Linq;

namespace ImmortalLang
{        
	public class Func : IComparable<Func>
	{
	    private List<byte> inParam;
	    private List<byte> outParam;
	    private List<byte> code;
	    private List<byte> local;
	    
	    private string label;
	    private byte index;
	    private bool export;
	    
	    private static byte GUID = 0x00;
    
	    public Func(string label, bool export = true)
		{
		    inParam = new List<byte>();
		    outParam = new List<byte>();
		    code = new List<byte>();
		    local = new List<byte>();
		    
		    this.label = label;
		    this.index = GUID++;
		    this.export = export;
		    
		    //TODO remove
		    this.export = true;
		    
		    local.Add(0x00);
		}
		
		public void initAddInt()
		{
			//label = "add";
			inParam.Clear();
			outParam.Clear();
			code.Clear();
			
			inParam.Add(Types.i32);
		    inParam.Add(Types.i32);
		    outParam.Add(Types.i32);
		    
		    code.AddRange(Op.get_local); code.AddRange(Encoder.uLEB128(0x00));
            code.AddRange(Op.get_local); code.AddRange(Encoder.uLEB128(0x01));
            code.AddRange(Op.i32_add);
		}
		
		public void pushCodeSingle(params byte[] opcode)
		{
			for(int i=0; i<opcode.Length; i++)
			{
				code.Add(opcode[i]);
			}
		}
		public void pushCode(params IEnumerable<byte>[] opcode)
		{
			for(int i=0; i<opcode.Length; i++)
			{
				code.AddRange(opcode[i]);
			}
		}
		
		public void clearCode()
		{
			code.Clear();
		}
		
		public List<byte> getInputParameters()
		{
			return inParam;
		}
		public void setInputParameters(params byte[] opcode)
		{
			inParam.Clear();
			
			for(int i=0; i<opcode.Length; i++)
			{
				inParam.Add(opcode[i]);
			}
		}
		
		public List<byte> getOutputParameters()
		{
			return outParam;
		}
		//ONLY 1 OUTPUT PARAMETER CURRENTLY ALLOWED. CAN EASILY EXTEND IF Wasm IS UPDATED
		public void setOutputParameters(byte opcode)
		{
			outParam.Clear();
			outParam.Add(opcode);
		}
		
		public void setCode(List<byte> newCode)
		{
			code = newCode;
		}
		
		public List<byte> getCode()
		{
			return code;
		}
		
		public List<byte> getLocal()
		{
			return local;
		}
		
		public List<byte> getEncodedBody()
		{
			List<byte> temp = new List<byte>();
			temp = Encoder.concatentate(getLocal(), getCode());
            temp.AddRange(Op.end);
            temp = Encoder.wrap(temp);
            
            return temp;
		}
		
		public List<byte> getTypeDefinition()
		{
			var temp = new List<byte>();
			
			temp.Add(Types.FUNC);
		    temp.AddRange(Encoder.wrap(inParam));
		    temp.AddRange(Encoder.wrap(outParam));
		    
			return temp;
		}
		
		//LABEL
		public List<byte> getEncodedLabel()
		{
			return Encoder.encodeString(label);
		}
		public string getLabel()
		{
			return label;
		}
		public void setLabel(string newLabel)
		{
			label = newLabel;
		}
		
		public byte getIndex()
		{
			return index;
		}
		public void setIndex(byte newIndex)
		{
			index = newIndex;
		}
		
		public void setExport(bool newExport)
		{
			this.export = newExport;
		}
		public bool getExport()
		{
			return this.export;
		}
		public List<byte> getExportDefinition()
		{
			List<byte> exportSection = new List<byte>();
			
			exportSection.AddRange(getEncodedLabel());
            exportSection.Add(ExportType.FUNC);
            exportSection.Add(getIndex()); //must be unique
            
            return exportSection;
		}
		
		//compare on index
		public int CompareTo(Func f)
		{
			return index.CompareTo(f.getIndex());
		}
	}
}
