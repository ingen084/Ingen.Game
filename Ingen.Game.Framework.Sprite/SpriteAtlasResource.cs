using Ingen.Game.Framework.Resources.Images;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Ingen.Game.Framework.Resources.Sprite
{
	public class SpriteAtlasResource : IResource
	{
		ImageResource ImageResource { get; set; }

		public SpriteAtlasResource(ImageResource baseImageResource)
		{
			ImageResource = baseImageResource;
		}

		public SpriteResource GetSprite(RawRectangle rect)
			=> new SpriteResource(this, rect);

		public void UpdateDevice(DeviceContext context)
		{
			ImageResource.UpdateDevice(context);
		}

		public void Dispose()
		{
			ImageResource?.Dispose();
			ImageResource = null;
		}
	}
}
