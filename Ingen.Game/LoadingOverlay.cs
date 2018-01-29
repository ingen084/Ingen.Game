using Ingen.Game.Framework;
using Ingen.Game.Framework.Resources.Brushes;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;

namespace Ingen.Game
{
	public class LoadingOverlay : Overlay
	{
		GameContainer Container { get; }

		TextFormat format;

		public override int Priority => 1;

		public LoadingOverlay(GameContainer container)
		{
			Resource.AddResource("Fore", new SolidColorBrushResource(new RawColor4(1, 0.64705882352941176470588235294118f, 0, 1)));
			Resource.AddResource("Back", new SolidColorBrushResource(new RawColor4(1, 0.64705882352941176470588235294118f, 0, .2f)));
			format = new TextFormat(container.DWFactory, "Nishiki-teki", 32);

			Container = container;

			LoopAnimation = new Animation(Container);
			FadeAnimation = new Animation(Container);

			parameter = new LayerParameters()
			{
				ContentBounds = RectangleF.Infinite,
			};
		}

		LayerParameters parameter;

		public override void Render()
		{
			if (isFadeOut && FadeAnimation.Value % 1 == 0)
				return;

			using (var layer = new Layer(RenderTarget))
			{
				parameter.Opacity = (isFadeOut ? 1 - FadeAnimation.Value : FadeAnimation.Value);
				RenderTarget.PushLayer(ref parameter, layer);

				using (var layout = new TextLayout(Container.DWFactory, $"Now Loading...", format, float.PositiveInfinity, float.PositiveInfinity))
				using (var layout2 = new TextLayout(Container.DWFactory, "🕛", format, float.PositiveInfinity, float.PositiveInfinity))
				{
					const int Radius = 5;
					const int Margin = 20;
					const int SplitMargin = 5;

					RenderTarget.FillRoundedRectangle(new RoundedRectangle
					{
						RadiusX = Radius,
						RadiusY = Radius,
						Rect = new RawRectangleF(Container.WindowWidth - layout.Metrics.Width - SplitMargin - layout2.Metrics.Width - Margin - Radius * 2, Container.WindowHeight - layout.Metrics.Height - Margin - Radius * 2, Container.WindowWidth - Radius * 2, Container.WindowHeight - Radius * 2)
					}, Resource.Get<BrushResource>("Back").Brush);

					RenderTarget.DrawTextLayout(new RawVector2(Container.WindowWidth - layout.Metrics.Width - Margin, Container.WindowHeight - layout.Metrics.Height - Margin), layout, Resource.Get<BrushResource>("Fore").Brush);

					var origin = new Vector2(Container.WindowWidth - layout.Metrics.Width - SplitMargin - layout2.Metrics.Width - Margin, Container.WindowHeight - layout.Metrics.Height - Margin);
					RenderTarget.Transform = Matrix3x2.Rotation(LoopAnimation.Value * 360 * (float)Math.PI / 180, origin + new Vector2(layout2.Metrics.Width / 2, layout.Metrics.Height / 2));
					RenderTarget.DrawTextLayout(origin, layout2, Resource.Get<BrushResource>("Fore").Brush);
					RenderTarget.Transform = Matrix3x2.Identity;
				}
				RenderTarget.PopLayer();
			}
		}

		Animation LoopAnimation;
		bool isFadeOut = true;
		Animation FadeAnimation;

		int count = 0;
		public override void Update()
		{
			count++;
			if (count % 60 == 0)
				IsShown = !IsShown;
		}

		private bool _isShown;
		public bool IsShown
		{
			get => _isShown;
			set
			{
				if (_isShown == value)
					return;
				_isShown = value;

				isFadeOut = !_isShown;
				FadeAnimation.Start(TimeSpan.FromSeconds(.2));
				if (IsShown)
					LoopAnimation.Start(TimeSpan.FromSeconds(1.25), true);
			}
		}

		public override void Dispose()
		{
			format.Dispose();
			base.Dispose();
		}
	}
}
