﻿using Ingen.Game.Framework;
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
		public override void UpdateDevice(GameContainer container)
		{
			base.UpdateDevice(container);
			CurrentScene?.UpdateDevice(container);
			NextScene?.UpdateDevice(container);
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
					using (var layer = new Layer(DeviceContext))
					{
						parameter.Opacity = FadingAnimation.Value;
						DeviceContext.PushLayer(ref parameter, layer);
						RenderLoadingScreen();
						DeviceContext.PopLayer();
					}
					break;
				case RenderState.FadeFromLoadScreen:
					RenderLoadingScreen();
					using (var layer = new Layer(DeviceContext))
					{
						parameter.Opacity = FadingAnimation.Value;
						DeviceContext.PushLayer(ref parameter, layer);
						NextScene.Render();
						DeviceContext.PopLayer();
					}
					break;
				case RenderState.FadeFromPreviousSceneToNextScene:
					CurrentScene.Render();
					using (var layer = new Layer(DeviceContext))
					{
						parameter.Opacity = FadingAnimation.Value;
						DeviceContext.PushLayer(ref parameter, layer);
						NextScene.Render();
						DeviceContext.PopLayer();
					}
					break;
			}
		}
		private void RenderLoadingScreen()
		{
			DeviceContext.Clear(Color.Black);
		}

		protected override async void Update()
		{
			//読み込み画面が必要な場合
			if (IsNeedLoadingScreen)
			{
				//読み込み画面へ移行
				CurrentState = RenderState.FadeToLoadScreen;
				await SkipTick(FadingAnimation, FadeTime);

				//読み込み待機
				CurrentState = RenderState.LoadScreen;
				Overlay.IsShown = true;
				await SkipTick(Task.Run(() =>
				{
					CurrentScene?.Dispose();
					CurrentScene = null;
					NextScene = (Scene)Container.Resolve(NextSceneType);
					Container.GameWindow.SetActionAndWaitNextFrame(() => NextScene.UpdateDevice(Container)); //todo FormsのDLLを使用しないといけないのはいささか不本意である。
				}));
				Overlay.IsShown = false;

				//新しい画面へ移行
				CurrentState = RenderState.FadeFromLoadScreen;
				await SkipTick(FadingAnimation, FadeTime);

				//移行完了で役目は終了
				Container.CurrentScene = NextScene;
				return;
			}

			//ロード画面を必要としないのであればインスタンスを生成する
			Overlay.IsShown = true;
			await SkipTick(Task.Run(() =>
			{
				NextScene = (Scene)Container.Resolve(NextSceneType);
				Container.GameWindow.SetActionAndWaitNextFrame(() => NextScene.UpdateDevice(Container)); //todo FormsのDLLを使用しないといけないのはいささか不本意である。
			}));

			//(読み込み画面が必要ないときに)読み込みが完了したので移行開始
			CurrentState = RenderState.FadeFromPreviousSceneToNextScene;
			Overlay.IsShown = false;
			await SkipTick(FadingAnimation, FadeTime);

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
