using System;

namespace Ingen.Game.Framework
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NavigateOptionsAttribute : Attribute
	{
		public NavigateOptionsAttribute(Timing initalizeTiming, Timing destroyTiming)
		{
			InitalizeTiming = initalizeTiming;
			DestroyTiming = destroyTiming;
		}

		public Timing InitalizeTiming { get; }
		public Timing DestroyTiming { get; }
	}
	public enum Timing
	{
		/// <summary>
		/// 次シーン移行前に破棄(ロード画面挟む)/生成
		/// </summary>
		Before,
		/// <summary>
		/// 次シーン移行後に破棄/生成(ロード画面挟む)
		/// </summary>
		After,
	}
}
