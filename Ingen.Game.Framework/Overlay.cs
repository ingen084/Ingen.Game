using SharpDX.Direct2D1;

namespace Ingen.Game.Framework
{
	public abstract class Overlay
	{
		public abstract void Render(RenderTarget target);
		public abstract void Logic();
	}
}
