using System;
using SharpDX.Direct2D1;

namespace Ingen.Game.Framework.Resources.Brushes
{
	public abstract class BrushResource : IResource
	{
		protected Brush _brush;
		public Brush Brush => _brush;

		protected abstract void CreateBrush(RenderTarget target);
		public void UpdateDevice(GameContainer container)
		{
			Dispose();
			CreateBrush(container.DeviceContext);
		}

		public void Dispose()
		{
			_brush?.Dispose();
			_brush = null;
		}
	}
}
