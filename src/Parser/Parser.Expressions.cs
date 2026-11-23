namespace MiniSharp.Parser;

using System.Collections.Generic;

using MiniSharp.Ast;
using MiniSharp.Lexer;

/// <summary>
/// Часть парсера, отвечающая за разбор выражений.
/// </summary>
public sealed partial class Parser
{
    /// <summary>
    /// expression = additive_expression ;
    /// </summary>
    private Expression ParseExpression()
    {
        return ParseAdditiveExpression();
    }

    /// <summary>
    /// additive_expression = multiplicative_expression, { ( "+" | "-" ), multiplicative_expression } ;
    /// </summary>
    private Expression ParseAdditiveExpression()
    {
        Expression left = ParseMultiplicativeExpression();
        while (CurrentToken().Type == TokenType.Plus || CurrentToken().Type == TokenType.Minus)
        {
            TokenType op = CurrentToken().Type;
            SourcePosition pos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
            Advance();
            Expression right = ParseMultiplicativeExpression();
            BinaryOperator binOp = op == TokenType.Plus ? BinaryOperator.Add : BinaryOperator.Subtract;
            left = new BinaryExpression(left, binOp, right, pos);
        }

        return left;
    }

    /// <summary>
    /// multiplicative_expression = unary_expression, { ( "*" | "/" ), unary_expression } ;
    /// </summary>
    private Expression ParseMultiplicativeExpression()
    {
        Expression left = ParseUnaryExpression();
        while (CurrentToken().Type == TokenType.Star || CurrentToken().Type == TokenType.Slash)
        {
            TokenType op = CurrentToken().Type;
            SourcePosition pos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
            Advance();
            Expression right = ParseUnaryExpression();
            BinaryOperator binOp = op == TokenType.Star ? BinaryOperator.Multiply : BinaryOperator.Divide;
            left = new BinaryExpression(left, binOp, right, pos);
        }

        return left;
    }

    /// <summary>
    /// unary_expression = ( "-", unary_expression ) | primary_expression ;
    /// </summary>
    private Expression ParseUnaryExpression()
    {
        if (CurrentToken().Type == TokenType.Minus)
        {
            SourcePosition pos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
            Advance();

            // Специальный случай: -9223372036854775808 сворачивается в литерал long.MinValue.
            if (CurrentToken().Type == TokenType.IntegerLiteral
                && CurrentToken().Lexeme == IntegerLimits.MaxLiteralMagnitudeLexeme)
            {
                Advance();
                return new IntegerLiteralExpression(long.MinValue, pos);
            }

            Expression operand = ParseUnaryExpression();
            return new UnaryExpression(UnaryOperator.Negate, operand, pos);
        }

        return ParsePrimaryExpression();
    }

    /// <summary>
    /// primary_expression = integer_literal | "readInt", "(", ")" | identifier, "(", [ argument_list ], ")" | "(", expression, ")" ;
    /// </summary>
    private Expression ParsePrimaryExpression()
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
            long value = checked((long)ulong.Parse(token.Lexeme));
            return new IntegerLiteralExpression(value, new SourcePosition(token.Line, token.Column));
        }

        if (token.Type == TokenType.ReadInt)
        {
            SourcePosition pos = new SourcePosition(token.Line, token.Column);
            Advance();
            Consume(TokenType.LeftParen);
            Consume(TokenType.RightParen);
            return new CallExpression("readInt", new List<Expression>(), pos);
        }

        if (token.Type == TokenType.Identifier)
        {
            string funcName = token.Lexeme;
            SourcePosition pos = new SourcePosition(token.Line, token.Column);
            Advance();
            Consume(TokenType.LeftParen);

            List<Expression> args = new List<Expression>();
            if (CurrentToken().Type != TokenType.RightParen)
            {
                args = ParseArgumentList();
            }

            Consume(TokenType.RightParen);
            ValidateBuiltinFunction(funcName, args.Count, token);
            return new CallExpression(funcName, args, pos);
        }

        if (token.Type == TokenType.LeftParen)
        {
            Advance();
            Expression value = ParseExpression();
            Consume(TokenType.RightParen);
            return value;
        }

        throw new ParserException($"Неожиданный токен '{token.Lexeme}'", token);
    }

    /// <summary>
    /// argument_list = expression, { ",", expression } ;
    /// </summary>
    private List<Expression> ParseArgumentList()
    {
        List<Expression> args = new List<Expression>();
        args.Add(ParseExpression());
        while (CurrentToken().Type == TokenType.Comma)
        {
            Advance();
            args.Add(ParseExpression());
        }

        return args;
    }

    /// <summary>
    /// Проверяет имя и арность встроенной функции при разборе.
    /// </summary>
    private void ValidateBuiltinFunction(string funcName, int argCount, Token token)
    {
        if (!BuiltinFunctionRegistry.TryGet(funcName, out BuiltinFunctionInfo? info) || info is null)
        {
            throw new ParserException($"Неизвестная встроенная функция '{funcName}'", token);
        }

        if (argCount != info.Arity)
        {
            throw new ParserException($"Функция '{funcName}': Ожидаемое количество аргументов: {info.Arity}", token);
        }
    }
}