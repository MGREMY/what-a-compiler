namespace WacLexer;

#pragma warning disable format // @formatter:off
public enum TokenKind
{
    Id, // Identifier (function name, variable name)
    If, Else, Return, // Keyword
    Void, Int, Float, String, Char, // Type
    IntLiteral, FloatLiteral, StringLiteral, CharLiteral, // Literals
    Plus, Minus, Star, Slash, LessThan, MoreThan, LessOrEqualThan, MoreOrEqualThan, EqualsTo, NotEqualsTo, // Operator
    Equal, Not, SemiColon, Comma,
    LeftParenthesis, RightParenthesis, LeftBracket, RightBracket, LeftBrace, RightBrace,
    Eof,
    InvalidToken, // Special token
}
#pragma warning restore format // @formatter:on

public struct TokenPosition
{
    public readonly int Line;
    public readonly int Column;

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

public struct Token
{
    public readonly TokenKind Kind;
    public readonly TokenPosition Position;
    public readonly string Text;

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