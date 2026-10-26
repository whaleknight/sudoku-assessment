namespace Sudoku.Tests;

public class ConsoleGameTests
{
    [Fact]
    public void Run_HandlesMovesChecksClearHintInvalidInputAndQuit()
    {
        var (output, _) = Run("A1 6\nA1 clear\nA3 0\nA3 3\ncheck\nA3 clear\ncheck\nhint\nbad\nhelp\nquit\n");

        Assert.Contains("Welcome to Sudoku!", output);
        Assert.Contains("1 2 3 | 4 5 6 | 7 8 9", output);
        Assert.Contains("A 5 3 _", output);
        Assert.Contains("Invalid move. A1 is pre-filled.", output);
        Assert.Contains("The number must be between 1 and 9.", output);
        Assert.Contains("Move accepted.", output);
        Assert.Contains("Number 3 already exists in Row A.", output);
        Assert.Contains("Cell cleared.", output);
        Assert.Contains("No rule violations detected.", output);
        Assert.Contains("Hint: Cell A3 = 4", output);
        Assert.Contains("Unknown command.", output);
        Assert.Contains("Commands are case-insensitive.", output);
        Assert.Contains("Goodbye!", output);
        Assert.DoesNotContain("successfully completed", output);
    }

    [Fact]
    public void Run_CompletesThePuzzleUsingEnteredNumbers()
    {
        var solution = TestPuzzles.Solution();
        var puzzle = TestPuzzles.Puzzle();
        var commands = new List<string>();
        for (var row = 0; row < 9; row++)
        for (var column = 0; column < 9; column++)
            if (puzzle[row, column] == 0)
                commands.Add($"{(char)('A' + row)}{column + 1} {solution[row, column]}");
        commands.Add("n");

        var (output, generator) = Run(string.Join('\n', commands));

        Assert.Contains("You have successfully completed the Sudoku puzzle!", output);
        Assert.Contains("Play again? (y/n):", output);
        Assert.Contains("I 3 4 5 | 2 8 6 | 1 7 9", output);
        Assert.Equal(1, generator.CallCount);
    }

    [Fact]
    public void Run_CompletesWithHints_ThenStartsANewPuzzleOnYes()
    {
        var commands = string.Concat(Enumerable.Repeat("hint\n", 51)) + "y\ncheck\nquit\n";

        var (output, generator) = Run(commands);

        Assert.Contains("successfully completed", output);
        Assert.Equal(2, generator.CallCount);
        Assert.Equal(2, output.Split("Here is your puzzle:").Length - 1);
        Assert.Contains("No rule violations detected.", output);
    }

    [Fact]
    public void Run_ExitsWhenInputEnds()
    {
        var (output, _) = Run("");

        Assert.Contains("Goodbye!", output);
        Assert.DoesNotContain("successfully completed", output);
    }

    [Fact]
    public void Run_ExitsWhenInputEndsAtTheReplayPrompt()
    {
        var (output, generator) = Run(string.Concat(Enumerable.Repeat("hint\n", 51)));

        Assert.Contains("successfully completed", output);
        Assert.Contains("Goodbye!", output);
        Assert.Equal(1, generator.CallCount);
    }

    [Fact]
    public void Run_CompletesWithTheRealGeneratorAndHints()
    {
        using var input = new StringReader(string.Concat(Enumerable.Repeat("hint\n", 51)) + "n\n");
        using var output = new StringWriter();

        new ConsoleGame(input, output, new SudokuGenerator(new Random(17))).Run();

        Assert.Contains("You have successfully completed the Sudoku puzzle!", output.ToString());
    }

    private static (string Output, FixedPuzzleGenerator Generator) Run(string commands)
    {
        using var input = new StringReader(commands);
        using var output = new StringWriter();
        var generator = new FixedPuzzleGenerator();
        new ConsoleGame(input, output, generator).Run();
        return (output.ToString(), generator);
    }

    private sealed class FixedPuzzleGenerator : IPuzzleGenerator
    {
        public int CallCount { get; private set; }

        public SudokuGame Generate()
        {
            CallCount++;
            return TestPuzzles.NewGame();
        }
    }
}
