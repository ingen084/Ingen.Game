using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace D2dControl
{
	/// <summary>
	/// Win32のQueryPerformanceCounterのラッパクラス
	/// 失敗すると普通にStopWatchを使用します。
	/// </summary>
	internal class HighPerformanceStopwatch
	{
		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);

		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(ref long lpFrequency);

		private long startCounter;
		private long frequency = 0;
		private Stopwatch sw;

		public bool IsRunning { get; private set; } = false;

		/// <summary>
		/// ストップウォッチの値を初期化し、計測を開始します。
		/// </summary>
		/// <returns>PCが高精度タイマーに対応しているかどうか</returns>
		public void Start()
		{
			if (QueryPerformanceCounter(ref startCounter))
			{
				sw = null;
				QueryPerformanceFrequency(ref frequency);
				IsRunning = true;
				return;
			}
			sw = sw ?? new Stopwatch();
			if (sw.IsRunning)
				sw.Stop();
			sw.Start();
			IsRunning = true;
		}

		/// <summary>
		/// 経過時間 Startする前に呼ぶと大変な時間が帰ってきます
		/// </summary>
		public TimeSpan Elapsed
		{
			get
			{
				if (sw != null)
					return sw.Elapsed;
				long stopCounter = 0;
				QueryPerformanceCounter(ref stopCounter);
				return TimeSpan.FromMilliseconds((stopCounter - startCounter) * 1000.0 / frequency);
			}
		}

		public void Stop()
		{
			IsRunning = false;
			if (sw?.IsRunning ?? false)
				sw.Stop();
		}
	}
}
