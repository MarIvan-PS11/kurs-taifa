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
    /// block = "{", { statement }, "}" ;
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
    /// statement = "print", expression, ";" | expression, ";" ;
    /// </summary>
    private Statement ParseStatement()
    {
        if (CurrentToken().Type == TokenType.Print)
        {
            SourcePosition pos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
            Advance();
            Expression value = ParseExpression();
            Consume(TokenType.Semicolon);
            return new PrintStatement(value, pos);
        }
        else
        {
            Expression expr = ParseExpression();
            Consume(TokenType.Semicolon);
            return new ExpressionStatement(expr, new SourcePosition(CurrentToken().Line, CurrentToken().Column));
        }
    }
}