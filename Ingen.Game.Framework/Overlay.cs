using Ingen.Game.Framework.Resources;
using SharpDX.Direct2D1;
using System;

namespace Ingen.Game.Framework
{
	public abstract class Overlay : IDisposable
	{
		protected ResourceLoader Resource { get; }
		public Overlay()
		{
			Resource = new ResourceLoader();
		}

		protected RenderTarget RenderTarget { get; private set; }

		public virtual void UpdateRenderTarget(RenderTarget target)
		{
			RenderTarget = target;
			Resource.UpdateRenderTarget(target);
		}

		public abstract void Render();
		public abstract void Update();

		public virtual void Dispose()
		{
			Resource.Dispose();
		}

		public abstract int Priority { get; }
	}
}
