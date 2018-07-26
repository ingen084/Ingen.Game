using SharpDX.Direct2D1;
using System;
using System.Collections;

namespace Ingen.Game.Framework.Resources
{
	public class ResourceLoader : IDisposable
	{
		Hashtable Hashtable { get; set; }

		public ResourceLoader()
		{
			Hashtable = new Hashtable();
		}

		public void AddResource(string key, IResource resource)
		{
			lock (Hashtable)
				Hashtable.Add(key, resource);
		}

		public IResource this[object key]
		{
			get => Hashtable[key] as IResource;
		}
		public TResource Get<TResource>(object key) where TResource : class, IResource
			=> Hashtable[key] as TResource;

		public void UpdateDevice(GameContainer container)
		{
			foreach (var resource in Hashtable.Values)
				(resource as IResource).UpdateDevice(container);
		}

		public void Dispose()
		{
			if (Hashtable == null) return;
			foreach (var resource in Hashtable.Values)
				(resource as IResource).Dispose();
			Hashtable.Clear();
			Hashtable = null;
		}
	}
}
