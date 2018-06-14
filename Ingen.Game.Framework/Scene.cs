using Ingen.Game.Framework.Resources;
using SharpDX.Direct2D1;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Attributes;

namespace Ingen.Game.Framework
{
	public abstract class Scene : INotifyCompletion, IDisposable
	{
		protected ResourceLoader Resource { get; }
		public Scene()
		{
			Resource = new ResourceLoader();
		}

		protected RenderTarget RenderTarget { get; private set; }

		public virtual void UpdateRenderTarget(RenderTarget target)
		{
			RenderTarget = target;
			Resource.UpdateRenderTarget(target);
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
		#endregion

	}
}
