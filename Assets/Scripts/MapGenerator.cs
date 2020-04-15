using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using CharacterController = Character.CharacterController;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{
    public int height;

    private List<int[,]> maps;

    public float seed;

    public TileBase tile;
    public Tilemap tilemap;
    public int width;

    private CharacterController _controller => GameObject.FindGameObjectWithTag("Player")
        .GetComponent<CharacterController>();

    /// <summary>
    /// </summary>
    private void Start()
    {
        maps = new List<int[,]>();
        Generate();

        /* place controller */
        var startTile = MapUtils.FindTopTile(tilemap, 0);
        if (startTile != null)
        {
            var pos = startTile.Value;
            _controller.transform.position = tilemap.CellToWorld(
                new Vector3Int(pos.x + 1, pos.y + 2, pos.z));
        }
    }

    /// <summary>
    /// </summary>
    private void Update()
    {
        if (maps.Count == 0) return;

        var position = tilemap.WorldToCell(_controller.transform.position);
        var factor = maps.Count == 1 ? 0.5f : (float) (maps.Count - 1) / maps.Count;
        var sector = maps.Count * width * factor;
        if (position.x > sector)
        {
            var newMap = GenerateArray(width, height, true);
            newMap = RandomWalkTop(newMap);
            maps.Add(newMap);

            for (var i = 0; i < maps.Count; i++)
            {
                var map = maps[i];
                RenderMap(i * width, map);
            }
        }
    }

    /// <summary>
    /// </summary>
    private void Generate()
    {
        var map = GenerateArray(width, height, true);
        map = RandomWalkTop(map);
        maps.Add(map);

        RenderMap(0, map);
    }

    /// <summary>
    ///     Generates an int array of the supplied width and height
    /// </summary>
    /// <param name="width">How wide you want the array</param>
    /// <param name="height">How high you want the array</param>
    /// <param name="empty"></param>
    /// <returns>The map array initialised</returns>
    private int[,] GenerateArray(int width, int height, bool empty)
    {
        var map = new int[width, height];
        for (var x = 0; x < map.GetUpperBound(0); x++)
        for (var y = 0; y < map.GetUpperBound(1); y++)
            if (empty)
                map[x, y] = 0;
            else
                map[x, y] = 1;
        return map;
    }

    /// <summary>
    ///     Draws the map to the screen
    /// </summary>
    /// <param name="start"></param>
    /// <param name="map">Map that we want to draw</param>
    /// <param name="tilemap">Tilemap we will draw onto</param>
    /// <param name="tile">Tile we will draw with</param>
    private void RenderMap(int start, int[,] map)
    {
        /* create connection tile */
        CreateConnection(start - 1);

        for (var x = 0; x < map.GetUpperBound(0); x++) //Loop through the width of the map
        for (var y = 0; y < map.GetUpperBound(1); y++) //Loop through the height of the map
            if (map[x, y] == 1) // 1 = tile, 0 = no tile
                tilemap.SetTile(new Vector3Int(start + x, y, 0), tile);
    }

    /// <summary>
    /// </summary>
    private void CreateConnection(int x)
    {
        var topTile = MapUtils.FindTopTile(tilemap, x - 1);
        if (topTile != null)
        {
            var position = topTile.Value;
            for (var i = position.y; i >= 0; i--) tilemap.SetTile(new Vector3Int(x, i, 0), tile);
        }
    }

    /// <summary>
    ///     Generates the top layer of our level using Random Walk
    /// </summary>
    /// <param name="map">Map that we are using to generate</param>
    /// <param name="seed">The seed we will use in our random</param>
    /// <returns>The random walk map generated</returns>
    private int[,] RandomWalkTop(int[,] map)
    {
        //Seed our random
        var rand = new Random(seed.GetHashCode());

        //Set our starting height
        var lastHeight = UnityEngine.Random.Range(0, map.GetUpperBound(1));

        //Cycle through our width
        for (var x = 0; x < map.GetUpperBound(0); x++)
        {
            //Flip a coin
            var nextMove = rand.Next(2);

            //If heads, and we aren't near the bottom, minus some height
            if (nextMove == 0 && lastHeight > 2)
                lastHeight--;
            //If tails, and we aren't near the top, add some height
            else if (nextMove == 1 && lastHeight < map.GetUpperBound(1) - 2) lastHeight++;

            //Circle through from the last height to the bottom
            for (var y = lastHeight; y >= 0; y--) map[x, y] = 1;
        }

        //Return the map
        return map;
    }
}