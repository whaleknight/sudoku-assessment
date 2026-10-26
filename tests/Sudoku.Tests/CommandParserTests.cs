namespace Sudoku.Tests;

public class CommandParserTests
{
    [Theory]
    [InlineData("B3 7", 1, 2, 7)]
    [InlineData(" a1   9 ", 0, 0, 9)]
    [InlineData("I9\t1", 8, 8, 1)]
    public void Parse_ReadsACellAndNumber(string input, int row, int column, int number)
    {
        var command = CommandParser.Parse(input);

        Assert.Equal(CommandType.Place, command.Type);
        Assert.Equal(row, command.Row);
        Assert.Equal(column, command.Column);
        Assert.Equal(number, command.Number);
    }

    [Theory]
    [InlineData("C5 clear")]
    [InlineData(" c5 CLEAR ")]
    public void Parse_ReadsAClearCommand(string input)
    {
        var command = CommandParser.Parse(input);

        Assert.Equal(CommandType.Clear, command.Type);
        Assert.Equal(2, command.Row);
        Assert.Equal(4, command.Column);
    }

    [Theory]
    [InlineData("hint", CommandType.Hint)]
    [InlineData(" CHECK ", CommandType.Check)]
    [InlineData("Help", CommandType.Help)]
    [InlineData("quit", CommandType.Quit)]
    public void Parse_ReadsSingleWordCommands(string input, CommandType expected)
    {
        Assert.Equal(expected, CommandParser.Parse(input).Type);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("unknown")]
    [InlineData("A3")]
    [InlineData("A3 4 extra")]
    [InlineData("A10 1")]
    [InlineData("J1 1")]
    [InlineData("A0 1")]
    [InlineData("1A 1")]
    [InlineData("A3 seven")]
    [InlineData("A3 4.5")]
    [InlineData("A3 9999999999999999999999")]
    [InlineData("hint now")]
    [InlineData("quit now")]
    public void Parse_RejectsMalformedInputWithoutThrowing(string input)
    {
        var command = CommandParser.Parse(input);

        Assert.Equal(CommandType.Invalid, command.Type);
        Assert.False(string.IsNullOrWhiteSpace(command.Error));
    }
}
