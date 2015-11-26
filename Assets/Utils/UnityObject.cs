using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Utils
{
	public class UnityObject<T> : MonoBehaviour where T : UnityObject<T>
	{
		private bool _initialized;
		private static int firstInstanceId;
		protected static T Create()
		{
			var component = new GameObject().AddComponent<T>();
			component._initialized = true;
			if (firstInstanceId == 0)
				firstInstanceId = component.gameObject.GetInstanceID();
            component.name = typeof(T).Name + "#" + Mathf.Abs((component.GetInstanceID() - firstInstanceId)/2);
			return component;
		}

		public virtual void Start()
		{
			DebugUtils.Log(this);
			if (!_initialized)
			{
				Debug.LogError(typeof(T).Name + " created without use of method Create!");
				Destroy(gameObject);
			}
		}
	}
}