namespace Sudoku;

public sealed class SudokuSolver
{
    // Only 0, 1, or 2 is needed: 2 means at least two solutions.
    public int CountSolutions(int[,] puzzle)
    {
        if (SudokuValidator.GetViolations(puzzle).Count != 0)
            return 0;

        return Search((int[,])puzzle.Clone(), 2);
    }

    private static int Search(int[,] grid, int limit)
    {
        var bestRow = -1;
        var bestColumn = -1;
        List<int>? bestCandidates = null;

        // Try the most constrained empty cell first to reduce backtracking.
        for (var row = 0; row < 9; row++)
        for (var column = 0; column < 9; column++)
        {
            if (grid[row, column] != 0)
                continue;

            var candidates = GetCandidates(grid, row, column);
            if (candidates.Count == 0)
                return 0;

            if (bestCandidates == null || candidates.Count < bestCandidates.Count)
            {
                bestRow = row;
                bestColumn = column;
                bestCandidates = candidates;
            }
        }

        if (bestCandidates == null)
            return 1;

        var count = 0;
        foreach (var number in bestCandidates)
        {
            grid[bestRow, bestColumn] = number;
            count += Search(grid, limit - count);
            grid[bestRow, bestColumn] = 0;
            if (count >= limit)
                break;
        }

        return count;
    }

    private static List<int> GetCandidates(int[,] grid, int row, int column)
    {
        var candidates = new List<int>();
        for (var number = 1; number <= 9; number++)
            if (SudokuValidator.CanPlace(grid, row, column, number))
                candidates.Add(number);

        return candidates;
    }
}
