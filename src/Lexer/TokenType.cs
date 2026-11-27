namespace MiniSharp.Lexer;

/// <summary>
/// Перечисление типов лексем (токенов), распознаваемых лексическим анализатором.
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Идентификатор (имя переменной или пользовательской функции).
    /// </summary>
    Identifier,

    /// <summary>
    /// Целочисленный литерал.
    /// </summary>
    IntegerLiteral,

    /// <summary>
    /// Строковый литерал (зарезервировано).
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Оператор сложения '+'.
    /// </summary>
    Plus,

    /// <summary>
    /// Оператор вычитания или унарного минуса '-'.
    /// </summary>
    Minus,

    /// <summary>
    /// Оператор умножения '*'.
    /// </summary>
    Star,

    /// <summary>
    /// Оператор деления '/'.
    /// </summary>
    Slash,

    /// <summary>
    /// Оператор присваивания '='.
    /// </summary>
    Assign,

    /// <summary>
    /// Оператор равенства '==' (зарезервировано).
    /// </summary>
    Equal,

    /// <summary>
    /// Оператор неравенства '!=' (зарезервировано).
    /// </summary>
    NotEqual,

    /// <summary>
    /// Логическое отрицание '!' (зарезервировано).
    /// </summary>
    Not,

    /// <summary>
    /// Оператор 'меньше или равно' '<=' (зарезервировано).
    /// </summary>
    LessEqual,

    /// <summary>
    /// Оператор 'меньше' '<' (зарезервировано).
    /// </summary>
    Less,

    /// <summary>
    /// Оператор 'больше или равно' '>=' (зарезервировано).
    /// </summary>
    GreaterEqual,

    /// <summary>
    /// Оператор 'больше' '>' (зарезервировано).
    /// </summary>
    Greater,

    /// <summary>
    /// Логическое 'И' '&&' (зарезервировано).
    /// </summary>
    And,

    /// <summary>
    /// Логическое 'ИЛИ' '||' (зарезервировано).
    /// </summary>
    Or,

    /// <summary>
    /// Открывающая круглая скобка '('.
    /// </summary>
    LeftParen,

    /// <summary>
    /// Закрывающая круглая скобка ')'.
    /// </summary>
    RightParen,

    /// <summary>
    /// Открывающая фигурная скобка '{'.
    /// </summary>
    LeftBrace,

    /// <summary>
    /// Закрывающая фигурная скобка '}'.
    /// </summary>
    RightBrace,

    /// <summary>
    /// Точка с запятой ';'.
    /// </summary>
    Semicolon,

    /// <summary>
    /// Запятая ','.
    /// </summary>
    Comma,

    /// <summary>
    /// Ключевое слово 'func'.
    /// </summary>
    Func,

    /// <summary>
    /// Ключевое слово 'main'.
    /// </summary>
    Main,

    /// <summary>
    /// Ключевое слово 'return'.
    /// </summary>
    Return,

    /// <summary>
    /// Ключевое слово 'print'.
    /// </summary>
    Print,

    /// <summary>
    /// Ключевое слово 'readInt'.
    /// </summary>
    ReadInt,

    /// <summary>
    /// Ключевое слово 'let' (зарезервировано).
    /// </summary>
    Let,

    /// <summary>
    /// Ключевое слово 'int' — единственный поддерживаемый тип данных.
    /// </summary>
    Int,

    /// <summary>
    /// Ключевое слово 'if' (зарезервировано).
    /// </summary>
    If,

    /// <summary>
    /// Ключевое слово 'else' (зарезервировано).
    /// </summary>
    Else,

    /// <summary>
    /// Ключевое слово 'while' (зарезервировано).
    /// </summary>
    While,

    /// <summary>
    /// Логический литерал 'true' (зарезервировано).
    /// </summary>
    True,

    /// <summary>
    /// Логический литерал 'false' (зарезервировано).
    /// </summary>
    False,

    /// <summary>
    /// Конец файла.
    /// </summary>
    Eof,
}