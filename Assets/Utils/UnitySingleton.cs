using System;
using System.Threading;
using UnityEngine;

namespace Ct3dRenderer.Utils
{
	public class UnitySingleton<TType> : MonoBehaviour where TType : UnitySingleton<TType>
	{
		private static TType _instance;
		private static bool _createdFromScripts = false;

		public static TType Instance
		{
			get
			{
				if (_instance == null)
				{
					_createdFromScripts = true;
					_instance = UnityUtils.Create<TType>();
					Debug.Log("Created " + typeof(TType).Name);
				}
				return _instance;
			}
		}

		public virtual void Awake()
		{
			if (System.Object.ReferenceEquals(_instance, null))
			{
				if (!_createdFromScripts)
				{
					Destroy(gameObject);
					throw new Exception("Singleton created from editor. This is illegal!");
				}
			}
			else if (!System.Object.ReferenceEquals(_instance, this))
			{
				Destroy(gameObject);
				throw new Exception("Second instance of singleton created. This is illegal!");
			}
		}
	}
}
