using Ingen.Game.Framework;
using Ingen.Game.Framework.Resources;
using Ingen.Game.Scenes;
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
			using (var container = new GameContainer(false, "Game Sample", 640, 480)
				{
					TpsRate = 30,
					CanResize = true
				}
				.UseMouseInputService())
			{
				var lo = container.Resolve<LoadingOverlay>();
				container.AddOverlay(lo);
				container.AddOverlay(container.Resolve<DebugOverlay>());
				container.AddOverlay(container.Resolve<MouseCursorTestOverlay>());
				var ts = new FadeTransitionScene(lo, container, TimeSpan.FromSeconds(.25));
				container.AddSingleton(ts);
				ts.Initalize<SampleScene>(null);
				container.Start(ts);
			}
		}

		[NavigateOptions(Timing.Before, Timing.After)]
		public sealed class SampleScene : Scene
		{
			public GameContainer Container { get; set; }
			FadeTransitionScene TransitionScene { get; }

			public SampleScene(GameContainer container, FadeTransitionScene tscene)
			{
				Container = container;
				TransitionScene = tscene;

				position = 10;
				Resource.AddSolidColorBrushResource("MainBrush", new RawColor4(1, 1, 1, 1));
				Resource.AddPngImageResource("Image", Container.ImagingFactory, @"D:\ingen\Desktop\saikoro_145.png");

				System.Threading.Thread.Sleep(1000);
			}

			public override void Render()
			{
				RenderTarget.Clear(Color.CornflowerBlue);

				RenderTarget.Transform = Matrix3x2.Rotation(position * 0.018f, new Vector2(position + 50, 150));
				RenderTarget.DrawBitmap(Resource, "Image", new RawRectangleF(position, 100, position + 100, 200));
				RenderTarget.Transform = Matrix3x2.Identity;
			}

			float position;
			protected override async void Update()
			{
				for (; position < 300; position += 2.4f)
					await SkipTick();

				for (; position > 10; position -= 1.2f)
					await SkipTick();

				Container.Navigate<SampleScene2>(TransitionScene);
			}

			public override void Dispose()
			{
				base.Dispose();
			}
		}

		[NavigateOptions(Timing.Before, Timing.Before)]
		public sealed class SampleScene2 : Scene
		{
			public GameContainer Container { get; set; }
			FadeTransitionScene TransitionScene { get; }

			public SampleScene2(GameContainer container, FadeTransitionScene tscene)
			{
				Container = container;
				TransitionScene = tscene;

				lastTime = DateTime.Now;
				position = 10;
				Resource.AddSolidColorBrushResource("MainBrush", new RawColor4(1, 1, 1, 1));
				Resource.AddPngImageResource("Image", Container.ImagingFactory, @"D:\ingen\Desktop\saikoro_145.png");

				System.Threading.Thread.Sleep(1000);
			}

			public override void Render()
			{
				RenderTarget.Clear(Color.MediumPurple);

				RenderTarget.Transform = Matrix3x2.Rotation(position * 0.018f, new Vector2(position + 100, 200));
				RenderTarget.DrawBitmap(Resource, "Image", new RawRectangleF(position, 100, position + 200, 300));
				RenderTarget.Transform = Matrix3x2.Identity;
			}

			DateTime lastTime;
			bool direction = false;
			float position;
			protected override void Update()
			{
				if (direction)
					position -= 1.2f;
				else
					position += 3.4f;
				if (position > 300)
				{
					position = 300;
					direction = true;
				}
				if (position < 10)
				{
					position = 10;
					direction = false;
					Container.Navigate<SampleScene>(TransitionScene);
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
