using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace flexington.WFC
{
    public class WaveFunctionCollapseComponent : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private Vector2Int _size;

        [SerializeField] private List<TileOption> _options;

        private Cell[] _grid;

        public void Start()
        {
            // Initialize grid as one-dimensional array
            _grid = CreateGrid(_size);

            int i = 0;
            int iMax = 100 + (_size.x * _size.y);
            while (_grid.Any(c => !c.IsCollapsed))
            {
                Cell cell = SelectNextCandidate(_grid);
                cell = Collapse(cell);
                _grid = ReduceNeighbours(_grid, cell.Position);
                Debug.Log($"Collapsed cell at {cell.Position} with direction {cell.Direction}");
                i++;
            }
            Debug.Log($"Collapsed {_grid.Count(c => c.IsCollapsed)} cells in {i} iterations");

            // Draw collapsed cells
            DrawCollapsedCells(_grid, _options);
        }

        private void DebugCollapse()
        {
            // Set first cell to collapsed
            _grid[0].IsCollapsed = true;
            _grid[0].Direction = Directions.Up;
        }

        /// <summary>
        /// Creates a grid of cells and initializes them with all possible directions.
        /// </summary>
        /// <param name="size">The size of the grid.</param>
        /// <returns>The grid.</returns>
        private Cell[] CreateGrid(Vector2Int size)
        {
            // Initialize grid as one-dimensional array
            Cell[] grid = new Cell[size.x * size.y];

            // Initialize each cell
            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = new Cell()
                {
                    Position = new Vector2Int(i % size.x, i / size.x),
                    IsCollapsed = false,
                    Direction = Directions.Blank | Directions.Up | Directions.Down | Directions.Left | Directions.Right
                };
            }

            return grid;
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
                .OrderBy(c => CountDirections(c.Direction)).ToArray();

            // Get first cell
            var cell = grid.First();

            // Get all cells with the  same amount of options
            var sameOptions = grid.Where(c => CountDirections(c.Direction) == CountDirections(cell.Direction)).ToArray();

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
            if (CountDirections(cell.Direction) == 1)
            {
                cell.IsCollapsed = true;
                return cell;
            }

            // Get all selected falgs
            var options = new BitArray(new[] { (int)cell.Direction }).Cast<bool>().ToArray();

            // Get index of elements that are true
            var optionsIndices = options.Select((b, i) => b ? i : -1).Where(i => i != -1).ToArray();

            var randomFlag = optionsIndices[UnityEngine.Random.Range(0, optionsIndices.Length)];

            // Set direction
            cell.Direction = (Directions)(1 << randomFlag);

            // Collapse cell
            cell.IsCollapsed = true;
            return cell;
        }

        /// <summary>
        /// Updates the given cell in the grid.
        /// </summary>
        /// <param name="grid">The grid to update.</param>
        /// <param name="cell">The cell to update.</param>
        /// <param name="position">The position of the cell.</param>
        /// <returns>The updated grid.</returns>
        private Cell[] UpdateCell(Cell[] grid, Cell cell, Vector2Int position)
        {
            // Get cell
            grid[position.x + position.y * _size.x] = cell;
            return grid;
        }

        /// <summary>
        /// Resets the grid.
        /// </summary>
        public void Reset()
        {
            // Destroy all children
            while (transform.childCount > 0)
            {
                // If in Editor, destroy the child
                if (Application.isEditor) DestroyImmediate(transform.GetChild(0).gameObject);
                else Destroy(transform.GetChild(0).gameObject);
            }

            _grid = null;
            Debug.ClearDeveloperConsole();
        }

        /// <summary>
        /// Draws the collapsed cells in the grid.
        /// </summary>
        /// <param name="grid">The grid to draw.</param>
        /// <param name="options">The available options.</param>
        private void DrawCollapsedCells(Cell[] grid, List<TileOption> options)
        {
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    var cell = grid[x + y * _size.x];
                    if (cell.IsCollapsed)
                    {
                        var tile = options.First(o => o.Direction == cell.Direction).Tile;
                        var tileObject = Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity);
                        tileObject.transform.parent = transform;
                    }
                }
            }
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

            // Get valid neighbours
            var validNeighbours = _options.Single(o => o.Direction == cell.Direction);

            // Top Neighbour
            var topNeighbourPosition = cellPosition + Vector2Int.up;
            if (topNeighbourPosition.y < _size.y)
            {
                var topNeighbour = grid[topNeighbourPosition.x + topNeighbourPosition.y * _size.x];
                if (!topNeighbour.IsCollapsed)
                {
                    topNeighbour.Direction &= ~GetInvalidDirections(validNeighbours.ValidNeighboursTop);
                }
            }

            // Right Neighbour
            var rightNeighbourPosition = cellPosition + Vector2Int.right;
            if (rightNeighbourPosition.x < _size.x)
            {
                var rightNeighbour = grid[rightNeighbourPosition.x + rightNeighbourPosition.y * _size.x];
                if (!rightNeighbour.IsCollapsed) rightNeighbour.Direction &= ~GetInvalidDirections(validNeighbours.ValidNeighboursRight);
            }

            // Bottom Neighbour
            var bottomNeighbourPosition = cellPosition + Vector2Int.down;
            if (bottomNeighbourPosition.y >= 0)
            {
                var bottomNeighbour = grid[bottomNeighbourPosition.x + bottomNeighbourPosition.y * _size.x];
                if (!bottomNeighbour.IsCollapsed) bottomNeighbour.Direction &= ~GetInvalidDirections(validNeighbours.ValidNeighboursBottom);
            }

            // Left Neighbour
            var leftNeighbourPosition = cellPosition + Vector2Int.left;
            if (leftNeighbourPosition.x >= 0)
            {
                var leftNeighbour = grid[leftNeighbourPosition.x + leftNeighbourPosition.y * _size.x];
                if (!leftNeighbour.IsCollapsed) leftNeighbour.Direction &= ~GetInvalidDirections(validNeighbours.ValidNeighboursLeft);
            }

            return grid;
        }

        /// <summary>
        /// Returns the Directions to be removed from a cell's options.
        /// </summary>
        /// <param name="directions">The valid directions.</param>
        /// <returns>The invalid directions.</returns>
        private Directions GetInvalidDirections(Directions directions)
        {

            return (Directions.Blank | Directions.Up | Directions.Down | Directions.Left | Directions.Right) & (~directions);
        }

        /// <summary>
        /// Counts the number of directions in the given directions.
        /// </summary>
        /// <param name="directions">The directions to count.</param>
        /// <returns>The number of directions.</returns>
        private int CountDirections(Directions directions)
        {
            return new BitArray(new[] { (int)directions }).Cast<bool>().Count(b => b);
        }

        /// <summary>
        /// Steps the simulation in the editor.
        /// </summary>
        public void Editor_Step()
        {
            if (_grid == null) _grid = CreateGrid(_size);

            if (_grid.Any(c => !c.IsCollapsed))
            {
                Debug.Log("Grid is already collapsed");
                return;
            }

            // Destroy all children
            while (transform.childCount > 0)
            {
                // If in Editor, destroy the child
                if (Application.isEditor) DestroyImmediate(transform.GetChild(0).gameObject);
                else Destroy(transform.GetChild(0).gameObject);
            }

            Cell cell = SelectNextCandidate(_grid);
            cell = Collapse(cell);
            _grid = ReduceNeighbours(_grid, cell.Position);
            Debug.Log($"Collapsed cell at {cell.Position} with direction {cell.Direction}");

            DrawCollapsedCells(_grid, _options);
        }
    }
}

public class Cell
{
    public Vector2Int Position { get; set; }
    public bool IsCollapsed { get; set; }
    public Directions Direction { get; set; }
}

[Flags]
public enum Directions
{
    Blank = 1 << 0,
    Up = 1 << 1,
    Down = 1 << 2,
    Left = 1 << 3,
    Right = 1 << 4
}

[Serializable]
public class TileOption
{
    [SerializeField] private TileComponent _tile;
    public TileComponent Tile
    {
        get { return _tile; }
        set { _tile = value; }
    }

    [SerializeField] private Directions _direction;
    public Directions Direction
    {
        get { return _direction; }
        set { _direction = value; }
    }

    [SerializeField] private Directions _validNeighboursTop;
    public Directions ValidNeighboursTop
    {
        get { return _validNeighboursTop; }
        set { _validNeighboursTop = value; }
    }

    [SerializeField] private Directions _validNeighboursRight;
    public Directions ValidNeighboursRight
    {
        get { return _validNeighboursRight; }
        set { _validNeighboursRight = value; }
    }

    [SerializeField] private Directions _validNeighboursBottom;
    public Directions ValidNeighboursBottom
    {
        get { return _validNeighboursBottom; }
        set { _validNeighboursBottom = value; }
    }

    [SerializeField] private Directions _validNeighboursLeft;
    public Directions ValidNeighboursLeft
    {
        get { return _validNeighboursLeft; }
        set { _validNeighboursLeft = value; }
    }
}
