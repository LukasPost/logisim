// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{
	/// <summary>
	/// Represents an immutable rectangular bounding box. This is analogous to
	/// java.awt's <code>Point</code> class, except
	/// that objects of this type are immutable.
	/// </summary>
	public sealed class Location : IComparable<Location>
	{
		private readonly int x;
		private readonly int y;

		public Location(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public int X
		{
			get
			{
				return x;
			}
		}

		public int Y
		{
			get
			{
				return y;
			}
		}

		public int manhattanDistanceTo(Location o)
		{
			return Math.Abs(o.x - this.x) + Math.Abs(o.y - this.y);
		}

		public int manhattanDistanceTo(int x, int y)
		{
			return Math.Abs(x - this.x) + Math.Abs(y - this.y);
		}

		public Location translate(int dx, int dy)
		{
			if (dx == 0 && dy == 0)
			{
				return this;
			}
			return new Location(x + dx, y + dy);
		}

		public Location translate(Direction dir, int dist)
		{
			return translate(dir, dist, 0);
		}

		public Location translate(Direction dir, int dist, int right)
		{
			if (dist == 0 && right == 0)
			{
				return this;
			}
			if (dir == Direction.East)
			{
				return new Location(x + dist, y + right);
			}
			if (dir == Direction.West)
			{
				return new Location(x - dist, y - right);
			}
			if (dir == Direction.South)
			{
				return new Location(x - right, y + dist);
			}
			if (dir == Direction.North)
			{
				return new Location(x + right, y - dist);
			}
			return new Location(x + dist, y + right);
		}

		// rotates this around (xc,yc) assuming that this is facing in the
		// from direction and the returned bounds should face in the to direction.
		public Location rotate(Direction from, Direction to, int xc, int yc)
		{
			int degrees = to.toDegrees() - from.toDegrees();
			while (degrees >= 360)
			{
				degrees -= 360;
			}
			while (degrees < 0)
			{
				degrees += 360;
			}

			int dx = x - xc;
			int dy = y - yc;
			if (degrees == 90)
			{
				return new Location(xc + dy, yc - dx);
			}
			else if (degrees == 180)
			{
				return new Location(xc - dx, yc - dy);
			}
			else if (degrees == 270)
			{
				return new Location(xc - dy, yc + dx);
			}
			else
			{
				return this;
			}
		}

		public override bool Equals(object other_obj)
		{
			return other_obj is Location other && this.x == other.X && this.y == other.Y;
		}

		public override int GetHashCode()
		{
			return x * 31 + y;
		}

		public int CompareTo(Location other)
		{
			if (this.x != other.X)
			{
				return this.x - other.X;
			}
			else
			{
				return this.y - other.Y;
			}
		}

		public override string ToString()
		{
			return "(" + x + "," + y + ")";
		}

		public static Location parse(string value)
		{
			string @base = value;

			value = value.Trim();
			if (value[0] == '(')
			{
				int len = value.Length;
				if (value[len - 1] != ')')
				{
					throw new System.FormatException("invalid point '" + @base + "'");
				}
				value = value.Substring(1, (len - 1) - 1);
			}
			value = value.Trim();
			int comma = value.IndexOf(',');
			if (comma < 0)
			{
				comma = value.IndexOf(' ');
				if (comma < 0)
				{
					throw new System.FormatException("invalid point '" + @base + "'");
				}
			}
			int x = int.Parse(value.Substring(0, comma).Trim());
			int y = int.Parse(value.Substring(comma + 1).Trim());
			return new Location(x, y);
		}
	}

}
