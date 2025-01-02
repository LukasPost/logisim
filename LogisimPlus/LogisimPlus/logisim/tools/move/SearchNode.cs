// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.move
{
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;

	internal class SearchNode : IComparable<SearchNode>
	{
		private const int CROSSING_PENALTY = 20;
		private const int TURN_PENALTY = 50;

		private readonly Location loc;
		private readonly Direction dir;
		private ConnectionData conn;
		private readonly Location dest;
		private int dist;
		private int heur;
		private bool extendsWire;
		private SearchNode prev;

		public SearchNode(ConnectionData conn, Location src, Direction srcDir, Location dst) : this(src, srcDir, conn, dst, 0, srcDir != null, null)
		{
		}

		private SearchNode(Location loc, Direction dir, ConnectionData conn, Location dest, int dist, bool extendsWire, SearchNode prev)
		{
			this.loc = loc;
			this.dir = dir;
			this.conn = conn;
			this.dest = dest;
			this.dist = dist;
			this.heur = dist + this.Heuristic;
			this.extendsWire = extendsWire;
			this.prev = prev;
		}

		private int Heuristic
		{
			get
			{
				Location cur = loc;
				Location dst = dest;
				Direction curDir = dir;
				int dx = dst.X - cur.X;
				int dy = dst.Y - cur.Y;
				int ret = -1;
				if (extendsWire)
				{
					ret = -1;
					if (curDir == Direction.East)
					{
						if (dx > 0)
						{
							ret = dx / 10 * 9 + Math.Abs(dy);
						}
					}
					else if (curDir == Direction.West)
					{
						if (dx < 0)
						{
							ret = -dx / 10 * 9 + Math.Abs(dy);
						}
					}
					else if (curDir == Direction.South)
					{
						if (dy > 0)
						{
							ret = Math.Abs(dx) + dy / 10 * 9;
						}
					}
					else if (curDir == Direction.North)
					{
						if (dy < 0)
						{
							ret = Math.Abs(dx) - dy / 10 * 9;
						}
					}
				}
				if (ret < 0)
				{
					ret = Math.Abs(dx) + Math.Abs(dy);
				}
				bool penalizeDoubleTurn = false;
				if (curDir == Direction.East)
				{
					penalizeDoubleTurn = dx < 0;
				}
				else if (curDir == Direction.West)
				{
					penalizeDoubleTurn = dx > 0;
				}
				else if (curDir == Direction.North)
				{
					penalizeDoubleTurn = dy > 0;
				}
				else if (curDir == Direction.South)
				{
					penalizeDoubleTurn = dy < 0;
				}
				else if (curDir == null)
				{
					if (dx != 0 || dy != 0)
					{
						ret += TURN_PENALTY;
					}
				}
				if (penalizeDoubleTurn)
				{
					ret += 2 * TURN_PENALTY;
				}
				else if (dx != 0 && dy != 0)
				{
					ret += TURN_PENALTY;
				}
				return ret;
			}
		}

		public virtual SearchNode next(Direction moveDir, bool crossing)
		{
			int newDist = dist;
			Direction connDir = conn.Direction;
			Location nextLoc = loc.translate(moveDir, 10);
			bool exWire = extendsWire && moveDir == connDir;
			if (exWire)
			{
				newDist += 9;
			}
			else
			{
				newDist += 10;
			}
			if (crossing)
			{
				newDist += CROSSING_PENALTY;
			}
			if (moveDir != dir)
			{
				newDist += TURN_PENALTY;
			}
			if (nextLoc.X < 0 || nextLoc.Y < 0)
			{
				return null;
			}
			else
			{
				return new SearchNode(nextLoc, moveDir, conn, dest, newDist, exWire, this);
			}
		}

		public virtual bool Start
		{
			get
			{
				return prev == null;
			}
		}

		public virtual bool Destination
		{
			get
			{
				return dest.Equals(loc);
			}
		}

		public virtual SearchNode Previous
		{
			get
			{
				return prev;
			}
		}

		public virtual int Distance
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

		public virtual Direction Direction
		{
			get
			{
				return dir;
			}
		}

		public virtual int HeuristicValue
		{
			get
			{
				return heur;
			}
		}

		public virtual Location Destination
		{
			get
			{
				return dest;
			}
		}

		public virtual bool ExtendingWire
		{
			get
			{
				return extendsWire;
			}
		}

		public virtual ConnectionData Connection
		{
			get
			{
				return conn;
			}
		}

		public override bool Equals(object other)
		{
			if (other is SearchNode)
			{
				SearchNode o = (SearchNode) other;
				return this.loc.Equals(o.loc) && (this.dir == null ? o.dir == null : this.dir.Equals(o.dir)) && this.dest.Equals(o.dest);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			int dirHash = dir == null ? 0 : dir.GetHashCode();
			return ((loc.GetHashCode() * 31) + dirHash) * 31 + dest.GetHashCode();
		}

		public virtual int CompareTo(SearchNode o)
		{
			int ret = this.heur - o.heur;

			if (ret == 0)
			{
				return this.GetHashCode() - o.GetHashCode();
			}
			else
			{
				return ret;
			}
		}

		public override string ToString()
		{
			return loc + "/" + (dir == null ? "null" : dir.ToString()) + (extendsWire ? "+" : "-") + "/" + dest + ":" + dist + "+" + (heur - dist);
		}
	}

}
