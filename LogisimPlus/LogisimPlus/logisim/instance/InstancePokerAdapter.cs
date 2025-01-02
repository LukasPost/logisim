// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{

	using CircuitState = logisim.circuit.CircuitState;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentUserEvent = logisim.comp.ComponentUserEvent;
	using Bounds = logisim.data.Bounds;
	using Canvas = logisim.gui.main.Canvas;
	using AbstractCaret = logisim.tools.AbstractCaret;
	using Caret = logisim.tools.Caret;
	using Pokable = logisim.tools.Pokable;

	internal class InstancePokerAdapter : AbstractCaret, Pokable
	{
		private InstanceComponent comp;
		private Canvas canvas;
		private InstancePoker poker;
		private InstanceStateImpl state;
		private ComponentDrawContext context;

		public InstancePokerAdapter(InstanceComponent comp, Type pokerClass)
		{
			try
			{
				this.comp = comp;
				poker = pokerClass.GetConstructor().newInstance();
			}
			catch (Exception t)
			{
				handleError(t, pokerClass);
				poker = null;
			}
		}

		private void handleError(Exception t, Type pokerClass)
		{
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = pokerClass.FullName;
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Console.Error.WriteLine("error while instantiating poker " + className + ": " + t.GetType().FullName);
			string msg = t.Message;
			if (!string.ReferenceEquals(msg, null))
			{
				Console.Error.WriteLine("  (" + msg + ")"); // OK
			}
		}

		public virtual Caret getPokeCaret(ComponentUserEvent @event)
		{
			if (poker == null)
			{
				return null;
			}
			else
			{
				canvas = @event.Canvas;
				CircuitState circState = @event.CircuitState;
				InstanceStateImpl state = new InstanceStateImpl(circState, comp);
				MouseEvent e = new MouseEvent(@event.Canvas, MouseEvent.MOUSE_PRESSED, DateTimeHelper.CurrentUnixTimeMillis(), 0, @event.X, @event.Y, 1, false);
				bool isAccepted = poker.init(state, e);
				if (isAccepted)
				{
					this.state = state;
					this.context = new ComponentDrawContext(@event.Canvas, @event.Canvas.Circuit, circState, null, null);
					mousePressed(e);
					return this;
				}
				else
				{
					poker = null;
					return null;
				}
			}
		}

		public override void mousePressed(MouseEvent e)
		{
			if (poker != null)
			{
				poker.mousePressed(state, e);
				checkCurrent();
			}
		}

		public override void mouseDragged(MouseEvent e)
		{
			if (poker != null)
			{
				poker.mouseDragged(state, e);
				checkCurrent();
			}
		}

		public override void mouseReleased(MouseEvent e)
		{
			if (poker != null)
			{
				poker.mouseReleased(state, e);
				checkCurrent();
			}
		}

		public override void keyPressed(KeyEvent e)
		{
			if (poker != null)
			{
				poker.keyPressed(state, e);
				checkCurrent();
			}
		}

		public override void keyReleased(KeyEvent e)
		{
			if (poker != null)
			{
				poker.keyReleased(state, e);
				checkCurrent();
			}
		}

		public override void keyTyped(KeyEvent e)
		{
			if (poker != null)
			{
				poker.keyTyped(state, e);
				checkCurrent();
			}
		}

		public override void stopEditing()
		{
			if (poker != null)
			{
				poker.stopEditing(state);
				checkCurrent();
			}
		}

		public override Bounds getBounds(Graphics g)
		{
			if (poker != null)
			{
				context.Graphics = g;
				InstancePainter painter = new InstancePainter(context, comp);
				return poker.getBounds(painter);
			}
			else
			{
				return Bounds.EMPTY_BOUNDS;
			}
		}

		public override void draw(Graphics g)
		{
			if (poker != null)
			{
				context.Graphics = g;
				InstancePainter painter = new InstancePainter(context, comp);
				poker.paint(painter);
			}
		}

		private void checkCurrent()
		{
			if (state != null && canvas != null)
			{
				CircuitState s0 = state.CircuitState;
				CircuitState s1 = canvas.CircuitState;
				if (s0 != s1)
				{
					state = new InstanceStateImpl(s1, comp);
				}
			}
		}
	}

}
