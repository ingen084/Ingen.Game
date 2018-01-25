using System;

namespace Ingen.Game.Framework
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NavigateOptionsAttribute : Attribute
	{
		public NavigateOptionsAttribute(Timing timing)
		{
			Timing = timing;
		}

		public Timing Timing { get; }
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
