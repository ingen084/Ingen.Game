using Ingen.Game.Framework;
using Ingen.Game.Framework.Resources;
using Ingen.Game.Framework.Resources.Images;
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
			using (var container = new GameContainer(true, "Game Sample", 1280, 720) { TpsRate = 30 })
			{
				var lo = container.Resolve<LoadingOverlay>();
				container.AddOverlay(lo);
				container.AddOverlay(container.Resolve<DebugOverlay>());
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

				lastTime = DateTime.Now;
				position = 10;
				Resource.AddSolidColorBrushResource("MainBrush", new RawColor4(1, 1, 1, 1));
				Resource.AddPngImageResource("Image", Container.ImagingFactory, @"D:\ingen\Desktop\saikoro_145.png");

				//System.Threading.Thread.Sleep(1000);
			}

			public override void Render()
			{
				RenderTarget.Clear(Color.CornflowerBlue);

				RenderTarget.Transform = Matrix3x2.Rotation(position * 0.018f, new Vector2(position + 50, 150));
				RenderTarget.DrawBitmap(Resource, "Image", new RawRectangleF(position, 100, position + 100, 200));
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
					Container.Navigate<SampleScene2>(TransitionScene);
				}

				lastTime = DateTime.Now;
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

				//System.Threading.Thread.Sleep(3000);
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
			public override void Update()
			{
				if (direction)
					position -= 1.2f;
				else
					position += 3.4f;
				if (position > 500)
				{
					position = 500;
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
