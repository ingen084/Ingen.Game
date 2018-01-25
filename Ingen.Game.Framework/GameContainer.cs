using Ingen.Game.Framework.Scenes;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace Ingen.Game.Framework.Navigator
{
	public class GameContainer : IDisposable
	{
		public UnityContainer Container { get; }

		public GameForm GameWindow { get; }

		public Scene CurrentScene { get; private set; }

		public bool IsLinkFrameAndLogic { get; }

		private CancellationTokenSource TasksCancellationTokenSource;
		private Task RenderTask;
		private Task LogicTask;

		public ushort TpsRate { get; set; } = 30;

		public GameContainer(bool isLinkFrameAndLogic, Size windowSize)
		{
			IsLinkFrameAndLogic = isLinkFrameAndLogic;

			Container = new UnityContainer();
			Container.RegisterInstance(this);
			Container.RegisterInstance(GameWindow = new GameForm(IsLinkFrameAndLogic));

			TasksCancellationTokenSource = new CancellationTokenSource();
			RenderTask = new Task(Render, TasksCancellationTokenSource.Token, TaskCreationOptions.LongRunning);
			if (!IsLinkFrameAndLogic)
				LogicTask = new Task(Logic, TasksCancellationTokenSource.Token, TaskCreationOptions.LongRunning);
		}

		public void Navigate()
		{
		}

		public void Begin(Scene startupScene)
		{
			CurrentScene = startupScene;
			RenderTask.Start();
			beforeTime = DateTime.Now;
			LogicTask?.Start();
		}

		public void Render()
		{
			if (!IsLinkFrameAndLogic)
				Logic();
			GameWindow.BeginDraw();
			CurrentScene.Render(GameWindow.RenderTarget);
			GameWindow.EndDraw();
		}
		DateTime beforeTime;
		public void Logic()
		{
			//ロジック
			if (!IsLinkFrameAndLogic)
			{
				var waitMs = (1000.0 / TpsRate) - (beforeTime - DateTime.Now).TotalMilliseconds;
				beforeTime = DateTime.Now;
				if (waitMs > 1)
					Thread.Sleep((int)waitMs);
			}

		}

		public void Dispose()
		{
			
			GameWindow?.Dispose();
			Container?.Dispose();
		}
	}
}
