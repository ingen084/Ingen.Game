using D2dControl;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf = System.Windows;
using WIC = SharpDX.WIC;
using SharpDX.IO;

namespace MapEditor.Views
{
	public class Game : D2dImage
	{
		TextFormat format;
		public Game()
		{
			if (IsInDesignMode) return;

			ResourceCache.Add("FrameBrush", t => new SolidColorBrush(t, Color.White));

			format = new TextFormat(DirectWriteFactory, "MS Gothic", 15);

			TileImage = new Image(ImagingFactory, @"D:\ingen\Desktop\tile7.png");
			SelectedTileImage = new Image(ImagingFactory, @"D:\ingen\Desktop\tileSelected.png");

			ResourceCache.Add("TileImage", t => Bitmap.FromWicBitmap(t, TileImage.FC, new BitmapProperties(new PixelFormat(SharpDX.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied))));
			ResourceCache.Add("SelectedTileImage", t => Bitmap.FromWicBitmap(t, SelectedTileImage.FC, new BitmapProperties(new PixelFormat(SharpDX.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied))));
		}

		Image TileImage;
		Image SelectedTileImage;

		int[,] SampleMap =
		{
			{ 1, 1, 1, 1, },
			{ 1, 0, 0, 1, },
			{ 1, 0, 0, 1, },
			{ 1, 1, 1, 1, },
		};

		const int XOffset = 400;
		const int YOffset = 0;

		const float TileScale = .75f;
		const float TileWidth = 200 * TileScale;
		const float TileHeight = 150 * TileScale;
		const double Slope = .75;

		public override void Render(RenderTarget target)
		{
			target.Clear(Color.Black);

			target.DrawText("モード: タイル設置", format, new RectangleF(5, 5, float.PositiveInfinity, float.PositiveInfinity), ResourceCache["FrameBrush"]);


			//上から描画していくループ
			var height = SampleMap.GetLength(0) + SampleMap.GetLength(1) - 1;
			for (var h = 0; h < height; ++h)
			{
				int mx = Math.Max(0, h - SampleMap.GetLength(0) - 1), my = Math.Min(SampleMap.GetLength(1) - 1, h);
				for (int x = mx, y = h - mx; x <= my; ++x, --y)
				{
					if (y > SampleMap.GetLength(0) - 1) continue;

					var rx = (x * 0.5 - y * 0.5) * TileWidth - TileWidth / 2;
					var ry = (x * 0.5 + y * 0.5) * TileHeight;

					if (SampleMap[y, x] != 0)
						target.DrawBitmap(ResourceCache["TileImage"],
							new RectangleF(
								ToRawX(rx + XOffset),
								ToRawY(ry + YOffset),
								ToRawX(TileWidth),
								ToRawY(TileHeight + 10 * TileScale)), 1, BitmapInterpolationMode.Linear);

					if (MousePoint is Wpf.Point point)
					{
						//中心座標
						var cx = rx + TileWidth / 2 + XOffset;
						var cy = ry + TileHeight / 2 + YOffset;

						//当たり判定
						if (rx + XOffset < point.X && point.X <= rx + XOffset + TileWidth
							&& ry + YOffset < point.Y + YOffset && point.Y <= ry + YOffset + TileHeight

							&& point.Y <= (Slope * (point.X - (cx - TileWidth / 2)) + cy)
							&& point.Y >= (Slope * (point.X - (cx + TileWidth / 2)) + cy)
							&& point.Y < (-Slope * (point.X - (cx + TileWidth / 2)) + cy)
							&& point.Y > (-Slope * (point.X - (cx - TileWidth / 2)) + cy)
							)
							target.DrawBitmap(ResourceCache["SelectedTileImage"],
													new RectangleF(
														ToRawX(rx + XOffset),
														ToRawY(ry + YOffset),
														ToRawX(TileWidth),
														ToRawY(TileHeight + 10 * TileScale)), 1, BitmapInterpolationMode.Linear);
					}
				}
			}

			//for (int x = 0; x < SampleMap.GetLength(1) + 1; x++)
			//	target.DrawLine(
			//		new Vector2(ToRawX(x * 0.5 * TileWidth + XOffset), ToRawY(x * 0.5 * TileHeight + YOffset)),
			//		new Vector2(ToRawX((x * 0.5 - SampleMap.GetLength(0) * 0.5) * TileWidth + XOffset), ToRawY((x * 0.5 + SampleMap.GetLength(0) * 0.5) * TileHeight + YOffset)), ResourceCache["FrameBrush"], 3);

			//for (int y = 0; y < SampleMap.GetLength(0) + 1; y++)
			//	target.DrawLine(
			//		new Vector2(ToRawX(-y * 0.5 * TileWidth + XOffset), ToRawY(y * 0.5 * TileHeight + YOffset)),
			//		new Vector2(ToRawX((SampleMap.GetLength(1) * 0.5 - y * 0.5) * TileWidth + XOffset), ToRawY((SampleMap.GetLength(1) * 0.5 + y * 0.5) * TileHeight + YOffset)), ResourceCache["FrameBrush"], 3);
		}

		public override void Dispose()
		{
			TileImage.Dispose();
			SelectedTileImage.Dispose();
			format.Dispose();
		}
	}
}
