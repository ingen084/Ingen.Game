using SharpDX;
using SharpDX.Direct3D;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using D2D1 = SharpDX.Direct2D1;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace Ingen.Game.Framework
{
	public class GameForm : Form
	{
		#region Direct3D Fields
		D3D11.Device _d3d11device;
		public ref D3D11.Device D3D11Device => ref _d3d11device;
		D3D11.Texture2D _backBuffer;
		public ref D3D11.Texture2D BackBuffer => ref _backBuffer;
		DXGI.SwapChain _swapChain;
		public ref DXGI.SwapChain SwapChain => ref _swapChain;
		D3D11.RenderTargetView _backBufferView;
		public ref D3D11.RenderTargetView BackBufferView => ref _backBufferView;
		#endregion
		#region Direct2D Fields
		D2D1.DeviceContext5 _d2d1DeviceContext;
		public ref D2D1.DeviceContext5 DeviceContext => ref _d2d1DeviceContext;
		D2D1.Factory1 _d2D1Factory;
		public ref D2D1.Factory1 D2D1Factory => ref _d2D1Factory;

		D2D1.Bitmap1 _d2D1BackBuffer;
		public ref D2D1.Bitmap1 D2D1BackBuffer => ref _d2D1BackBuffer;
		#endregion

		public GameForm()
		{
			StartPosition = FormStartPosition.CenterScreen;
			FormBorderStyle = CanResize ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
			BackColor = System.Drawing.Color.Black;
		}

		private bool _canResize = false;
		public bool CanResize
		{
			get => _canResize;
			set
			{
				_canResize = value;
				if (InvokeRequired)
					Invoke(new Action(() => FormBorderStyle = CanResize ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle));
				else
					FormBorderStyle = CanResize ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
			}
		}

		public void Initalize()
		{
			#region Direct3D Initalize
			// SwapChain description
			var desc = new DXGI.SwapChainDescription()
			{
				BufferCount = 1,
				ModeDescription = new DXGI.ModeDescription(0, 0, new DXGI.Rational(60, 1), DXGI.Format.R8G8B8A8_UNorm),
				IsWindowed = true,
				OutputHandle = Handle,
				SampleDescription = new DXGI.SampleDescription(1, 0),
				SwapEffect = DXGI.SwapEffect.Discard,
				Usage = DXGI.Usage.RenderTargetOutput
			};

			// Create Device and SwapChain
			D3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3D11.DeviceCreationFlags.BgraSupport, new[] { FeatureLevel.Level_9_3 }, desc, out D3D11Device, out SwapChain);

			// Ignore all windows events
			using (DXGI.Factory factory = SwapChain.GetParent<DXGI.Factory>())
				factory.MakeWindowAssociation(Handle, DXGI.WindowAssociationFlags.IgnoreAll);
			#endregion

			ResizeBuffer = true;
		}

		public event Action RenderTargetUpdated;
		public event Action<Size2> WindowSizeChanged;
		private ManualResetEventSlim ResizeMre { get; } = new ManualResetEventSlim();
		private bool ResizeBuffer { get; set; }
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
			ResizeMre.Reset();
			ResizeBuffer = true;
			ResizeMre.Wait(100);
			WindowSizeChanged?.Invoke(new Size2(ClientSize.Width, ClientSize.Height));
		}


		private ManualResetEventSlim NextRenderLogicTaskMre { get; set; } = new ManualResetEventSlim(true);
		private Action NextRenderLogicTask;

		//memo これ、ロジックスレッドを止めちゃうことになるけどいいのだろうか？
		/// <summary>
		/// 指定したデリゲートを描画スレッドで実行させる
		/// </summary>
		/// <param name="action"></param>
		public void SetActionAndWaitNextFrame(Action action)
		{
			NextRenderLogicTaskMre.Set();
			NextRenderLogicTask = new Action(() =>
			{
				action();
				NextRenderLogicTask = null;
				NextRenderLogicTaskMre.Reset();
			});
			NextRenderLogicTaskMre.Wait();
		}

		public void BeginDraw()
		{
			NextRenderLogicTask?.Invoke();
			if (ResizeBuffer)
			{
				D2D1BackBuffer?.Dispose();
				DeviceContext?.Dispose();
				BackBufferView?.Dispose();
				BackBuffer?.Dispose();

				SwapChain.ResizeBuffers(1, 0, 0, DXGI.Format.R8G8B8A8_UNorm, DXGI.SwapChainFlags.None);
				BackBuffer = D3D11.Resource.FromSwapChain<D3D11.Texture2D>(SwapChain, 0);

				BackBufferView = new D3D11.RenderTargetView(D3D11Device, BackBuffer);
				using (var surface = BackBuffer.QueryInterface<DXGI.Surface>())
				{
					using (var context0 = new D2D1.DeviceContext(surface))
						DeviceContext = context0.QueryInterface<D2D1.DeviceContext5>();
					D2D1BackBuffer = new D2D1.Bitmap1(DeviceContext, surface);
				}
				RenderTargetUpdated?.Invoke();

				ResizeBuffer = false;
			}

			DeviceContext.BeginDraw();
		}
		public void EndDraw()
		{
			DeviceContext.EndDraw();
			SwapChain.Present(1, DXGI.PresentFlags.UseDuration);
			ResizeMre.Set();
		}

		public bool IsClosing { get; set; }
		protected override void OnClosing(CancelEventArgs e)
		{
			if (!IsForceClosing)
			{
				e.Cancel = true;
				return;
			}
			base.OnClosing(e);
		}
		bool IsForceClosing = false;
		internal void ForceClose()
		{
			IsForceClosing = true;
			try
			{
				Invoke(new Action(() => Close()));
			}
			catch (ObjectDisposedException) { } //Invoke使ってるので仕方ない
			return;
		}

		private void D3dDispose()
		{
			D3D11Device?.Dispose();
			D3D11Device = null;
			SwapChain?.Dispose();
			SwapChain = null;
			BackBuffer?.Dispose();
			BackBuffer = null;
			BackBufferView?.Dispose();
			BackBufferView = null;

			D2D1Factory?.Dispose();
			D2D1Factory = null;
			DeviceContext?.Dispose();
			DeviceContext = null;
			D2D1BackBuffer?.Dispose();
			D2D1BackBuffer = null;
		}
		protected override void Dispose(bool disposing)
		{
			D3dDispose();
			base.Dispose(disposing);
		}
	}
}
