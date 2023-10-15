using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace flexington.WFC
{
    public class WaveFunctionCollapse
    {
        private Cell[] _grid;
        public Cell[] Grid => _grid;
        private Vector2Int _size;
        private List<TileOptionObject> _options;

        public WaveFunctionCollapse(Vector2Int size, List<TileOptionObject> options)
        {
            _options = options;
            _size = size;

            // Initialize grid as one-dimensional array
            _grid = new Cell[size.x * size.y];

            // Initialize each cell
            for (int i = 0; i < _grid.Length; i++)
            {
                _grid[i] = new Cell()
                {
                    Position = new Vector2Int(i % size.x, i / size.x),
                    IsCollapsed = false,
                    Options = new List<TileOptionObject>(_options)
                };
            }
        }

        public void Simulate()
        {
            while (_grid.Any(c => !c.IsCollapsed))
            {
                Cell cell = SelectNextCandidate(_grid);
                cell = Collapse(cell);
                _grid = ReduceNeighbours(_grid, cell.Position);
            }
        }

        public void Step()
        {
            if (!_grid.Any(c => !c.IsCollapsed)) throw new InvalidOperationException("Grid is already collapsed");

            Cell cell = SelectNextCandidate(_grid);
            cell = Collapse(cell);
            _grid = ReduceNeighbours(_grid, cell.Position);
        }

        /// <summary>
        /// Selects the next candidate cell to collapse.
        /// </summary>
        /// <param name="grid">The grid to select from.</param>
        /// <returns>The selected cell.</returns>
        private Cell SelectNextCandidate(Cell[] grid)
        {
            // Get all cells that are not collapsed
            grid = grid.Where(c => !c.IsCollapsed)
                .OrderBy(c => c.Options.Count).ToArray();

            // Get first cell
            var cell = grid.First();

            // Get all cells with the  same amount of options
            var sameOptions = grid.Where(c => c.Options.Count == cell.Options.Count).ToArray();

            // Get random cell from same options
            cell = sameOptions[UnityEngine.Random.Range(0, sameOptions.Length)];

            return cell;
        }

        /// <summary>
        /// Collapses the given cell.
        /// </summary>
        /// <param name="cell">The cell to collapse.</param>
        /// <returns>The updated cell.</returns>
        private Cell Collapse(Cell cell)
        {
            // If cell is already collapsed, return
            if (cell.IsCollapsed) return cell;

            // If cell has only one direction remaining, collapse it
            if (cell.Options.Count == 1)
            {
                cell.Option = cell.Options[0];
                cell.Options = null;
                cell.IsCollapsed = true;
                return cell;
            }

            // Select randomly from remaining options
            cell.Option = cell.Options[UnityEngine.Random.Range(0, cell.Options.Count)];

            // Set options null
            cell.Options = null;

            // Collapse cell
            cell.IsCollapsed = true;
            return cell;
        }

        /// <summary>
        /// Reduce options of the neighbours of the given cell.
        /// </summary>
        /// <param name="grid">The grid to reduce.</param>
        /// <param name="Vector2Int">Cell grid position.</param>
        /// <returns>The updated grid.</returns>
        private Cell[] ReduceNeighbours(Cell[] grid, Vector2Int cellPosition)
        {
            // Get cell
            var cell = grid[cellPosition.x + cellPosition.y * _size.x];

            ReduceOptions(grid, cell, Vector2Int.up);
            ReduceOptions(grid, cell, Vector2Int.right);
            ReduceOptions(grid, cell, Vector2Int.down);
            ReduceOptions(grid, cell, Vector2Int.left);

            return grid;
        }

        /// <summary>
        /// Reduces the options of a given position in the grid.
        /// </summary>
        /// <param name="grid">The grid to reduce.</param>
        /// <param name="position">The position to reduce.</param>
        /// <returns>The updated grid.</returns>
        private Cell[] ReduceOptions(Cell[] grid, Cell cell, Vector2Int offset)
        {
            // Check if position is within the grid
            var position = cell.Position + offset;
            if (position.x < 0 || position.x >= _size.x || position.y < 0 || position.y >= _size.y) return grid;

            // Get neighbour
            var neighbour = grid[position.x + position.y * _size.x];

            // Check if neighbour is already collapsed
            if (neighbour.IsCollapsed) return grid;

            // Get direction
            var direction = offset.x == 0 ? (offset.y == 1 ? Direction.Up : Direction.Down) : (offset.x == 1 ? Direction.Right : Direction.Left);

            // Remove all options of the neighbour where the given direction has not the same connection type
            var connection = cell.Option.Connection.Single(c => c.Directions == direction);

            // Get opposite direction
            var oppositeDirection = offset.x == 0 ? (offset.y == 1 ? Direction.Down : Direction.Up) : (offset.x == 1 ? Direction.Left : Direction.Right);

            for (int i = neighbour.Options.Count - 1; i >= 0; i--)
            {
                var option = neighbour.Options[i];
                var connections = option.Connection.Any(c => c.Directions == oppositeDirection && c.ConnectionType == connection.ConnectionType);
                if (!connections) neighbour.Options.RemoveAt(i);
            }

            return grid;
        }
    }
}