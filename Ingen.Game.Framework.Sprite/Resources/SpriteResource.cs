using Ingen.Game.Framework.Resources.Images;
using Ingen.Game.Framework.Sprite;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;
using SharpDX.Mathematics.Interop;
using System;

namespace Ingen.Game.Framework.Resources.Sprite
{
	public class SpriteResource : IResource
	{
		ImageResource BaseImage;
		readonly SpriteAtlasResource ParentAtlas;
		public RawRectangleF Rect { get; private set; }

		private Crop CropEffect;


		public SpriteResource(ImageResource baseImage)
		{
			BaseImage = baseImage ?? throw new ArgumentNullException(nameof(baseImage));
		}
		public SpriteResource(SpriteAtlasResource parentAtlas, RawRectangle rect)
		{
			ParentAtlas = parentAtlas ?? throw new ArgumentNullException(nameof(parentAtlas));
			Rect = new RawRectangleF(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}

		public void UpdateDevice(GameContainer container)
		{

			if (BaseImage != null)
			{
				BaseImage.UpdateDevice(container);
				var size = BaseImage.Image.PixelSize;
				Rect = new RawRectangleF(0, 0, size.Width, size.Height);
				return;
			}

			CropEffect?.Dispose();
			CropEffect = new Crop(container.DeviceContext);
		}

		internal void Render(DeviceContext context)
		{
			if (BaseImage == null)
			{
				CropEffect.SetInput(0, ParentAtlas.ImageResource.Image, true);
				CropEffect.SetValue((int)CropProperties.Rectangle, Rect);
				context.DrawImage(CropEffect);
				CropEffect.SetInput(0, null, true);
				return;
			}
			context.DrawImage(BaseImage.Image);
		}

		public void Dispose()
		{
			if (BaseImage == null)
			{
				CropEffect.Dispose();
				CropEffect = null;
				return;
			}
			BaseImage.Dispose();
			BaseImage = null;
		}
	}
}
