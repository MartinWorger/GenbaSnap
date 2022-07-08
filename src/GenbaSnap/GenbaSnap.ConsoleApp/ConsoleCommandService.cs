namespace GenbaSnap.ConsoleApp
{
    /// <summary>
    /// ICommandService implementation using <see cref="Console"/>.
    /// </summary>
    internal class ConsoleCommandService : ICommandService
    {
        public void ReadKey()
        {
            Console.ReadKey();
        }

        public string? ReadLine()
        {
            return Console.ReadLine();
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }
        public void WriteLine(string? value)
        {
            Console.WriteLine(value);
        }
    }
}
