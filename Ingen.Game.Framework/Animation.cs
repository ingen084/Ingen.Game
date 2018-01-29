using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ingen.Game.Framework
{
	public class Animation
	{
		public Animation(GameContainer container)
		{
			Container = container;
		}

		GameContainer Container { get; }

		public bool IsStarted { get; private set; } = false;

		public bool IsLoopMode { get; set; }
		long TimeTick;
		long BaseTick;

		public void Start(TimeSpan time, bool isLoopMode = false)
		{
			IsLoopMode = isLoopMode;
			BaseTick = Container.Elapsed.Ticks;
			TimeTick = time.Ticks;
			IsStarted = true;
		}

		public void Stop()
		{
			IsStarted = false;
			TimeTick = 0;
		}

		public float Value
		{
			get
			{
				if (TimeTick == 0)
					return 0;
				double value = (Container.Elapsed.Ticks - BaseTick) / (double)TimeTick;
				if (value > 1)
					if (IsLoopMode)
						BaseTick += TimeTick * (int)Math.Floor(value);
					else
						IsStarted = false;
				return (float)Math.Min(1.0, (Container.Elapsed.Ticks - BaseTick) / (double)TimeTick);
			}
		}
	}
}
