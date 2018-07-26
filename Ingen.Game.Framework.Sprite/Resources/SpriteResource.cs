using Ingen.Game.Framework.Resources.Images;
using Ingen.Game.Framework.Sprite;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;
using SharpDX.Mathematics.Interop;
using System;

namespace Ingen.Game.Framework.Resources.Sprite
{
	public class SpriteResource : IResource, ISprite
	{
		ImageResource BaseImage;
		readonly SpriteAtlasResource ParentAtlas;
		RawRectangleF Rect;

		public Effect Effect => CropEffect ?? (BitmapSourceEffect as Effect);
		private Crop CropEffect;
		private BitmapSource BitmapSourceEffect;

		Size2 Size;

		public SpriteResource(ImageResource baseImage)
		{
			BaseImage = baseImage ?? throw new ArgumentNullException(nameof(baseImage));
			Size = BaseImage.Image.PixelSize;
		}
		public SpriteResource(SpriteAtlasResource parentAtlas, RawRectangle rect)
		{
			ParentAtlas = parentAtlas ?? throw new ArgumentNullException(nameof(parentAtlas));
			Rect = new RawRectangleF(rect.Left, rect.Top, rect.Right, rect.Bottom);
			Size = new Size2(rect.Right - rect.Left, rect.Bottom - rect.Top);
		}

		public void UpdateDevice(GameContainer container)
		{

			if (BaseImage != null)
			{
				BaseImage.UpdateDevice(container);
				BitmapSourceEffect?.Dispose();
				BitmapSourceEffect = new BitmapSource(container.DeviceContext);
				BitmapSourceEffect.SetInput(0, BaseImage.Image, true);
				return;
			}

			CropEffect?.Dispose();
			CropEffect = new Crop(container.DeviceContext);
		}

		internal void Render(DeviceContext context)
		{
			CropEffect.SetInput(0, ParentAtlas.ImageResource.Image, true);
			CropEffect.SetValue((int)CropProperties.Rectangle, Rect);
			context.DrawImage(Effect);
			CropEffect.SetInput(0, null, true);
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
