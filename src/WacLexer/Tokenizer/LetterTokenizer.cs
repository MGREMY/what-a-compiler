namespace WacLexer.Tokenizer;

public static class LetterTokenizer
{
    public static Token Tokenize(string source, ref LexerState state)
    {
        var start = state.Position;
        var next = Helper.Peek(source, state.Position);

        while (char.IsLetter(next) ||
               next == '_')
        {
            next = Helper.Peek(source, state.Position);
            state.Position++;
        }

        state.Position++;

        var word = source.Substring(start, state.Position - start);
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

        state.Column += word.Length;

        return new Token(kind, new TokenPosition(state.Line, state.Column), word);
    }
}