using WacLexer.Exception;

namespace WacLexer.Tokenizer;

internal class QuoteTokenizer : ITokenizer
{
    public bool CanTokenize(string source, ref LexerState state)
    {
        return source[state.Position] == '\'';
    }

    public Token Tokenize(string source, ref LexerState state)
    {
        var start = state.Position;
        var position = state.Position;
        var current = source[state.Position];
        var count = 0;

        while (position < source.Length &&
               count < 2)
        {
            if (current == '\'') count++;
            if (Helper.IsEscape(current)) position++;

            position++;
            current = source[position];
        }

        Token token;
        var word = source.Substring(start, position - start);

        if (word.Length > 2 && Helper.IsEscape(word[1]))
        {
            if (word is not (@"'\\'" or @"'\n'" or @"'\t'" or @"'\''"))
                throw new InvalidTokenException(new TokenPosition(state.Line, state.Column), word);

            token = new Token(TokenKind.CharLiteral, new TokenPosition(state.Line, state.Column),
                word);
        }
        else if (word.Length != 3)
        {
            throw new InvalidTokenException(new TokenPosition(state.Line, state.Column), word);
        }
        else
        {
            token = new Token(TokenKind.CharLiteral, new TokenPosition(state.Line, state.Column), word);
        }

        state.Position = position;
        state.Column += word.Length;

        return token;
    }
}