using System.Collections.Generic;
using Audio.Spectrum.Destroyable;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Audio.Spectrum
{
    /// <summary>
    /// This class is used to convert the given audio spectres to Tilemaps for the game.
    /// This class is derived from a class by jesse-scam, where it was called PlotController.
    /// It was adapted for my use.
    /// Please find the original here: https://github.com/jesse-scam/algorithmic-beat-mapping-unity/blob/master/Assets/Lib/Internal/PlotController.cs
    /// </summary>
    public class TileController : MonoBehaviour
    {
        
        private readonly List<Vector3> positions = new List<Vector3>();
        
        private float difficulty = 1f;

        [SerializeField] private int displayWindowSize = 100;
        
        [SerializeField] private TileBase baseTile;
        [SerializeField] private Tilemap baseTilemap;
        [SerializeField] private TileBase fluxTile;
        [SerializeField] private Tilemap fluxTilemap;
        
        [SerializeField] private TileBase peakTile;
        [SerializeField] private Tilemap peakTilemap;
        [SerializeField] private TileBase threshTile;
        [SerializeField] private Tilemap threshTileMap;

        public float multiplier = 40f;
        public float width = 1f;

        /// <summary>
        /// Initializes the TileController by building the positions array and instantiating a base Tilemap.
        /// </summary>
        public void Initialize()
        {
            for (var i = 0; i < displayWindowSize; i++)
            {
                var pointX = displayWindowSize / 2 * -1 * width + i * width;
                var pos = new Vector3(pointX, 0, 0);
                positions.Add(pos);
                baseTilemap.SetTile(baseTilemap.LocalToCell(new Vector3(pos.x, -5, pos.z)), baseTile);
            }
        }

        /// <summary>
        /// Every frame, all of the provided Tilemaps are cleared and updated.
        /// A window is calculated by using the given window size.
        /// For each plot inside the window, new positions for peaks, fluxs and thresholds will be calculated.
        /// </summary>
        /// <param name="pointInfo">Spectral flux information for the whole track</param>
        /// <param name="curIndex">Current time index</param>
        public void UpdateTiles(List<SpectralFluxInfo> pointInfo, int curIndex = -1)
        {
            if (positions.Count < displayWindowSize - 1)
                return;

            var numPlotted = 0;
            int windowStart;
            int windowEnd;

            /* calculate a window to use for updating our tilemaps */
            if (curIndex > 0)
            {
                windowStart = Mathf.Max(0, curIndex - displayWindowSize / 2);
                windowEnd = Mathf.Min(curIndex + displayWindowSize / 2, pointInfo.Count - 1);
            }
            else
            {
                windowStart = Mathf.Max(0, pointInfo.Count - displayWindowSize - 1);
                windowEnd = Mathf.Min(windowStart + displayWindowSize, pointInfo.Count);
            }

            peakTilemap.ClearAllTiles();
            threshTileMap.ClearAllTiles();
            fluxTilemap.ClearAllTiles();

            for (var i = windowStart; i < windowEnd; i++)
            {
                var plotIndex = numPlotted;
                numPlotted++;

                var isPeak = pointInfo[i].isPeak;
                var peakPos = Calculate(positions[plotIndex], isPeak ? pointInfo[i].spectralFlux : 0f);
                var fluxPos = Calculate(positions[plotIndex], isPeak ? 0f : pointInfo[i].spectralFlux);
                var threshPos = Calculate(positions[plotIndex], pointInfo[i].threshold);

                if (isPeak)
                {
                    SetConditionalTile(peakTilemap, peakTile, peakPos);
                }
                else
                {
                    if (fluxPos.y > threshPos.y) SetConditionalTile(fluxTilemap, fluxTile, fluxPos);
                }

                SetTile(threshTileMap, threshTile, threshPos);
            }
        }
        
        /// <summary>
        /// Calculates a new vector combining the old vector and the height for the given position.
        /// </summary>
        /// <param name="position">Given position</param>
        /// <param name="height">Given height</param>
        /// <returns>Calculated vector</returns>
        private Vector3 Calculate(Vector3 position, float height)
        {
            return new Vector3(position.x, height * multiplier * difficulty, position.z);
        }

        /// <summary>
        /// For the given Tilemap, set a conditional tile at the given position.
        /// Conditional tiles can be points or obstacles.
        /// They are not placed, if they were destroyed in earlier frames.
        /// </summary>
        /// <param name="tileMap">Given Tilemap</param>
        /// <param name="tile">Given Tile</param>
        /// <param name="position">Given position</param>
        private void SetConditionalTile(Tilemap tileMap, TileBase tile, Vector3 position)
        {
            var pos = tileMap.LocalToCell(position);
            if (!DestroyHandler.Instance.IsAvailable(pos)) return;

            tileMap.SetTile(pos, tile);
            var obj = tileMap.GetInstantiatedObject(pos);

            if (obj.name.Contains("Note")) obj.GetComponent<Note>().position = pos;
            else obj.GetComponent<Obstacle>().position = pos;
        }

        /// <summary>
        /// For the given Tilemap, set a tile at the given position.
        /// </summary>
        /// <param name="tileMap">Given Tilemap</param>
        /// <param name="tile">Given Tile</param>
        /// <param name="position">Given position</param>
        /// <returns>Instantiated GameObject at the given position</returns>
        private GameObject SetTile(Tilemap tileMap, TileBase tile, Vector3 position)
        {
            var tilePos = tileMap.LocalToCell(position);
            tileMap.SetTile(tilePos, tile);
            return tileMap.GetInstantiatedObject(tilePos);
        }
    }
}