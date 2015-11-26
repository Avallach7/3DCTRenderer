using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Utils
{
	public class UnityObject<T> : MonoBehaviour where T : UnityObject<T>
	{
		protected bool _initialized;
		private static int _firstInstanceId;

		protected static T Create()
		{
			var component = new GameObject().AddComponent<T>();
			component._initialized = true;
			if (_firstInstanceId == 0)
				_firstInstanceId = component.gameObject.GetInstanceID();
            component.name = typeof(T).Name + "#" + Mathf.Abs((component.GetInstanceID() - _firstInstanceId)/2);
			return component;
		}

		public virtual void Start()
		{
			//DebugUtils.Log(this);
			if (!_initialized)
			{
				Debug.LogError(typeof(T).Name + " created without use of method Create!");
				Destroy(gameObject);
			}
		}
	}
}