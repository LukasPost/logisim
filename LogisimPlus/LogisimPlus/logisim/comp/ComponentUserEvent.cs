// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{
	using CircuitState = logisim.circuit.CircuitState;
	using Canvas = logisim.gui.main.Canvas;

	public class ComponentUserEvent
	{
		private Canvas canvas;
		private int x = 0;
		private int y = 0;

		internal ComponentUserEvent(Canvas canvas)
		{
			this.canvas = canvas;
		}

		public ComponentUserEvent(Canvas canvas, int x, int y)
		{
			this.canvas = canvas;
			this.x = x;
			this.y = y;
		}

		public virtual Canvas Canvas
		{
			get
			{
				return canvas;
			}
		}

		public virtual CircuitState CircuitState
		{
			get
			{
				return canvas.CircuitState;
			}
		}

		public virtual int X
		{
			get
			{
				return x;
			}
		}

		public virtual int Y
		{
			get
			{
				return y;
			}
		}
	}

}
