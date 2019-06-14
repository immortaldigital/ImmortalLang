# ImmortalLang
My goal is to create my own programming language, parser, and compiler. I'm starting out following this tutorial: http://blog.scottlogic.com/2019/05/17/webassembly-compiler.html

## Usage
Compile the source files and run, it will generate the binary web assembly file. Currently it is hardcoded to produce a basic LinkedList. It has the following exported functions:
- listNew(optional value): list
- listAdd(list, value)
- listSum(list): value
- allocate(value): memaddr.
Where list/memaddr are both int32 ptr and value is an int32.

It can also parse binary expressions. However you must either use brackets for everything or not at all ie *a + b - c;* or *((a + b) - c);* It fully supports operator precedence but I'll be adding mixed support for brackets and precedence soon.



## Eventual Features:
- Basic var types (int, float, double, long, array, pointer)
- Simple classes (properties and methods)
- Control flow (if/else if/else)
- Loops (for, while)
- Some arithmetic functions (bitwise operators, exponentiation etc)

Ideally I will implement all the features required to build the ImmortalLang compiler, then rewrite it in itself (I mean, all the best languages are written in themselves so for ImmortalLang to truely reach that level of immortality it is basically a requirement).

## Current Features:
- Compiles a C# representation of WAT to Wasm.
- Supports all opcodes/features of Wasm (including memory)
- Proof of concept LinkedList included
- Parsing of binary expressions

## Next steps:
`[DONE]` Figure out how local variables work (instead of allocating part of the memory to each function variable).
`[DONE]` Begin parsing simple .imlang code and figure out how to translate that into wasm.
`[DONE]` Binary expression parsing
`[Todo]` Control flow statements
`[Todo]` Add variables
`[Todo]` Functions/classes
