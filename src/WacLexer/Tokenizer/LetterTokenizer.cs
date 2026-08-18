namespace WacLexer.Tokenizer;

internal class LetterTokenizer : ITokenizer
{
    public bool CanTokenize(string source, ref LexerState state)
    {
        return char.IsLetter(source[state.Position]) ||
               source[state.Position] == '_';
    }

    public Token Tokenize(string source, ref LexerState state)
    {
        var start = state.Position;
        var position = state.Position;
        var current = source[state.Position];

        while (position < source.Length &&
               char.IsLetter(current) ||
               current == '_')
        {
            position++;
            current = source[position];
        }

        var word = source.Substring(start, position - start);
        var kind = word switch
        {
            "if" => TokenKind.If,
            "else" => TokenKind.Else,
            "return" => TokenKind.Return,
            "void" => TokenKind.Void,
            "int" => TokenKind.Int,
            "float" => TokenKind.Float,
            "string" => TokenKind.String,
            "char" => TokenKind.Char,
            _ => TokenKind.Id,
        };

        var token = new Token(kind, new TokenPosition(state.Line, state.Column), word);

        state.Position = position;
        state.Column += word.Length;

        return token;
    }
}