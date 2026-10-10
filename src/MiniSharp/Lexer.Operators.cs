namespace MiniSharp.Lexer;

/// <summary>
/// Часть лексера, отвечающая за чтение операторов и разделителей.
/// </summary>
public partial class Lexer
{
    private Token ReadOperatorOrDelimiter(int startLine, int startColumn)
    {
        char c = Peek();
        Advance();

        switch (c)
        {
            case '+': return new Token(TokenType.Plus, "+", startLine, startColumn);
            case '-': return new Token(TokenType.Minus, "-", startLine, startColumn);
            case '*': return new Token(TokenType.Star, "*", startLine, startColumn);
            case '/': return new Token(TokenType.Slash, "/", startLine, startColumn);
            case '(': return new Token(TokenType.LeftParen, "(", startLine, startColumn);
            case ')': return new Token(TokenType.RightParen, ")", startLine, startColumn);
            case '{': return new Token(TokenType.LeftBrace, "{", startLine, startColumn);
            case '}': return new Token(TokenType.RightBrace, "}", startLine, startColumn);
            case '[': return new Token(TokenType.LeftBracket, "[", startLine, startColumn);
            case ']': return new Token(TokenType.RightBracket, "]", startLine, startColumn);
            case ';': return new Token(TokenType.Semicolon, ";", startLine, startColumn);
            case ',': return new Token(TokenType.Comma, ",", startLine, startColumn);
            case '.': return new Token(TokenType.Dot, ".", startLine, startColumn);
        }

        if (c == '=')
        {
            if (Peek() == '=')
            {
                return AdvanceAndReturn(TokenType.Equal, "==", startLine, startColumn);
            }

            return new Token(TokenType.Assign, "=", startLine, startColumn);
        }

        if (c == '!')
        {
            if (Peek() == '=')
            {
                return AdvanceAndReturn(TokenType.NotEqual, "!=", startLine, startColumn);
            }

            return new Token(TokenType.Not, "!", startLine, startColumn);
        }

        if (c == '<')
        {
            if (Peek() == '=')
            {
                return AdvanceAndReturn(TokenType.LessEqual, "<=", startLine, startColumn);
            }

            return new Token(TokenType.Less, "<", startLine, startColumn);
        }

        if (c == '>')
        {
            if (Peek() == '=')
            {
                return AdvanceAndReturn(TokenType.GreaterEqual, ">=", startLine, startColumn);
            }

            return new Token(TokenType.Greater, ">", startLine, startColumn);
        }

        if (c == '&')
        {
            if (Peek() == '&')
            {
                return AdvanceAndReturn(TokenType.And, "&&", startLine, startColumn);
            }

            throw new LexerException("Ожидалось '&&', но встречен одиночный '&'", new SourcePosition(startLine, startColumn));
        }

        if (c == '|')
        {
            if (Peek() == '|')
            {
                return AdvanceAndReturn(TokenType.Or, "||", startLine, startColumn);
            }

            throw new LexerException("Ожидалось '||', но встречен одиночный '|'", new SourcePosition(startLine, startColumn));
        }

        throw new LexerException($"Неизвестный символ '{c}'", new SourcePosition(startLine, startColumn));
    }

    private Token AdvanceAndReturn(TokenType type, string lexeme, int line, int column)
    {
        Advance();
        return new Token(type, lexeme, line, column);
    }
}