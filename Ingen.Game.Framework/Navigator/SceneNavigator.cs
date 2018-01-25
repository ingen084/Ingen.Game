using Ingen.Game.Framework.Scenes;
using System;
using Unity;

namespace Ingen.Game.Framework.Navigator
{
	public class SceneNavigator : IDisposable
	{
		public UnityContainer Container { get; }

		public Scene CurrentScene { get; private set; }

		public SceneNavigator()
		{
			Container = new UnityContainer();
		}

		public void Render()
		{

		}
		public void Logic()
		{

		}

		public void Dispose()
		{
			Container.Dispose();
		}
	}
}
