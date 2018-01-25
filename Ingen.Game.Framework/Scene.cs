using Ingen.Game.Framework.Resources;
using System;

namespace Ingen.Game.Framework
{
	public abstract class Scene : IDisposable
	{
		protected ResourceLoader Resource { get; }
		public Scene()
		{
			Resource = new ResourceLoader();
		}

		public abstract void Render();
		public abstract void Logic();

		public virtual void Dispose()
		{
			Resource.Dispose();
		}
	}
}
