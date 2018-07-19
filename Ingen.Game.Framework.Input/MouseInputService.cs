using SharpDX.DirectInput;
using SharpDX.Mathematics.Interop;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ingen.Game.Framework.Input
{
	public class MouseInputService : IGameService
	{
		GameContainer Container;
		DirectInput directInput;
		Mouse device;

		public MouseInputService(GameContainer container)
		{
			Container = container;
			directInput = new DirectInput();
			device = new Mouse(directInput);
			device.Acquire();
		}

		public MouseState LastMouseState { get; private set; }
		public RawVector2 LastPosition { get; private set; }

		public void Render()
		{
		}

		public void Update()
		{
			LastMouseState = device?.GetCurrentState();
			Point pos = Cursor.Position;
			NativeMethods.ScreenToClient(Container.GameWindowPtr, ref pos);
			LastPosition = new RawVector2(pos.X, pos.Y);
		}

		public void Dispose()
		{
			device?.Dispose();
			device = null;
			directInput?.Dispose();
			directInput = null;
		}
	}
}
