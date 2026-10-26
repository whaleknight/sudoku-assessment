namespace Sudoku.Tests;

public class SudokuSolverTests
{
    private readonly SudokuSolver solver = new();

    [Fact]
    public void CountSolutions_ReturnsOneForAKnownUniquePuzzle_WithoutChangingIt()
    {
        var grid = TestPuzzles.Puzzle();
        var before = grid.Cast<int>().ToArray();

        Assert.Equal(1, solver.CountSolutions(grid));
        Assert.Equal(before, grid.Cast<int>().ToArray());
    }

    [Fact]
    public void CountSolutions_ReturnsOneForACompletedValidGrid()
    {
        Assert.Equal(1, solver.CountSolutions(TestPuzzles.Solution()));
    }

    [Fact]
    public void CountSolutions_StopsAtTwoWhenThereAreMultipleSolutions()
    {
        Assert.Equal(2, solver.CountSolutions(new int[9, 9]));
    }

    [Fact]
    public void CountSolutions_ReturnsZeroForConflictingClues()
    {
        var grid = TestPuzzles.Puzzle();
        grid[0, 2] = 3;

        Assert.Equal(0, solver.CountSolutions(grid));
    }

    [Fact]
    public void CountSolutions_ReturnsZeroForAnUnsolvableGridWithoutDuplicates()
    {
        var grid = new int[9, 9];
        for (var column = 0; column < 8; column++)
            grid[0, column] = column + 1;
        grid[1, 8] = 9;

        Assert.Empty(SudokuValidator.GetViolations(grid));
        Assert.Equal(0, solver.CountSolutions(grid));
    }
}
