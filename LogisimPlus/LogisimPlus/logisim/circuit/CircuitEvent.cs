// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{
	public class CircuitEvent
	{
		public const int ACTION_SET_NAME = 0; // name changed
		public const int ACTION_ADD = 1; // component added
		public const int ACTION_REMOVE = 2; // component removed
		public const int ACTION_CHANGE = 3; // component changed
		public const int ACTION_INVALIDATE = 4; // component invalidated (pin types changed)
		public const int ACTION_CLEAR = 5; // entire circuit cleared
		public const int TRANSACTION_DONE = 6;

		private int action;
		private Circuit circuit;
		private object data;

		internal CircuitEvent(int action, Circuit circuit, object data)
		{
			this.action = action;
			this.circuit = circuit;
			this.data = data;
		}

		// access methods
		public virtual int Action
		{
			get
			{
				return action;
			}
		}

		public virtual Circuit Circuit
		{
			get
			{
				return circuit;
			}
		}

		public virtual object Data
		{
			get
			{
				return data;
			}
		}

		public virtual CircuitTransactionResult Result
		{
			get
			{
				return (CircuitTransactionResult) data;
			}
		}
	}

}
