using System.Collections.Generic;

namespace Ct3dRenderer.Utils
{
	public class Stopwatch
	{
		private static readonly Stack<Stopwatch> Stack = new Stack<Stopwatch>();
		private readonly string _description;
		private readonly System.Diagnostics.Stopwatch _stopwatch;

		public Stopwatch(string description)
		{
			_description = description;
			_stopwatch = new System.Diagnostics.Stopwatch();
			_stopwatch.Start();
			//DebugUtils.Log(description + ": start");
		}

		public void Stop()
		{
			_stopwatch.Stop();
			DebugUtils.Log(_description + ": finished, took " + _stopwatch.ElapsedMilliseconds + "ms");
		}

		public static void Start(string description)
		{
			Stack.Push(new Stopwatch(description));
		}

		public static void StopCurrent()
		{
			Stack.Pop().Stop();
		}
	}
}