using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Ingen.Game.Framework.Sprite
{
	public interface ISprite
	{
		void Render(RenderTarget target, RawVector2 position);
		void Render(RenderTarget target, RawRectangleF rect);

		void Render(RenderTarget target, RawVector2 position, float rotateDeg, RawRectangleF rotateOrigin);
		void Render(RenderTarget target, RawRectangleF rect, float rotateDeg, RawRectangleF rotateOrigin);
	}
}
