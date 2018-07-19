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
			try
			{
				var pos = (Point)Container.GameWindow.Invoke(new Func<Point>(() => Container.GameWindow.PointToClient(Cursor.Position)));
				LastMousePosition = new RawVector2(pos.X, pos.Y);
			}
			catch (ObjectDisposedException) { }
		}

		public void Dispose()
		{
		}
	}
}
