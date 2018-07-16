using System;

namespace Ingen.Game.Framework
{
	/// <summary>
	/// サービスは他のシーンなどから共有して利用される前提のもの。
	/// <para>必ず処理開始前に呼ばれる。</para>
	/// </summary>
	public interface IGameService : IDisposable
	{
		void Render();
		void Update();
	}
}
