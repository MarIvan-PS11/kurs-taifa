namespace MiniSharp.Parser;

using MiniSharp.Lexer;

/// <summary>
/// Синтаксический анализатор с немедленной интерпретацией (рекурсивный спуск).
/// </summary>
public sealed partial class Parser
{
    private readonly List<Token> _tokens;
    private int _position;
    private readonly IEnvironment _env;

    public Parser(List<Token> tokens, IEnvironment env)
    {
        _tokens = tokens;
        _position = 0;
        _env = env;
    }

    /// <summary>
    /// program = "func", "main", "(", ")", block, end_of_file ;
    /// </summary>
    public void ParseProgram()
    {
        Consume(TokenType.Func);
        Consume(TokenType.Main);
        Consume(TokenType.LeftParen);
        Consume(TokenType.RightParen);
        ParseBlock();
        Consume(TokenType.Eof);
    }

    private Token CurrentToken()
    {
        if (_position >= _tokens.Count)
        {
            return _tokens[_tokens.Count - 1];
        }

        return _tokens[_position];
    }

    private void Advance()
    {
        if (_position < _tokens.Count - 1)
        {
            _position++;
        }
    }

    private void Consume(TokenType expectedType)
    {
        if (CurrentToken().Type == expectedType)
        {
            Advance();
        }
        else
        {
            throw new ParserException($"Ожидался токен '{expectedType}', но получен '{CurrentToken().Type}'", CurrentToken());
        }
    }
}