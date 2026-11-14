namespace MiniSharp.Parser;

/// <summary>
/// Интерфейс для абстракции ввода-вывода, используемый парсером.
/// </summary>
public interface IEnvironment
{
    void Print(long value);

    long ReadInt();
}