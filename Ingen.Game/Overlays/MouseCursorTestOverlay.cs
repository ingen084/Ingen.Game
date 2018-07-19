using Ingen.Game.Framework;
using Ingen.Game.Framework.Input;
using Ingen.Game.Framework.Resources.Brushes;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ingen.Game
{
	public class MouseCursorTestOverlay : Overlay
	{
		MouseRawInputService MouseRawInputService { get; }
		MouseWindowInputService MouseWindowInputService { get; }
		GameContainer Container { get; }

		RawVector2 CurrentPos = new RawVector2();

		TextFormat format;
		public MouseCursorTestOverlay(MouseRawInputService rawInputService, MouseWindowInputService windowInputService,GameContainer container)
		{
			Container = container;
			MouseRawInputService = rawInputService;
			MouseWindowInputService = windowInputService;

			Resource.AddResource("ForegroundBrush", new SolidColorBrushResource(new RawColor4(1, 1, 1, .9f)));
			format = new TextFormat(Container.DWFactory, "MS Gothic", FontWeight.Bold, FontStyle.Normal, 32);
		}

		public override int Priority => 2;

		public override void Render()
		{
			using (var layout = new TextLayout(Container.DWFactory, $"{CurrentPos.X},{CurrentPos.Y}", format, float.PositiveInfinity, float.PositiveInfinity))
				RenderTarget.DrawTextLayout(new RawVector2(0, 0), layout, Resource.Get<BrushResource>("ForegroundBrush").Brush);
			using (var layout = new TextLayout(Container.DWFactory, "↖", format, float.PositiveInfinity, float.PositiveInfinity))
				RenderTarget.DrawTextLayout(CurrentPos, layout, Resource.Get<BrushResource>("ForegroundBrush").Brush);
		}

		protected override void Update()
		{
			if (MouseRawInputService.LastMouseState != null)
			{
				CurrentPos.X += MouseRawInputService.LastMouseState.X;
				CurrentPos.Y += MouseRawInputService.LastMouseState.Y;

				if (Container.WindowWidth < CurrentPos.X)
					CurrentPos.X = Container.WindowWidth;
				if (CurrentPos.X < 0)
					CurrentPos.X = 0;

				if (Container.WindowHeight < CurrentPos.Y)
					CurrentPos.Y = Container.WindowHeight;
				if (CurrentPos.Y < 0)
					CurrentPos.Y = 0;
				return;
			}
			CurrentPos = MouseWindowInputService.LastMousePosition;
			//memo 終了テスト
			if (CurrentPos.X < -100 && CurrentPos.Y < -100)
				Container.Shutdown();
		}

		public override void Dispose()
		{
			format?.Dispose();
			format = null;

			base.Dispose();
		}
	}
}
