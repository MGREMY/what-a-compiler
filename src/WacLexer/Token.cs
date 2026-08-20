namespace WacLexer;

#pragma warning disable format // @formatter:off
public enum TokenKind
{
    Id, // Identifier (function name, variable name)
    If, Elif, Else, While, Return, // Keyword
    Void, Int, Float, String, Char, // Type
    IntLiteral, FloatLiteral, StringLiteral, CharLiteral, // Literals
    Plus, Minus, Star, Slash, LessThan, MoreThan, LessOrEqualThan, MoreOrEqualThan, EqualsTo, NotEqualsTo, // Operator
    Equal, Not, SemiColon, Comma,
    LeftParenthesis, RightParenthesis, LeftBracket, RightBracket, LeftBrace, RightBrace,
    Eof,
}
#pragma warning restore format // @formatter:on

public struct TokenPosition
{
    public int Line { get; }
    public int Column { get; }

    public TokenPosition(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public override string ToString()
    {
        return $"(Line: {Line}; Column: {Column})";
    }
}

public record Token
{
    public TokenKind Kind { get; }
    public TokenPosition Position { get; }
    public string Text { get; }

    public Token(TokenKind kind, TokenPosition position, string text)
    {
        Kind = kind;
        Position = position;
        Text = text;
    }

    public override string ToString()
    {
        return $"Token: {{ Kind: {Kind}; Position: {Position}; Text: {Text} }}";
    }
}