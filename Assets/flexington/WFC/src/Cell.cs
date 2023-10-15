using System.Collections.Generic;
using UnityEngine;

namespace flexington.WFC
{
    /// <summary>
    /// A cell in the wave function collapse grid.
    /// Holds the information required to collapse and render a tile.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// The position of the cell in the grid.
        /// </summary>
        public Vector2Int Position { get; set; }

        /// <summary>
        /// Whether or not the cell has been collapsed.
        /// </summary>
        public bool IsCollapsed { get; set; }

        /// <summary>
        /// Holds the direction and valid connections for each side of the tile.
        /// </summary>
        public List<TileOptionObject> Options { get; set; }

        /// <summary>
        /// The selected option after the cell has been collapsed.
        /// </summary>
        public TileOptionObject Option { get; set; }
    }
}