using SharpDX.DirectInput;
using System;
using System.Linq;

namespace Ingen.Game.Framework.Input
{
	public class InputService : IGameService
	{
		DirectInput directInput;
		Keyboard keyboardDevice;
		Mouse mouseDevice;
		Joystick joystickDevice;

		//todo サービス分割
		public InputService()
		{
			directInput = new DirectInput();

			//キーボード
			if (directInput.GetDevices(DeviceType.Keyboard, DeviceEnumerationFlags.AllDevices).Any())
			{
				keyboardDevice = new Keyboard(directInput);
				keyboardDevice.Acquire();
			}

			//マウス
			if (directInput.GetDevices(DeviceType.Mouse, DeviceEnumerationFlags.AllDevices).Any())
			{
				mouseDevice = new Mouse(directInput);
				mouseDevice.Acquire();
			}

			//ジョイスティック
			var stickGuid = directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices).FirstOrDefault()?.InstanceGuid ?? Guid.Empty;
			if (stickGuid != Guid.Empty)
			{
				joystickDevice = new Joystick(directInput, stickGuid);
				joystickDevice.Acquire();
			}
		}

		public void Render()
		{
		}

		public void Update()
		{
			//var state = mouseDevice.GetCurrentState();
			//System.Diagnostics.Debug.WriteLine($"{state.X},{state.Y},{state.Z}");
		}

		public void Dispose()
		{
			keyboardDevice?.Dispose();
			keyboardDevice = null;
			mouseDevice?.Dispose();
			mouseDevice = null;
			directInput?.Dispose();
			directInput = null;
		}
	}
}
