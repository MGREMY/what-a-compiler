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
               current is '.' or 'e' or '_')
        {
            position++;
            current = source[position];
        }

        Token token;
        var word = source.Substring(start, position - start).Replace("_", string.Empty);

        if (word.Contains('.'))
        {
            var count = word.Count(x => x == '.');

            token = count > 1
                ? new Token(TokenKind.InvalidToken, new TokenPosition(state.Line, state.Column), word)
                : new Token(TokenKind.FloatLiteral, new TokenPosition(state.Line, state.Column), word);
        }
        else
        {
            token = new Token(TokenKind.IntLiteral, new TokenPosition(state.Line, state.Column), word);
        }

        state.Position = position;
        state.Column += word.Length;

        return token;
    }
}