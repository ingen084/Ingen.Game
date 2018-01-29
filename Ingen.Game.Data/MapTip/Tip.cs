using ProtoBuf;

namespace Ingen.Game.Data.MapTip
{
	[ProtoContract]
	public struct Tip
	{
		[ProtoMember(1)]
		public TipType Type { get; set; }
		[ProtoMember(2)]
		public MoveDirection Direction { get; set; }
	}
}
