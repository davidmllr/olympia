using UnityEngine;
using UnityEngine.Tilemaps;
using CharacterController = Character.CharacterController;

namespace Map
{
    public class MapHandler : MonoBehaviour
    {
        public TileBase tile;
        public Tilemap tilemap;
        public int width;

        private CharacterController _controller => GameObject.FindGameObjectWithTag("Player")
            .GetComponent<CharacterController>();
    }
}