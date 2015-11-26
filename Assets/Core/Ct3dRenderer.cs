using System;
using System.Linq;
using Assets.Utils;
using Ct3dRenderer.Data;
using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Core
{
	public class Ct3dRenderer : MonoBehaviour
	{
		public void Start()
		{
			Main(Environment.GetCommandLineArgs());
		}

		public static void Main(String[] args)
		{
			DebugUtils.Log("Application start");
			//TempBenchmark.Run();
			Scene.Create(new CtScanChunkLoader(GetProjectPath(args)));
			//var p = new CTScanChunkLoader(GetProjectPath(args));
			//p.GetChunk(new IntVector3());

			DebugUtils.Log("Application finished");
			Application.Quit();
			Debug.DebugBreak();
			//Environment.Exit(0);
			//throw new Exception();
		}

		private static String GetProjectPath(String[] args)
		{
			return (args.Length > 1 && args[1] != "-rebuildlibrary") ? args[1] : "DemoCTScan";
		}
	}
}