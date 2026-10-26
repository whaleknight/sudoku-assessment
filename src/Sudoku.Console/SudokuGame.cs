namespace Sudoku;

public enum MoveResult
{
    Accepted,
    FixedCell,
    InvalidNumber,
    InvalidPosition
}

public class Hint
{
    public int Row { get; }
    public int Column { get; }
    public int Number { get; }

    public Hint(int row, int column, int number)
    {
        Row = row;
        Column = column;
        Number = number;
    }
}

public sealed class SudokuGame
{
    private readonly int[,] cells;
    private readonly int[,] solution;
    private readonly bool[,] givenCells = new bool[9, 9];

    public SudokuGame(int[,] puzzle, int[,] solution)
    {
        SudokuValidator.ValidateGrid(puzzle);
        SudokuValidator.ValidateGrid(solution);
        if (HasEmptyCells(solution) || SudokuValidator.GetViolations(solution).Count != 0)
            throw new ArgumentException("The solution must be a completed, valid Sudoku grid.", nameof(solution));

        for (var row = 0; row < 9; row++)
        for (var column = 0; column < 9; column++)
        {
            if (puzzle[row, column] != 0 && puzzle[row, column] != solution[row, column])
                throw new ArgumentException("Every starting clue must match the solution.", nameof(puzzle));
            givenCells[row, column] = puzzle[row, column] != 0;
        }

        cells = (int[,])puzzle.Clone();
        this.solution = (int[,])solution.Clone();
    }

    public bool IsComplete
    {
        get { return !HasEmptyCells(cells) && Check().Count == 0; }
    }

    public int GetCell(int row, int column)
    {
        ValidatePosition(row, column);
        return cells[row, column];
    }

    public bool IsGiven(int row, int column)
    {
        ValidatePosition(row, column);
        return givenCells[row, column];
    }

    public MoveResult PlaceNumber(int row, int column, int number)
    {
        if (!IsValidPosition(row, column))
            return MoveResult.InvalidPosition;
        if (givenCells[row, column])
            return MoveResult.FixedCell;
        if (number < 1 || number > 9)
            return MoveResult.InvalidNumber;

        // Conflicts are allowed here, as in the assessment's examples.
        // The check command reports them separately.
        cells[row, column] = number;
        return MoveResult.Accepted;
    }

    public MoveResult ClearCell(int row, int column)
    {
        if (!IsValidPosition(row, column))
            return MoveResult.InvalidPosition;
        if (givenCells[row, column])
            return MoveResult.FixedCell;

        cells[row, column] = 0;
        return MoveResult.Accepted;
    }

    public IReadOnlyList<string> Check() => SudokuValidator.GetViolations(cells);

    public Hint? RevealHint()
    {
        // Repair a wrong entry first. This also handles a full but invalid board.
        for (var row = 0; row < 9; row++)
        for (var column = 0; column < 9; column++)
            if (cells[row, column] != 0 && cells[row, column] != solution[row, column])
                return RevealCell(row, column);

        for (var row = 0; row < 9; row++)
        for (var column = 0; column < 9; column++)
            if (cells[row, column] == 0)
                return RevealCell(row, column);

        return null;
    }

    private Hint RevealCell(int row, int column)
    {
        cells[row, column] = solution[row, column];
        return new Hint(row, column, cells[row, column]);
    }

    private static bool HasEmptyCells(int[,] grid)
    {
        foreach (var number in grid)
        {
            if (number == 0)
                return true;
        }

        return false;
    }

    private static bool IsValidPosition(int row, int column) =>
        row >= 0 && row < 9 && column >= 0 && column < 9;

    private static void ValidatePosition(int row, int column)
    {
        if (row < 0 || row >= 9)
            throw new ArgumentOutOfRangeException(nameof(row));
        if (column < 0 || column >= 9)
            throw new ArgumentOutOfRangeException(nameof(column));
    }
}
