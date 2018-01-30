using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace D2dControl
{
	internal class Dx11ImageSource : D3DImage, IDisposable
	{

		// - field -----------------------------------------------------------------------

		private static int ActiveClients;
		private static Direct3DEx D3DContext;
		private static DeviceEx D3DDevice;

		private Texture renderTarget;

		// - property --------------------------------------------------------------------

		//public int RenderWait { get; set; } = 2; // default: 2ms

		// - public methods --------------------------------------------------------------

		public Dx11ImageSource()
		{
			StartD3D();
			ActiveClients++;
		}

		public void Dispose()
		{
			SetRenderTarget(null);

			Disposer.SafeDispose(ref renderTarget);

			ActiveClients--;
			EndD3D();
		}

		public void InvalidateD3DImage()
		{
			if (renderTarget != null)
			{
				Lock();
				//if (RenderWait != 0)
				//	Thread.Sleep(RenderWait);
				AddDirtyRect(new System.Windows.Int32Rect(0, 0, PixelWidth, PixelHeight));
				Unlock();
			}
		}

		public void SetRenderTarget(Texture2D target)
		{
			if (renderTarget != null)
			{
				renderTarget = null;

				Lock();
				SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
				Unlock();
			}

			if (target == null)
				return;

			var format = TranslateFormat(target);
			var handle = GetSharedHandle(target);

			if (!IsShareable(target))
				throw new ArgumentException("Texture must be created with ResouceOptionFlags.Shared");

			if (format == SharpDX.Direct3D9.Format.Unknown)
				throw new ArgumentException("Texture format is not compatible with OpenSharedResouce");

			if (handle == IntPtr.Zero)
				throw new ArgumentException("Invalid handle");

			renderTarget = new Texture(D3DDevice, target.Description.Width, target.Description.Height, 1, SharpDX.Direct3D9.Usage.RenderTarget, format, Pool.Default, ref handle);

			using (var surface = renderTarget.GetSurfaceLevel(0))
			{
				Lock();
				SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
				Unlock();
			}
		}

		// - private methods -------------------------------------------------------------

		private void StartD3D()
		{
			if (ActiveClients != 0)
				return;

			D3DContext = new Direct3DEx();
			D3DDevice = new DeviceEx(D3DContext, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve, GetPresentParameters());
		}

		private void EndD3D()
		{
			if (ActiveClients != 0)
				return;

			Disposer.SafeDispose(ref renderTarget);
			Disposer.SafeDispose(ref D3DDevice);
			Disposer.SafeDispose(ref D3DContext);
		}

		private static void ResetD3D()
		{
			if (ActiveClients == 0)
				return;

			var presentParams = GetPresentParameters();
			D3DDevice.ResetEx(ref presentParams);
		}

		private static SharpDX.Direct3D9.PresentParameters GetPresentParameters()
			=> new SharpDX.Direct3D9.PresentParameters
			{
				Windowed = true,
				SwapEffect = SharpDX.Direct3D9.SwapEffect.Discard,
				DeviceWindowHandle = NativeMethods.GetDesktopWindow(),
				PresentationInterval = PresentInterval.Default
			};

		private IntPtr GetSharedHandle(Texture2D texture)
		{
			using (var resource = texture.QueryInterface<SharpDX.DXGI.Resource>())
				return resource.SharedHandle;
		}

		private static SharpDX.Direct3D9.Format TranslateFormat(Texture2D texture)
		{
			switch (texture.Description.Format)
			{
				case SharpDX.DXGI.Format.R10G10B10A2_UNorm:
					return SharpDX.Direct3D9.Format.A2B10G10R10;
				case SharpDX.DXGI.Format.R16G16B16A16_Float:
					return SharpDX.Direct3D9.Format.A16B16G16R16F;
				case SharpDX.DXGI.Format.B8G8R8A8_UNorm:
					return SharpDX.Direct3D9.Format.A8R8G8B8;
				default:
					return SharpDX.Direct3D9.Format.Unknown;
			}
		}

		private static bool IsShareable(Texture2D texture)
			=> (texture.Description.OptionFlags & ResourceOptionFlags.Shared) != 0;
	}
}
