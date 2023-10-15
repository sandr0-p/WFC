using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace flexington.WFC
{
    public class WaveFunctionCollapse
    {
        /// <summary>
        /// The grid of cells.
        /// </summary>
        public Cell[] Grid => _grid;
        private Cell[] _grid;

        private Vector2Int _size;
        private List<SpriteRenderer> _options;

        public WaveFunctionCollapse(Vector2Int size, List<SpriteRenderer> options)
        {
            this._size = size;
            this._options = options;

            _grid = CreateGrid(size, options);
        }

        /// <summary>
        /// Creates and initializes a new grid.
        /// </summary>
        /// <param name="size">The size of the grid.</param>
        /// <param name="options">The available options.</param>
        /// <returns>The created grid.</returns>
        private Cell[] CreateGrid(Vector2Int size, List<SpriteRenderer> options)
        {
            var grid = new Cell[size.x * size.y];

            var tiles = new List<Tile>();
            foreach (var option in options)
            {
                var tile = new Tile(option);
                tiles.Add(tile);
            }

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    grid[Index(x, y)] = new Cell
                    {
                        Position = new Vector2Int(x, y),
                        IsCollapsed = false,
                        Tiles = new List<Tile>(tiles)
                    };
                }
            }

            return grid;
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
                .OrderBy(c => c.Tiles.Count).ToArray();

            // Get first cell
            var cell = grid.First();

            // Get all cells with the  same amount of options
            var sameOptions = grid.Where(c => c.Tiles.Count == cell.Tiles.Count).ToArray();

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
            if (cell.Tiles.Count == 1)
            {
                cell.Tile = cell.Tiles[0];
                cell.Tiles = null;
                cell.IsCollapsed = true;
                return cell;
            }

            // Select randomly from remaining options
            cell.Tile = cell.Tiles[UnityEngine.Random.Range(0, cell.Tiles.Count)];

            // Set options null
            cell.Tiles = null;

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
            var cell = grid[Index(cellPosition.x, cellPosition.y)];

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
            var neighbour = grid[Index(position.x, position.y)];

            // Check if neighbour is already collapsed
            if (neighbour.IsCollapsed) return grid;

            // Get direction
            var direction = offset.x == 0 ? (offset.y == 1 ? Direction.Up : Direction.Down) : (offset.x == 1 ? Direction.Right : Direction.Left);

            // Remove all options of the neighbour where the given direction has not the same connection type
            var connection = cell.Tile.Sockets.Single(c => c.Direction == direction);

            // Get opposite direction
            var oppositeDirection = offset.x == 0 ? (offset.y == 1 ? Direction.Down : Direction.Up) : (offset.x == 1 ? Direction.Left : Direction.Right);

            for (int i = neighbour.Tiles.Count - 1; i >= 0; i--)
            {
                var option = neighbour.Tiles[i];
                var connections = option.Sockets.Any(c => c.Direction == oppositeDirection && c.Hash == connection.Hash);
                if (!connections) neighbour.Tiles.RemoveAt(i);
            }

            return grid;
        }

        private int Index(int x, int y)
        {
            return x + y * _size.x;
        }
    }
}