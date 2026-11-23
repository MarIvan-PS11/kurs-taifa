namespace MiniSharp.Lexer;

using System.Globalization;

/// <summary>
/// Границы диапазона 64-битного знакового целого, общие для лексера и парсера.
/// </summary>
public static class IntegerLimits
{
    /// <summary>
    /// Максимальный модуль целочисленного литерала: 2^63.
    /// Допускается только после унарного минуса (для long.MinValue).
    /// </summary>
    public const ulong MaxLiteralMagnitude = 9223372036854775808UL;

    /// <summary>
    /// Лексема литерала с модулем 2^63 (без знака).
    /// </summary>
    public static readonly string MaxLiteralMagnitudeLexeme =
        MaxLiteralMagnitude.ToString(CultureInfo.InvariantCulture);
}