using SharpDX.DirectInput;
using SharpDX.Mathematics.Interop;
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
		public bool IsRendertimeUpdate { get; set; }

		public void Render()
		{
			if (IsRendertimeUpdate)
				UpdatePosition();
		}

		public void Update()
		{
			if (!IsRendertimeUpdate)
				UpdatePosition();
		}
		private void UpdatePosition()
		{
			LastMouseState = device?.GetCurrentState();
			Point pos = Cursor.Position;
			NativeMethods.ScreenToClient(Container.GameWindowHandle, ref pos);
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
