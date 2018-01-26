using SharpDX.Direct3D;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using D2D1 = SharpDX.Direct2D1;
using D3D11 = SharpDX.Direct3D11;
using DW = SharpDX.DirectWrite;
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
		DW.Factory _dWFactory;
		public ref DW.Factory DWFactory => ref _dWFactory;
		D2D1.RenderTarget _renderTarget;
		public ref D2D1.RenderTarget RenderTarget => ref _renderTarget;
		#endregion

		private bool IsEnableVSync;
		public GameForm(bool isEnableVSync)
		{
			IsEnableVSync = isEnableVSync;
		}

		public void Initalize()
		{
			ClientSize = new System.Drawing.Size(1280, 720);
			StartPosition = FormStartPosition.CenterScreen;
			MaximizeBox = false;
			FormBorderStyle = FormBorderStyle.FixedSingle;

			#region Direct3D Initalize
			// SwapChain description
			var desc = new DXGI.SwapChainDescription()
			{
				BufferCount = 1,
				ModeDescription = new DXGI.ModeDescription(ClientSize.Width, ClientSize.Height, new DXGI.Rational(60, 1), DXGI.Format.R8G8B8A8_UNorm),
				IsWindowed = true,
				OutputHandle = Handle,
				SampleDescription = new DXGI.SampleDescription(1, 0),
				SwapEffect = DXGI.SwapEffect.Discard,
				Usage = DXGI.Usage.RenderTargetOutput
			};

			// Create Device and SwapChain
			D3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3D11.DeviceCreationFlags.BgraSupport, new[] { FeatureLevel.Level_10_0 }, desc, out D3D11Device, out SwapChain);

			// Ignore all windows events
			using (DXGI.Factory factory = SwapChain.GetParent<DXGI.Factory>())
				factory.MakeWindowAssociation(Handle, DXGI.WindowAssociationFlags.IgnoreAll);

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

			DWFactory = new DW.Factory();

			//using (var dxgiDevice = D3D11Device.QueryInterface<DXGI.Device>())
			//using (var d2d1Device = new D2D1.Device(dxgiDevice))
			//	D2D1DeviceContext = new D2D1.DeviceContext(d2d1Device, D2D1.DeviceContextOptions.EnableMultithreadedOptimizations);
			#endregion
		}

		public void BeginDraw()
		{
			D3D11Device.ImmediateContext.Rasterizer.SetViewport(0, 0, ClientSize.Width, ClientSize.Height);
			D3D11Device.ImmediateContext.OutputMerger.SetTargets(_backBufferView);

			RenderTarget.BeginDraw();
			RenderTarget.Clear(null);
		}
		public void EndDraw()
		{
			RenderTarget.EndDraw();
			SwapChain.Present(IsEnableVSync ? 1 : 0, DXGI.PresentFlags.None);
		}


		protected override void Dispose(bool disposing)
		{
			D3D11Device?.Dispose();
			SwapChain?.Dispose();
			BackBuffer?.Dispose();
			BackBufferView?.Dispose();

			D2D1Factory?.Dispose();
			DWFactory?.Dispose();
			RenderTarget?.Dispose();
			D2D1DeviceContext?.Dispose();
			base.Dispose(disposing);
		}
	}
}
