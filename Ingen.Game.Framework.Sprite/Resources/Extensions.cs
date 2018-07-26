using Ingen.Game.Framework.Resources.Images;
using Ingen.Game.Framework.Resources.Sprite;
using SharpDX.Direct2D1;

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

		public static void DrawSprite(this DeviceContext context, ResourceLoader loader, string key)
			=> loader.Get<SpriteResource>(key).Render(context);
	}
}
