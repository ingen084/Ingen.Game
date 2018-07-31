using Ingen.Game.Framework.Resources.Sprite;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2D1SpriteBatch = SharpDX.Direct2D1.SpriteBatch;

namespace Ingen.Game.Framework.Sprite
{
	public class SpriteBatch
	{
		GameContainer Container { get; }
		LinkedList<SpriteRenderParameter> RenderParameters { get; }

		public SpriteBatch(GameContainer container)
		{
			Container = container;
			RenderParameters = new LinkedList<SpriteRenderParameter>();
		}

		bool Begining;
		public void Begin()
		{
			if (Begining)
				throw new InvalidOperationException("描画操作はすでに開始されています。");
			Begining = true;
			RenderParameters.Clear();
		}

		/// <summary>
		/// スプライトを描画キューに登録します。
		/// </summary>
		/// <param name="sprite">描画するスプライト</param>
		/// <param name="rect">描画範囲</param>
		/// <param name="rotate">回転</param>
		/// <param name="rotateOrigin">中心回転座標(スプライトに対して0~1で指定する)</param>
		public void Draw(SpriteResource sprite, RawRectangleF rect, float rotate, RawVector2 rotateOrigin)
		{
			if (!Begining)
				throw new InvalidOperationException("描画操作が開始していません。");
			RenderParameters.AddLast(new SpriteRenderParameter(sprite, rotate, rotateOrigin, rect));
		}
		public void Draw(SpriteResource sprite, RawVector2 origin, float rotate, RawVector2 rotateOrigin)
			=> Draw(sprite, new RawRectangleF(origin.X, origin.Y, origin.X + (sprite.Rect.Right - sprite.Rect.Left), origin.Y + (sprite.Rect.Bottom - sprite.Rect.Top)), rotate, rotateOrigin);
		public void Draw(SpriteResource sprite, RawVector2 origin, float rotate)
			=> Draw(sprite, origin, rotate, new RawVector2(.5f, .5f));
		public void Draw(SpriteResource sprite, RawVector2 origin)
			=> Draw(sprite, origin, 0);



		public void End()
		{
			if (!Begining)
				throw new InvalidOperationException("描画操作が開始していません。");
			Begining = false;
			if (RenderParameters.Count == 0)
				return;

			var oldTransform = Container.DeviceContext.Transform;
			foreach (var parameter in RenderParameters)
				parameter.Render(Container.DeviceContext);
			Container.DeviceContext.Transform = oldTransform;
		}
	}
	public class SpriteRenderParameter
	{
		public SpriteRenderParameter(SpriteResource sprite, float rotate, RawVector2 rotateOrigin, RawRectangleF location)
		{
			Sprite = sprite ?? throw new ArgumentNullException(nameof(sprite));
			Rotate = rotate;
			RotateOrigin = rotateOrigin;
			Location = location;
		}

		SpriteResource Sprite { get; }
		float Rotate { get; }
		RawVector2 RotateOrigin { get; } //0-1
		RawRectangleF Location { get; }

		public void Render(DeviceContext context)
		{
			//todo 画面外の場合は描画しないほうがいいのでは

			var Width = Location.Right - Location.Left;
			var Height = Location.Bottom - Location.Top;

			//拡大縮小して先頭分ずらして回転して本来の位置に戻すついでにオフセット付加
			context.Transform =
				Matrix3x2.Scaling(Width / (Sprite.Rect.Right - Sprite.Rect.Left), Height / (Sprite.Rect.Bottom - Sprite.Rect.Top)) *
				Matrix3x2.Translation(-(RotateOrigin.X * Width), -(RotateOrigin.Y * Height)) *
				Matrix3x2.Rotation(Rotate) *
				Matrix3x2.Translation((RotateOrigin.X * Width) + Location.Left, (RotateOrigin.Y * Height) + Location.Top);

			Sprite.Render(context);
		}
	}
}
