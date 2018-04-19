using System;
using WIC = SharpDX.WIC;
using SharpDX.IO;

namespace MapEditor.Views
{
	public class Image : IDisposable
	{
		WIC.FormatConverter FormatConverter;
		public WIC.FormatConverter FC => FormatConverter;

		public Image(WIC.ImagingFactory factory, string path)
		{
			using (var decoder = new WIC.PngBitmapDecoder(factory))
			{
				using (var inputStream = new WIC.WICStream(factory, path, NativeFileAccess.Read))
					decoder.Initialize(inputStream, WIC.DecodeOptions.CacheOnLoad);

				FormatConverter = new WIC.FormatConverter(factory);
				FormatConverter.Initialize(decoder.GetFrame(0), WIC.PixelFormat.Format32bppPRGBA);
			}
		}

		public void Dispose()
		{
			FormatConverter.Dispose();
		}
	}
}
