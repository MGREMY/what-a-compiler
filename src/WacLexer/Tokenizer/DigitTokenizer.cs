namespace WacLexer.Tokenizer;

internal class DigitTokenizer : ITokenizer
{
    public bool CanTokenize(string source, ref LexerState state)
    {
        return char.IsDigit(source[state.Position]);
    }

    public Token Tokenize(string source, ref LexerState state)
    {
        var start = state.Position;
        var position = state.Position;
        var current = source[state.Position];

        while (position < source.Length &&
               char.IsDigit(current) ||
               current is '.' or 'e')
        {
            position++;
            current = source[position];
        }

        var word = source.Substring(start, position - start);

        var token = word.Contains('.')
            ? new Token(TokenKind.FloatLiteral, new TokenPosition(state.Line, state.Column), word)
            : new Token(TokenKind.IntLiteral, new TokenPosition(state.Line, state.Column), word);

        state.Position = position;
        state.Column += word.Length;

        return token;
    }
}