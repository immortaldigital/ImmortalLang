# ImmortalLang
My goal is to create my own programming language, parser, and compiler. I'm starting out following this tutorial: http://blog.scottlogic.com/2019/05/17/webassembly-compiler.html

Eventual Features:
- Basic var types (int, float, double, long, array, pointer)
- Simple classes (properties and methods)
- Control flow (if/else if/else)
- Loops (for, while)
- Some arithmetic functions (bitwise operators, exponentiation etc)

Ideally I will implement all the features required to build the ImmortalLang compiler, then rewrite it in itself (I mean, all the best languages are written in themself)

Current Features:
- Compiles a C# representation of WAT to WASM
