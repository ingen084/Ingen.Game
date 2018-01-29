using ProtoBuf;

namespace Ingen.Game.Data.MapTip
{
	[ProtoContract]
	public enum TipType
	{
		/// <summary>
		/// 虚無
		/// </summary>
		Transparent,
		/// <summary>
		/// 効果なし
		/// </summary>
		Blank,

		/// <summary>
		/// ボーナスマス
		/// </summary>
		Bonus,
		/// <summary>
		/// カードドロップ
		/// </summary>
		Card,
		/// <summary>
		/// ムーブ
		/// </summary>
		Move,
		/// <summary>
		/// ワープ
		/// </summary>
		Warp,

		/// <summary>
		/// 2xボーナス
		/// </summary>
		Bonus2x,
		/// <summary>
		/// 2xカードドロップ
		/// </summary>
		Card2x,
		/// <summary>
		/// ムーブ
		/// </summary>
		Move2x,
		/// <summary>
		/// ワープ
		/// </summary>
		Warp2x,

		/// <summary>
		/// ボス
		/// </summary>
		Boss,
	}
}
