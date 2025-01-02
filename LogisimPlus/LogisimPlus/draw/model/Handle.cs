// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.model
{
	using Location = logisim.data.Location;

	public class Handle
	{
		private CanvasObject @object;
		private int x;
		private int y;

		public Handle(CanvasObject @object, int x, int y)
		{
			this.@object = @object;
			this.x = x;
			this.y = y;
		}

		public Handle(CanvasObject @object, Location loc) : this(@object, loc.X, loc.Y)
		{
		}

		public virtual CanvasObject Object
		{
			get
			{
				return @object;
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

		public virtual Location Location
		{
			get
			{
				return new Location(x, y);
			}
		}

		public virtual bool isAt(Location loc)
		{
			return x == loc.X && y == loc.Y;
		}

		public virtual bool isAt(int xq, int yq)
		{
			return x == xq && y == yq;
		}

		public override bool Equals(object other)
		{
			if (other is Handle)
			{
				Handle that = (Handle) other;
				return this.@object.Equals(that.@object) && this.x == that.x && this.y == that.y;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return (this.@object.GetHashCode() * 31 + x) * 31 + y;
		}
	}

}
