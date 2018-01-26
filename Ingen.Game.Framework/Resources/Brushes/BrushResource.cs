using System;
using SharpDX.Direct2D1;

namespace Ingen.Game.Framework.Resources.Brushes
{
	public abstract class BrushResource : IResource
	{
		protected Brush _brush;
		public Brush Brush => _brush;

		protected abstract void CreateBrush(RenderTarget target);
		public void UpdateRenderTarget(RenderTarget target)
		{
			System.Diagnostics.Debug.WriteLine("Brush Updated");
			_brush?.Dispose();
			CreateBrush(target);
		}

		public void Dispose()
		{
			_brush?.Dispose();
		}
	}
}
