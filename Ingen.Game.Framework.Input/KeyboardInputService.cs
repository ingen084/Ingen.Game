using SharpDX.DirectInput;
using System.Linq;
using Unity;
using Unity.Lifetime;

namespace Ingen.Game.Framework.Input
{
	public class KeyboardInputService : IGameService
	{
		DirectInput directInput;
		Keyboard keyboardDevice;

		public KeyboardInputService(GameContainer container)
		{
			directInput = new DirectInput();

			//キーボード
			if (directInput.GetDevices(DeviceType.Keyboard, DeviceEnumerationFlags.AllDevices).Any())
			{
				keyboardDevice = new Keyboard(directInput);
				keyboardDevice.Acquire();
			}
		}

		public KeyboardState LastKeyboardState { get; private set; }

		public void Render()
		{
		}

		public void Update()
		{
			LastKeyboardState = keyboardDevice?.GetCurrentState();
		}

		public void Dispose()
		{
			keyboardDevice?.Dispose();
			keyboardDevice = null;
			directInput?.Dispose();
			directInput = null;
		}
	}
}
