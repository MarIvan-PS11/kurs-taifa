namespace MiniSharp.Ast;

/// <summary>
/// Интерфейс посетителя для обхода узлов абстрактного синтаксического дерева.
/// Реализация определяет семантику обхода (интерпретация, генерация кода и т.п.).
/// </summary>
public interface IAstVisitor
{
    /// <summary>Посещает корневой узел программы.</summary>
    void Visit(ProgramNode node);

    /// <summary>Посещает объявление функции.</summary>
    void Visit(FunctionDeclaration node);

    /// <summary>Посещает блок инструкций.</summary>
    void Visit(BlockStatement node);

    /// <summary>Посещает инструкцию вывода значения.</summary>
    void Visit(PrintStatement node);

    /// <summary>Посещает инструкцию-выражение.</summary>
    void Visit(ExpressionStatement node);

    /// <summary>Посещает целочисленный литерал.</summary>
    void Visit(IntegerLiteralExpression node);

    /// <summary>Посещает унарное выражение.</summary>
    void Visit(UnaryExpression node);

    /// <summary>Посещает бинарное выражение.</summary>
    void Visit(BinaryExpression node);

    /// <summary>Посещает вызов функции (встроенной или пользовательской).</summary>
    void Visit(CallExpression node);
}