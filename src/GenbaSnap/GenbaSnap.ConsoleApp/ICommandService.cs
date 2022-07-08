namespace GenbaSnap.ConsoleApp
{
    /// <summary>
    /// Abstraction of Console
    /// </summary>
    internal interface ICommandService
    {
        public void ReadKey();
        public string? ReadLine();

        public void WriteLine();
        public void WriteLine(string? value);

    }
}
