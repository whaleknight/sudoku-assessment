namespace Sudoku.Tests;

public class SudokuGeneratorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(7)]
    [InlineData(42)]
    [InlineData(12345)]
    public void Generate_CreatesExactlyThirtyClues_AndOneSolution(int seed)
    {
        var game = new SudokuGenerator(new Random(seed)).Generate();
        var grid = TestPuzzles.ReadGrid(game);

        Assert.Equal(30, grid.Cast<int>().Count(number => number != 0));
        Assert.Empty(SudokuValidator.GetViolations(grid));
        Assert.Equal(1, new SudokuSolver().CountSolutions(grid));
        Assert.False(game.IsComplete);

        for (var row = 0; row < 9; row++)
        for (var column = 0; column < 9; column++)
            Assert.Equal(grid[row, column] != 0, game.IsGiven(row, column));

        // Revealing all 51 missing values must finish a valid grid.
        for (var index = 0; index < 51; index++)
            Assert.NotNull(game.RevealHint());
        Assert.True(game.IsComplete);
        Assert.Null(game.RevealHint());
    }

    [Fact]
    public void Generate_UsesNewPuzzleStateOnEachCall()
    {
        var generator = new SudokuGenerator(new Random(42));
        var first = generator.Generate();
        var firstGrid = TestPuzzles.ReadGrid(first).Cast<int>().ToArray();
        var second = generator.Generate();

        Assert.NotEqual(firstGrid, TestPuzzles.ReadGrid(second).Cast<int>().ToArray());
        first.RevealHint();
        Assert.Equal(30, TestPuzzles.ReadGrid(second).Cast<int>().Count(number => number != 0));
    }
}
