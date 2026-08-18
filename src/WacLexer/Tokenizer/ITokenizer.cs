namespace WacLexer.Tokenizer;

internal interface ITokenizer
{
    bool CanTokenize(string source, ref LexerState state);
    Token Tokenize(string source, ref LexerState state);
}