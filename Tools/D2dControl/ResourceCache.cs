using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;

namespace D2dControl
{
	public class ResourceCache
	{
		// - field -----------------------------------------------------------------------

		private Dictionary<string, Func<RenderTarget, object>> generators = new Dictionary<string, Func<RenderTarget, object>>();
		private Dictionary<string, dynamic> resources = new Dictionary<string, object>();
		private RenderTarget renderTarget = null;

		// - property --------------------------------------------------------------------

		public RenderTarget RenderTarget
		{
			get => renderTarget;
			set
			{
				renderTarget = value;
				UpdateResources();
			}
		}

		public int Count
			=> resources.Count;

		public dynamic this[string key]
			=> resources[key];

		public Dictionary<string, dynamic>.KeyCollection Keys
			=> resources.Keys;

		public Dictionary<string, dynamic>.ValueCollection Values
			=> resources.Values;

		// - public methods --------------------------------------------------------------

		public void Add(string key, Func<RenderTarget, dynamic> gen)
		{
			if (resources.TryGetValue(key, out dynamic resOld))
			{
				Disposer.SafeDispose(ref resOld);
				generators.Remove(key);
				resources.Remove(key);
			}

			if (renderTarget == null)
			{
				generators.Add(key, gen);
				resources.Add(key, null);
				return;
			}
			var res = gen(renderTarget);
			generators.Add(key, gen);
			resources.Add(key, res);
		}

		public void Clear()
		{
			foreach (var key in resources.Keys)
			{
				var res = resources[key];
				Disposer.SafeDispose(ref res);
			}
			generators.Clear();
			resources.Clear();
		}

		public bool ContainsKey(string key)
			=> resources.ContainsKey(key);

		public bool ContainsValue(object val)
			=> resources.ContainsValue(val);

		public Dictionary<string, dynamic>.Enumerator GetEnumerator()
			=> resources.GetEnumerator();

		public bool Remove(string key)
		{
			if (resources.TryGetValue(key, out dynamic res))
			{
				Disposer.SafeDispose(ref res);
				generators.Remove(key);
				resources.Remove(key);
				return true;
			}
			return false;
		}

		public bool TryGetValue(string key, out dynamic res)
			=> resources.TryGetValue(key, out res);

		// - private methods -------------------------------------------------------------

		private void UpdateResources()
		{
			if (renderTarget == null) return;

			foreach (var g in generators)
			{
				var key = g.Key;

				if (resources.TryGetValue(key, out dynamic resOld))
				{
					Disposer.SafeDispose(ref resOld);
					resources.Remove(key);
				}

				resources.Add(key, g.Value(renderTarget));
			}
		}
	}
}
