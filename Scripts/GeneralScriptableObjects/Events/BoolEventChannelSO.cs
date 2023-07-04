﻿using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
	/// <summary>
	/// This class is used for Events that have a bool argument.
	/// Example: An event to toggle a UI interface
	/// </summary>

	[CreateAssetMenu(menuName = "Events/Bool Event Channel")]
	public class BoolEventChannelSO : ScriptableObject
	{
		public event UnityAction<bool> OnEventRaised;

		public void RaiseEvent(bool value)
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke(value);
		}
	}
}