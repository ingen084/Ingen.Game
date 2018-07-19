using System;
using System.Runtime.InteropServices;

namespace Ingen.Game.Framework.Input
{
	internal static class NativeMethods
	{
		[DllImport("user32.dll")]
		public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

		public struct POINT
		{
			public int X;
			public int Y;
		}
	}
}
