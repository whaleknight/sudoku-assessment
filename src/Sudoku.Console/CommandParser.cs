using System.Globalization;

namespace Sudoku;

public enum CommandType
{
    Invalid,
    Place,
    Clear,
    Hint,
    Check,
    Help,
    Quit
}

public class GameCommand
{
    public CommandType Type { get; }
    public int Row { get; }
    public int Column { get; }
    public int Number { get; }
    public string? Error { get; }

    public GameCommand(CommandType type, int row = -1, int column = -1,
        int number = 0, string? error = null)
    {
        Type = type;
        Row = row;
        Column = column;
        Number = number;
        Error = error;
    }
}

public static class CommandParser
{
    public static GameCommand Parse(string input)
    {
        var parts = input.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            switch (parts[0].ToLowerInvariant())
            {
                case "hint":
                    return new GameCommand(CommandType.Hint);
                case "check":
                    return new GameCommand(CommandType.Check);
                case "help":
                    return new GameCommand(CommandType.Help);
                case "quit":
                    return new GameCommand(CommandType.Quit);
            }
        }

        if (parts.Length != 2)
            return Invalid("Unknown command. Use A3 4, C5 clear, hint, check, help, or quit.");

        var position = parts[0].ToUpperInvariant();
        if (position.Length != 2 || position[0] < 'A' || position[0] > 'I' ||
            position[1] < '1' || position[1] > '9')
            return Invalid("Invalid cell. Use a row from A-I and a column from 1-9, such as B3.");

        var row = position[0] - 'A';
        var column = position[1] - '1';
        if (parts[1].Equals("clear", StringComparison.OrdinalIgnoreCase))
            return new GameCommand(CommandType.Clear, row, column);

        if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
            return Invalid("Invalid number. Enter a whole number from 1 to 9, or use clear.");

        return new GameCommand(CommandType.Place, row, column, number);
    }

    private static GameCommand Invalid(string error)
    {
        return new GameCommand(CommandType.Invalid, error: error);
    }
}
