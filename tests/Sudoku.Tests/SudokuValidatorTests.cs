namespace Sudoku.Tests;

public class SudokuValidatorTests
{
    [Fact]
    public void Check_AcceptsAnEmptyGrid_AndAValidPartialGrid()
    {
        Assert.Empty(SudokuValidator.GetViolations(new int[9, 9]));
        Assert.Empty(SudokuValidator.GetViolations(TestPuzzles.Puzzle()));
        Assert.Empty(SudokuValidator.GetViolations(TestPuzzles.Solution()));
    }

    [Fact]
    public void Check_ReportsARowDuplicate()
    {
        var grid = TestPuzzles.Puzzle();
        grid[0, 2] = 3;

        Assert.Contains("Number 3 already exists in Row A.", SudokuValidator.GetViolations(grid));
    }

    [Fact]
    public void Check_ReportsAColumnDuplicate()
    {
        var grid = TestPuzzles.Puzzle();
        grid[2, 0] = 5;

        Assert.Contains("Number 5 already exists in Column 1.", SudokuValidator.GetViolations(grid));
    }

    [Fact]
    public void Check_ReportsASubgridDuplicateWithoutARowOrColumnDuplicate()
    {
        var grid = new int[9, 9];
        grid[0, 0] = 8;
        grid[1, 1] = 8;

        Assert.Equal(new[] { "Number 8 already exists in 3x3 subgrid A1-C3." },
            SudokuValidator.GetViolations(grid));
    }

    [Fact]
    public void Check_ReportsEachDuplicateNumberOncePerUnit()
    {
        var grid = new int[9, 9];
        grid[0, 0] = grid[0, 3] = grid[0, 6] = 2;

        Assert.Equal(new[] { "Number 2 already exists in Row A." },
            SudokuValidator.GetViolations(grid));
    }

    [Fact]
    public void Check_CoversTheLastRowColumnAndSubgrid()
    {
        var grid = new int[9, 9];
        grid[8, 8] = grid[8, 7] = grid[7, 8] = 9;

        var violations = SudokuValidator.GetViolations(grid);

        Assert.Contains("Number 9 already exists in Row I.", violations);
        Assert.Contains("Number 9 already exists in Column 9.", violations);
        Assert.Contains("Number 9 already exists in 3x3 subgrid G7-I9.", violations);
    }

    [Fact]
    public void Check_RejectsMalformedGrids()
    {
        Assert.Throws<ArgumentNullException>(() => SudokuValidator.GetViolations(null!));
        Assert.Throws<ArgumentException>(() => SudokuValidator.GetViolations(new int[8, 9]));
        var grid = new int[9, 9];
        grid[0, 0] = 10;
        Assert.Throws<ArgumentException>(() => SudokuValidator.GetViolations(grid));
        grid[0, 0] = -1;
        Assert.Throws<ArgumentException>(() => SudokuValidator.GetViolations(grid));
    }
}
