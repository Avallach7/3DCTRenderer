using System;
using UnityEngine;

namespace Ct3dRenderer.Utils
{
	static class UnityUtils
	{
		private static int _firstObjId;
		private static int ObjectsCount()
		{
			return UnityEngine.Object.FindObjectsOfType(typeof (UnityEngine.Object)).Length;
		}

		public static T Create<T>() where T : MonoBehaviour
		{
			var component = new GameObject().AddComponent<T>();
			if (_firstObjId == 0)
			{
				_firstObjId = component.GetInstanceID();
			}
			component.name = typeof (T).Name + "#" + (Math.Abs(component.GetInstanceID() - _firstObjId)/2);// + " / " + ObjectsCount(); // + "." + (uint)RuntimeHelpers.GetHashCode(component);
			return component;
		}

		public static void ListObjects()
		{
			var objects = UnityEngine.Object.FindObjectsOfType(typeof (UnityEngine.Object));
			for (int i = 0; i < objects.Length; i++)
			{
				Debug.Log(i + "\t" + 
						objects[i].GetInstanceID() + "\t" + 
						objects[i].ToString());
			}
		}
	}
}