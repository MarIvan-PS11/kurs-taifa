namespace MiniSharp.Parser;

using MiniSharp.Lexer;

/// <summary>
/// Часть парсера, отвечающая за разбор инструкций и блоков.
/// </summary>
public sealed partial class Parser
{
    /// <summary>
    /// block = "{", { statement }, "}" ;
    /// </summary>
    private void ParseBlock()
    {
        Consume(TokenType.LeftBrace);
        while (CurrentToken().Type != TokenType.RightBrace && CurrentToken().Type != TokenType.Eof)
        {
            ParseStatement();
        }

        Consume(TokenType.RightBrace);
    }

    /// <summary>
    /// statement = "print", expression, ";" | expression, ";" ;
    /// </summary>
    private void ParseStatement()
    {
        if (CurrentToken().Type == TokenType.Print)
        {
            Advance();
            long value = ParseExpression();
            Consume(TokenType.Semicolon);
            _env.Print(value);
        }
        else
        {
            ParseExpression();
            Consume(TokenType.Semicolon);
        }
    }
}