using WIC = SharpDX.WIC;
using DXGI = SharpDX.DXGI;
using SharpDX.Direct2D1;

namespace Ingen.Game.Framework.Resources.Images
{
	public abstract class ImageResource : IResource
	{
		Bitmap _bitmap;
		public Bitmap Image => _bitmap;
		protected WIC.FormatConverter FormatConverter;

		public void UpdateRenderTarget(RenderTarget target)
		{
			_bitmap?.Dispose();
			_bitmap = Bitmap.FromWicBitmap(target, FormatConverter, new BitmapProperties(new PixelFormat(DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied)));
		}

		public void Dispose()
		{
			FormatConverter?.Dispose();
			_bitmap?.Dispose();
		}
	}
}
