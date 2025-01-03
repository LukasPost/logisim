// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace gray
{
	using BitWidth = logisim.data.BitWidth;
	using Value = logisim.data.Value;
	using InstanceData = logisim.instance.InstanceData;
	using InstanceState = logisim.instance.InstanceState;

	/// <summary>
	/// Represents the state of a counter. </summary>
	internal class CounterData : InstanceData
	{
		/// <summary>
		/// Retrieves the state associated with this counter in the circuit state, generating the state if necessary.
		/// </summary>
		public static CounterData get(InstanceState state, BitWidth width)
		{
			CounterData ret = (CounterData) state.Data;
			if (ret == null)
			{
				// If it doesn't yet exist, then we'll set it up with our default
				// values and put it into the circuit state so it can be retrieved
				// in future propagations.
				ret = new CounterData(null, Value.createKnown(width, 0));
				state.Data = ret;
			}
			else if (!ret.value.BitWidth.Equals(width))
			{
				ret.value = ret.value.extendWidth(width.Width, Value.FALSE);
			}
			return ret;
		}

		/// <summary>
		/// The last clock input value observed. </summary>
		private Value lastClock;

		/// <summary>
		/// The current value emitted by the counter. </summary>
		private Value value;

		/// <summary>
		/// Constructs a state with the given values. </summary>
		public CounterData(Value lastClock, Value value)
		{
			this.lastClock = lastClock;
			this.value = value;
		}

		/// <summary>
		/// Returns a copy of this object. </summary>
		public virtual object Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>
		/// Updates the last clock observed, returning true if triggered. </summary>
		public virtual bool updateClock(Value value)
		{
			Value old = lastClock;
			lastClock = value;
			return old == Value.FALSE && value == Value.TRUE;
		}

		/// <summary>
		/// Returns the current value emitted by the counter. </summary>
		public virtual Value Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

	}

}
