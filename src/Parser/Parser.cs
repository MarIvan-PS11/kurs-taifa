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
    /// Инициализирует парсер для указанного потока токенов.
    /// </summary>
    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
    }

    /// <summary>
    /// Разбирает всю программу, включая пользовательские функции и main.
    /// </summary>
    public ProgramNode ParseProgram()
    {
        SourcePosition startPos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
        List<FunctionDeclaration> functions = new List<FunctionDeclaration>();

        while (CurrentToken().Type == TokenType.Func)
        {
            if (PeekNext().Type == TokenType.Main)
            {
                break;
            }

            functions.Add(ParseFunctionDeclaration());
        }

        Consume(TokenType.Func);
        Consume(TokenType.Main);
        Consume(TokenType.LeftParen);
        Consume(TokenType.RightParen);
        BlockStatement body = ParseBlock();
        Consume(TokenType.Eof);

        FunctionDeclaration mainFunc = new FunctionDeclaration("main", new List<string>(), body, startPos);
        return new ProgramNode(functions, mainFunc, startPos);
    }

    private Token PeekNext()
    {
        int nextPos = _position + 1;
        return nextPos < _tokens.Count ? _tokens[nextPos] : _tokens[_tokens.Count - 1];
    }

    private FunctionDeclaration ParseFunctionDeclaration()
    {
        SourcePosition pos = new SourcePosition(CurrentToken().Line, CurrentToken().Column);
        Consume(TokenType.Func);
        string name = CurrentToken().Lexeme;
        Consume(TokenType.Identifier);
        Consume(TokenType.LeftParen);

        List<string> parameters = new List<string>();
        if (CurrentToken().Type != TokenType.RightParen)
        {
            do
            {
                parameters.Add(CurrentToken().Lexeme);
                Consume(TokenType.Identifier);
            }
            while (ConsumeIf(TokenType.Comma));
        }

        Consume(TokenType.RightParen);
        BlockStatement body = ParseBlock();

        return new FunctionDeclaration(name, parameters, body, pos);
    }

    private bool ConsumeIf(TokenType type)
    {
        if (CurrentToken().Type == type)
        {
            Advance();
            return true;
        }

        return false;
    }

    private Token CurrentToken() => _position < _tokens.Count ? _tokens[_position] : _tokens[_tokens.Count - 1];

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