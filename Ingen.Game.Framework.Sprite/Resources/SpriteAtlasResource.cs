using Ingen.Game.Framework.Resources.Images;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Ingen.Game.Framework.Resources.Sprite
{
	public class SpriteAtlasResource : IResource
	{
		public ImageResource ImageResource { get; private set; }
		GameContainer Container;

		public SpriteAtlasResource(ImageResource baseImageResource)
		{
			ImageResource = baseImageResource;
		}

		public SpriteResource MakeSprite(RawRectangle rect)
		{
			var resource = new SpriteResource(this, rect);
			if (Container != null)
				resource.UpdateDevice(Container);
			return resource;
		}

		public void UpdateDevice(GameContainer container)
		{
			ImageResource.UpdateDevice(container);
			Container = container;
		}

		public void Dispose()
		{
			ImageResource?.Dispose();
			ImageResource = null;
		}
	}
}
