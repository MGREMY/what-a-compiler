# WacLexer — Internal Architecture

## Core Types

| Type                           | Purpose                                                                                        |
|--------------------------------|------------------------------------------------------------------------------------------------|
| `LexerState` (`LexerState.cs`) | Struct tracking `Position`, `Line`, `Column` during tokenization                               |
| `TokenKind` (`Token.cs`)       | 31 enum values: keywords, types, literals, operators, punctuation, `Eof`                       |
| `Token` (`Token.cs`)           | Record with `Kind` (`TokenKind`), `Position` (`TokenPosition`), `Text` (`string`)              |
| `Helper` (`Helper.cs`)         | Static methods: `Peek(source, pos)` → next char without advancing; `IsEscape(c)` → checks `\\` |

## Tokenization Flow (`Lexer.Tokenize`)

Single method, generator (`IEnumerable<Token>`), processes source left-to-right:

1. **Initialize** `LexerState(0, 1, 1)` — position 0, line/col 1
2. **Main loop** `while (state.Position < source.Length)` — iterates per character
3. **Branches** (dispatch on current char):

    - **Whitespace** — spaces/tabs increment `Column`; `'\n'` increments `Line`, resets `Column = 1`
    - **Comments** — `//` skips to next newline; `/* */` scans for closing, handles newline counting, allows nested `/*`
      (ends at first `*/`)
    - **Identifiers/Keywords** — `char.IsLetter | '_'` reads a word; `switch` maps known keywords (`if`, `elif`, `else`,
      `while`, `return`, `void`, `int`, `float`, `string`, `char`); default → `TokenKind.Id`
    - **Numbers** — `char.IsDigit` reads digits, `'.'`, `'e'`, `'_'`; underscores stripped; single `.` → `FloatLiteral`,
      else `IntLiteral`; `e` triggers float classification
    - **String literals** (`"..."`) — scans for closing `"` with escape awareness via `Helper.IsEscape`
    - **Character literals** (`'c'`) — validates exactly 3 chars or specific escape patterns; throws
      `InvalidTokenException` otherwise
    - **Operators** — switch on char, `Helper.Peek` enables multi-char operators: `<=`, `>=`, `==`, `!=`; `default` →
      `UnknownTokenException`

4. **EOF** — after loop, yields `Token(TokenKind.Eof, position, "EOF")`

## Error Handling

| Exception               | Carries                            |
|-------------------------|------------------------------------|
| `InvalidTokenException` | `TokenPosition`, bad token text    |
| `UnknownTokenException` | `TokenPosition`, unknown character |

Both extend `System.Exception`, store data in `Data` dictionary.

## Design Highlights

- Positional tracking (line/column) incremented as characters are consumed
- Escape-aware parsing for strings and chars
- Underscores in numbers stripped before type determination
- Look-ahead via `Helper.Peek` for operator disambiguation (`<=`, `>=`, `==`, `!=`)
- Comment nesting: inner `/*` does not prevent outer `*/` match

## TokenKind Enum (31 values)

**Keywords**: `Id`, `If`, `Elif`, `Else`, `While`, `Return`

**Types**: `Void`, `Int`, `Float`, `String`, `Char`

**Literals**: `IntLiteral`, `FloatLiteral`, `StringLiteral`, `CharLiteral`

**Operators**: `Plus`, `Minus`, `Star`, `Slash`, `LessThan`, `MoreThan`, `LessOrEqualThan`, `MoreOrEqualThan`,
`EqualsTo`, `NotEqualsTo`, `Equal`, `Not`

**Punctuation**: `SemiColon`, `Comma`, `LeftParenthesis`, `RightParenthesis`, `LeftBracket`, `RightBracket`,
`LeftBrace`, `RightBrace`, `Eof`