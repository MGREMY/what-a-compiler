# WHAT A COMPILER

WhatACompiler — a compiler for a custom programming language, built with .NET/C#.

## Overview

The WhatACompiler project implements a complete compiler pipeline for a custom C-like language, targeting .NET/C#. The
compiler consists of the following pipeline stages:

- Lexical analysis — tokenizing source code
- Parsing — building AST from tokens
- Semantic analysis — type checking and validation
- Code generation — producing intermediate or target code

## Language Syntax (C-like)

### Expressions

| Construct           | Example            |
|---------------------|--------------------|
| Variable assignment | `x = 42;`          |
| Arithmetic          | `x + y * z;`       |
| Parenthesized       | `(a + b) * c;`     |
| Function call       | `foo(arg1, arg2);` |

### Statements

| Construct            | Example                          |
|----------------------|----------------------------------|
| Expression statement | `x = 42;`                        |
| Compound statement   | `{ ... }`                        |
| Return               | `return x;`                      |
| If-else              | `if (cond) { ... } else { ... }` |

### Declarations

| Construct            | Example                        |
|----------------------|--------------------------------|
| Variable declaration | `int x;`                       |
| Type annotation      | `var x = 42;`                  |
| Function declaration | `int foo(int x) { return x; }` |

### Comments

```c
// Single-line comment
/* Multi-line comment */
```

## Project Structure

```
WhatACompiler/
├── src/                # Source code (lexer, parser, codegen)
├── tests/              # Unit tests
├── docs/               # Additional documentation
└── WhatACompiler.slnx  # Solution file
```

## Getting Started

- **Prerequisites**: .NET 8.0 SDK or later
- **Build**: `dotnet build`
- **Run**: `dotnet run --project src/WhatACompiler.csproj`
- **Test**: `dotnet test`

## Roadmap / Status

| Stage             | Description                        | Status     |
|-------------------|------------------------------------|------------|
| Lexer             | Tokenizer implementation           | ☐ Planned |
| Parser            | AST construction                   | ☐ Planned |
| Semantic analysis | Type checking and scope resolution | ☐ Planned |
| Code generation   | Producing target code              | ☐ Planned |

## License

MIT (or your preferred license)