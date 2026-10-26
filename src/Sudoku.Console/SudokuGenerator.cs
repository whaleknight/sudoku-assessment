namespace Sudoku;

public interface IPuzzleGenerator
{
    SudokuGame Generate();
}

public sealed class SudokuGenerator : IPuzzleGenerator
{
    private const int ClueCount = 30;
    private const int MaxAttempts = 20;
    private readonly Random random;
    private readonly SudokuSolver solver = new();

    public SudokuGenerator(Random? random = null)
    {
        this.random = random ?? Random.Shared;
    }

    public SudokuGame Generate()
    {
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var solution = CreateSolution();
            var puzzle = (int[,])solution.Clone();
            var positions = Enumerable.Range(0, 81).ToArray();
            Shuffle(positions);
            var remaining = 81;

            foreach (var position in positions)
            {
                var row = position / 9;
                var column = position % 9;
                var number = puzzle[row, column];
                puzzle[row, column] = 0;

                // Removing clues can introduce a second solution. Keep only
                // removals that leave exactly one, so hints are unambiguous.
                if (solver.CountSolutions(puzzle) != 1)
                {
                    puzzle[row, column] = number;
                    continue;
                }

                if (--remaining == ClueCount)
                    return new SudokuGame(puzzle, solution);
            }
        }

        throw new InvalidOperationException("Unable to generate a 30-clue puzzle. Please restart the game.");
    }

    private int[,] CreateSolution()
    {
        var rows = ShuffledGroups();
        var columns = ShuffledGroups();
        var numbers = Enumerable.Range(1, 9).ToArray();
        Shuffle(numbers);
        var grid = new int[9, 9];

        // This pattern creates a valid grid. Permuting digits, bands/stacks,
        // and rows/columns within each group preserves all Sudoku rules.
        for (var row = 0; row < 9; row++)
        for (var column = 0; column < 9; column++)
            grid[row, column] = numbers[(rows[row] * 3 + rows[row] / 3 + columns[column]) % 9];

        return grid;
    }

    private int[] ShuffledGroups()
    {
        var groups = new[] { 0, 1, 2 };
        Shuffle(groups);
        var indexes = new List<int>();
        foreach (var group in groups)
        {
            var offsets = new[] { 0, 1, 2 };
            Shuffle(offsets);
            indexes.AddRange(offsets.Select(offset => group * 3 + offset));
        }

        return indexes.ToArray();
    }

    private void Shuffle(int[] values)
    {
        for (var index = values.Length - 1; index > 0; index--)
        {
            var other = random.Next(index + 1);
            (values[index], values[other]) = (values[other], values[index]);
        }
    }
}
