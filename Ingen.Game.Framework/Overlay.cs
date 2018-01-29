using SharpDX.Direct2D1;

namespace Ingen.Game.Framework
{
	public abstract class Overlay : Scene
	{
		public abstract int Priority { get; }
	}
}
