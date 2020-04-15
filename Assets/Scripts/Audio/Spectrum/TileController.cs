using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Audio.Spectrum
{
    /// <summary>
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
        
        public float multiplier = 30f;
        public float width = 1f;

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            for (var i = 0; i < displayWindowSize; i++)
            {
                var pointX = displayWindowSize / 2 * -1 * width + i * width;
                var pos = new Vector3(pointX, 0, 0);
                positions.Add(pos);

                baseTilemap.SetTile(baseTilemap.LocalToCell(new Vector3(pos.x, pos.y - 5, pos.z)), baseTile);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="pointInfo"></param>
        /// <param name="curIndex"></param>
        public void UpdateTiles(List<SpectralFluxInfo> pointInfo, int curIndex = -1)
        {
            if (positions.Count < displayWindowSize - 1)
                return;

            var numPlotted = 0;
            int windowStart;
            int windowEnd;

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
        /// </summary>
        /// <param name="position"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Vector3 Calculate(Vector3 position, float height)
        {
            return new Vector3(position.x, height * multiplier * difficulty, position.z);
        }

        /// <summary>
        /// </summary>
        /// <param name="tileMap"></param>
        /// <param name="tile"></param>
        /// <param name="position"></param>
        private void SetConditionalTile(Tilemap tileMap, TileBase tile, Vector3 position)
        {
            var pos = tileMap.LocalToCell(position);
            if (!NoteHandler.Instance.IsAvailable(pos)) return;

            tileMap.SetTile(pos, tile);
            var obj = tileMap.GetInstantiatedObject(pos);
            var note = obj.GetComponent<Note>();
            note.position = pos;
        }

        /// <summary>
        /// </summary>
        /// <param name="tileMap"></param>
        /// <param name="tile"></param>
        /// <param name="position"></param>
        private GameObject SetTile(Tilemap tileMap, TileBase tile, Vector3 position)
        {
            var tilePos = tileMap.LocalToCell(position);
            tileMap.SetTile(tilePos, tile);
            return tileMap.GetInstantiatedObject(tilePos);
        }
    }
}