namespace Sudoku;

public sealed class ConsoleGame
{
    private readonly TextReader input;
    private readonly TextWriter output;
    private readonly IPuzzleGenerator generator;

    public ConsoleGame(TextReader input, TextWriter output, IPuzzleGenerator generator)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(generator);
        this.input = input;
        this.output = output;
        this.generator = generator;
    }

    public void Run()
    {
        output.WriteLine("Welcome to Sudoku!");
        while (true)
        {
            var game = generator.Generate();
            output.WriteLine();
            output.WriteLine("Here is your puzzle:");
            WriteGrid(game);

            while (!game.IsComplete)
            {
                output.WriteLine();
                output.WriteLine("Enter command (e.g., A3 4, C5 clear, hint, check, help, quit):");
                var line = input.ReadLine();
                if (line == null)
                {
                    output.WriteLine("Goodbye!");
                    return;
                }

                var command = CommandParser.Parse(line);
                if (command.Type == CommandType.Quit)
                {
                    output.WriteLine("Goodbye!");
                    return;
                }

                if (Execute(game, command))
                {
                    output.WriteLine();
                    output.WriteLine("Current grid:");
                    WriteGrid(game);
                }
            }

            output.WriteLine("You have successfully completed the Sudoku puzzle!");
            output.WriteLine("Play again? (y/n):");
            if (!string.Equals(input.ReadLine()?.Trim(), "y", StringComparison.OrdinalIgnoreCase))
            {
                output.WriteLine("Goodbye!");
                return;
            }
        }
    }

    private bool Execute(SudokuGame game, GameCommand command)
    {
        switch (command.Type)
        {
            case CommandType.Place:
                return WriteMoveResult(game.PlaceNumber(command.Row, command.Column, command.Number), command);
            case CommandType.Clear:
                return WriteMoveResult(game.ClearCell(command.Row, command.Column), command);
            case CommandType.Hint:
                var hint = game.RevealHint();
                output.WriteLine(hint == null
                    ? "There are no cells left to reveal."
                    : $"Hint: Cell {(char)('A' + hint.Row)}{hint.Column + 1} = {hint.Number}");
                return hint != null;
            case CommandType.Check:
                var violations = game.Check();
                if (violations.Count == 0)
                    output.WriteLine("No rule violations detected.");
                else
                    foreach (var violation in violations)
                        output.WriteLine(violation);
                break;
            case CommandType.Help:
                output.WriteLine("A3 4: enter a number. C5 clear: empty a cell. hint: reveal one correct number.");
                output.WriteLine("check: report duplicates. quit: leave the game. Commands are case-insensitive.");
                output.WriteLine("Starting clues cannot be changed. Conflicting entries are allowed; use check to find them.");
                output.WriteLine("A hint corrects an earlier wrong entry first, otherwise it fills an empty cell.");
                break;
            default:
                output.WriteLine(command.Error);
                break;
        }

        return false;
    }

    private bool WriteMoveResult(MoveResult result, GameCommand command)
    {
        var position = $"{(char)('A' + command.Row)}{command.Column + 1}";
        output.WriteLine(result switch
        {
            MoveResult.Accepted => command.Type == CommandType.Clear ? "Cell cleared." : "Move accepted.",
            MoveResult.FixedCell => $"Invalid move. {position} is pre-filled.",
            MoveResult.InvalidNumber => "Invalid move. The number must be between 1 and 9.",
            _ => "Invalid move. The cell must be between A1 and I9."
        });
        return result == MoveResult.Accepted;
    }

    private void WriteGrid(SudokuGame game)
    {
        output.WriteLine("    1 2 3 | 4 5 6 | 7 8 9");
        for (var row = 0; row < 9; row++)
        {
            if (row > 0 && row % 3 == 0)
                output.WriteLine("    ------+-------+------");
            output.Write($"  {(char)('A' + row)} ");
            for (var column = 0; column < 9; column++)
            {
                if (column > 0 && column % 3 == 0)
                    output.Write("| ");
                var number = game.GetCell(row, column);
                output.Write(number == 0 ? "_" : number.ToString());
                if (column < 8)
                    output.Write(' ');
            }
            output.WriteLine();
        }
    }
}
