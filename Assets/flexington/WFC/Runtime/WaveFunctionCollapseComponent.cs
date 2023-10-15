using System.Collections.Generic;
using UnityEngine;

namespace flexington.WFC
{
    public class WaveFunctionCollapseComponent : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private Vector2Int _size;

        [SerializeField] private List<SpriteRenderer> _tiles;

        private WaveFunctionCollapse _wfc;

        public void Start()
        {
            _wfc = new WaveFunctionCollapse(_size, _tiles);
            _wfc.Simulate();
            DrawCollapsedCells(_wfc.Grid);
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

            _wfc = null;
        }

        /// <summary>
        /// Draws the collapsed cells in the grid.
        /// </summary>
        /// <param name="grid">The grid to draw.</param>
        /// <param name="options">The available options.</param>
        private void DrawCollapsedCells(Cell[] grid)
        {
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    var cell = grid[x + y * _size.x];
                    if (cell.IsCollapsed)
                    {
                        var tile = cell.Tile.GameObject;
                        var tileObject = Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity);
                        tileObject.transform.parent = transform;
                    }
                }
            }
        }

        /// <summary>
        /// Steps the simulation in the editor.
        /// </summary>
        public void Editor_Step()
        {
            if (_wfc == null) _wfc = new WaveFunctionCollapse(_size, _tiles);

            _wfc.Step();
            DeleteChildren();
            DrawCollapsedCells(_wfc.Grid);
        }

        private void DeleteChildren()
        {
            while (transform.childCount > 0)
            {
                // If in Editor, destroy the child
                if (Application.isEditor) DestroyImmediate(transform.GetChild(0).gameObject);
                else Destroy(transform.GetChild(0).gameObject);
            }
        }
    }
}