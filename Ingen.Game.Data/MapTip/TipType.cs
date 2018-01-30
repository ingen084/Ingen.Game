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
		/// ホーム
		/// </summary>
		Home,
		/// <summary>
		/// ボス
		/// </summary>
		Boss,

		/// <summary>
		/// ボーナス
		/// </summary>
		Bonus,
		/// <summary>
		/// カードドロップ
		/// </summary>
		Card,
		/// <summary>
		/// バトル
		/// </summary>
		Battle,
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
		/// 2xバトル
		/// </summary>
		Battle2x,
		/// <summary>
		/// ムーブ
		/// </summary>
		Move2x,
		/// <summary>
		/// ワープ
		/// </summary>
		Warp2x,
	}
}
