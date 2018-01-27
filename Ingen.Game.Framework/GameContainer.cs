using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace Ingen.Game.Framework.Navigator
{
	public class GameContainer : IDisposable
	{
		public UnityContainer Container { get; }

		public GameForm GameWindow { get; }

		public Scene CurrentScene { get; private set; }

		public bool IsLinkFrameAndLogic { get; }

		private HighPerformanceStopwatch Stopwatch { get; }
		public TimeSpan Elapsed => Stopwatch.Elapsed;

		private CancellationTokenSource TasksCancellationTokenSource;
		private Task RenderTask;
		private Task LogicTask;

		public ushort TpsRate { get; set; } = 30;

		private List<Overlay> Overlays { get; } = new List<Overlay>();
		public void AddOverlay(Overlay overlay)
		{
			overlay.UpdateRenderTarget(GameWindow.RenderTarget);
			Overlays.Add(overlay);
		}

		public GameContainer(bool isLinkFrameAndLogic, string windowTitle, int windowWidth, int windowHeight)
		{
			IsLinkFrameAndLogic = isLinkFrameAndLogic;

			Container = new UnityContainer();
			Container.RegisterInstance(this, new ContainerControlledLifetimeManager());
			Container.RegisterInstance(GameWindow = new GameForm() { ClientSize = new Size(windowWidth, windowHeight), Text = windowTitle }, new ContainerControlledLifetimeManager());

			TasksCancellationTokenSource = new CancellationTokenSource();
			RenderTask = new Task(Render, TasksCancellationTokenSource.Token, TaskCreationOptions.LongRunning);
			if (!IsLinkFrameAndLogic)
				LogicTask = new Task(Logic, TasksCancellationTokenSource.Token, TaskCreationOptions.LongRunning);

			Stopwatch = new HighPerformanceStopwatch();
		}

		public T Resolve<T>()
			=> Container.Resolve<T>();
		public void AddSingleton<T>(T instance = null) where T : class
		{
			if (instance == null)
				Container.RegisterType<T>(new ContainerControlledLifetimeManager());
			else
				Container.RegisterInstance(instance, new ContainerControlledLifetimeManager());
		}

		public void Navigate<TScene>(LoadingScene loadingScene) where TScene : Scene
			=> Navigate(loadingScene, Container.Resolve<TScene>());
		public void Navigate(LoadingScene loadingScene, Scene nextScene)
		{
			if (CurrentScene is LoadingScene)
				throw new InvalidOperationException("LoadingSceneからNavigateすることはできません。");

			loadingScene.Initalize(CurrentScene, nextScene);
			CurrentScene = loadingScene;
		}

		public void Start(Scene startupScene)
		{
			CurrentScene = startupScene;
			GameWindow.Initalize();
			Container.RegisterInstance(GameWindow.DWFactory, new ContainerControlledLifetimeManager());

			CurrentScene.UpdateRenderTarget(GameWindow.RenderTarget);

			Stopwatch.Start();
			RenderTask.Start();

			beforeTime = Stopwatch.Elapsed;
			LogicTask?.Start();

			GameWindow.ShowDialog();

			TasksCancellationTokenSource.Cancel();
			LogicTask?.Wait();
			RenderTask.Wait();
			Stopwatch.Stop();
		}

		public void Render()
		{
			while (!TasksCancellationTokenSource.Token.IsCancellationRequested)
			{
				if (IsLinkFrameAndLogic)
					CurrentScene.Update();
				GameWindow.BeginDraw();
				CurrentScene.Render();
				GameWindow.EndDraw();
			}
		}

		TimeSpan beforeTime;
		public void Logic()
		{
			while (!TasksCancellationTokenSource.Token.IsCancellationRequested)
			{
				if (!IsLinkFrameAndLogic)
				{
					var wait = (1000.0 / TpsRate) - (Stopwatch.Elapsed - beforeTime).TotalMilliseconds;
					if (wait >= 1)
						Thread.Sleep((int)wait);
				}
				beforeTime = Stopwatch.Elapsed;
				CurrentScene.Update();
			}
		}

		private bool isDisposed = false;
		public void Dispose()
		{
			if (isDisposed)
				return;
			isDisposed = true;
			GameWindow?.Dispose();
			Container?.Dispose();
		}
	}
}
