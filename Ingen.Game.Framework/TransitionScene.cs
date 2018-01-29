using System;

namespace Ingen.Game.Framework
{
	public abstract class TransitionScene : Scene
	{
		public abstract void Initalize(Scene currentScene, Scene nextScene);
	}
}
