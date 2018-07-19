using Ingen.Game.Framework.Resources.Images;
using Ingen.Game.Framework.Sprite;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;

namespace Ingen.Game.Framework.Resources.Sprite
{
	public class SpriteResource : IResource, ISprite
	{
		ImageResource BaseImage;
		readonly SpriteAtlasResource ParentAtlas;
		RawRectangleF Rect;

		Effect AtlasEffect;

		Size2 Size;

		static Guid AtlasEffectGuid = new Guid(0x913e2be4, 0xfdcf, 0x4fe2, 0xa5, 0xf0, 0x24, 0x54, 0xf1, 0x4f, 0xf4, 0x8);

		public SpriteResource(ImageResource baseImage)
		{
			BaseImage = baseImage ?? throw new ArgumentNullException(nameof(baseImage));
			Size = BaseImage.Image.PixelSize;
		}
		public SpriteResource(SpriteAtlasResource parentAtlas, RawRectangle rect)
		{
			ParentAtlas = parentAtlas ?? throw new ArgumentNullException(nameof(parentAtlas));
			Size = BaseImage.Image.PixelSize;
			Rect = new RawRectangleF(rect.Left, rect.Top, rect.Right, rect.Bottom);
			Size = new Size2(rect.Right - rect.Left, rect.Bottom - rect.Top);
		}

		public void UpdateDevice(DeviceContext context)
		{
			if (BaseImage == null)
			{
				AtlasEffect = new Effect(context, AtlasEffectGuid);
				AtlasEffect.SetValue((int)AtlasProperties.InputRectangle, Rect);
				return;
			}
			BaseImage.UpdateDevice(context);
		}

		public void Dispose()
		{
			if (BaseImage == null)
			{
				AtlasEffect.Dispose();
				AtlasEffect = null;
				return;
			}
			BaseImage.Dispose();
			BaseImage = null;
		}
	}
}
