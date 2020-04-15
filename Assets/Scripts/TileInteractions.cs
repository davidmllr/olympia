using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using CharacterController = Character.CharacterController;

public class TileInteractions : MonoBehaviour
{
    public TileBase DefaultTile;
    public float generationTime;
    private bool isCoroutineRunning;

    private Vector3Int lastTile;

    [Header("Procedural Values")] public int offset;

    public TileBase PosTile;
    public int size;

    [Header("Tilemap")] public Tilemap Tilemap;

    private CharacterController _controller => GameObject.FindGameObjectWithTag("Player")
        .GetComponent<CharacterController>();

    /// <summary>
    /// </summary>
    private void Update()
    {
        var playerPosOrig = _controller.transform.position;
        var playerPos = Tilemap.WorldToCell(
            new Vector3(playerPosOrig.x, playerPosOrig.y, playerPosOrig.z));


        var tilePos = new Vector3Int(playerPos.x, playerPos.y - 1, playerPos.z);
        var playerTile = Tilemap.GetTile(tilePos);

        if (playerTile != null)
            if (lastTile != tilePos)
            {
                Tilemap.SetTile(tilePos, PosTile);
                Tilemap.SetTile(lastTile, DefaultTile);
                lastTile = tilePos;
            }

        /* find tile for offset */

        var x = playerPos.x + Random.Range(-offset, offset);
        // find highest y for x
        var highestTile = MapUtils.FindTopTile(Tilemap, x);
        if (highestTile != null)
            if (!isCoroutineRunning)
            {
                isCoroutineRunning = true;

                var position = highestTile.Value;

                var coin = Random.Range(0, 10);

                StartCoroutine(coin >= 5
                    ? RemoveTilesFromTop(position)
                    : AddTilesOnTop(position));
            }
    }

    /// <summary>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="size"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    private IEnumerator AddTilesOnTop(Vector3Int position)
    {
        for (var i = 0; i < size; i++)
        {
            yield return new WaitForSeconds(generationTime);
            var pos = new Vector3Int(position.x, position.y + i, position.z);
            Tilemap.SetTile(pos, DefaultTile);
            yield return null;
        }

        isCoroutineRunning = false;
    }

    /// <summary>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="size"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    private IEnumerator RemoveTilesFromTop(Vector3Int position)
    {
        for (var i = 0; i < size; i++)
        {
            yield return new WaitForSeconds(generationTime);
            var pos = new Vector3Int(position.x, position.y - i, position.z);
            Tilemap.SetTile(pos, null);
            yield return null;
        }

        isCoroutineRunning = false;
    }
}