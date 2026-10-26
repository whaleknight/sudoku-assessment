namespace Sudoku;

internal static class Program
{
    private static int Main()
    {
        try
        {
            var game = new ConsoleGame(Console.In, Console.Out, new SudokuGenerator());
            game.Run();
            return 0;
        }
        catch (IOException exception)
        {
            Console.Error.WriteLine($"Unable to read or write to the console: {exception.Message}");
            return 1;
        }
        catch (InvalidOperationException exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }
}
