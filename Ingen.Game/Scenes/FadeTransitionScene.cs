using Ingen.Game.Framework;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Threading.Tasks;

namespace Ingen.Game.Scenes
{
	public class FadeTransitionScene : TransitionScene
	{
		TimeSpan FadeTime;
		GameContainer Container;
		LoadingOverlay Overlay;
		public FadeTransitionScene(LoadingOverlay overlay, GameContainer container, TimeSpan fadeTime)
		{
			Container = container;
			Overlay = overlay;
			FadingAnimation = new Animation(container);
			FadeTime = fadeTime;
			parameter = new LayerParameters()
			{
				ContentBounds = RectangleF.Infinite,
			};
		}

		private enum State
		{
			/// <summary>
			/// 初期化直後
			/// </summary>
			Neutral,
			/// <summary>
			/// ロード画面への推移エフェクト発生中
			/// </summary>
			FadingToLoadScreen,
			/// <summary>
			/// ロード画面からの推移エフェクト発生中
			/// </summary>
			FadingFromLoadScreen,
			/// <summary>
			/// ロード画面なしで新画面へ推移エフェクト発生中
			/// </summary>
			FadingFromOldSceneToNewScene,
			/// <summary>
			/// 前の画面でタスク待機
			/// </summary>
			WaitingOnOldScene,
			/// <summary>
			/// 読み込み画面の状態でタスク待機
			/// </summary>
			WaitingOnLoadScreen,
		}
		private State CurrentState;

		bool isNeedLoadingScreen;
		Timing DisposeTiming;
		Timing InitalizeTiming;

		Scene CurrentScene;

		Type NewSceneType;
		Scene NewScene;

		public override void Initalize<TScene>(Scene currentScene)
		{
			CurrentState = State.Neutral;

			DisposeTiming = currentScene.GetType().GetNavigateOptionsAttribute()?.DestroyTiming ?? Timing.Before;
			InitalizeTiming = typeof(TScene).GetNavigateOptionsAttribute()?.InitalizeTiming ?? Timing.After;

			CurrentScene = currentScene;
			NewSceneType = typeof(TScene);
			NewScene = null;

			isNeedLoadingScreen = (DisposeTiming == Timing.Before || InitalizeTiming == Timing.After);
		}

		Task SceneWorkTask;
		LayerParameters parameter;
		public override void Render()
		{
			switch (CurrentState)
			{
				case State.Neutral:
				case State.WaitingOnOldScene:
					CurrentScene.Render();
					break;
				case State.WaitingOnLoadScreen:
					RenderLoadingScreen();
					break;
				case State.FadingToLoadScreen:
					CurrentScene?.Render();
					using (var layer = new Layer(RenderTarget))
					{
						parameter.Opacity = FadingAnimation.Value;
						RenderTarget.PushLayer(ref parameter, layer);
						RenderLoadingScreen();
						RenderTarget.PopLayer();
					}
					break;
				case State.FadingFromLoadScreen:
					RenderLoadingScreen();
					using (var layer = new Layer(RenderTarget))
					{
						parameter.Opacity = FadingAnimation.Value;
						RenderTarget.PushLayer(ref parameter, layer);
						NewScene.Render();
						RenderTarget.PopLayer();
					}
					break;
				case State.FadingFromOldSceneToNewScene:
					CurrentScene.Render();
					using (var layer = new Layer(RenderTarget))
					{
						parameter.Opacity = FadingAnimation.Value;
						RenderTarget.PushLayer(ref parameter, layer);
						NewScene.Render();
						RenderTarget.PopLayer();
					}
					break;
			}
		}
		private void RenderLoadingScreen()
		{
			RenderTarget.Clear(Color.Blue);
		}

		public override void Update()
		{
			switch (CurrentState)
			{
				//初回
				case State.Neutral:
					//読み込み画面が必要であれば移行開始
					if (isNeedLoadingScreen)
					{
						FadingAnimation.Start(FadeTime);
						CurrentState = State.FadingToLoadScreen;
					}
					//必要なければインスタンス生成
					else
					{
						Overlay.IsShown = true;
						SceneWorkTask = Task.Run(() => {
							NewScene = (Scene)Container.Resolve(NewSceneType);
							NewScene.UpdateRenderTarget(RenderTarget);
						});
						CurrentState = State.WaitingOnOldScene;
					}
					break;
				case State.FadingToLoadScreen:
					//読み込み画面へのアニメーションが終了したら破棄して読み込み
					if (FadingAnimation.IsStarted)
						break;
					Overlay.IsShown = true;
					SceneWorkTask = Task.Run(() =>
					{
						CurrentScene.Dispose();
						CurrentScene = null;
						NewScene = (Scene)Container.Resolve(NewSceneType);
						NewScene.UpdateRenderTarget(RenderTarget);
					});
					CurrentState = State.WaitingOnLoadScreen;
					break;
				case State.WaitingOnLoadScreen:
					//(読み込み画面が必要なときに)読み込みが完了したので移行開始
					if (SceneWorkTask.Status != TaskStatus.RanToCompletion)
						break;
					Overlay.IsShown = false;
					FadingAnimation.Start(FadeTime);
					CurrentState = State.FadingFromLoadScreen;
					break;
				case State.FadingFromLoadScreen:
					//移行完了したら役目は終了!
					if (FadingAnimation.IsStarted)
						break;
					Container.CurrentScene = NewScene;
					break;

				case State.WaitingOnOldScene:
					//(読み込み画面が必要ないときに)読み込みが完了したので移行開始
					if (SceneWorkTask.Status != TaskStatus.RanToCompletion)
						break;
					Overlay.IsShown = false;
					FadingAnimation.Start(FadeTime);
					CurrentState = State.FadingFromOldSceneToNewScene;
					break;
				case State.FadingFromOldSceneToNewScene:
					//移行完了したら役目は終了!
					if (FadingAnimation.IsStarted)
						break;
					Task.Run(() =>
					{
						CurrentScene.Dispose();
						CurrentScene = null;
					});
					Container.CurrentScene = NewScene;
					break;
			}
		}

		Animation FadingAnimation;
	}
}
