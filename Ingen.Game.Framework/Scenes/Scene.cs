using Ingen.Game.Framework.Resources;
using SharpDX.Direct2D1;
using System;

namespace Ingen.Game.Framework.Scenes
{
	public abstract class Scene : IDisposable
	{
		protected ResourceLoader Resource { get; }
		public Scene()
		{
			Resource = new ResourceLoader();
		}

		public abstract void Render(RenderTarget target);
		public abstract void Logic();

		public virtual void Dispose()
		{
			Resource.Dispose();
		}
	}
}
