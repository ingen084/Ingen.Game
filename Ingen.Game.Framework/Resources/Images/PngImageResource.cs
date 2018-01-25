using WIC = SharpDX.WIC;
using SharpDX.Direct2D1;
using SharpDX.IO;

namespace Ingen.Game.Framework.Resources.Images
{
	public class PngImageResource : ImageResource
	{
		public PngImageResource(WIC.ImagingFactory imagingFactory, RenderTarget target, string path)
		{
			using (var decoder = new WIC.PngBitmapDecoder(imagingFactory))
			{
				using (var inputStream = new WIC.WICStream(imagingFactory, path, NativeFileAccess.Read))
					decoder.Initialize(inputStream, WIC.DecodeOptions.CacheOnLoad);

				FormatConverter = new WIC.FormatConverter(imagingFactory);
				FormatConverter.Initialize(decoder.GetFrame(0), WIC.PixelFormat.Format32bppPRGBA);
			}
		}
	}
}
