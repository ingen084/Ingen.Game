using WIC = SharpDX.WIC;
using SharpDX.IO;

namespace Ingen.Game.Framework.Resources.Images
{
	public class PngImageResource : ImageResource
	{
		public PngImageResource(WIC.ImagingFactory imagingFactory, string path)
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
