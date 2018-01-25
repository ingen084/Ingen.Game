using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
