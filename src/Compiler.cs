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
        private Binary binary;

        public void BuildWASM()
        {
            // Binary class will automatically sort functions based on index to ensure the export definition index matches the function code index
            // This means functions can be added in whatever order desired
            
            //binary = MakeLinkedListBinary();
            List<Token> tokens = Tokeniser.getTokenArray(@"return ((((1*2) + 4) * 1) + ((2 + 3) + (4*(1+2))));");
            /*
             ((((1*2) + 4) * 1) + ((2 + 3) + (4*(1+2))))
             */
            Parser p = new Parser(tokens);
            p.Parse();
            p.Dump();
            List<Statement> statements = p.getStatements();
            
            binary = new Binary();
            Func f = new Func("temp");
            
            f.setInputParameters();
            f.setOutputParameters(Types.i32);
            f.setLocalCounts();
            //*
            Console.WriteLine("Printing pretty statements");
            foreach(Statement s in statements)
            {
            	s.Tree.PrintPretty("", true);
        		f.pushCode(EncodeStatement(s));
            }
            //*/
            
            
            binary.addFunction(f);
            
            byte[] bits = binary.getBinary();

            File.Delete(outFilename);
            FileStream fs = File.OpenWrite(outFilename);
            fs.Write(bits, 0, bits.Length);
            fs.Close();
        }
        
        private List<byte> EncodeStatement(Statement s)
        {
        	List<byte> code = new List<byte>();
        	
        	if (s.Tree.Details.Value == "return")
        	{
        		code.AddRange(EncodeBinaryExpression(s.Tree.Children[0])); //first expression must be binary expression
        	}
        	
        	return code;
        }
        
        private List<byte> EncodeBinaryExpression(Node n)
        {
        	List<byte> code = new List<byte>();
        	
        	if(n.Details.Group == TokenCodes.NUMBER)
        	{
        		code.AddRange(Op.i32_const);
        		code.AddRange(Encoder.uLEB128(Int32.Parse(n.Details.Value)));
        	} else if(n.Details.Group == TokenCodes.EXPRESSION) {
        		code.AddRange(EncodeBinaryExpression(n.Children[0]));
        		code.AddRange(EncodeBinaryExpression(n.Children[1]));
        		
        		Console.WriteLine("Order: " + n.Details.Value);
        		
        		if(n.Details.Value == "+") {
        			code.AddRange(Op.i32_add);
        		} else if(n.Details.Value == "-") {
        			code.AddRange(Op.i32_sub);
        		} else if(n.Details.Value == "*") {
        			code.AddRange(Op.i32_mul);
        		} else if(n.Details.Value == "/") {
        			code.AddRange(Op.i32_div_s);
        		}
        	}
        	
        	
        	return code;
        }
		
        public Binary MakeLinkedListBinary()
        {
            Binary bin = new Binary();

            Func allocate = new Func("allocate", true);

            Func nodeNew = new Func("newNode", false);
            Func nodeGetNext = new Func("nodeGetNext", false);
            Func nodeGetPrev = new Func("nodeGetPrev", false);
            Func nodeGetData = new Func("nodeGetData", false);
            Func nodeSetNext = new Func("nodeSetNext", false);
            Func nodeSetPrev = new Func("nodeSetPrev", false);
            Func nodeSetData = new Func("nodeSetData", false);

            Func listGetHead = new Func("listGetHead", false);
            Func listGetTail = new Func("listGetTail", false);
            Func listGetTemp = new Func("listGetTemp", false);
            Func listGetTempRef = new Func("listGetTempRef", false);
            Func listSetHead = new Func("listSetHead", false);
            Func listSetTail = new Func("listSetTail", false);
            Func listSetTemp = new Func("listSetTemp", false);
            
            Func listNew = new Func("listNew", true);
            Func listAdd = new Func("listAdd", true);
            Func listSum = new Func("listSum", true);

            allocate.setInputParameters(Types.i32);
            allocate.setOutputParameters(Types.i32);

            nodeNew.setInputParameters(Types.i32);
            nodeNew.setOutputParameters(Types.i32);
            nodeGetNext.setInputParameters(Types.i32);
            nodeGetNext.setOutputParameters(Types.i32);
            nodeGetPrev.setInputParameters(Types.i32);
            nodeGetPrev.setOutputParameters(Types.i32);
            nodeGetData.setInputParameters(Types.i32);
            nodeGetData.setOutputParameters(Types.i32);

            nodeSetNext.setInputParameters(Types.i32, Types.i32);
            nodeSetPrev.setInputParameters(Types.i32, Types.i32);
            nodeSetData.setInputParameters(Types.i32, Types.i32);

            listNew.setInputParameters(Types.i32);
            listNew.setOutputParameters(Types.i32);
            listGetTail.setInputParameters(Types.i32);
            listGetTail.setOutputParameters(Types.i32);
            listGetHead.setInputParameters(Types.i32);
            listGetHead.setOutputParameters(Types.i32);
            listGetTemp.setInputParameters(Types.i32);
            listGetTemp.setOutputParameters(Types.i32);
            listGetTempRef.setInputParameters(Types.i32);
            listGetTempRef.setOutputParameters(Types.i32);

            listSetHead.setInputParameters(Types.i32, Types.i32);
            listSetTail.setInputParameters(Types.i32, Types.i32);
            listSetTemp.setInputParameters(Types.i32, Types.i32);

            listAdd.setInputParameters(Types.i32, Types.i32);
            listSum.setInputParameters(Types.i32);
            listSum.setOutputParameters(Types.i32);

            allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x00), Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x01), Op.i32_add, Op.i32_store);
            allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x04), Op.i32_const, Encoder.uLEB128(0x20), Op.i32_store);
            allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x08), Op.i32_const, Encoder.uLEB128(0x00), Op.i32_store);

            allocate.pushCode(Op.block, Op.loop);
            {
                allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x04), Op.i32_load, Op.i32_load, Op.i32_const, Encoder.uLEB128(0x00), Op.i32_eq);

                allocate.pushCode(Op._if);
                
                    allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x08), Op.i32_const, Encoder.uLEB128(0x01), Op.i32_const, Encoder.uLEB128(0x08), Op.i32_load, Op.i32_add, Op.i32_store);
                    allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x04), Op.i32_const, Encoder.uLEB128(0x04), Op.i32_const, Encoder.uLEB128(0x04), Op.i32_load, Op.i32_add, Op.i32_store);
                
                allocate.pushCode(Op._else);
                
                    allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x08), Op.i32_const, Encoder.uLEB128(0x00), Op.i32_store);
                    allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x04), Op.i32_const, Encoder.uLEB128(0x04), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x04), Op.i32_load, Op.i32_load);
                    allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x04), Op.i32_mul, Op.i32_add, Op.i32_const, Encoder.uLEB128(0x04), Op.i32_add, Op.i32_store);
                
                allocate.pushCode(Op.end);

                //compare found segment to the required length        		
                allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x08), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x00), Op.i32_load, Op.i32_eq);
                allocate.pushCode(Op.br_if, Encoder.uLEB128(0x01)); //if long enough ,break out of loop and allocate it
                allocate.pushCode(Op.br, Encoder.uLEB128(0x00)); //else keep searching
            }
            allocate.pushCode(Op.end, Op.end);

            allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x04), Op.i32_const, Encoder.uLEB128(0x04), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x08), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x04), Op.i32_mul, Op.i32_sub, Op.i32_store);

            allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x04), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x00), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x01), Op.i32_sub, Op.i32_store);

            //load new candidate address
            allocate.pushCode(Op.i32_const, Encoder.uLEB128(0x04), Op.i32_load);

            //*********************************************************************************************************************************************************

            nodeNew.pushCode(Op.i32_const, Encoder.uLEB128(0x0c), Op.i32_const, Encoder.uLEB128(0x03), Op.call, Encoder.uLEB128(allocate.getIndex()), Op.i32_store); //call allocate function
            nodeNew.pushCode(Op.i32_const, Encoder.uLEB128(0x0c), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(nodeSetNext.getIndex()));
            nodeNew.pushCode(Op.i32_const, Encoder.uLEB128(0x0c), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(nodeSetPrev.getIndex()));
            nodeNew.pushCode(Op.i32_const, Encoder.uLEB128(0x0c), Op.i32_load, Op.get_local, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(nodeSetData.getIndex()));
            nodeNew.pushCode(Op.i32_const, Encoder.uLEB128(0x0c), Op.i32_load); //return pointer to this.

            nodeGetNext.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x04), Op.i32_add, Op.i32_load);
            nodeGetPrev.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x08), Op.i32_add, Op.i32_load);
            nodeGetData.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x0c), Op.i32_add, Op.i32_load);

            nodeSetNext.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x04), Op.i32_add, Op.get_local, Encoder.uLEB128(0x01), Op.i32_store);
            nodeSetPrev.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x08), Op.i32_add, Op.get_local, Encoder.uLEB128(0x01), Op.i32_store);
            nodeSetData.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x0c), Op.i32_add, Op.get_local, Encoder.uLEB128(0x01), Op.i32_store);

            
            listNew.pushCode(Op.i32_const, Encoder.uLEB128(0x10), Op.i32_const, Encoder.uLEB128(0x03), Op.call, Encoder.uLEB128(allocate.getIndex()), Op.i32_store);
            
            listNew.pushCode(Op.i32_const, Encoder.uLEB128(0x10), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x04), Op.i32_add);
            listNew.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(nodeNew.getIndex()), Op.i32_store);
            
            listNew.pushCode(Op.i32_const, Encoder.uLEB128(0x10), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x08), Op.i32_add, Op.i32_const, Encoder.uLEB128(0x10), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x04), Op.i32_add, Op.i32_load, Op.i32_store);
            listNew.pushCode(Op.i32_const, Encoder.uLEB128(0x10), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x0c), Op.i32_add, Op.i32_const, Encoder.uLEB128(0x10), Op.i32_load, Op.i32_const, Encoder.uLEB128(0x04), Op.i32_add, Op.i32_load, Op.i32_store);
            listNew.pushCode(Op.i32_const, Encoder.uLEB128(0x10), Op.i32_load); //return pointer to this

            
            listGetHead.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x04), Op.i32_add, Op.i32_load);
            listGetTail.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x08), Op.i32_add, Op.i32_load);
            listGetTemp.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x0c), Op.i32_add, Op.i32_load);
            listGetTempRef.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x0c), Op.i32_add);

            listSetHead.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x04), Op.i32_add, Op.get_local, Encoder.uLEB128(0x01), Op.i32_store);
            listSetTail.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x08), Op.i32_add, Op.get_local, Encoder.uLEB128(0x01), Op.i32_store);
            listSetTemp.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x0c), Op.i32_add, Op.get_local, Encoder.uLEB128(0x01), Op.i32_store);

            listAdd.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.get_local, Encoder.uLEB128(0x01), Op.call, Encoder.uLEB128(nodeNew.getIndex()), Op.call, Encoder.uLEB128(listSetTemp.getIndex()));
            listAdd.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(listGetTemp.getIndex()), Op.get_local, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(listGetTail.getIndex()), Op.call, Encoder.uLEB128(nodeSetPrev.getIndex()));
            listAdd.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(listGetTail.getIndex()), Op.get_local, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(listGetTemp.getIndex()), Op.call, Encoder.uLEB128(nodeSetNext.getIndex()));
            listAdd.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.get_local, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(listGetTemp.getIndex()), Op.call, Encoder.uLEB128(listSetTail.getIndex()));

            listSum.pushCode(Op.i32_const, Encoder.uLEB128(0x00), Op.i32_const, Encoder.uLEB128(0x00), Op.i32_store);
            listSum.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.get_local, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(listGetHead.getIndex()), Op.call, Encoder.uLEB128(listSetTemp.getIndex()));

            listSum.pushCode(Op.block, Op.loop);
            {
                listSum.pushCode(Op.i32_const, Encoder.uLEB128(0x00));
                {
                    listSum.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(listGetTemp.getIndex()), Op.call, Encoder.uLEB128(nodeGetData.getIndex()), Op.i32_const, Encoder.uLEB128(0x00), Op.i32_load, Op.i32_add);
                }
                listSum.pushCode(Op.i32_store);

                listSum.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.get_local, Encoder.uLEB128(0x00));
                listSum.pushCode(Op.call, Encoder.uLEB128(listGetTemp.getIndex()), Op.call, Encoder.uLEB128(nodeGetNext.getIndex()), Op.call, Encoder.uLEB128(listSetTemp.getIndex()));

                listSum.pushCode(Op.get_local, Encoder.uLEB128(0x00), Op.call, Encoder.uLEB128(listGetTemp.getIndex()), Op.i32_const, Encoder.uLEB128(0x00), Op.i32_eq);

                listSum.pushCode(Op.br_if, Encoder.uLEB128(0x01)); //if long enough ,break out of loop and allocate it
                listSum.pushCode(Op.br, Encoder.uLEB128(0x00)); //else keep searching
            }
            listSum.pushCode(Op.end, Op.end);

            listSum.pushCode(Op.i32_const, Encoder.uLEB128(0x00), Op.i32_load);
            
            bin.addFunction(allocate);
            
            bin.addFunction(nodeNew);
            bin.addFunction(nodeGetNext);
            bin.addFunction(nodeGetPrev);
            bin.addFunction(nodeGetData);
            bin.addFunction(nodeSetNext);
            bin.addFunction(nodeSetPrev);
            bin.addFunction(nodeSetData);
            
            bin.addFunction(listNew);
            bin.addFunction(listGetHead);
            bin.addFunction(listGetTail);
            bin.addFunction(listGetTemp);
            bin.addFunction(listGetTempRef);
            bin.addFunction(listSetHead);
            bin.addFunction(listSetTail);
            bin.addFunction(listSetTemp);
            
            bin.addFunction(listAdd);
            bin.addFunction(listSum);

            return bin;
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