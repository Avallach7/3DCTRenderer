using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = System.Object;

namespace Ct3dRenderer.Utils
{
	static class DebugUtils
	{
		public static String LogFilePath = null;

		public static string ToDebugString(this Object obj, int maxDepth = 3)
		{
			if (obj == null)
				return "null";
			else if (Convert.GetTypeCode(obj) != TypeCode.Object)
				return Convert.ToString(obj);
			else if (obj is IntPtr | obj is UIntPtr)
				return "*" + obj.ToString();

			StringBuilder builder = new StringBuilder();
			if (maxDepth == 0)
				return "...";
			else if (obj is IEnumerable)
			{
				builder.Append("[");
				int counter = 0;
				foreach (var item in ((IEnumerable) obj))
				{
					builder.Append(item.ToDebugString(maxDepth-1) + ",");
					if (counter > maxDepth)
					{
						builder.Append("...");
						break;
					}
					counter ++;
				}
				builder.Append("]");
			}
			else
			{
				var type = obj.GetType();
				builder.Append(type.Name);
				FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				var fieldDescriptions = fields.Select(
						info => info.Name + "=" + (info.FieldType.IsPointer ? "*???" : ToDebugString(info.GetValue(obj), maxDepth - 1)))
						.ToArray();
                builder.Append("{").Append(String.Join(",", fieldDescriptions)).Append("}");
			}
			return builder.ToString();
		}

		public static void Log(Object obj)
		{
			Log(ToDebugString(obj));
		}

		private static int _logCounter;
		
		public static void Log(String msg)
		{
			try
			{
				Debug.Log(msg);
				if (LogFilePath == null)
					return;
				int logId = Interlocked.Increment(ref _logCounter);
				var stackFrame =
					new StackTrace(1, true).GetFrames()
						.First(f => Path.GetFileName(f.GetFileName()) != typeof (DebugUtils).Name + ".cs");
				var extendedMsg = //(isError ? "!!!\r\n" : "") +
								logId.ToString().PadRight(4) + " " +
								DateTime.Now.ToString("HH:mm:ss.ffff") + " " +
								(String.IsNullOrEmpty(stackFrame.GetFileName())
									? ""
									: (Path.GetFileName(stackFrame.GetFileName()) + ":" + stackFrame.GetFileLineNumber() + " ")).PadRight(25) +
								msg +
								" (thread#" + Thread.CurrentThread.ManagedThreadId + ")";
				WriteToLogFile(extendedMsg);
				//if (isError)
				//	Debug.Break();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void WriteToLogFile(string msg)
		{
			//needs to be synchronized, otherwise might cause "IOException: Sharing violation on path..."
			var writer = File.AppendText(LogFilePath);
			writer.WriteLine(msg);
			writer.Dispose();
		}
	}
}