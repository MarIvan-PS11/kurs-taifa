namespace MiniSharp.Ast;

using System.Collections.Generic;

using MiniSharp.Lexer;

/// <summary>
/// Узел AST, представляющий объявление пользовательской функции.
/// </summary>
public sealed class FunctionDeclaration : Declaration
{
    /// <summary>
    /// Инициализирует новый экземпляр объявления функции.
    /// </summary>
    public FunctionDeclaration(string name, IReadOnlyList<string> parameters, BlockStatement body, SourcePosition position)
        : base(position)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
    }

    /// <summary>
    /// Имя функции.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Список имен параметров функции.
    /// </summary>
    public IReadOnlyList<string> Parameters { get; }

    /// <summary>
    /// Тело функции.
    /// </summary>
    public BlockStatement Body { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}