using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace EnvironmentalScripts
{
    public class AnimatedWhenPowered : MonoBehaviour
    {
        private Tilemap tilemap;
   
        public AnimatedTile[] animatedTiles;

        public int animationSpeed = 8;

        private void Awake()
        {
            GetAnimatedTiles();
        }

        [Button]
        public void GetAnimatedTiles()
        {
            tilemap = GetComponent<Tilemap>();
            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
            TileBase[] tiles = allTiles.Where(x => x != null).ToArray();
            animatedTiles = Array.ConvertAll(tiles, item => item as AnimatedTile);
        }

        public void PlayAnimation()
        {
            tilemap.animationFrameRate = 1;
        }

        public void StopAnimation()
        {
            tilemap.animationFrameRate = 0;
        }

        [Button]
        public void ClearTilemap()
        {
            tilemap.ClearAllTiles();
        }
    }
}
