using Ingen.Game.Framework;
using Ingen.Game.Framework.Navigator;
using Ingen.Game.Framework.Resources.Brushes;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ingen.Game
{
	internal class Program
	{
		public static void Main()
		{
			using (var container = new GameContainer(false, 1280, 720) { TpsRate = 10 })
			using (var scene = container.Resolve<SampleScene>())
				container.Start(scene);
		}
		public class SampleScene : Scene
		{
			GameForm Form { get; }
			TextFormat format;
			public SampleScene(GameForm form)
			{
				Form = form;
				lastTime = DateTime.Now;
				Resource.AddResource("MainBrush", new SolidColorBrushResource(new RawColor4(1, 1, 1, 1)));
			}

			public override void UpdateRenderTarget(RenderTarget target)
			{
				if (format == null)
					format = new TextFormat(Form.DWFactory, "Consolas", FontWeight.Light, FontStyle.Normal, 32);
				base.UpdateRenderTarget(target);
			}

			public override void Render()
			{
				RenderTarget.DrawText(lastTime.ToString("yyyy/MM/dd HH:mm:ss.fff"), format, new RawRectangleF(10, 10, 500, 500), Resource.Get<BrushResource>("MainBrush").Brush);
			}

			DateTime lastTime;
			public override void Update()
			{
				lastTime = DateTime.Now;
			}

			public override void Dispose()
			{
				format?.Dispose();
				base.Dispose();
			}
		}
	}
}
