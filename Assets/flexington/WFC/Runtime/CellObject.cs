using System.Collections.Generic;
using UnityEngine;

namespace flexington.WFC
{

    [CreateAssetMenu(fileName = "Cell", menuName = "flexington/WFC/Cell", order = 0)]
    public class CellObject : ScriptableObject
    {
        [SerializeField] private List<TileComponent> _availableTiles;
        public List<TileComponent> AvailableTiles
        {
            get { return _availableTiles; }
            set { _availableTiles = value; }
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}