using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.ComponentModel;
using System.Windows;

namespace D2dControl
{
	public abstract class D2dControl : System.Windows.Controls.Image, IDisposable
	{
		private SharpDX.Direct3D11.Device device;
		private Texture2D renderTarget;
		private Dx11ImageSource d3DSurface;
		private RenderTarget d2DRenderTarget;

		private SharpDX.Direct2D1.Factory1 _d2dFactory;
		protected ref SharpDX.Direct2D1.Factory1 D2dFactory => ref _d2dFactory;

		private readonly HighPerformanceStopwatch renderTimer = new HighPerformanceStopwatch();

		protected ResourceCache ResourceCache { get; } = new ResourceCache();

		private SharpDX.DirectWrite.Factory _directWriteFactory = new SharpDX.DirectWrite.Factory();
		protected ref SharpDX.DirectWrite.Factory DirectWriteFactory => ref _directWriteFactory;

		private SharpDX.WIC.ImagingFactory _imagingFactory = new SharpDX.WIC.ImagingFactory();
		protected ref SharpDX.WIC.ImagingFactory ImagingFactory => ref _imagingFactory;

		private SharpDX.Direct2D1.DeviceContext d2dDeviceContext;
		protected ref SharpDX.Direct2D1.DeviceContext D2dDeviceContext => ref d2dDeviceContext;


		public static bool IsInDesignMode
			=> (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;

		//public int RenderWait
		//{
		//	get => (int)GetValue(RenderWaitProperty);
		//	set => SetValue(RenderWaitProperty, value);
		//}
		//public static DependencyProperty RenderWaitProperty = DependencyProperty.Register(
		//		"RenderWait",
		//		typeof(int),
		//		typeof(D2dControl),
		//		new FrameworkPropertyMetadata(2, OnRenderWaitChanged)
		//	);


		public D2dControl()
		{
			Loaded += Window_Loaded;
			Unloaded += Window_Closing;

			Stretch = System.Windows.Media.Stretch.Fill;
		}

		public abstract void Render(RenderTarget target);

		#region EventHandler
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (IsInDesignMode)
				return;

			StartD3D();
			StartRendering();
		}

		private void Window_Closing(object sender, RoutedEventArgs e)
		{
			if (IsInDesignMode)
				return;

			StopRendering();
			EndD3D();
		}

		private void OnRendering(object sender, EventArgs e)
		{
			if (!renderTimer.IsRunning)
				return;

			PrepareAndCallRender();
			d3DSurface.InvalidateD3DImage();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			CreateAndBindTargets();
			base.OnRenderSizeChanged(sizeInfo);
		}

		private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (d3DSurface.IsFrontBufferAvailable)
				StartRendering();
			else
				StopRendering();
		}

		//private static void OnRenderWaitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		//{
		//	var control = (D2dControl)d;
		//	control.d3DSurface.RenderWait = (int)e.NewValue;
		//}
		#endregion


		private void StartD3D()
		{
			device = new SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);

			d3DSurface = new Dx11ImageSource();
			d3DSurface.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

			CreateAndBindTargets();

			Source = d3DSurface;
		}
		private void EndD3D()
		{
			d3DSurface.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
			Source = null;

			Disposer.SafeDispose(ref d2DRenderTarget);
			Disposer.SafeDispose(ref D2dDeviceContext);
			Disposer.SafeDispose(ref D2dFactory);
			Disposer.SafeDispose(ref d3DSurface);
			Disposer.SafeDispose(ref renderTarget);
			Disposer.SafeDispose(ref device);

			ResourceCache.Clear();

			Disposer.SafeDispose(ref DirectWriteFactory);
			Disposer.SafeDispose(ref ImagingFactory);

			Dispose();
		}

		private Point GetDpiScaleFactor()
		{
			var source = PresentationSource.FromVisual(this);
			if (source != null && source.CompositionTarget != null)
			{
				return new Point(
					source.CompositionTarget.TransformToDevice.M11,
					source.CompositionTarget.TransformToDevice.M22);
			}

			return new Point(1.0, 1.0);
		}
		private Point _dpiScale;
		protected Point DpiScale => _dpiScale == default(Point) ? (_dpiScale = GetDpiScaleFactor()) : _dpiScale;

		private void CreateAndBindTargets()
		{
			d3DSurface.SetRenderTarget(null);

			Disposer.SafeDispose(ref d2DRenderTarget);
			Disposer.SafeDispose(ref D2dDeviceContext);
			Disposer.SafeDispose(ref D2dFactory);
			Disposer.SafeDispose(ref renderTarget);

			var scale = DpiScale;
			var width = Math.Max((int)(ActualWidth * scale.X), 100);
			var height = Math.Max((int)(ActualHeight * scale.Y), 100);

			var renderDesc = new Texture2DDescription
			{
				BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
				Format = Format.B8G8R8A8_UNorm,
				Width = width,
				Height = height,
				MipLevels = 1,
				SampleDescription = new SampleDescription(1, 0),
				Usage = ResourceUsage.Default,
				OptionFlags = ResourceOptionFlags.Shared,
				CpuAccessFlags = CpuAccessFlags.None,
				ArraySize = 1
			};

			renderTarget = new Texture2D(device, renderDesc);

			var surface = renderTarget.QueryInterface<Surface>();

			D2dFactory = new SharpDX.Direct2D1.Factory1();
			var rtp = new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied));
			d2DRenderTarget = new RenderTarget(D2dFactory, surface, rtp);
			D2dDeviceContext = new SharpDX.Direct2D1.DeviceContext(surface);
			ResourceCache.RenderTarget = d2DRenderTarget;

			d3DSurface.SetRenderTarget(renderTarget);

			device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);
		}

		private void StartRendering()
		{
			if (renderTimer.IsRunning)
				return;

			System.Windows.Media.CompositionTarget.Rendering += OnRendering;
			renderTimer.Start();
		}
		private void StopRendering()
		{
			if (!renderTimer.IsRunning)
				return;

			System.Windows.Media.CompositionTarget.Rendering -= OnRendering;
			renderTimer.Stop();
		}

		private void PrepareAndCallRender()
		{
			if (device == null)
				return;

			d2DRenderTarget.BeginDraw();
			Render(d2DRenderTarget);
			d2DRenderTarget.EndDraw();

			device.ImmediateContext.Flush();
		}

		public abstract void Dispose();
	}
}
