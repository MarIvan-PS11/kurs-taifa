namespace MiniSharp.Parser;

using MiniSharp.Lexer;

/// <summary>
/// Часть парсера, отвечающая за разбор выражений.
/// </summary>
public sealed partial class Parser
{
    /// <summary>
    /// expression = additive_expression ;
    /// </summary>
    private long ParseExpression()
    {
        return ParseAdditiveExpression();
    }

    /// <summary>
    /// additive_expression = multiplicative_expression, { ( "+" | "-" ), multiplicative_expression } ;
    /// </summary>
    private long ParseAdditiveExpression()
    {
        try
        {
            long left = ParseMultiplicativeExpression();
            while (CurrentToken().Type == TokenType.Plus || CurrentToken().Type == TokenType.Minus)
            {
                TokenType op = CurrentToken().Type;
                Advance();
                long right = ParseMultiplicativeExpression();
                if (op == TokenType.Plus)
                {
                    left = checked(left + right);
                }
                else
                {
                    left = checked(left - right);
                }
            }

            return left;
        }
        catch (OverflowException ex)
        {
            throw new InterpreterException($"Арифметическое переполнение: {ex.Message}", CurrentToken());
        }
    }

    /// <summary>
    /// multiplicative_expression = unary_expression, { ( "*" | "/" ), unary_expression } ;
    /// </summary>
    private long ParseMultiplicativeExpression()
    {
        try
        {
            long left = ParseUnaryExpression();
            while (CurrentToken().Type == TokenType.Star || CurrentToken().Type == TokenType.Slash)
            {
                TokenType op = CurrentToken().Type;
                Advance();
                long right = ParseUnaryExpression();
                if (op == TokenType.Star)
                {
                    left = checked(left * right);
                }
                else
                {
                    if (right == 0)
                    {
                        throw new InterpreterException("Деление на ноль", CurrentToken());
                    }

                    left = checked(left / right);
                }
            }

            return left;
        }
        catch (OverflowException ex)
        {
            throw new InterpreterException($"Арифметическое переполнение: {ex.Message}", CurrentToken());
        }
    }

    /// <summary>
    /// unary_expression = ( "-", unary_expression ) | primary_expression ;
    /// </summary>
    private long ParseUnaryExpression()
    {
        try
        {
            if (CurrentToken().Type == TokenType.Minus)
            {
                Advance();

                if (CurrentToken().Type == TokenType.IntegerLiteral
                    && CurrentToken().Lexeme == "9223372036854775808")
                {
                    Advance();
                    return long.MinValue;
                }

                long value = ParseUnaryExpression();
                return checked(-value);
            }

            return ParsePrimaryExpression();
        }
        catch (OverflowException ex)
        {
            throw new InterpreterException($"Арифметическое переполнение: {ex.Message}", CurrentToken());
        }
    }

    /// <summary>
    /// primary_expression = integer_literal | "readint", "(", ")" | identifier, "(", [ argument_list ], ")" | "(", expression, ")" ;
    /// </summary>
    private long ParsePrimaryExpression()
    {
        Token token = CurrentToken();

        if (token.Type == TokenType.IntegerLiteral)
        {
            Advance();

            if (!ulong.TryParse(token.Lexeme, out ulong value) || value > long.MaxValue)
            {
                throw new InterpreterException($"Число '{token.Lexeme}' слишком большое", token);
            }

            return (long)value;
        }

        if (token.Type == TokenType.ReadInt)
        {
            Advance();
            Consume(TokenType.LeftParen);
            Consume(TokenType.RightParen);
            return _env.ReadInt();
        }

        if (token.Type == TokenType.Identifier)
        {
            string funcName = token.Lexeme;
            Advance();
            Consume(TokenType.LeftParen);
            List<long> args = new List<long>();
            if (CurrentToken().Type != TokenType.RightParen)
            {
                args = ParseArgumentList();
            }

            Consume(TokenType.RightParen);
            return EvaluateBuiltinFunction(funcName, args, token);
        }

        if (token.Type == TokenType.LeftParen)
        {
            Advance();
            long value = ParseExpression();
            Consume(TokenType.RightParen);
            return value;
        }

        throw new ParserException($"Неожиданный токен '{token.Lexeme}'", token);
    }

    /// <summary>
    /// argument_list = expression, { ",", expression } ;
    /// </summary>
    private List<long> ParseArgumentList()
    {
        List<long> args = new List<long>();
        args.Add(ParseExpression());
        while (CurrentToken().Type == TokenType.Comma)
        {
            Advance();
            args.Add(ParseExpression());
        }

        return args;
    }
}