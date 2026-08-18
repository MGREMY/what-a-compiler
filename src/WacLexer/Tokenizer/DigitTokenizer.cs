namespace WacLexer.Tokenizer;

public static class DigitTokenizer
{
    public static Token Tokenize(string source, ref LexerState state)
    {
        var start = state.Position;
        var next = Helper.Peek(source, state.Position);

        while (char.IsDigit(next) ||
               next == '.' ||
               next == 'e')
        {
            next = Helper.Peek(source, state.Position);
            state.Position++;
        }

        state.Position++;

        var word = source.Substring(start, state.Position - start);

        state.Position += word.Length;

        return word.Contains('.')
            ? new Token(TokenKind.FloatLiteral, new TokenPosition(state.Line, state.Column), word)
            : new Token(TokenKind.IntLiteral, new TokenPosition(state.Line, state.Column), word);
    }
}