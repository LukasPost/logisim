// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{
	using Value = logisim.data.Value;
	using StdAttr = logisim.instance.StdAttr;

	internal class ClockState : ICloneable
	{
		private Value lastClock;

		public ClockState()
		{
			lastClock = Value.FALSE;
		}

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual bool updateClock(Value newClock, object trigger)
		{
			Value oldClock = lastClock;
			lastClock = newClock;
			if (trigger == null || trigger == StdAttr.TRIG_RISING)
			{
				return oldClock == Value.FALSE && newClock == Value.TRUE;
			}
			else if (trigger == StdAttr.TRIG_FALLING)
			{
				return oldClock == Value.TRUE && newClock == Value.FALSE;
			}
			else if (trigger == StdAttr.TRIG_HIGH)
			{
				return newClock == Value.TRUE;
			}
			else if (trigger == StdAttr.TRIG_LOW)
			{
				return newClock == Value.FALSE;
			}
			else
			{
				return oldClock == Value.FALSE && newClock == Value.TRUE;
			}
		}
	}

}
