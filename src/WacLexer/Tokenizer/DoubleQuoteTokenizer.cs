namespace WacLexer.Tokenizer;

public static class DoubleQuoteTokenizer
{
    public static Token Tokenize(string source, ref LexerState state)
    {
        var start = state.Position;
        var next = Helper.Peek(source, state.Position);

        while (next != '"')
        {
            next = Helper.Peek(source, state.Position);
            state.Position++;
        }

        state.Position += 2;

        var word = source.Substring(start, state.Position - start);

        state.Column += word.Length;

        return new Token(TokenKind.StringLiteral, new TokenPosition(state.Line, state.Column), word);
    }
}