using Ingen.Game.Framework;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ingen.Game.Scenes
{
	public class FadeTransitionScene : TransitionScene
	{
		TimeSpan FadeTime { get; }
		GameContainer Container { get; }
		LoadingOverlay Overlay { get; }
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

		enum RenderState
		{
			/// <summary>
			/// 移行前のシーン描画
			/// </summary>
			PreviousScene,
			/// <summary>
			/// ロード画面への推移エフェクト発生中
			/// </summary>
			FadeToLoadScreen,
			/// <summary>
			/// ロード画面からの推移エフェクト発生中
			/// </summary>
			FadeFromLoadScreen,
			/// <summary>
			/// ロード画面なしで新画面へ推移エフェクト発生中
			/// </summary>
			FadeFromPreviousSceneToNextScene,
			/// <summary>
			/// 読み込み画面の状態でタスク待機
			/// </summary>
			LoadScreen,
		}
		RenderState CurrentState { get; set; }

		bool IsNeedLoadingScreen { get; set; }

		Scene CurrentScene { get; set; }

		Type NextSceneType { get; set; }
		Scene NextScene { get; set; }

		Animation FadingAnimation { get; set; }

		public override void Initalize<TScene>(Scene currentScene)
		{
			CurrentScene = currentScene;
			NextSceneType = typeof(TScene);
			NextScene = null;

			if (currentScene == null)
			{
				CurrentState = RenderState.FadeToLoadScreen;
				IsNeedLoadingScreen = true;
				return;
			}

			CurrentState = RenderState.PreviousScene;

			var DisposeTiming = currentScene.GetType().GetNavigateOptionsAttribute()?.DestroyTiming ?? Timing.Before;
			var InitalizeTiming = typeof(TScene).GetNavigateOptionsAttribute()?.InitalizeTiming ?? Timing.After;

			IsNeedLoadingScreen = (DisposeTiming == Timing.Before || InitalizeTiming == Timing.After);
		}

		LayerParameters parameter;
		public override void Render()
		{
			switch (CurrentState)
			{
				case RenderState.PreviousScene:
					CurrentScene.Render();
					break;
				case RenderState.LoadScreen:
					RenderLoadingScreen();
					break;
				case RenderState.FadeToLoadScreen:
					CurrentScene?.Render();
					using (var layer = new Layer(RenderTarget))
					{
						parameter.Opacity = FadingAnimation.Value;
						RenderTarget.PushLayer(ref parameter, layer);
						RenderLoadingScreen();
						RenderTarget.PopLayer();
					}
					break;
				case RenderState.FadeFromLoadScreen:
					RenderLoadingScreen();
					using (var layer = new Layer(RenderTarget))
					{
						parameter.Opacity = FadingAnimation.Value;
						RenderTarget.PushLayer(ref parameter, layer);
						NextScene.Render();
						RenderTarget.PopLayer();
					}
					break;
				case RenderState.FadeFromPreviousSceneToNextScene:
					CurrentScene.Render();
					using (var layer = new Layer(RenderTarget))
					{
						parameter.Opacity = FadingAnimation.Value;
						RenderTarget.PushLayer(ref parameter, layer);
						NextScene.Render();
						RenderTarget.PopLayer();
					}
					break;
			}
		}
		private void RenderLoadingScreen()
		{
			RenderTarget.Clear(Color.Black);
		}

		protected override async void Update()
		{
			//読み込み画面が必要な場合
			if (IsNeedLoadingScreen)
			{
				//読み込み画面へ移行
				CurrentState = RenderState.FadeToLoadScreen;
				this.StartAnimationAndRegistCompleteCondition(FadingAnimation, FadeTime);
				await SkipTick();

				//読み込み待機
				CurrentState = RenderState.LoadScreen;
				Overlay.IsShown = true;
				this.RegistTaskCompleteCondition(Task.Run(() =>
					{
						CurrentScene?.Dispose();
						CurrentScene = null;
						NextScene = (Scene)Container.Resolve(NextSceneType);
						NextScene.UpdateRenderTarget(RenderTarget);
					}));
				await SkipTick();
				Overlay.IsShown = false;

				//新しい画面へ移行
				CurrentState = RenderState.FadeFromLoadScreen;
				this.StartAnimationAndRegistCompleteCondition(FadingAnimation, FadeTime);
				await SkipTick();

				//移行完了で役目は終了
				Container.CurrentScene = NextScene;
				return;
			}

			//ロード画面を必要としないのであればインスタンスを生成する
			Overlay.IsShown = true;
			this.RegistTaskCompleteCondition(Task.Run(() =>
			{
				NextScene = (Scene)Container.Resolve(NextSceneType);
				NextScene.UpdateRenderTarget(RenderTarget);
			}));
			await SkipTick();

			//(読み込み画面が必要ないときに)読み込みが完了したので移行開始
			CurrentState = RenderState.FadeFromPreviousSceneToNextScene;
			Overlay.IsShown = false;
			this.StartAnimationAndRegistCompleteCondition(FadingAnimation, FadeTime);
			await SkipTick();

			//移行完了したら役目は終了 昔のシーンは破棄する
			ThreadPool.QueueUserWorkItem(s =>
			{
				CurrentScene.Dispose();
				CurrentScene = null;
			});
			Container.CurrentScene = NextScene;
		}
	}
}
