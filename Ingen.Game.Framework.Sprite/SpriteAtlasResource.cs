using Ingen.Game.Framework.Resources.Images;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Ingen.Game.Framework.Resources.Sprite
{
	public class SpriteAtlasResource : IResource
	{
		ImageResource ImageResource { get; set; }
		internal DeviceContext DeviceContext { get; set; }
		GameContainer Container { get; }

		public SpriteAtlasResource(ImageResource baseImageResource, GameContainer container)
		{
			Container = container;
			ImageResource = baseImageResource;
			DeviceContext = container.GameWindow.D2D1DeviceContext;
		}

		public SpriteResource GetSprite(RawRectangle rect)
			=> new SpriteResource(this, rect);

		public void UpdateRenderTarget(RenderTarget target)
		{
			ImageResource.UpdateRenderTarget(target);
			DeviceContext = Container.GameWindow.D2D1DeviceContext;
		}

		public void Dispose()
		{
			ImageResource?.Dispose();
			ImageResource = null;
		}
	}
}
