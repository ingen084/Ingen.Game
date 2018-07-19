using SharpDX.DirectInput;
using System.Linq;
using Unity;
using Unity.Lifetime;

namespace Ingen.Game.Framework.Input
{
	public class MouseRawInputService : IGameService
	{
		DirectInput directInput;
		Mouse mouseDevice;
		public bool Initalized => mouseDevice != null;

		public MouseRawInputService(GameContainer container /*, DirectInput directInput*/)
		{
			if (directInput == null)
			{
				directInput = new DirectInput();
				container.Container.RegisterInstance(directInput, new ContainerControlledLifetimeManager());
			}

			//マウス
			if (directInput.GetDevices(DeviceType.Mouse, DeviceEnumerationFlags.AllDevices).Any())
			{
				mouseDevice = new Mouse(directInput);
				mouseDevice.Acquire();
			}
		}

		public MouseState LastMouseState { get; private set; }

		public void Render()
		{
		}

		public void Update()
		{
			LastMouseState = mouseDevice?.GetCurrentState();
		}

		public void Dispose()
		{
			mouseDevice?.Dispose();
			mouseDevice = null;
			directInput?.Dispose();
			directInput = null;
		}
	}
}
