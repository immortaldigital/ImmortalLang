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
            List<byte> codeTotal = new List<byte>();

            List<byte> codeSection = new List<byte>();
            List<byte> typeSection = new List<byte>();
            List<byte> funcSection = new List<byte>();
            List<byte> exportSection = new List<byte>();

            List<byte> functionType = new List<byte>();
            List<byte> functionCode = new List<byte>();
            List<byte> functionLocal = new List<byte>();
            List<byte> functionBody = new List<byte>();
            List<byte> functionInputParam = new List<byte>();
            List<byte> functionOutputParam = new List<byte>();

            functionInputParam.Add(Types.f32);
            functionInputParam.Add(Types.f32);
            functionOutputParam.Add(Types.f32);
            functionType.Add(Types.FUNC);
            functionType.AddRange(Encoder.wrap(functionInputParam));
            functionType.AddRange(Encoder.wrap(functionOutputParam));

            //typeSection.AddRange(functionType);
            //functionType.Insert(0, 0x01); //for some reason the length inserted has to be 1
            typeSection = Encoder.createSection(Section.TYPE, Encoder.wrapLists(functionType));

            funcSection.Add(0x00);
            funcSection = Encoder.wrap(funcSection);
            funcSection = Encoder.createSection(Section.FUNC, funcSection);

            exportSection.AddRange(Encoder.encodeString("add"));
            exportSection.Add(ExportType.FUNC);
            exportSection.Add(0x00); //function index
            //exportSection = ;
            exportSection = Encoder.createSection(Section.EXPORT, Encoder.wrapLists(exportSection));

            functionLocal.Add(0x00);
            functionCode.Add(Opcodes.get_local); functionCode.AddRange(Encoder.unsignedLEB128(0x00));
            functionCode.Add(Opcodes.get_local); functionCode.AddRange(Encoder.unsignedLEB128(0x01));
            functionCode.Add(Opcodes.f32_add);

            //functionBody.AddRange(functionLocal);
            //functionBody.AddRange(functionCode);
            functionBody = Encoder.concatentate(functionLocal, functionCode);
            functionBody.Add(Opcodes.function_end);
            functionBody = Encoder.wrap(functionBody);
            codeSection = Encoder.createSection(Section.CODE, Encoder.wrapLists(functionBody));
            //codeSection.Add(Section.CODE);
            //codeSection.AddRange(Encoder.wrap(functionBody));

            //codeTotal.AddRange(Headers.magicModule);
            //codeTotal.AddRange(Headers.moduleVersion);
            //codeTotal.AddRange(typeSection);

            //codeTotal.AddRange(funcSection);

            //codeTotal.AddRange(exportSection);

            //codeTotal.AddRange(codeSection);
            
            codeTotal = Encoder.concatentate(Headers.magicModule, Headers.moduleVersion, typeSection, funcSection, exportSection, codeSection);

            File.Delete(outFilename);
            FileStream fs = File.OpenWrite(outFilename);
            fs.Write(codeTotal.ToArray(), 0, codeTotal.Count);
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
