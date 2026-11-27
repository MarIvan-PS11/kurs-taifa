namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Узел AST, представляющий объявление переменной с инициализацией.
/// </summary>
public sealed class VariableDeclarationStatement : Statement
{
    /// <summary>
    /// Инициализирует новый экземпляр узла объявления переменной.
    /// </summary>
    public VariableDeclarationStatement(string name, Expression initializer, SourcePosition position)
        : base(position)
    {
        Name = name;
        Initializer = initializer;
    }

    /// <summary>
    /// Имя объявляемой переменной.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Выражение, инициализирующее переменную.
    /// </summary>
    public Expression Initializer { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}