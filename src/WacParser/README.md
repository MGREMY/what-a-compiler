# WacParser — Internal Architecture

## Overview

WacParser is a recursive descent parser that builds an Abstract Syntax Tree (AST) from tokens produced by WacLexer. The
parser is organized as 12 partial static classes within `TokenParser`, each responsible for one grammar rule. A
lightweight `ParserState` struct tracks the current token index.

## Core Types

| Type                               | Purpose                                                           |
|------------------------------------|-------------------------------------------------------------------|
| `AstNodeKind` (`AstNodeKind.cs`)   | Enum: `Program`, `FunctionDeclaration`, `Expression`, `Statement` |
| `ParserState` (`ParserState.cs`)   | Struct tracking current token `Position` in the token array       |
| `IAstNode` (`NodeType/AstNode.cs`) | Interface with `Position` and `Kind` property                     |
| `ProgramNode`                      | Root node containing `Functions` collection                       |
| `FunctionNode`                     | Function declaration with `Type`, `Name`, `Parameters`, `Block`   |
| `ExpressionNode` (abstract)        | Base for all expression types                                     |
| `StatementNode` (abstract)         | Base for all statement types                                      |

## 12 Parser Rules (TokenParser files)

Each file implements one `Parse*` method, dispatched by `TokenKind`:

| File                     | Parse Method               | Handles                                                                                                                                        |
|--------------------------|----------------------------|------------------------------------------------------------------------------------------------------------------------------------------------|
| `Program.cs`             | `ParseProgram`             | Top-level: loops calling `ParseFunction` until `Eof`                                                                                           |
| `Function.cs`            | `ParseFunction`            | Function declaration: type keyword, identifier name, parameter list, body block                                                                |
| `Statement.cs`           | `ParseStatement`           | Dispatches to: variable declaration, assignment, return, if/elif/else, while, block                                                            |
| `Expression.cs`          | `ParseExpression`          | Recursive descent: literals, identifiers, unary `not`, binary operators (`+`, `-`, `*`, `/`, `==`, `!=`, `<`, `>`, `<=`, `>=`), and assignment |
| `Assignment.cs`          | `ParseAssignment`          | `Name = Expression` pattern                                                                                                                    |
| `VariableDeclaration.cs` | `ParseVariableDeclaration` | Type keyword + optional initializer, delegates to `ParseAssignment`                                                                            |
| `If.cs`                  | `ParseIf`                  | `if (Condition) { Statement }`                                                                                                                 |
| `Elif.cs`                | `ParseElif`                | `elif (Condition) { Statement }`                                                                                                               |
| `Else.cs`                | `ParseElse`                | `else { Statement }`                                                                                                                           |
| `While.cs`               | `ParseWhile`               | `while (Condition) { Statement }`                                                                                                              |
| `Return.cs`              | `ParseReturn`              | `return Expression;`                                                                                                                           |
| `Block.cs`               | `ParseBlock`               | `{ Statement* }` — semicolons skipped, `}` terminates parsing                                                                                  |

## Expression Parsing Flow

`ParseExpression` processes a primary token (literal, identifier, or prefix operator), then checks for:

1. **Assignment** (`=`): binds right-hand side expression
2. **Binary operators** (`+`, `-`, `*`, `/`, comparison operators): creates `BinaryExpressionNode` with left/right
   operands
3. **Arithmetic operators** (`+`, `-`, `*`, `/`): creates `ArithmeticExpressionNode`

The method recurses for the right operand, enabling nested expressions and operator precedence.

## Statement Dispatch (`ParseStatement`)

The method inspects the current token `Kind` and routes:

- **Type keywords** (`int`, `float`, `char`, `string`): → `ParseVariable`
- **Identifier**: → `ParseAssignment` (if followed by `=`)
- **`return`**: → `ParseReturn`
- **`if`**: → `ParseIf`
- **`elif`**: → `ParseElif`
- **`else`**: → `ParseElse`
- **`while`**: → `ParseWhile`
- **`{`**: → `ParseBlock`

If no rule matches, `ParsingException` is thrown.

## Error Handling

`ParsingException` (`Exception/ParsingException.cs`) carries an optional `Token` property and message. All parse methods
use `TryPeekTo` to safely inspect the current token without advancing position, throwing if the expected token kind is
not found.

## Design Highlights

- **Partial classes** across 12 files keep each grammar rule isolated while sharing the `TokenParser` namespace
- **`ref ParserState`** passing avoids cloning state through recursive calls
- **Look-ahead via `TryPeekTo`** enables safe token inspection and backtracking when needed
- **Expression precedence** is handled by the recursive structure: `ParseExpression` calls itself for the right operand
  of binary operators
- **Control flow** if/elif/else and while loops each follow the same pattern: match keyword, parse `(` ... `)`, parse
  condition, parse body block
- **Function bodies** parse a type, name, parameter list (comma-separated `VariableDeclaration` patterns), then a
  `{ ... }` block

## TokenKind Categories

- **Keywords**: `if`, `elif`, `else`, `while`, `return`, `void`, `int`, `float`, `string`, `char`
- **Types**: `Void`, `Int`, `Float`, `String`, `Char`
- **Literals**: `IntLiteral`, `FloatLiteral`, `StringLiteral`, `CharLiteral`
- **Operators**: `Plus`, `Minus`, `Star`, `Slash`, `LessThan`, `MoreThan`, `LessOrEqualThan`, `MoreOrEqualThan`,
  `EqualsTo`, `NotEqualsTo`, `Equal`, `Not`
- **Punctuation**: `SemiColon`, `Comma`, `LeftParenthesis`, `RightParenthesis`, `LeftBrace`, `RightBrace`
- **Identifiers**: `Id`
- **End**: `Eof`