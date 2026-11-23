namespace MiniSharp.Execution
{
    using MiniSharp.Lexer;

    /// <summary>
    /// Исключение, возникающее во время выполнения программы.
    /// </summary>
    public sealed class InterpreterException : Exception
    {
        /// <summary>
        /// Создаёт исключение времени выполнения без сообщения.
        /// </summary>
        public InterpreterException()
        {
        }

        /// <summary>
        /// Создаёт исключение времени выполнения с указанным сообщением.
        /// </summary>
        public InterpreterException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Создаёт исключение времени выполнения с сообщением и внутренним исключением.
        /// </summary>
        public InterpreterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Создаёт исключение времени выполнения, привязанное к позиции в исходном коде.
        /// </summary>
        public InterpreterException(string message, SourcePosition position)
            : base($"{message} (строка {position.Line}, столбец {position.Column})")
        {
            Position = position;
        }

        /// <summary>
        /// Позиция в коде, где произошла ошибка.
        /// </summary>
        public SourcePosition Position { get; } = new SourcePosition(0, 0);
    }
}