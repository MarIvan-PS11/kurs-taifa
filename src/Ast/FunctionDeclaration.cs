namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Объявление функции с именем и телом.
/// </summary>
public sealed class FunctionDeclaration : Declaration
{
    /// <summary>
    /// Создаёт объявление функции.
    /// </summary>
    public FunctionDeclaration(string name, BlockStatement body, SourcePosition position)
        : base(position)
    {
        Name = name;
        Body = body;
    }

    /// <summary>
    /// Имя функции.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Тело функции в виде блока инструкций.
    /// </summary>
    public BlockStatement Body { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}