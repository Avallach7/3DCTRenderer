using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ct3dRenderer.Utils
{
	class EngineThreadUtil : UnitySingleton<EngineThreadUtil>
	{
		private List<Action> _onAwakeEvents = new List<Action>();
		private List<Action> _onStartEvents = new List<Action>();
		private List<Action> _onUpdateEvents = new List<Action>();

		public static void RunOnAwake(Action action)
		{
			if (Instance._onAwakeEvents == null)
				throw new Exception("Already after Awake!");
			Instance._onAwakeEvents.Add(action);
		}

		public static void RunOnStart(Action action)
		{
			if (Instance._onStartEvents == null)
				throw new Exception("Already after Start!");
			Instance._onStartEvents.Add(action);
		}

		public static void RunOnEngineThread(Action action)
		{
			var events = Instance._onUpdateEvents;
			lock(events)
				events.Add(action);
		}

		public override void Awake()
		{
			base.Awake();
			_onAwakeEvents.ForEach(e => e());
			_onAwakeEvents = null;
		}

		public void Start()
		{
			_onStartEvents.ForEach(e => e());
			_onStartEvents = null;
		}

		public void Update()
		{
			var events = _onUpdateEvents;
			lock (events)
			{
				_onUpdateEvents.ForEach(e => e());
				_onUpdateEvents.Clear();
			}
		}
	}
}
