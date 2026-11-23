namespace MiniSharp.Lexer;

/// <summary>
/// Часть лексера, отвечающая за чтение литералов.
/// </summary>
public partial class Lexer
{
    private Token ReadNumber(int startLine, int startColumn)
    {
        int startPos = _position;
        while (!IsAtEnd() && IsAsciiDigit(Peek()))
        {
            Advance();
        }

        if (!IsAtEnd() && (IsAsciiLetter(Peek()) || Peek() == '_'))
        {
            throw new LexerException(
                "Идентификатор не может начинаться сразу после цифры без разделителя",
                new SourcePosition(startLine, startColumn));
        }

        string lexeme = _source.Substring(startPos, _position - startPos);
        if (!ulong.TryParse(lexeme, out ulong value) || value > IntegerLimits.MaxLiteralMagnitude)
        {
            throw new LexerException(
                $"Целочисленный литерал '{lexeme}' выходит за диапазон 64-битного знакового целого",
                new SourcePosition(startLine, startColumn));
        }

        return new Token(TokenType.IntegerLiteral, lexeme, startLine, startColumn);
    }

    private Token ReadString(int startLine, int startColumn)
    {
        int startPos = _position;
        Advance();

        while (!IsAtEnd() && Peek() != '"')
        {
            char c = Peek();
            if (c == '\n' || c == '\r')
            {
                throw new LexerException("Незакрытая строковая константа: запрещён перенос строки внутри литерала", CurrentPosition());
            }

            if (c == '\\')
            {
                Advance();
                if (IsAtEnd())
                {
                    throw new LexerException("Незавершенная escape-последовательность в строке", CurrentPosition());
                }

                char escaped = Peek();
                Advance();
                switch (escaped)
                {
                    case 'n':
                    case 't':
                    case '"':
                    case '\\':
                        break;
                    default:
                        throw new LexerException($"Недопустимая escape-последовательность '\\{escaped}'", CurrentPosition());
                }
            }
            else
            {
                Advance();
            }
        }

        if (IsAtEnd())
        {
            throw new LexerException("Незакрытая строковая константа", CurrentPosition());
        }

        Advance();
        string rawLexeme = _source.Substring(startPos, _position - startPos);
        return new Token(TokenType.StringLiteral, rawLexeme, startLine, startColumn);
    }
}