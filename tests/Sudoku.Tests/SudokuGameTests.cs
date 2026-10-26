namespace Sudoku.Tests;

public class SudokuGameTests
{
    [Fact]
    public void PlaceNumber_UpdatesAnEditableCell()
    {
        var game = TestPuzzles.NewGame();

        var result = game.PlaceNumber(0, 2, 4);

        Assert.Equal(MoveResult.Accepted, result);
        Assert.Equal(4, game.GetCell(0, 2));
    }

    [Fact]
    public void PlaceNumber_AllowsConflictsSoCheckCanReportThem()
    {
        var game = TestPuzzles.NewGame();

        Assert.Equal(MoveResult.Accepted, game.PlaceNumber(0, 2, 3));
        Assert.Contains("Number 3 already exists in Row A.", game.Check());
        Assert.False(game.IsComplete);
    }

    [Fact]
    public void GivenCells_CannotBeChangedOrCleared()
    {
        var game = TestPuzzles.NewGame();

        Assert.Equal(MoveResult.FixedCell, game.PlaceNumber(0, 0, 6));
        Assert.Equal(MoveResult.FixedCell, game.ClearCell(0, 0));
        Assert.Equal(5, game.GetCell(0, 0));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(int.MaxValue)]
    public void PlaceNumber_RejectsNumbersOutsideOneToNine(int number)
    {
        var game = TestPuzzles.NewGame();

        Assert.Equal(MoveResult.InvalidNumber, game.PlaceNumber(0, 2, number));
        Assert.Equal(0, game.GetCell(0, 2));
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(9, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 9)]
    public void Moves_RejectPositionsOutsideTheGrid(int row, int column)
    {
        var game = TestPuzzles.NewGame();

        Assert.Equal(MoveResult.InvalidPosition, game.PlaceNumber(row, column, 1));
        Assert.Equal(MoveResult.InvalidPosition, game.ClearCell(row, column));
    }

    [Fact]
    public void ClearCell_RemovesAnEnteredNumber_AndCanClearAnEmptyCell()
    {
        var game = TestPuzzles.NewGame();
        game.PlaceNumber(0, 2, 4);

        Assert.Equal(MoveResult.Accepted, game.ClearCell(0, 2));
        Assert.Equal(0, game.GetCell(0, 2));
        Assert.Equal(MoveResult.Accepted, game.ClearCell(0, 2));
    }

    [Fact]
    public void PlaceNumber_CanReplaceAnEarlierEntry()
    {
        var game = TestPuzzles.NewGame();
        game.PlaceNumber(0, 2, 3);

        Assert.Equal(MoveResult.Accepted, game.PlaceNumber(0, 2, 4));
        Assert.Empty(game.Check());
    }

    [Fact]
    public void Hint_RevealsOneCorrectNumber_AndLeavesItEditable()
    {
        var game = TestPuzzles.NewGame();

        var hint = game.RevealHint();

        Assert.NotNull(hint);
        Assert.Equal(0, hint.Row);
        Assert.Equal(2, hint.Column);
        Assert.Equal(4, hint.Number);
        Assert.Equal(4, game.GetCell(0, 2));
        Assert.False(game.IsGiven(0, 2));
        Assert.Equal(MoveResult.Accepted, game.ClearCell(0, 2));
    }

    [Fact]
    public void Hint_CorrectsAnEarlierWrongEntryBeforeFillingAnEmptyCell()
    {
        var game = TestPuzzles.NewGame();
        game.PlaceNumber(8, 0, 2);

        var hint = game.RevealHint();

        Assert.NotNull(hint);
        Assert.Equal(8, hint.Row);
        Assert.Equal(0, hint.Column);
        Assert.Equal(3, hint.Number);
        Assert.Equal(3, game.GetCell(8, 0));
        Assert.Equal(0, game.GetCell(0, 2));
    }

    [Fact]
    public void CompletedGame_HasNoFurtherHint()
    {
        var game = new SudokuGame(TestPuzzles.Solution(), TestPuzzles.Solution());

        Assert.True(game.IsComplete);
        Assert.Null(game.RevealHint());
    }

    [Fact]
    public void FullButInvalidGrid_IsNotComplete_AndAHintCanRepairIt()
    {
        var game = TestPuzzles.NewGame();
        var solution = TestPuzzles.Solution();
        for (var row = 0; row < 9; row++)
        for (var column = 0; column < 9; column++)
            if (!game.IsGiven(row, column))
                game.PlaceNumber(row, column, solution[row, column]);

        game.PlaceNumber(0, 2, 3);
        Assert.False(game.IsComplete);
        Assert.NotEmpty(game.Check());

        var hint = game.RevealHint();
        Assert.NotNull(hint);
        Assert.Equal(0, hint.Row);
        Assert.Equal(2, hint.Column);
        Assert.Equal(4, hint.Number);
        Assert.True(game.IsComplete);
    }

    [Fact]
    public void Constructor_CopiesBothInputArrays()
    {
        var puzzle = TestPuzzles.Puzzle();
        var solution = TestPuzzles.Solution();
        var game = new SudokuGame(puzzle, solution);
        puzzle[0, 0] = 1;
        solution[0, 2] = 9;

        Assert.Equal(5, game.GetCell(0, 0));
        Assert.True(game.IsGiven(0, 0));
        var hint = game.RevealHint();
        Assert.NotNull(hint);
        Assert.Equal(0, hint.Row);
        Assert.Equal(2, hint.Column);
        Assert.Equal(4, hint.Number);
    }

    [Fact]
    public void Constructor_RejectsCluesThatDoNotMatchTheSolution()
    {
        var puzzle = new int[9, 9];
        puzzle[0, 0] = 1;

        Assert.Throws<ArgumentException>(() => new SudokuGame(puzzle, TestPuzzles.Solution()));
    }

    [Fact]
    public void Constructor_RejectsAnIncompleteOrInvalidSolution()
    {
        Assert.Throws<ArgumentException>(() => new SudokuGame(TestPuzzles.Puzzle(), TestPuzzles.Puzzle()));
        var invalid = TestPuzzles.Solution();
        invalid[0, 0] = 3;
        Assert.Throws<ArgumentException>(() => new SudokuGame(TestPuzzles.Puzzle(), invalid));
    }

    [Fact]
    public void IncompleteButValidGrid_IsNotComplete()
    {
        var game = TestPuzzles.NewGame();

        Assert.Empty(game.Check());
        Assert.False(game.IsComplete);
    }
}
