using Ingen.Game.Framework.Resources.Brushes;
using Ingen.Game.Framework.Resources.Images;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using BitmapInterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode;

namespace Ingen.Game.Framework.Resources
{
	public static class Extensions
	{
		public static void AddSolidColorBrushResource(this ResourceLoader loader, string key, RawColor4 color)
			=> loader.AddResource(key, new SolidColorBrushResource(color));

		public static void AddPngImageResource(this ResourceLoader loader, string key, ImagingFactory factory, string filePath)
			=> loader.AddResource(key, new PngImageResource(factory, filePath));

		public static void DrawBitmap(this RenderTarget target, ResourceLoader loader, string key, RawRectangleF rectangle, float opacity = 1, BitmapInterpolationMode bitmapInterpolationMode = BitmapInterpolationMode.Linear)
			=> target.DrawBitmap(loader.Get<ImageResource>(key).Image, rectangle, opacity, bitmapInterpolationMode);
	}
}
