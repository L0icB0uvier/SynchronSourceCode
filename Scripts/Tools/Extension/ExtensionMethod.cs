using UnityEngine;

namespace Tools.Extension
{
	public static class ExtensionMethod
	{
		public static bool Contains(this LayerMask layers, GameObject gameObject)
		{
			return 0 != (layers.value & 1 << gameObject.layer);
		}
	}
}