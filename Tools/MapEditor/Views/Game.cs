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

namespace MapEditor.Views
{
	public class Game : D2dImage
	{
		TextFormat format;
		public Game()
		{
			ResourceCache.Add("FrameBrush", t => new SolidColorBrush(t, Color.White));

			format = new TextFormat(DirectWriteFactory, "MS Gothic", FontWeight.Bold, FontStyle.Normal, 15);
		}

		int[,] SampleMap =
		{
			{ 0, 0, 0, 0 },
			{ 0, 1, 1, 0 },
			{ 0, 1, 1, 0 },
			{ 0, 0, 0, 0 },
		};
		public override void Render(RenderTarget target)
		{
			target.Clear(null);

			const int XOffset = 400;
			const int YOffset = 0;

			const int TileWidth = 200;
			const float TileHeight = 150;
			const double Slope = .5;

			target.DrawText("hello", format, new RectangleF(5, 5, float.PositiveInfinity, float.PositiveInfinity), ResourceCache["FrameBrush"]);

			for (int x = 0; x < SampleMap.GetLength(0) + 1; x++)
				target.DrawLine(
					new Vector2(ToRawX(x * 0.5 * TileWidth + XOffset), ToRawY(x * 0.5 * TileHeight + YOffset)),
					new Vector2(ToRawX((x * 0.5 - SampleMap.GetLength(1) * 0.5) * TileWidth + XOffset), ToRawY((x * 0.5 + SampleMap.GetLength(1) * 0.5) * TileHeight + YOffset)), ResourceCache["FrameBrush"]);

			for (int y = 0; y < SampleMap.GetLength(1) + 1; y++)
				target.DrawLine(
					new Vector2(ToRawX(-y * 0.5 * TileWidth + XOffset), ToRawY(y * 0.5 * TileHeight + YOffset)),
					new Vector2(ToRawX((-y * 0.5 + SampleMap.GetLength(0) * 0.5) * TileWidth + XOffset), ToRawY((y * 0.5 + SampleMap.GetLength(0) * 0.5) * TileHeight + YOffset)), ResourceCache["FrameBrush"]);

			//using (var geo = new PathGeometry(D2dFactory))
			//{
			//	using (var sink = geo.Open())
			//	{
			//		sink.BeginFigure(new Vector2(150, 10), FigureBegin.Filled);
			//		sink.AddLine(new Vector2(150 + TileWidth / 2, 10 + TileHeight / 2));
			//		sink.AddLine(new Vector2(150, 10 + TileHeight));
			//		sink.AddLine(new Vector2(150 - TileWidth / 2, 10 + TileHeight / 2));
			//		sink.EndFigure(FigureEnd.Closed);
			//		sink.Close();
			//	}
			//	target.FillGeometry(geo, ResourceCache["FrameBrush"]);
			//}

			//for (int x = 0; x < SampleMap.GetLength(0); x++)
			//	for (int y = 0; y < SampleMap.GetLength(1); y++)
			//	{
			//		var rx = (x * 0.5 - y * 0.5) * TileWidth;
			//		var ry = (x * 0.5 + y * 0.5) * TileHeight;

			//		Bitmap bitmap = ResourceCache["TileImage1"];

			//		if (MousePoint is Wpf.Point point)
			//		{
			//			//中心座標
			//			var cx = rx + TileWidth / 2 + XOffset;
			//			var cy = ry + TileHeight / 2 + YOffset;

			//			//当たり判定
			//			if (rx + XOffset <= point.X && point.X <= rx + XOffset + TileWidth
			//				&& ry + YOffset <= point.Y + YOffset && point.Y <= ry + YOffset + TileHeight

			//				&& point.Y <= (Slope * (point.X - (cx - TileHeight)) + cy)
			//				&& point.Y >= (Slope * (point.X - (cx + TileHeight)) + cy)
			//				&& point.Y <= (-Slope * (point.X - (cx + TileHeight)) + cy)
			//				&& point.Y >= (-Slope * (point.X - (cx - TileHeight)) + cy))
			//				bitmap = null;
			//		}

			//		//switch (SampleMap[y, x])
			//		//{
			//		//	case 1:
			//		//		bitmap = ResourceCache["TileImage1"];
			//		//		break;
			//		//	case 2:
			//		//		//bitmap = ResourceCache["TileImage2"];
			//		//		break;
			//		//	case 3:
			//		//		//bitmap = ResourceCache["TileImage3"];
			//		//		break;
			//		//	case 4:
			//		//		//bitmap = ResourceCache["TileImage4"];
			//		//		break;
			//		//}
			//		target.DrawRectangle(new RectangleF(ToRawX(rx + XOffset), ToRawY(ry + YOffset), ToRawX(rx + 128 + XOffset), ToRawY(ry + 64 + YOffset)), ResourceCache["TileBrush"]);
			//		//if (bitmap != null)
			//		//	target.DrawBitmap(bitmap, new RawRectangleF(ToRawX(rx + XOffset), ToRawY(ry + YOffset), ToRawX(rx + 128 + XOffset), ToRawY(ry + 64 + YOffset)), 1, BitmapInterpolationMode.Linear);

			//		target.DrawText($"{x},{y}", ResourceCache["MainFont"], new RectangleF(ToRawX(rx + XOffset), ToRawY(ry + 10), ToRawX(rx + XOffset + 100), ToRawY(ry + YOffset + 100)), ResourceCache["ForegroundBrush"]);
			//	}

		}

		public override void Dispose()
		{
			format.Dispose();
		}
	}
}
