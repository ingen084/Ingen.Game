using Ingen.Game.Framework.Input;

namespace Ingen.Game.Framework
{
	public static class Extensions
	{
		public static GameContainer UseInputService(this GameContainer container)
		{
			container.GetService<InputService>();
			return container;
		}
	}
}
