using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Ingen.Game.Framework.Input
{
	internal static class NativeMethods
	{
		[DllImport("user32.dll")]
		public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);
	}
}
