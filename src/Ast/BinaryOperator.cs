namespace MiniSharp.Ast;

/// <summary>
/// Перечисление бинарных операторов, используемых в узлах бинарных выражений AST.
/// </summary>
public enum BinaryOperator
{
    /// <summary>
    /// Оператор сложения '+'.
    /// </summary>
    Add,

    /// <summary>
    /// Оператор вычитания '-'.
    /// </summary>
    Subtract,

    /// <summary>
    /// Оператор умножения '*'.
    /// </summary>
    Multiply,

    /// <summary>
    /// Оператор целочисленного деления '/'.
    /// </summary>
    Divide,

    /// <summary>
    /// Оператор присваивания '=' (правоассоциативный).
    /// </summary>
    Assign,
}