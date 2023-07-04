using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tools
{
	public class TilemapCleaner : MonoBehaviour
	{
		Tilemap[] m_Tilemaps;

		private void Reset()
		{
			FindTilemaps();
		}

		void FindTilemaps()
		{
			m_Tilemaps = transform.GetComponentsInChildren<Tilemap>();
		}

		[Button]
		public void ClearAllTileMap()
		{
			if(m_Tilemaps == null)
			{
				FindTilemaps();
			}

			foreach(Tilemap tilemap in m_Tilemaps)
			{
				tilemap.ClearAllTiles();
			}
		}
	}
}
