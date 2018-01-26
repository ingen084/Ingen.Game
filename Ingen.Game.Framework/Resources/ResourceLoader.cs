using SharpDX.Direct2D1;
using System;
using System.Collections;
using System.Threading;

namespace Ingen.Game.Framework.Resources
{
	public class ResourceLoader : IDisposable
	{
		ReaderWriterLockSlim HashtableLock { get; }
		Hashtable Hashtable { get; }

		public ResourceLoader()
		{
			HashtableLock = new ReaderWriterLockSlim();
			Hashtable = new Hashtable();
		}

		public void AddResource(string key, IResource resource)
		{
			HashtableLock.EnterWriteLock();
			try
			{
				Hashtable.Add(key, resource);
			}
			finally
			{
				HashtableLock.ExitWriteLock();
			}
		}

		public IResource this[object key]
		{
			get => Hashtable[key] as IResource;
		}
		public TResource Get<TResource>(object key) where TResource : class, IResource
			=> Hashtable[key] as TResource;

		public void UpdateRenderTarget(RenderTarget target)
		{
			HashtableLock.EnterWriteLock();
			try
			{
				foreach (var resource in Hashtable.Values)
					(resource as IResource)?.UpdateRenderTarget(target);
			}
			finally
			{
				HashtableLock.ExitWriteLock();
			}
		}

		public void Dispose()
		{
			foreach (var resource in Hashtable)
				(resource as IDisposable)?.Dispose();
			lock (Hashtable)
				Hashtable.Clear();
		}
	}
}
