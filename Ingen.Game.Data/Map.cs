using Ingen.Game.Data.MapTip;
using ProtoBuf;

namespace Ingen.Game.Data
{
	[ProtoContract]
	public class Map
	{
		[ProtoMember(1)]
		public string Name { get; set; }

		[ProtoMember(2)]
		public Tip[,] Data { get; set; }
	}
}
