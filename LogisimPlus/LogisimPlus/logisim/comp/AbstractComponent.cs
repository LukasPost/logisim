// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{

	using CircuitState = logisim.circuit.CircuitState;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;

	public abstract class AbstractComponent : Component
	{
		public abstract object getFeature(object key);
		public abstract void draw(ComponentDrawContext context);
		public abstract void expose(ComponentDrawContext context);
		public abstract logisim.data.AttributeSet AttributeSet {get;}
		public abstract void removeComponentListener(ComponentListener l);
		public abstract void addComponentListener(ComponentListener l);
		protected internal AbstractComponent()
		{
		}

		//
		// basic information methods
		//
		public abstract ComponentFactory Factory {get;}

		//
		// location/extent methods
		//
		public abstract Location Location {get;}

		public abstract Bounds Bounds {get;}

		public virtual Bounds getBounds(Graphics g)
		{
			return Bounds;
		}

		public virtual bool contains(Location pt)
		{
			Bounds bds = Bounds;
			if (bds == null)
			{
				return false;
			}
			return bds.contains(pt, 1);
		}

		public virtual bool contains(Location pt, Graphics g)
		{
			Bounds bds = getBounds(g);
			if (bds == null)
			{
				return false;
			}
			return bds.contains(pt, 1);
		}

		//
		// propagation methods
		//
		public abstract IList<EndData> Ends {get;}

		public virtual EndData getEnd(int index)
		{
			return Ends[index];
		}

		public virtual bool endsAt(Location pt)
		{
			foreach (EndData data in Ends)
			{
				if (data.Location.Equals(pt))
				{
					return true;
				}
			}
			return false;
		}

		public abstract void propagate(CircuitState state);
	}

}
