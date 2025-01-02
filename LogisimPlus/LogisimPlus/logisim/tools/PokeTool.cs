// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{

	using Circuit = logisim.circuit.Circuit;
	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using RadixOption = logisim.circuit.RadixOption;
	using Wire = logisim.circuit.Wire;
	using WireSet = logisim.circuit.WireSet;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentUserEvent = logisim.comp.ComponentUserEvent;
	using AttributeSet = logisim.data.AttributeSet;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Canvas = logisim.gui.main.Canvas;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Project = logisim.proj.Project;
	using Icons = logisim.util.Icons;

	public class PokeTool : Tool
	{
		private static readonly Icon toolIcon = Icons.getIcon("poke.gif");
		private static readonly Color caretColor = new Color(255, 255, 150);

		private class WireCaret : AbstractCaret
		{
			internal Canvas canvas;
			internal Wire wire;
			internal int x;
			internal int y;

			internal WireCaret(Canvas c, Wire w, int x, int y)
			{
				canvas = c;
				wire = w;
				this.x = x;
				this.y = y;
			}

			public override void draw(Graphics g)
			{
				Value v = canvas.CircuitState.getValue(wire.End0);
				RadixOption radix1 = RadixOption.decode(AppPreferences.POKE_WIRE_RADIX1.get());
				RadixOption radix2 = RadixOption.decode(AppPreferences.POKE_WIRE_RADIX2.get());
				if (radix1 == null)
				{
					radix1 = RadixOption.RADIX_2;
				}
				string vStr = radix1.toString(v);
				if (radix2 != null && v.Width > 1)
				{
					vStr += " / " + radix2.toString(v);
				}

				FontMetrics fm = g.getFontMetrics();
				g.setColor(caretColor);
				g.fillRect(x + 2, y + 2, fm.stringWidth(vStr) + 4, fm.getAscent() + fm.getDescent() + 4);
				g.setColor(Color.BLACK);
				g.drawRect(x + 2, y + 2, fm.stringWidth(vStr) + 4, fm.getAscent() + fm.getDescent() + 4);
				g.fillOval(x - 2, y - 2, 5, 5);
				g.drawString(vStr, x + 4, y + 4 + fm.getAscent());
			}
		}

		private class Listener : CircuitListener
		{
			private readonly PokeTool outerInstance;

			public Listener(PokeTool outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void circuitChanged(CircuitEvent @event)
			{
				Circuit circ = outerInstance.pokedCircuit;
				if (@event.Circuit == circ && circ != null && (@event.Action == CircuitEvent.ACTION_REMOVE || @event.Action == CircuitEvent.ACTION_CLEAR) && !circ.contains(outerInstance.pokedComponent))
				{
					outerInstance.removeCaret(false);
				}
			}
		}

		private static Cursor cursor = Cursor.getPredefinedCursor(Cursor.HAND_CURSOR);

		private Listener listener;
		private Circuit pokedCircuit;
		private Component pokedComponent;
		private Caret pokeCaret;

		public PokeTool()
		{
			this.listener = new Listener(this);
		}

		public override bool Equals(object other)
		{
			return other is PokeTool;
		}

		public override int GetHashCode()
		{
			return typeof(PokeTool).GetHashCode();
		}

		public override string Name
		{
			get
			{
				return "Poke Tool";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("pokeTool");
			}
		}

		private void removeCaret(bool normal)
		{
			Circuit circ = pokedCircuit;
			Caret caret = pokeCaret;
			if (caret != null)
			{
				if (normal)
				{
					caret.stopEditing();
				}
				else
				{
					caret.cancelEditing();
				}
				circ.removeCircuitListener(listener);
				pokedCircuit = null;
				pokedComponent = null;
				pokeCaret = null;
			}
		}

		private void setPokedComponent(Circuit circ, Component comp, Caret caret)
		{
			removeCaret(true);
			pokedCircuit = circ;
			pokedComponent = comp;
			pokeCaret = caret;
			if (caret != null)
			{
				circ.addCircuitListener(listener);
			}
		}

		public override string Description
		{
			get
			{
				return Strings.get("pokeToolDesc");
			}
		}

		public override void draw(Canvas canvas, ComponentDrawContext context)
		{
			if (pokeCaret != null)
			{
				pokeCaret.draw(context.Graphics);
			}
		}

		public override void deselect(Canvas canvas)
		{
			removeCaret(true);
			canvas.HighlightedWires = WireSet.EMPTY;
		}

		public override void mousePressed(Canvas canvas, Graphics g, MouseEvent e)
		{
			int x = e.getX();
			int y = e.getY();
			Location loc = new Location(x, y);
			bool dirty = false;
			canvas.HighlightedWires = WireSet.EMPTY;
			if (pokeCaret != null && !pokeCaret.getBounds(g).contains(loc))
			{
				dirty = true;
				removeCaret(true);
			}
			if (pokeCaret == null)
			{
				ComponentUserEvent @event = new ComponentUserEvent(canvas, x, y);
				Circuit circ = canvas.Circuit;
				foreach (Component c in circ.getAllContaining(loc, g))
				{
					if (pokeCaret != null)
					{
						break;
					}

					if (c is Wire)
					{
						Caret caret = new WireCaret(canvas, (Wire) c, x, y);
						setPokedComponent(circ, c, caret);
						canvas.HighlightedWires = circ.getWireSet((Wire) c);
					}
					else
					{
						Pokable p = (Pokable) c.getFeature(typeof(Pokable));
						if (p != null)
						{
							Caret caret = p.getPokeCaret(@event);
							setPokedComponent(circ, c, caret);
							AttributeSet attrs = c.AttributeSet;
							if (attrs != null && attrs.Attributes.Count > 0)
							{
								Project proj = canvas.Project;
								proj.Frame.viewComponentAttributes(circ, c);
							}
						}
					}
				}
			}
			if (pokeCaret != null)
			{
				dirty = true;
				pokeCaret.mousePressed(e);
			}
			if (dirty)
			{
				canvas.Project.repaintCanvas();
			}
		}

		public override void mouseDragged(Canvas canvas, Graphics g, MouseEvent e)
		{
			if (pokeCaret != null)
			{
				pokeCaret.mouseDragged(e);
				canvas.Project.repaintCanvas();
			}
		}

		public override void mouseReleased(Canvas canvas, Graphics g, MouseEvent e)
		{
			if (pokeCaret != null)
			{
				pokeCaret.mouseReleased(e);
				canvas.Project.repaintCanvas();
			}
		}

		public override void keyTyped(Canvas canvas, KeyEvent e)
		{
			if (pokeCaret != null)
			{
				pokeCaret.keyTyped(e);
				canvas.Project.repaintCanvas();
			}
		}

		public override void keyPressed(Canvas canvas, KeyEvent e)
		{
			if (pokeCaret != null)
			{
				pokeCaret.keyPressed(e);
				canvas.Project.repaintCanvas();
			}
		}

		public override void keyReleased(Canvas canvas, KeyEvent e)
		{
			if (pokeCaret != null)
			{
				pokeCaret.keyReleased(e);
				canvas.Project.repaintCanvas();
			}
		}

		public override void paintIcon(ComponentDrawContext c, int x, int y)
		{
			Graphics g = c.Graphics;
			if (toolIcon != null)
			{
				toolIcon.paintIcon(c.Destination, g, x + 2, y + 2);
			}
			else
			{
				g.setColor(Color.black);
				g.drawLine(x + 4, y + 2, x + 4, y + 17);
				g.drawLine(x + 4, y + 17, x + 1, y + 11);
				g.drawLine(x + 4, y + 17, x + 7, y + 11);

				g.drawLine(x + 15, y + 2, x + 15, y + 17);
				g.drawLine(x + 15, y + 2, x + 12, y + 8);
				g.drawLine(x + 15, y + 2, x + 18, y + 8);
			}
		}

		public override Cursor Cursor
		{
			get
			{
				return cursor;
			}
		}
	}

}
