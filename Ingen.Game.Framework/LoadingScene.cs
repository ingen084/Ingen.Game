using System;

namespace Ingen.Game.Framework
{
	public abstract class LoadingScene : Scene
	{
		public abstract void Initalize(Scene currentScene, Scene nextScene);
	}
}
