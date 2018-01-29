using Ingen.Game.Framework.Resources;
using SharpDX.Direct2D1;
using System;
using Unity.Attributes;

namespace Ingen.Game.Framework
{
	public abstract class Scene : IDisposable
	{
		protected ResourceLoader Resource { get; }
		public Scene()
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
	}
}
