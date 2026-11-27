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
    /// Разбирает выражение верхнего уровня.
    /// </summary>
    private Expression ParseExpression() => ParseAssignment();

    /// <summary>
    /// Разбирает выражение присваивания с правой ассоциативностью.
    /// </summary>
    private Expression ParseAssignment()
    {
        Expression left = ParseAdditive();

        if (CurrentToken().Type == TokenType.Assign)
        {
            if (left is not IdentifierExpression)
            {
                throw new ParserException("Левая часть присваивания должна быть идентификатором", CurrentToken());
            }

            SourcePosition pos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
            Advance();
            Expression right = ParseAssignment();
            return new BinaryExpression(left, BinaryOperator.Assign, right, pos);
        }

        return left;
    }

    /// <summary>
    /// Разбирает выражения сложения и вычитания.
    /// </summary>
    private Expression ParseAdditive()
    {
        Expression left = ParseMultiplicative();
        while (CurrentToken().Type == TokenType.Plus || CurrentToken().Type == TokenType.Minus)
        {
            TokenType op = CurrentToken().Type;
            SourcePosition pos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
            Advance();
            Expression right = ParseMultiplicative();
            BinaryOperator binOp = op == TokenType.Plus ? BinaryOperator.Add : BinaryOperator.Subtract;
            left = new BinaryExpression(left, binOp, right, pos);
        }

        return left;
    }

    /// <summary>
    /// Разбирает выражения умножения и деления.
    /// </summary>
    private Expression ParseMultiplicative()
    {
        Expression left = ParseUnary();
        while (CurrentToken().Type == TokenType.Star || CurrentToken().Type == TokenType.Slash)
        {
            TokenType op = CurrentToken().Type;
            SourcePosition pos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
            Advance();
            Expression right = ParseUnary();
            BinaryOperator binOp = op == TokenType.Star ? BinaryOperator.Multiply : BinaryOperator.Divide;
            left = new BinaryExpression(left, binOp, right, pos);
        }

        return left;
    }

    /// <summary>
    /// Разбирает унарные выражения.
    /// </summary>
    private Expression ParseUnary()
    {
        if (CurrentToken().Type == TokenType.Minus)
        {
            SourcePosition pos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
            Advance();
            if (CurrentToken().Type == TokenType.IntegerLiteral && CurrentToken().Lexeme == IntegerLimits.MaxLiteralMagnitudeLexeme)
            {
                Advance();
                return new IntegerLiteralExpression(long.MinValue, pos);
            }

            Expression operand = ParseUnary();
            return new UnaryExpression(UnaryOperator.Negate, operand, pos);
        }

        return ParsePrimary();
    }

    /// <summary>
    /// Разбирает первичные выражения.
    /// </summary>
    private Expression ParsePrimary()
    {
        Token token = CurrentToken();

        if (token.Type == TokenType.IntegerLiteral)
        {
            if (token.Lexeme == IntegerLimits.MaxLiteralMagnitudeLexeme)
            {
                throw new ParserException("Положительный целочисленный литерал выходит за диапазон 64-битного знакового целого", token);
            }

            Advance();
            return new IntegerLiteralExpression(checked((long)ulong.Parse(token.Lexeme)), new SourcePosition(token.Line, token.Column));
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
            string name = token.Lexeme;
            SourcePosition pos = new SourcePosition(token.Line, token.Column);
            Advance();

            if (CurrentToken().Type == TokenType.LeftParen)
            {
                Consume(TokenType.LeftParen);
                List<Expression> args = new List<Expression>();
                if (CurrentToken().Type != TokenType.RightParen)
                {
                    do
                    {
                        args.Add(ParseExpression());
                    }
                    while (ConsumeIf(TokenType.Comma));
                }

                Consume(TokenType.RightParen);

                if (BuiltinFunctionRegistry.TryGet(name, out BuiltinFunctionInfo? info) && info != null)
                {
                    if (args.Count != info.Arity)
                    {
                        throw new ParserException($"Функция '{name}': Ожидаемое количество аргументов: {info.Arity}", token);
                    }
                }

                return new CallExpression(name, args, pos);
            }

            return new IdentifierExpression(name, pos);
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
}