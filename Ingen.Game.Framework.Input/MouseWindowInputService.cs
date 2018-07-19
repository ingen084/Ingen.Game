using SharpDX.Mathematics.Interop;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ingen.Game.Framework.Input
{
	public class MouseWindowInputService : IGameService
	{
		GameContainer Container;

		public MouseWindowInputService(GameContainer container)
		{
			Container = container;
		}

		public RawVector2 LastMousePosition { get; private set; }

		public void Render()
		{
		}

		public void Update()
		{
			NativeMethods.POINT pos = new NativeMethods.POINT
			{
				X = Cursor.Position.X,
				Y = Cursor.Position.Y,
			};
			NativeMethods.ScreenToClient(Container.GameWindowPtr, ref pos);
			LastMousePosition = new RawVector2(pos.X, pos.Y);
		}

		public void Dispose()
		{
		}
	}
}
