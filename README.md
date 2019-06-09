# ImmortalLang
My goal is to create my own programming language, parser, and compiler. I'm starting out following this tutorial: http://blog.scottlogic.com/2019/05/17/webassembly-compiler.html

# Usage
Compile the source files and run, it will generate the binary web assembly file. Currently it is hardcoded to output a function 'add(f32 f32) (f32)' which does what you would expect. Put the generated imlang.wasm in the bin folder and open the html page to gaze upon the magic <3

# Eventual Features:
- Basic var types (int, float, double, long, array, pointer)
- Simple classes (properties and methods)
- Control flow (if/else if/else)
- Loops (for, while)
- Some arithmetic functions (bitwise operators, exponentiation etc)

Ideally I will implement all the features required to build the ImmortalLang compiler, then rewrite it in itself (I mean, all the best languages are written in themselves so for ImmortalLang to truely reach that level of immortality it is basically a requirement).

# Current Features:
- Compiles a C# representation of WAT to WASM.
