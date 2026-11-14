namespace MiniSharp.Interpreter.AcceptanceTests;

using System;
using System.IO;

using Xunit;

/// <summary>
/// Приёмочные тесты консольного запуска интерпретатора целиком:
/// код возврата, stdout и stderr для успешных и аварийных сценариев.
/// </summary>
public sealed class ProgramRunTests
{
    /// <summary>
    /// Проверяет успешное выполнение программы из файла: код 0, вывод в stdout, пустой stderr.
    /// </summary>
    [Fact]
    public void Run_ValidProgram_ReturnsZeroAndWritesOutput()
    {
        string path = CreateTempSourceFile("func main() { print 6 * 7; }");
        using StringReader input = new StringReader(string.Empty);
        using StringWriter output = new StringWriter();
        using StringWriter error = new StringWriter();
        output.NewLine = "\n";

        int exitCode = Program.Run(new[] { path }, input, output, error);

        File.Delete(path);
        Assert.Equal(0, exitCode);
        Assert.Equal("42\n", output.ToString());
        Assert.Equal(string.Empty, error.ToString());
    }

    /// <summary>
    /// Проверяет чтение числа из потока ввода через ConsoleEnvironment.
    /// </summary>
    [Fact]
    public void Run_ProgramWithReadint_ReadsFromInputStream()
    {
        string path = CreateTempSourceFile("func main() { print readint() + 1; }");
        using StringReader input = new StringReader("41\n");
        using StringWriter output = new StringWriter();
        using StringWriter error = new StringWriter();
        output.NewLine = "\n";

        int exitCode = Program.Run(new[] { path }, input, output, error);

        File.Delete(path);
        Assert.Equal(0, exitCode);
        Assert.Equal("42\n", output.ToString());
    }

    /// <summary>
    /// Проверяет код возврата 1 и сообщение в stderr при ошибке лексического анализа.
    /// </summary>
    [Fact]
    public void Run_LexerError_ReturnsOneAndWritesStderr()
    {
        string path = CreateTempSourceFile("func main() { print @; }");
        using StringReader input = new StringReader(string.Empty);
        using StringWriter output = new StringWriter();
        using StringWriter error = new StringWriter();

        int exitCode = Program.Run(new[] { path }, input, output, error);

        File.Delete(path);
        Assert.Equal(1, exitCode);
        Assert.Equal(string.Empty, output.ToString());
        Assert.Contains("Ошибка лексического анализа", error.ToString());
    }

    /// <summary>
    /// Проверяет код возврата 1 и сообщение в stderr при ошибке разбора.
    /// </summary>
    [Fact]
    public void Run_ParseError_ReturnsOneAndWritesStderr()
    {
        string path = CreateTempSourceFile("func main() { print ; }");
        using StringReader input = new StringReader(string.Empty);
        using StringWriter output = new StringWriter();
        using StringWriter error = new StringWriter();

        int exitCode = Program.Run(new[] { path }, input, output, error);

        File.Delete(path);
        Assert.Equal(1, exitCode);
        Assert.Equal(string.Empty, output.ToString());
        Assert.Contains("Ошибка разбора", error.ToString());
    }

    /// <summary>
    /// Проверяет код возврата 1 и сообщение в stderr при ошибке выполнения.
    /// </summary>
    [Fact]
    public void Run_RuntimeError_ReturnsOneAndWritesStderr()
    {
        string path = CreateTempSourceFile("func main() { print 1 / 0; }");
        using StringReader input = new StringReader(string.Empty);
        using StringWriter output = new StringWriter();
        using StringWriter error = new StringWriter();

        int exitCode = Program.Run(new[] { path }, input, output, error);

        File.Delete(path);
        Assert.Equal(1, exitCode);
        Assert.Equal(string.Empty, output.ToString());
        Assert.Contains("Ошибка выполнения", error.ToString());
    }

    /// <summary>
    /// Проверяет код возврата 1 при некорректных данных на входе readint.
    /// </summary>
    [Fact]
    public void Run_InvalidInput_ReturnsOneAndWritesStderr()
    {
        string path = CreateTempSourceFile("func main() { print readint(); }");
        using StringReader input = new StringReader("abc\n");
        using StringWriter output = new StringWriter();
        using StringWriter error = new StringWriter();

        int exitCode = Program.Run(new[] { path }, input, output, error);

        File.Delete(path);
        Assert.Equal(1, exitCode);
        Assert.Contains("Ошибка выполнения", error.ToString());
    }

    /// <summary>
    /// Проверяет код возврата 1 при исчерпанном потоке ввода для readint.
    /// </summary>
    [Fact]
    public void Run_EmptyInputForReadint_ReturnsOneAndWritesStderr()
    {
        string path = CreateTempSourceFile("func main() { print readint(); }");
        using StringReader input = new StringReader(string.Empty);
        using StringWriter output = new StringWriter();
        using StringWriter error = new StringWriter();

        int exitCode = Program.Run(new[] { path }, input, output, error);

        File.Delete(path);
        Assert.Equal(1, exitCode);
        Assert.Contains("Ошибка выполнения", error.ToString());
    }

    /// <summary>
    /// Проверяет код возврата 1 и подсказку об использовании при отсутствующем аргументе.
    /// </summary>
    [Fact]
    public void Run_MissingArgument_ReturnsOneAndWritesUsage()
    {
        using StringReader input = new StringReader(string.Empty);
        using StringWriter output = new StringWriter();
        using StringWriter error = new StringWriter();

        int exitCode = Program.Run(Array.Empty<string>(), input, output, error);

        Assert.Equal(1, exitCode);
        Assert.Contains("Использование:", error.ToString());
    }

    /// <summary>
    /// Проверяет код возврата 1 при несуществующем файле с исходным кодом.
    /// </summary>
    [Fact]
    public void Run_MissingFile_ReturnsOneAndWritesError()
    {
        string path = Path.Combine(Path.GetTempPath(), $"minisharp-missing-{Guid.NewGuid():N}.ms");
        using StringReader input = new StringReader(string.Empty);
        using StringWriter output = new StringWriter();
        using StringWriter error = new StringWriter();

        int exitCode = Program.Run(new[] { path }, input, output, error);

        Assert.Equal(1, exitCode);
        Assert.Contains("не найден", error.ToString());
    }

    /// <summary>
    /// Создаёт временный файл с исходным кодом программы и возвращает путь к нему.
    /// </summary>
    private static string CreateTempSourceFile(string source)
    {
        string path = Path.Combine(Path.GetTempPath(), $"minisharp-{Guid.NewGuid():N}.ms");
        File.WriteAllText(path, source);
        return path;
    }
}