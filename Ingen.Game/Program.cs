using Ingen.Game.Framework;
using Ingen.Game.Framework.Navigator;
using Ingen.Game.Framework.Resources.Brushes;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;

namespace Ingen.Game
{
	internal class Program
	{
		public static void Main()
		{
			using (var container = new GameContainer(false, "Game Sample", 1280, 720))
			using (var scene = container.Resolve<SampleScene>())
				container.Start(scene);
		}

		[NavigateOptions(Timing.Before, Timing.After)]
		public class SampleScene : Scene
		{
			GameForm Form { get; }
			GameContainer Container { get; }

			TextFormat format;
			public SampleScene(GameForm form, GameContainer container)
			{
				Form = form;
				Container = container;

				lastTime = DateTime.Now;
				position = 10;
				Resource.AddResource("MainBrush", new SolidColorBrushResource(new RawColor4(1, 1, 1, 1)));
			}

			public override void UpdateRenderTarget(RenderTarget target)
			{
				if (format == null)
					format = new TextFormat(Form.DWFactory, "MS Gothic", FontWeight.Light, FontStyle.Normal, 32);
				base.UpdateRenderTarget(target);
			}

			public override void Render()
			{
				RenderTarget.DrawText(lastTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "\n" + Container.Elapsed.ToString(), format, new RawRectangleF(10, 10, 500, 500), Resource.Get<BrushResource>("MainBrush").Brush);
				RenderTarget.FillRectangle(new RawRectangleF(position, 100, position + 100, 200), Resource.Get<BrushResource>("MainBrush").Brush);
			}

			DateTime lastTime;
			bool direction = false;
			float position;
			public override void Update()
			{
				if (direction)
					position -= 1.2f;
				else
					position += 2.4f;
				if (position > 200)
				{
					position = 200;
					direction = true;
				}
				if (position < 10)
				{
					position = 10;
					direction = false;
				}

				lastTime = DateTime.Now;
			}

			public override void Dispose()
			{
				format.Dispose();
				base.Dispose();
			}
		}
	}
}
