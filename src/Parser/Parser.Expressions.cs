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
        return EvaluateChecked(() =>
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
        });
    }

    /// <summary>
    /// multiplicative_expression = unary_expression, { ( "*" | "/" ), unary_expression } ;
    /// </summary>
    private long ParseMultiplicativeExpression()
    {
        return EvaluateChecked(() =>
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
        });
    }

    /// <summary>
    /// unary_expression = ( "-", unary_expression ) | primary_expression ;
    /// </summary>
    private long ParseUnaryExpression()
    {
        return EvaluateChecked(() =>
        {
            if (CurrentToken().Type == TokenType.Minus)
            {
                Advance();
                if (CurrentToken().Type == TokenType.IntegerLiteral
                    && CurrentToken().Lexeme == IntegerLimits.MaxLiteralMagnitudeLexeme)
                {
                    Advance();
                    return long.MinValue;
                }

                long value = ParseUnaryExpression();
                return checked(-value);
            }

            return ParsePrimaryExpression();
        });
    }

    /// <summary>
    /// primary_expression = integer_literal | "readInt", "(", ")" | identifier, "(", [ argument_list ], ")" | "(", expression, ")" ;
    /// </summary>
    private long ParsePrimaryExpression()
    {
        Token token = CurrentToken();
        if (token.Type == TokenType.IntegerLiteral)
        {
            if (token.Lexeme == IntegerLimits.MaxLiteralMagnitudeLexeme)
            {
                throw new ParserException(
                    "Положительный целочисленный литерал выходит за диапазон 64-битного знакового целого",
                    token);
            }

            Advance();
            return checked((long)ulong.Parse(token.Lexeme));
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