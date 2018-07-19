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
		D2D1.DeviceContext _d2d1DeviceContext;
		public ref D2D1.DeviceContext D2D1DeviceContext => ref _d2d1DeviceContext;
		D2D1.Factory1 _d2D1Factory;
		public ref D2D1.Factory1 D2D1Factory => ref _d2D1Factory;
		public ref D2D1.RenderTarget RenderTarget => ref _renderTarget;
		D2D1.RenderTarget _renderTarget;
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

		public void Initalize(System.Drawing.Size? nSize = null, IntPtr? nhWnd = null)
		{
			System.Drawing.Size size = nSize ?? System.Drawing.Size.Empty;
			IntPtr hWnd = nhWnd ?? IntPtr.Zero;
			if (size == System.Drawing.Size.Empty || hWnd == IntPtr.Zero)
			{
				size = ClientSize;
				hWnd = Handle;
			}

			#region Direct3D Initalize
			// SwapChain description
			var desc = new DXGI.SwapChainDescription()
			{
				BufferCount = 1,
				ModeDescription = new DXGI.ModeDescription(size.Width, size.Height, new DXGI.Rational(60, 1), DXGI.Format.R8G8B8A8_UNorm),
				IsWindowed = true,
				OutputHandle = hWnd,
				SampleDescription = new DXGI.SampleDescription(1, 0),
				SwapEffect = DXGI.SwapEffect.Discard,
				Usage = DXGI.Usage.RenderTargetOutput
			};

			// Create Device and SwapChain
			D3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3D11.DeviceCreationFlags.BgraSupport, new[] { FeatureLevel.Level_9_3 }, desc, out D3D11Device, out SwapChain);

			// Ignore all windows events
			using (DXGI.Factory factory = SwapChain.GetParent<DXGI.Factory>())
				factory.MakeWindowAssociation(hWnd, DXGI.WindowAssociationFlags.IgnoreAll);

			// New RenderTargetView from the backbuffer
			BackBuffer = D3D11.Resource.FromSwapChain<D3D11.Texture2D>(SwapChain, 0);

			BackBufferView = new D3D11.RenderTargetView(D3D11Device, BackBuffer);
			#endregion
			#region Direct2D Initalize
			D2D1Factory = new D2D1.Factory1();
			using (var surface = BackBuffer.QueryInterface<DXGI.Surface>())
			{
				RenderTarget = new D2D1.RenderTarget(D2D1Factory, surface, new D2D1.RenderTargetProperties(new D2D1.PixelFormat(DXGI.Format.Unknown, D2D1.AlphaMode.Premultiplied)));
				D2D1DeviceContext = new D2D1.DeviceContext(surface);
			}
			RenderTarget.AntialiasMode = D2D1.AntialiasMode.PerPrimitive;
			#endregion
			RenderTargetUpdated?.Invoke();
		}

		public event Action RenderTargetUpdated;
		public event Action<Size2> WindowSizeChanged;
		private System.Drawing.Size? NewSize { get; set; }
		protected override void OnSizeChanged(EventArgs e)
		{
			NewSize = ClientSize;
			WindowSizeChanged?.Invoke(new Size2(ClientSize.Width, ClientSize.Height));
			base.OnClientSizeChanged(e);
		}


		private ManualResetEventSlim RenderPauseMre { get; set; } = new ManualResetEventSlim(true);
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

		protected override void OnPaint(PaintEventArgs e)
		{
			RenderPauseMre.Wait();
			base.OnPaint(e);
		}
		public void BeginDraw()
		{
			NextRenderLogicTask?.Invoke();
			if (NewSize is System.Drawing.Size newSize)
			{
				(System.Drawing.Size size, IntPtr hWnd) = ((System.Drawing.Size, IntPtr))Invoke(new Func<(System.Drawing.Size, IntPtr)>(() => (ClientSize, Handle)));

				RenderPauseMre.Reset();
				D3dDispose();
				Initalize(size, hWnd);
				NewSize = null;
			}
			RenderPauseMre.Set();
			D3D11Device.ImmediateContext.Rasterizer.SetViewport(0, 0, ClientSize.Width, ClientSize.Height);
			D3D11Device.ImmediateContext.OutputMerger.SetTargets(_backBufferView);

			RenderTarget.BeginDraw();
		}
		public void EndDraw()
		{
			RenderTarget.EndDraw();
			SwapChain.Present(1, DXGI.PresentFlags.UseDuration);
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
			RenderTarget?.Dispose();
			RenderTarget = null;
			D2D1DeviceContext?.Dispose();
			D2D1DeviceContext = null;
		}
		protected override void Dispose(bool disposing)
		{
			D3dDispose();
			base.Dispose(disposing);
		}
	}
}
