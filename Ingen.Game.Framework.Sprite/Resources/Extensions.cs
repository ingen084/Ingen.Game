using Ingen.Game.Framework.Resources.Images;
using Ingen.Game.Framework.Resources.Sprite;
using SharpDX.Direct2D1;
using SharpDX.WIC;

namespace Ingen.Game.Framework.Resources
{
	public static class Extensions
	{
		public static SpriteAtlasResource AddSpriteAtlas(this ResourceLoader loader, string key, ImageResource imageResource)
		{
			var resource = new SpriteAtlasResource(imageResource);
			loader.AddResource(key, resource);
			return resource;
		}
		public static SpriteAtlasResource AddSpriteAtlas(this ResourceLoader loader, string key, ImagingFactory factory, string path)
		{
			var resource = new SpriteAtlasResource(new PngImageResource(factory, path));
			loader.AddResource(key, resource);
			return resource;
		}

		public static void AddSprite(this ResourceLoader loader, string key, ImageResource imageResource)
			=> loader.AddResource(key, new SpriteResource(imageResource));
		public static void AddPngImageSprite(this ResourceLoader loader, string key, ImagingFactory factory, string path)
			=> loader.AddResource(key, new SpriteResource(new PngImageResource(factory, path)));

		public static void DrawSprite(this DeviceContext context, ResourceLoader loader, string key)
			=> loader.Get<SpriteResource>(key).Render(context);
	}
}
