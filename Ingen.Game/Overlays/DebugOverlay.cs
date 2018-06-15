using Ingen.Game.Framework;
using Ingen.Game.Framework.Resources.Brushes;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System.Collections.Generic;
using System.Linq;

namespace Ingen.Game
{
	public class DebugOverlay : Overlay
	{
		Queue<double> FrameTimeQueue { get; } = new Queue<double>();
		Queue<double> UpdateTimeQueue { get; } = new Queue<double>();

		GameContainer Container { get; }

		public override int Priority => int.MaxValue;

		TextFormat format;
		public DebugOverlay(GameContainer container)
		{
			Container = container;

			Resource.AddResource("ForegroundBrush", new SolidColorBrushResource(new RawColor4(1, 1, 1, .9f)));
			format = new TextFormat(Container.DWFactory, "Consolas", FontWeight.Normal, FontStyle.Normal, 16);

			UpdateTimeQueue.Enqueue(0);//dummy
		}

		double beforeFrameTime = 0;
		public override void Render()
		{
			var current = Container.Elapsed.TotalMilliseconds;
			FrameTimeQueue.Enqueue(current - beforeFrameTime);
			beforeFrameTime = current;
			if (FrameTimeQueue.Count > 120)
				FrameTimeQueue.Dequeue();

			lock (UpdateTimeQueue)
				using (var layout = new TextLayout(Container.DWFactory,
					$"Elapsed: {Container.Elapsed.ToString(@"dd\.hh\:mm\:ss\.fff")}\n" +
					$"FPS    : {(1000.0 / FrameTimeQueue.Average()).ToString("0.0")}\n" +
					$"TPS    : {(1000.0 / UpdateTimeQueue.Average()).ToString("0.0")}\n" +
					$"Scene  : {Container.CurrentScene.GetType().Name}", format, float.PositiveInfinity, float.PositiveInfinity))
					RenderTarget.DrawTextLayout(new RawVector2(0, Container.WindowHeight - layout.Metrics.Height), layout, Resource.Get<BrushResource>("ForegroundBrush").Brush);
		}

		double beforeUpdateTime = 0;
		protected override void Update()
		{
			lock (UpdateTimeQueue)
			{
				var current = Container.Elapsed.TotalMilliseconds;
				UpdateTimeQueue.Enqueue(current - beforeUpdateTime);
				if (UpdateTimeQueue.Count > 120)
					UpdateTimeQueue.Dequeue();
				beforeUpdateTime = current;
			}
		}

		public override void Dispose()
		{
			format?.Dispose();
			format = null;
			base.Dispose();
		}
	}
}
