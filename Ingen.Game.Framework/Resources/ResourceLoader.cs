using SharpDX.Direct2D1;
using System;
using System.Collections;

namespace Ingen.Game.Framework.Resources
{
	public class ResourceLoader : IDisposable
	{
		Hashtable _hashtable;

		public ResourceLoader()
		{
			_hashtable = new Hashtable();
		}

		public void AddResource(object key, IResource resource)
		{
			//memo インスタンス生成(主に時間がかかる読み込み)は外部でやらせてるので影響は少ない
			lock (_hashtable)
				_hashtable.Add(key, resource);
		}

		public IResource this[object key]
		{
			get => _hashtable[key] as IResource;
		}

		public void UpdateRenderTarget(RenderTarget target)
		{
			lock (_hashtable)
				foreach (var resource in _hashtable)
					(resource as IResource)?.Update(target);
		}

		public void Dispose()
		{
			foreach (var resource in _hashtable)
				(resource as IDisposable)?.Dispose();
			lock (_hashtable)
				_hashtable.Clear();
		}
	}
}
