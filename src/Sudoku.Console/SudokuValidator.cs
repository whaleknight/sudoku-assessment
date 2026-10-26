namespace Sudoku;

public static class SudokuValidator
{
    public const int GridSize = 9;

    public static IReadOnlyList<string> GetViolations(int[,] grid)
    {
        ValidateGrid(grid);
        var violations = new List<string>();

        for (var index = 0; index < GridSize; index++)
        {
            var rowValues = new int[GridSize];
            var columnValues = new int[GridSize];
            for (var offset = 0; offset < GridSize; offset++)
            {
                rowValues[offset] = grid[index, offset];
                columnValues[offset] = grid[offset, index];
            }

            AddDuplicates(rowValues, $"Row {(char)('A' + index)}", violations);
            AddDuplicates(columnValues, $"Column {index + 1}", violations);
        }

        for (var row = 0; row < GridSize; row += 3)
        for (var column = 0; column < GridSize; column += 3)
        {
            var boxValues = new int[GridSize];
            var index = 0;
            for (var r = row; r < row + 3; r++)
            for (var c = column; c < column + 3; c++)
                boxValues[index++] = grid[r, c];

            var label = $"3x3 subgrid {(char)('A' + row)}{column + 1}-{(char)('A' + row + 2)}{column + 3}";
            AddDuplicates(boxValues, label, violations);
        }

        return violations;
    }

    internal static void ValidateGrid(int[,] grid)
    {
        ArgumentNullException.ThrowIfNull(grid);
        if (grid.GetLength(0) != GridSize || grid.GetLength(1) != GridSize)
            throw new ArgumentException("The grid must have 9 rows and 9 columns.", nameof(grid));

        foreach (var number in grid)
            if (number < 0 || number > 9)
                throw new ArgumentException("Cells must contain 0 (empty) or a number from 1 to 9.", nameof(grid));
    }

    internal static bool CanPlace(int[,] grid, int row, int column, int number)
    {
        for (var index = 0; index < GridSize; index++)
        {
            if (grid[row, index] == number || grid[index, column] == number)
                return false;
        }

        var startRow = row / 3 * 3;
        var startColumn = column / 3 * 3;
        for (var r = startRow; r < startRow + 3; r++)
        for (var c = startColumn; c < startColumn + 3; c++)
            if (grid[r, c] == number)
                return false;

        return true;
    }

    private static void AddDuplicates(int[] numbers, string unit, List<string> violations)
    {
        var counts = new int[10];
        foreach (var number in numbers)
        {
            if (number == 0)
                continue;

            counts[number]++;
            // Report a repeated number once per unit, even if it appears three times.
            if (counts[number] == 2)
                violations.Add($"Number {number} already exists in {unit}.");
        }
    }
}
