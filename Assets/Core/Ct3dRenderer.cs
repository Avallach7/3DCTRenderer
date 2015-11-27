using System;
using System.Linq;
using Ct3dRenderer.Data;
using Ct3dRenderer.Gui;
using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Core
{
	public class Ct3dRenderer : MonoBehaviour
	{
		public void Start()
		{
			DebugUtils.LogFilePath = Application.persistentDataPath + "/log.txt";
			Main(Environment.GetCommandLineArgs());
		}

		public static void Main(String[] args)
		{
			Debug.Log("Application start");
			Debug.Log("args = " + args.ToDebugString());
			Debug.Log("Application.persistentDataPath = " + Application.persistentDataPath);
			UnityUtils.Create<HudFps>();
			Visualisation.Create(new CtScanChunkLoader(GetProjectPath(args)));
		}

		private static String GetProjectPath(String[] args)
		{
			return (Application.persistentDataPath + "/DemoCTScan");
		}
	}
}