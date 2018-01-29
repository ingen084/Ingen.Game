namespace Ingen.Game.Framework
{
	public abstract class TransitionScene : Scene
	{
		public abstract void Initalize<TScene>(Scene currentScene) where TScene : Scene;
	}
}
