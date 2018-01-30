using System.Windows.Input;
using Wpf = System.Windows;

namespace D2dControl
{
	public abstract class D2dImage : D2dControl
	{
		private float dpi;
		protected float Dpi => dpi == default(float) ? (dpi = (float)(Wpf.PresentationSource.FromVisual(this)?.CompositionTarget?.TransformToDevice.M11 ?? 1)) : dpi;

		/// <summary>
		/// マウスの相対座標　高DPIの場合でも内部座標を使用しています
		/// <para>DpiScaleを掛けると生の画面座標になります。</para>
		/// </summary>
		protected Wpf.Point? MousePoint { get; private set; }

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (MousePoint != null)
				MousePoint = e.GetPosition(this);
			base.OnMouseMove(e);
		}
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			MousePoint = e.GetPosition(this);
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			MousePoint = null;
			base.OnMouseLeave(e);
		}

		protected float ToRawX(double dpix)
			=> (float)(dpix * DpiScale.X);
		protected float ToRawY(double dpiy)
			=> (float)(dpiy * DpiScale.Y);
	}
}
