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
		public static GameContainer UseMouseInputService(this GameContainer container)
		{
			container.GetService<MouseInputService>();
			return container;
		}
	}
}
