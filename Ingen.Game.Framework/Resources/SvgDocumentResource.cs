using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ingen.Game.Framework.Resources
{
	public class SvgDocumentResource : IResource
	{
		SvgDocument document;

		public void UpdateDevice(GameContainer container)
		{
			//container.DeviceContext.CreateSvgDocument
		}

		public void Dispose()
		{
			document?.Dispose();
			document = null;
		}
	}
}
