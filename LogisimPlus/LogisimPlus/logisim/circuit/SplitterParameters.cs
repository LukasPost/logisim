// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{
	using Direction = logisim.data.Direction;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

	internal class SplitterParameters
	{
		private int dxEnd0; // location of split end 0 relative to origin
		private int dyEnd0;
		private int ddxEnd; // distance from split end i to split end (i + 1)
		private int ddyEnd;
		private int dxEndSpine; // distance from split end to spine
		private int dyEndSpine;
		private int dxSpine0; // distance from origin to far end of spine
		private int dySpine0;
		private int dxSpine1; // distance from origin to near end of spine
		private int dySpine1;
		private int textAngle; // angle to rotate text
		private int halign; // justification of text
		private int valign;

		internal SplitterParameters(SplitterAttributes attrs)
		{

			object appear = attrs.appear;
			int fanout = attrs.fanout;
			Direction facing = attrs.facing;

			int justify;
			if (appear == SplitterAttributes.APPEAR_CENTER || appear == SplitterAttributes.APPEAR_LEGACY)
			{
				justify = 0;
			}
			else if (appear == SplitterAttributes.APPEAR_RIGHT)
			{
				justify = 1;
			}
			else
			{
				justify = -1;
			}
			int width = 20;

			int offs = 6;
			if (facing == Direction.North || facing == Direction.South)
			{ // ^ or V
				int m = facing == Direction.North ? 1 : -1;
				dxEnd0 = justify == 0 ? 10 * ((fanout + 1) / 2 - 1) : m * justify < 0 ? -10 : 10 * fanout;
				dyEnd0 = -m * width;
				ddxEnd = -10;
				ddyEnd = 0;
				dxEndSpine = 0;
				dyEndSpine = m * (width - offs);
				dxSpine0 = m * justify * (10 * fanout - 1);
				dySpine0 = -m * offs;
				dxSpine1 = m * justify * offs;
				dySpine1 = -m * offs;
				textAngle = 90;
				halign = m > 0 ? JGraphicsUtil.H_RIGHT : JGraphicsUtil.H_LEFT;
				valign = m * justify <= 0 ? JGraphicsUtil.V_BASELINE : JGraphicsUtil.V_TOP;
			}
			else
			{ // > or <
				int m = facing == Direction.West ? -1 : 1;
				dxEnd0 = m * width;
				dyEnd0 = justify == 0 ? -10 * (fanout / 2) : m * justify > 0 ? 10 : -10 * fanout;
				ddxEnd = 0;
				ddyEnd = 10;
				dxEndSpine = -m * (width - offs);
				dyEndSpine = 0;
				dxSpine0 = m * offs;
				dySpine0 = m * justify * (10 * fanout - 1);
				dxSpine1 = m * offs;
				dySpine1 = m * justify * offs;
				textAngle = 0;
				halign = m > 0 ? JGraphicsUtil.H_LEFT : JGraphicsUtil.H_RIGHT;
				valign = m * justify < 0 ? JGraphicsUtil.V_TOP : JGraphicsUtil.V_BASELINE;
			}
		}

		public virtual int End0X
		{
			get
			{
				return dxEnd0;
			}
		}

		public virtual int End0Y
		{
			get
			{
				return dyEnd0;
			}
		}

		public virtual int EndToEndDeltaX
		{
			get
			{
				return ddxEnd;
			}
		}

		public virtual int EndToEndDeltaY
		{
			get
			{
				return ddyEnd;
			}
		}

		public virtual int EndToSpineDeltaX
		{
			get
			{
				return dxEndSpine;
			}
		}

		public virtual int EndToSpineDeltaY
		{
			get
			{
				return dyEndSpine;
			}
		}

		public virtual int Spine0X
		{
			get
			{
				return dxSpine0;
			}
		}

		public virtual int Spine0Y
		{
			get
			{
				return dySpine0;
			}
		}

		public virtual int Spine1X
		{
			get
			{
				return dxSpine1;
			}
		}

		public virtual int Spine1Y
		{
			get
			{
				return dySpine1;
			}
		}

		public virtual int TextAngle
		{
			get
			{
				return textAngle;
			}
		}

		public virtual int TextHorzAlign
		{
			get
			{
				return halign;
			}
		}

		public virtual int TextVertAlign
		{
			get
			{
				return valign;
			}
		}
	}

}
