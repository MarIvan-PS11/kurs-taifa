namespace MiniSharp.Parser;

using System.Collections.Generic;

using MiniSharp.Ast;
using MiniSharp.Lexer;

/// <summary>
/// Часть парсера, отвечающая за разбор инструкций и блоков.
/// </summary>
public sealed partial class Parser
{
    /// <summary>
    /// Разбирает блок инструкций в фигурных скобках.
    /// </summary>
    private BlockStatement ParseBlock()
    {
        SourcePosition startPos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
        Consume(TokenType.LeftBrace);

        List<Statement> statements = new List<Statement>();
        while (CurrentToken().Type != TokenType.RightBrace && CurrentToken().Type != TokenType.Eof)
        {
            statements.Add(ParseStatement());
        }

        Consume(TokenType.RightBrace);
        return new BlockStatement(statements, startPos);
    }

    /// <summary>
    /// Разбирает отдельную инструкцию.
    /// </summary>
    private Statement ParseStatement()
    {
        SourcePosition pos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);

        if (CurrentToken().Type == TokenType.Print)
        {
            Advance();
            Expression value = ParseExpression();
            Consume(TokenType.Semicolon);
            return new PrintStatement(value, pos);
        }

        if (CurrentToken().Type == TokenType.Return)
        {
            Advance();
            Expression? value = CurrentToken().Type != TokenType.Semicolon ? ParseExpression() : null;
            Consume(TokenType.Semicolon);
            return new ReturnStatement(value, pos);
        }

        if (CurrentToken().Type == TokenType.Int)
        {
            Advance();
            string name = CurrentToken().Lexeme;
            Consume(TokenType.Identifier);
            Consume(TokenType.Assign);
            Expression initializer = ParseExpression();
            Consume(TokenType.Semicolon);
            return new VariableDeclarationStatement(name, initializer, pos);
        }

        if (CurrentToken().Type == TokenType.LeftBrace)
        {
            return ParseBlock();
        }

        Expression expr = ParseExpression();
        Consume(TokenType.Semicolon);
        return new ExpressionStatement(expr, pos);
    }
}