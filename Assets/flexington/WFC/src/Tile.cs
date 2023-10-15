using System.Collections.Generic;
using UnityEngine;

namespace flexington.WFC
{
    /// <summary>
    /// A tile in the wave function collapse grid.
    /// </summary>
    public class Tile
    {
        public List<Socket> Sockets { get; private set; }

        /// <summary>
        /// The GameObject of the tile.
        /// </summary>
        public GameObject GameObject => _sprite.gameObject;

        private SpriteRenderer _sprite;

        public Tile(SpriteRenderer spriteRenderer)
        {
            _sprite = spriteRenderer;
            MakeEdges();
        }

        /// <summary>
        /// Reads the color of the tile at three points along all edges and saves the hash of the color.
        /// </summary>
        private void MakeEdges()
        {
            // Get sprite width and height
            var width = _sprite.sprite.texture.width;
            var height = _sprite.sprite.texture.height;

            // Initialize sockets
            Sockets = new List<Socket>();

            var texture = GetTextureFromAtlas(_sprite.sprite.texture, _sprite.sprite.textureRect);

            var up = (texture.GetPixel(0, height - 1) +
                texture.GetPixel(width / 2, height - 1) +
                texture.GetPixel(width - 1, height - 1)
            ).GetHashCode();
            Sockets.Add(new Socket(Direction.Up, up));

            var right = (texture.GetPixel(width - 1, height - 1) +
                texture.GetPixel(width - 1, height / 2) +
                texture.GetPixel(width - 1, 0)
            ).GetHashCode();
            Sockets.Add(new Socket(Direction.Right, right));

            var down = (texture.GetPixel(width - 1, 0) +
                texture.GetPixel(width / 2, 0) +
                texture.GetPixel(0, 0)
            ).GetHashCode();
            Sockets.Add(new Socket(Direction.Down, down));

            var left = (texture.GetPixel(0, 0) +
                texture.GetPixel(0, height / 2) +
                texture.GetPixel(0, height - 1)
            ).GetHashCode();
            Sockets.Add(new Socket(Direction.Left, left));
        }

        /// <summary>
        /// Returns the sprite texture from a sprite atlas.
        /// </summary>
        /// <param name="atlas">The texture of the atlas.</param>
        /// <param name="rect">The rect of the sprite in the atlas.</param>
        /// <returns>The sprite texture.</returns>
        private Texture2D GetTextureFromAtlas(Texture2D atlas, Rect rect)
        {
            var texture = new Texture2D((int)rect.width, (int)rect.height);
            texture.SetPixels(atlas.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
            texture.Apply();
            return texture;
        }
    }
}