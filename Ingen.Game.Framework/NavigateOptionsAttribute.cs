using System;
using System.Reflection;

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
		/// 次シーン移行前に破棄(ロード画面挟む)
		/// <para>ロード画面移行前に生成</para>
		/// </summary>
		Before,
		/// <summary>
		/// 次シーン移行後に破棄
		/// <para>ロード画面移行後に生成(ロード画面挟む)</para>
		/// </summary>
		After,
	}

	public static class TypeExtensions
	{
		public static NavigateOptionsAttribute GetNavigateOptionsAttribute(this Type type)
			=> type.GetTypeInfo().GetCustomAttribute<NavigateOptionsAttribute>();
	}
}
