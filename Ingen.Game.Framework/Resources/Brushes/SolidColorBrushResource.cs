using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Ingen.Game.Framework.Resources.Brushes
{
	public class SolidColorBrushResource : BrushResource
	{
		RawColor4 _color;

		public SolidColorBrushResource(RawColor4 color)
		{
			_color = color;
		}

		protected override void CreateBrush(RenderTarget target)
		{
			_brush = new SolidColorBrush(target, _color);
		}
	}
}
