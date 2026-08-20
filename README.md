# WHAT A COMPILER

WhatACompiler — a compiler for a custom programming language, built with .NET/C#.

## Overview

The WhatACompiler project implements a complete compiler pipeline for a custom C-like language, targeting .NET/C#. The
compiler consists of the following pipeline stages:

- **Lexical analysis** — tokenizing source code
- **Parsing** — building AST from tokens
- **Semantic analysis** — type checking and validation
- **Code generation** — producing intermediate or target code

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

- **Prerequisites**: .NET 10.0 SDK or later
- **Build**: `dotnet build`
- **Run**: `dotnet run --project src/WhatACompiler.csproj`
- **Test**: `dotnet test`

## Project Status

| Stage             | Description                        | Status      |
|-------------------|------------------------------------|-------------|
| Lexer             | Tokenizer implementation           | [x] Done    |
| Parser            | AST construction                   | [x] Done    |
| Semantic analysis | Type checking and scope resolution | [ ] Pending |
| Code generation   | Producing target code              | [ ] Pending |

## Architecture

### Lexer (`src/WacLexer/`)

Tokenizes source code with support for:

- Whitespace and newline handling
- Single-line (`//`) and multi-line (`/* */`) comments
- Keywords: `if`, `elif`, `else`, `while`, `return`, `void`, `int`, `float`, `string`, `char`
- Literals: integers, floats (with exponent notation like `1.5e2`), chars, strings
- Escape sequences in char/string literals
- Operators: `+`, `-`, `*`, `/`, `<`, `>`, `<=`, `>=`, `==`, `!=`, `=`
- Identifiers, parentheses, brackets, braces, semicolons, commas

### Parser (`src/WacParser/`)

Recursive descent parser building AST with 12 partial token parser files:

- `Program`, `Function`, `Statement`, `Expression`, `Assignment`
- `If`, `While`, `Return`, `VariableDeclaration`, `Else`, `Elif`, `Block`

AST nodes use `[JsonDerivedType]` attributes for polymorphic JSON serialization. Supports:

- Function declarations with parameters
- Variable declarations with type annotations
- Assignments, if/elif/else control flow
- While loops, return statements
- Binary expressions (`+`, `-`, `*`, `/`, comparisons)
- Unary expressions, literals, identifiers

### Driver (`src/Driver/`)

Main entry point that:

- Reads `Resources/CodeSample.wac`
- Executes lexer then parser pipeline
- Outputs JSON to `out/lexer` and `out/parser`
- Supports benchmark mode via `BENCHMARK` argument

## Language Features Implemented

- Variable declarations: `int x = 42;`
- Type-annotated: `var x = 42;`
- Function declarations with params and body
- If-elif-else conditional chains
- While loops with brace bodies
- Return statements with expressions
- Arithmetic: `+`, `-`, `*`, `/`
- Comparison: `>`, `>=`, `<`, `<=`, `==`, `!=`
- Operator precedence in expressions

## WacLexer Internals

### Core Types

| Type         | Purpose                                                                                        |
|--------------|------------------------------------------------------------------------------------------------|
| `LexerState` | Struct tracking `Position`, `Line`, `Column` during tokenization                               |
| `TokenKind`  | 31 enum values: keywords, types, literals, operators, punctuation, `Eof`                       |
| `Token`      | Record with `Kind` (`TokenKind`), `Position` (`TokenPosition`), `Text` (`string`)              |
| `Helper`     | Static methods: `Peek(source, pos)` → next char without advancing; `IsEscape(c)` → checks `\\` |

### Tokenization Flow

Single method `Tokenize(string source)` → `IEnumerable<Token>`, processes source left-to-right:

1. **Initialize** `LexerState(0, 1, 1)` — position 0, line/col 1
2. **Main loop** `while (state.Position < source.Length)` — iterates per character
3. **Branches** (dispatch on current char):
    - **Whitespace** — spaces/tabs increment `Column`; `'\n'` increments `Line`, resets `Column = 1`
    - **Comments** — `//` skips to next newline; `/* */` scans for closing, handles newline counting, allows nested `/*`
      (ends at first `*/`)
    - **Identifiers/Keywords** — `char.IsLetter | '_'` reads a word; `switch` maps known keywords; default →
      `TokenKind.Id`
    - **Numbers** — `char.IsDigit` reads digits, `'.'`, `'e'`, `'_'`; underscores stripped; single `.` → `FloatLiteral`,
      else `IntLiteral`; `e` triggers float classification
    - **String literals** (`"..."`) — scans for closing `"` with escape awareness via `Helper.IsEscape`
    - **Character literals** (`'c'`) — validates exactly 3 chars or specific escape patterns; throws
      `InvalidTokenException` otherwise
    - **Operators** — switch on char, `Helper.Peek` enables multi-char operators: `<=`, `>=`, `==`, `!=`; `default` →
      `UnknownTokenException`
4. **EOF** — after loop, yields `Token(TokenKind.Eof, position, "EOF")`

### Error Handling

| Exception               | Carries                            |
|-------------------------|------------------------------------|
| `InvalidTokenException` | `TokenPosition`, bad token text    |
| `UnknownTokenException` | `TokenPosition`, unknown character |

Both extend `System.Exception`, store data in `Data` dictionary.

### Design Highlights

- Positional tracking (line/column) incremented as characters consumed
- Escape-aware parsing for strings/chars
- Underscores in numbers stripped before type determination
- Look-ahead via `Helper.Peek` for operator disambiguation
- Comment nesting: inner `/*` does not prevent outer `*/` match
