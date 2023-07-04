using Pathfinding;
using UnityEngine;

namespace Tools.Extension
{
	public class PathCalculation : MonoBehaviour
	{
		/*static PathFiltered OnPathOrdered;
	static int pathRequestCount;
	static List<Path> availablePathToTransform = new List<Path>();
	static int pathTested;

	static bool calculatingShortestPath = false;

	static List<ShorterPathRequest> queue = new List<ShorterPathRequest>();

	public static void RequestShorterPath(Vector3 origin, Transform[] objectTransform, PathFiltered cheapestPathFound)
	{
		queue.Add(new ShorterPathRequest(origin, objectTransform, cheapestPathFound));

		if (calculatingShortestPath)
			return;

		else
		{
			StartCalculatingPath();
		}
	}

	private static void StartCalculatingPath()
	{
		calculatingShortestPath = true;

		if (AstarPath.active == null) return;

		OnPathOrdered = queue[0].m_Callback;
		pathRequestCount = queue[0].m_ObjectTransform.Length;

		foreach (Transform transform in queue[0].m_ObjectTransform)
		{
			var p = ABPath.Construct(queue[0].m_Origin, transform.position, transform, OnPathTested);

			AstarPath.StartPath(p);
		}
	}

	private static void OnPathTested(Path p)
	{
		if (!p.error)
		{
			if (availablePathToTransform.Count == 0)
				availablePathToTransform.Add(p);

			else
			{
				for (int i = 0; i <= availablePathToTransform.Count; i++)
				{
					if (i == availablePathToTransform.Count)
					{
						availablePathToTransform.Add(p);
						break;
					}


					else if (p.GetTotalLength() < availablePathToTransform[i].GetTotalLength())
					{
						availablePathToTransform.Insert(i, p);
						break;
					}
				}
			}

			pathTested++;
		}

		else
			pathTested++;


		//When all the requested path have been tested, will look for the cheapest path
		if (pathTested == pathRequestCount)
		{
			pathTested = 0;
			pathRequestCount = 0;

			List<ABPath> orderedPath = new List<ABPath>();

			foreach (ABPath path in availablePathToTransform)
				orderedPath.Add(path);

			availablePathToTransform.Clear();

			OnPathOrdered(orderedPath.ToArray());
			queue.RemoveAt(0);

			if (queue.Count > 0)
				StartCalculatingPath();

			else
				calculatingShortestPath = false;

		}
	}*/
	}

	public class ShorterPathRequest
	{
		public Vector3 m_Origin;
		public Transform[] m_ObjectTransform;
		public PathFiltered m_Callback;

		public ShorterPathRequest(Vector3 origin, Transform[] objectsTransform, PathFiltered callback)
		{
			m_Origin = origin;
			m_ObjectTransform = objectsTransform;
			m_Callback = callback;
		}
	}

	public delegate void PathFiltered(ABPath[] searchPoint);
}