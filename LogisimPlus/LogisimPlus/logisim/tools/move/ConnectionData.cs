// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.move
{

	using Wire = logisim.circuit.Wire;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;

	internal class ConnectionData
	{
		private Location loc;

		private Direction dir;

		/// <summary>
		/// The list of wires leading up to this point - we may well want to truncate this path somewhat.
		/// </summary>
		private IList<Wire> wirePath;

		private Location wirePathStart;

		public ConnectionData(Location loc, Direction dir, IList<Wire> wirePath, Location wirePathStart)
		{
			this.loc = loc;
			this.dir = dir;
			this.wirePath = wirePath;
			this.wirePathStart = wirePathStart;
		}

		public virtual Location Location
		{
			get
			{
				return loc;
			}
		}

		public virtual Direction Direction
		{
			get
			{
				return dir;
			}
		}

		public virtual IList<Wire> WirePath
		{
			get
			{
				return wirePath;
			}
		}

		public virtual Location WirePathStart
		{
			get
			{
				return wirePathStart;
			}
		}

		public override bool Equals(object other)
		{
			if (other is ConnectionData)
			{
				ConnectionData o = (ConnectionData) other;
				return this.loc.Equals(o.loc) && this.dir.Equals(o.dir);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return loc.GetHashCode() * 31 + (dir == null ? 0 : dir.GetHashCode());
		}
	}

}
