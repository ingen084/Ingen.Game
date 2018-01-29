using ProtoBuf;
using System;

namespace Ingen.Game.Data.MapTip
{
	/// <summary>
	/// 進行可能方向指定
	/// </summary>
	[Flags, ProtoContract]
	public enum MoveDirection : byte
	{
		/// <summary>
		/// 進行方向指定なし
		/// </summary>
		Uniform = 0b0000_0000,
		/// <summary>
		/// 上部にのみ進行可
		/// </summary>
		Top = 0b0000_0001,
		/// <summary>
		/// 左にのみ進行可
		/// </summary>
		Left = 0b0000_0010,
		/// <summary>
		/// 下にのみ進行可
		/// </summary>
		Bottom = 0b0000_0100,
		/// <summary>
		/// 右にのみ進行可
		/// </summary>
		Right = 0b0000_1000,
	}
}
