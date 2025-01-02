// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	internal class PropagationPoints
	{
		private class Entry
		{
			internal CircuitState state;
			internal Location loc;

			internal Entry(CircuitState state, Location loc)
			{
				this.state = state;
				this.loc = loc;
			}

			public override bool Equals(object other)
			{
				if (!(other is Entry))
				{
					return false;
				}
				Entry o = (Entry) other;
				return state.Equals(o.state) && loc.Equals(o.loc);
			}

			public override int GetHashCode()
			{
				return state.GetHashCode() * 31 + loc.GetHashCode();
			}
		}

		private HashSet<Entry> data;

		internal PropagationPoints()
		{
			this.data = new HashSet<Entry>();
		}

		internal virtual void add(CircuitState state, Location loc)
		{
			data.Add(new Entry(state, loc));
		}

		internal virtual void clear()
		{
			data.Clear();
		}

		internal virtual bool Empty
		{
			get
			{
				return data.Count == 0;
			}
		}

		internal virtual void draw(ComponentDrawContext context)
		{
			if (data.Count == 0)
			{
				return;
			}

			CircuitState state = context.CircuitState;
			Dictionary<CircuitState, CircuitState> stateMap = new Dictionary<CircuitState, CircuitState>();
			foreach (CircuitState s in state.Substates)
			{
				addSubstates(stateMap, s, s);
			}

			JGraphics g = context.Graphics;
			GraphicsUtil.switchToWidth(g, 2);
			foreach (Entry e in data)
			{
				if (e.state == state)
				{
					Location p = e.loc;
					g.drawOval(p.X - 4, p.Y - 4, 8, 8);
				}
				else if (stateMap.ContainsKey(e.state))
				{
					CircuitState substate = stateMap[e.state];
					Component subcirc = substate.Subcircuit;
					Bounds b = subcirc.Bounds;
					g.drawRect(b.X, b.Y, b.Width, b.Height);
				}
			}
			GraphicsUtil.switchToWidth(g, 1);
		}

		private void addSubstates(Dictionary<CircuitState, CircuitState> map, CircuitState source, CircuitState value)
		{
			map[source] = value;
			foreach (CircuitState s in source.Substates)
			{
				addSubstates(map, s, value);
			}
		}
	}

}
