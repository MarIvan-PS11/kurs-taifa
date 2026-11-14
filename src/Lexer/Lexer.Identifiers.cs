namespace MiniSharp.Lexer;

/// <summary>
/// Часть лексера, отвечающая за чтение идентификаторов и ключевых слов.
/// </summary>
public partial class Lexer
{
    private static bool IsAsciiLetter(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');

    private static bool IsAsciiDigit(char c) => c >= '0' && c <= '9';

    private static bool IsAsciiLetterOrDigit(char c) => IsAsciiLetter(c) || IsAsciiDigit(c);

    private Token ReadIdentifierOrKeyword(int startLine, int startColumn)
    {
        int startPos = _position;

        while (!IsAtEnd() && (IsAsciiLetterOrDigit(Peek()) || Peek() == '_'))
        {
            Advance();
        }

        string text = _source.Substring(startPos, _position - startPos);

        if (Keywords.TryGetKeyword(text, out TokenType keywordType))
        {
            return new Token(keywordType, text, startLine, startColumn);
        }

        return new Token(TokenType.Identifier, text, startLine, startColumn);
    }
}