// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.move
{
	internal class MoveRequest
	{
		private MoveGesture gesture;
		private int dx;
		private int dy;

		public MoveRequest(MoveGesture gesture, int dx, int dy)
		{
			this.gesture = gesture;
			this.dx = dx;
			this.dy = dy;
		}

		public virtual MoveGesture MoveGesture
		{
			get
			{
				return gesture;
			}
		}

		public virtual int DeltaX
		{
			get
			{
				return dx;
			}
		}

		public virtual int DeltaY
		{
			get
			{
				return dy;
			}
		}

		public override bool Equals(object other)
		{
			if (other is MoveRequest)
			{
				MoveRequest o = (MoveRequest) other;
				return this.gesture == o.gesture && this.dx == o.dx && this.dy == o.dy;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return (gesture.GetHashCode() * 31 + dx) * 31 + dy;
		}
	}

}
