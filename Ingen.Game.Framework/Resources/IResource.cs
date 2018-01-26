using SharpDX.Direct2D1;
using System;

namespace Ingen.Game.Framework.Resources
{
	public interface IResource : IDisposable
	{
		void UpdateRenderTarget(RenderTarget target);
	}
}
