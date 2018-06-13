using WIC = SharpDX.WIC;
using DXGI = SharpDX.DXGI;
using SharpDX.Direct2D1;

namespace Ingen.Game.Framework.Resources.Images
{
	public abstract class ImageResource : IResource
	{
		public Bitmap Image { get; private set; }

		protected WIC.FormatConverter FormatConverter;

		public void UpdateRenderTarget(RenderTarget target)
		{
			Image?.Dispose();
			Image = Bitmap.FromWicBitmap(target, FormatConverter, new BitmapProperties(new PixelFormat(DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied)));
		}

		public void Dispose()
		{
			FormatConverter?.Dispose();
			Image?.Dispose();
		}
	}
}
