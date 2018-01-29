using Ingen.Game.Framework;
using Ingen.Game.Framework.Resources.Brushes;
using Ingen.Game.Framework.Resources.Images;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;

namespace Ingen.Game
{
	internal class Program
	{
		public static void Main()
		{
			using (var container = new GameContainer(false, "Game Sample", 1280, 720) { TpsRate = 30 })
			{
				using (var scene = container.Resolve<SampleScene>())
				{
					container.AddOverlay(container.Resolve<DebugOverlay>());
					container.AddOverlay(container.Resolve<LoadingOverlay>());
					container.Start(scene);
				}
			}
		}

		[NavigateOptions(Timing.Before, Timing.After)]
		public class SampleScene : Scene
		{
			public GameContainer Container { get; set; }

			public SampleScene(GameContainer container)
			{
				Container = container;

				lastTime = DateTime.Now;
				position = 10;
				Resource.AddResource("MainBrush", new SolidColorBrushResource(new RawColor4(1, 1, 1, 1)));
				Resource.AddResource("Image", new PngImageResource(Container.ImagingFactory, @"D:\ingen\Desktop\saikoro_145.png"));
			}

			public override void UpdateRenderTarget(RenderTarget target)
			{
				base.UpdateRenderTarget(target);
			}

			public override void Render()
			{
				RenderTarget.Transform = Matrix3x2.Rotation(position * 0.018f, new Vector2(position + 50, 150));
				RenderTarget.DrawBitmap(Resource.Get<ImageResource>("Image").Image, new RawRectangleF(position, 100, position + 100, 200), 1, BitmapInterpolationMode.Linear);
				RenderTarget.Transform = Matrix3x2.Identity;
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
				base.Dispose();
			}
		}
	}
}
