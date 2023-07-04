using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SceneManagement.NavigationPoints
{
	[System.Serializable]
	public class BehaviorAtLocation
	{
		public bool lookAround;

		private bool LookAroundAtLocation()
		{
			if (lookAround)
				return true;
			else return false;
		}

		private bool WaitAtLocation()
		{
			if (!lookAround)
				return true;
			else return false;
		}

		[Tooltip("The time to wait at angle")]
		[LabelWidth(100)]
		[Range(0, 7)]
		[ShowIf("WaitAtLocation")]
		public int waitingTime = 1;

		[ShowIf("LookAroundAtLocation")]
		public List<LookAroundInfo> lookAroundAngles = new List<LookAroundInfo>();
	}


	[System.Serializable]
	public class LookAroundInfo
	{
		[HideLabel]
		[Range(0, 360)]
		public int lookingAngle = 0;

		[Range(0, 360)]
		public int rotationSpeed = 90;

		[Tooltip("The time to wait at angle")]
		[LabelWidth(100)]
		[Range(0, 80)]
		public int waitingTime = 1;
	}
}