namespace MiniSharp.Execution
{
    /// <summary>
    /// Интерфейс для абстракции ввода-вывода, используемый вычислителем AST.
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// Печатает целое число в стандартный поток вывода.
        /// </summary>
        void Print(long value);

        /// <summary>
        /// Читает целое число из стандартного потока ввода для readInt().
        /// </summary>
        long ReadInt();
    }
}