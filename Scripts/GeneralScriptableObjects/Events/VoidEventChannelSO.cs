using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
	/// <summary>
	/// This class is used for Events that have no arguments (Example: Exit game event)
	/// </summary>

	[CreateAssetMenu(menuName = "Events/Void Event Channel")]
	public class VoidEventChannelSO : ScriptableObject
	{
		public UnityAction onEventRaised;

		public void RaiseEvent()
		{
			onEventRaised?.Invoke();
		}
	}
}


