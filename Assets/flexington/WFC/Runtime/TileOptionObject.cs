using System;
using System.Collections.Generic;
using UnityEngine;

namespace flexington.WFC
{
    [CreateAssetMenu(fileName = "TileOption", menuName = "flexington/WFC/TileOption", order = 0)]
    public class TileOptionObject : ScriptableObject
    {
        [SerializeField] private TileComponent _tile;
        public TileComponent Tile
        {
            get { return _tile; }
            set { _tile = value; }
        }

        [SerializeField] private Direction _direction;
        public Direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }


        [SerializeField] private List<Connection> _connection;
        public List<Connection> Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }
    }

    [Serializable]
    public class Connection
    {
        [SerializeField] private Direction _direction;
        public Direction Directions
        {
            get { return _direction; }
            set { _direction = value; }
        }

        [SerializeField] private ConnectionType _connection;
        public ConnectionType ConnectionType
        {
            get { return _connection; }
            set { _connection = value; }
        }
    }

    public enum ConnectionType
    {
        None,
        Blank,
        Street
    }
}