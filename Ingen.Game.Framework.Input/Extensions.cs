using Ingen.Game.Framework.Input;

namespace Ingen.Game.Framework
{
	public static class Extensions
	{
		public static GameContainer UseKeyboardInputService(this GameContainer container)
		{
			container.GetService<KeyboardInputService>();
			return container;
		}
		public static GameContainer UseMouseRawInputService(this GameContainer container)
		{
			container.GetService<MouseRawInputService>();
			return container;
		}
		public static GameContainer UseMouseWindowInputService(this GameContainer container)
		{
			container.GetService<MouseWindowInputService>();
			return container;
		}
	}
}
