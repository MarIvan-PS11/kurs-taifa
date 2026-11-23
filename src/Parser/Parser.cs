namespace MiniSharp.Parser;

using System.Collections.Generic;

using MiniSharp.Ast;
using MiniSharp.Lexer;

/// <summary>
/// Синтаксический анализатор, строящий абстрактное синтаксическое дерево.
/// </summary>
public sealed partial class Parser
{
    private readonly List<Token> _tokens;
    private int _position;

    /// <summary>
    /// Создаёт парсер для указанного потока токенов.
    /// </summary>
    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
    }

    /// <summary>
    /// program = "func", "main", "(", ")", block, end_of_file ;
    /// </summary>
    public ProgramNode ParseProgram()
    {
        SourcePosition startPos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
        Consume(TokenType.Func);
        Consume(TokenType.Main);
        Consume(TokenType.LeftParen);
        Consume(TokenType.RightParen);
        BlockStatement body = ParseBlock();
        Consume(TokenType.Eof);

        FunctionDeclaration mainFunc = new FunctionDeclaration("main", body, startPos);
        return new ProgramNode(mainFunc, startPos);
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