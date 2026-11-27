namespace MiniSharp.Execution;

using System;

/// <summary>
/// Исключение, используемое для немедленного возврата значения из функции.
/// </summary>
public sealed class ReturnException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса ReturnException.
    /// </summary>
    public ReturnException()
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса ReturnException с указанным сообщением.
    /// </summary>
    public ReturnException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса ReturnException с сообщением и внутренним исключением.
    /// </summary>
    public ReturnException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Инициализирует исключение с возвращаемым значением.
    /// </summary>
    public ReturnException(long value)
        : base("Return from function")
    {
        Value = value;
    }

    /// <summary>
    /// Возвращаемое значение.
    /// </summary>
    public long Value { get; }
}