using Ingen.Game.Framework.Resources;
using SharpDX.Direct2D1;
using DXGI = SharpDX.DXGI;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Ingen.Game.Framework
{
	public abstract class Scene : INotifyCompletion, IDisposable
	{
		protected ResourceLoader Resource { get; }
		internal Bitmap1 BackBuffer { get; private set; }
		public Scene()
		{
			Resource = new ResourceLoader();
		}

		protected DeviceContext DeviceContext { get; private set; }

		public virtual void UpdateDevice(GameContainer container)
		{
			DeviceContext = container.DeviceContext;
			Resource.UpdateDevice(container);

			BackBuffer?.Dispose();
			BackBuffer = null;
			//BackBuffer = new Bitmap1(container.DeviceContext, container.D2D1BackBuffer.PixelSize);
		}

		public abstract void Render();

		public void DoUpdate()
		{
			if (SkipCount > 1)
			{
				SkipCount--;
				return;
			}
			if (ResumeConditionChecker?.Invoke() == false)
				return;
			if (Continuation != null)
			{
				SkipCount = 0;
				IsCompleted = true;
				ResumeConditionChecker = null;
				Continuation();
				return;
			}
			Update();
		}
		protected abstract void Update();

		public virtual void Dispose()
		{
			BackBuffer?.Dispose();
			BackBuffer = null;
			Resource.Dispose();
		}

		#region INotifyCompletion member
		private Action Continuation { get; set; }
		/// <summary>
		/// 待機時の再開条件チェックメソッド trueを返すと再開します。
		/// </summary>
		public Func<bool> ResumeConditionChecker { get; set; }

		public uint SkipCount { get; internal set; }

		public Scene GetAwaiter() => this;
		public bool IsCompleted { get; private set; } = true;
		public void OnCompleted(Action continuation)
			=> Continuation = continuation;
		public void GetResult()
		{ }

		public Scene SkipTick(uint waitTicks = 1)
		{
			SkipCount = waitTicks;
			IsCompleted = false;
			return this;
		}
		public Scene SkipTick(Task task)
		{
			if (task.Status == TaskStatus.Created)
				task.Start();
			ResumeConditionChecker = () => task.IsCompleted || task.IsFaulted;
			return SkipTick();
		}
		public Scene SkipTick(Animation animation)
		{
			ResumeConditionChecker = () => !animation.IsStarted;
			return SkipTick();
		}
		public Scene SkipTick(Animation animation, TimeSpan dulation)
		{
			animation.Start(dulation);
			ResumeConditionChecker = () => !animation.IsStarted;
			return SkipTick();
		}
		#endregion

	}
}
