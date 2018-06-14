using System;
using System.Threading.Tasks;

namespace Ingen.Game.Framework
{
	public static class Extensions
	{
		public static void RegistAnimationCompleteCondition(this Scene scene, Animation animation)
		{
			if (!animation.IsStarted)
				throw new Exception("アニメーションが開始されていません。");
			if (animation.IsLoopMode)
				throw new Exception("ループモードのアニメーションは待機できません。");
			scene.ResumeConditionChecker = () => !animation.IsStarted;
		}
		public static void StartAnimationAndRegistCompleteCondition(this Scene scene, Animation animation, TimeSpan time)
		{
			animation.Start(time);
			scene.RegistAnimationCompleteCondition(animation);
		}

		public static void RegistTaskCompleteCondition(this Scene scene, Task task)
		{
			if (task.Status == TaskStatus.Created)
				task.Start();
			scene.ResumeConditionChecker = () => task.IsCompleted || task.IsCanceled;
		}
	}
}
