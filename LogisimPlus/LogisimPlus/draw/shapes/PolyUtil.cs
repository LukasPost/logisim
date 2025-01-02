// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.shapes
{
	using Handle = draw.model.Handle;
	using Location = logisim.data.Location;

	public class PolyUtil
	{
		private PolyUtil()
		{
		}

		public class ClosestResult
		{
			internal double dist;
			internal Location loc;
			internal Handle prevHandle;
			internal Handle nextHandle;

			public virtual double DistanceSq
			{
				get
				{
					return dist;
				}
			}

			public virtual Location Location
			{
				get
				{
					return loc;
				}
			}

			public virtual Handle PreviousHandle
			{
				get
				{
					return prevHandle;
				}
			}

			public virtual Handle NextHandle
			{
				get
				{
					return nextHandle;
				}
			}
		}

		public static ClosestResult getClosestPoint(Location loc, bool closed, Handle[] hs)
		{
			int xq = loc.X;
			int yq = loc.Y;
			ClosestResult ret = new ClosestResult();
			ret.dist = double.MaxValue;
			if (hs.Length > 0)
			{
				Handle h0 = hs[0];
				int x0 = h0.X;
				int y0 = h0.Y;
				int stop = closed ? hs.Length : (hs.Length - 1);
				for (int i = 0; i < stop; i++)
				{
					Handle h1 = hs[(i + 1) % hs.Length];
					int x1 = h1.X;
					int y1 = h1.Y;
					double d = LineUtil.ptDistSqSegment(x0, y0, x1, y1, xq, yq);
					if (d < ret.dist)
					{
						ret.dist = d;
						ret.prevHandle = h0;
						ret.nextHandle = h1;
					}
					h0 = h1;
					x0 = x1;
					y0 = y1;
				}
			}
			if (ret.dist == double.MaxValue)
			{
				return null;
			}
			else
			{
				Handle h0 = ret.prevHandle;
				Handle h1 = ret.nextHandle;
				double[] p = LineUtil.nearestPointSegment(xq, yq, h0.X, h0.Y, h1.X, h1.Y);
				ret.loc = new Location((int) (long)Math.Round(p[0], MidpointRounding.AwayFromZero), (int) (long)Math.Round(p[1], MidpointRounding.AwayFromZero));
				return ret;
			}
		}
	}

}
