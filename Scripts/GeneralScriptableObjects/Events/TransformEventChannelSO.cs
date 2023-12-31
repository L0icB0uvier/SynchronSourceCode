﻿using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
	/// <summary>
	/// This class is used for Events that have one transform argument.
	/// Example: Spawn system initializes player and fire event, where the transform is the position of player.
	/// </summary>

	[CreateAssetMenu(menuName = "Events/Transform Event Channel")]
	public class TransformEventChannelSO : ScriptableObject
	{
		public UnityAction<Transform> OnEventRaised;

		public void RaiseEvent(Transform value)
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke(value);
		}
	}
}
