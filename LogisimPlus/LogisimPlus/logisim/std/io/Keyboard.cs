// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.io
{

	using logisim.data;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Value = logisim.data.Value;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstancePoker = logisim.instance.InstancePoker;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;

	public class Keyboard : InstanceFactory
	{
		private const int CLR = 0;
		private const int CK = 1;
		private const int RE = 2;
		private const int AVL = 3;
		private const int OUT = 4;

		private const int DELAY0 = 9;
		private const int DELAY1 = 11;

		internal const int WIDTH = 145;
		internal const int HEIGHT = 25;

		private static readonly Font DEFAULT_FONT = new Font("monospaced", Font.PLAIN, 12);
		private const char FORM_FEED = '\u000c'; // control-L

		private static readonly Attribute<int> ATTR_BUFFER = Attributes.forIntegerRange("buflen", Strings.getter("keybBufferLengthAttr"), 1, 256);

		public Keyboard() : base("Keyboard", Strings.getter("keyboardComponent"))
		{
			setAttributes(new Attribute[] {ATTR_BUFFER, StdAttr.EDGE_TRIGGER}, new object[] {Convert.ToInt32(32), StdAttr.TRIG_RISING});
			OffsetBounds = Bounds.create(0, -15, WIDTH, HEIGHT);
			IconName = "keyboard.gif";
			InstancePoker = typeof(Poker);

			Port[] ps = new Port[5];
			ps[CLR] = new Port(20, 10, Port.INPUT, 1);
			ps[CK] = new Port(0, 0, Port.INPUT, 1);
			ps[RE] = new Port(10, 10, Port.INPUT, 1);
			ps[AVL] = new Port(130, 10, Port.OUTPUT, 1);
			ps[OUT] = new Port(140, 10, Port.OUTPUT, 7);
			ps[CLR].setToolTip(Strings.getter("keybClearTip"));
			ps[CK].setToolTip(Strings.getter("keybClockTip"));
			ps[RE].setToolTip(Strings.getter("keybEnableTip"));
			ps[AVL].setToolTip(Strings.getter("keybAvailTip"));
			ps[OUT].setToolTip(Strings.getter("keybOutputTip"));
			setPorts(ps);
		}

		public override void propagate(InstanceState circState)
		{
			object trigger = circState.getAttributeValue(StdAttr.EDGE_TRIGGER);
			KeyboardData state = getKeyboardState(circState);
			Value clear = circState.getPort(CLR);
			Value clock = circState.getPort(CK);
			Value enable = circState.getPort(RE);
			char c;

			lock (state)
			{
				Value lastClock = state.setLastClock(clock);
				if (clear == Value.TRUE)
				{
					state.clear();
				}
				else if (enable != Value.FALSE)
				{
					bool go;
					if (trigger == StdAttr.TRIG_FALLING)
					{
						go = lastClock == Value.TRUE && clock == Value.FALSE;
					}
					else
					{
						go = lastClock == Value.FALSE && clock == Value.TRUE;
					}
					if (go)
					{
						state.dequeue();
					}
				}

				c = state.getChar(0);
			}
			Value @out = Value.createKnown(BitWidth.create(7), c & 0x7F);
			circState.setPort(OUT, @out, DELAY0);
			circState.setPort(AVL, c != '\0' ? Value.TRUE : Value.FALSE, DELAY1);
		}

		public override void paintInstance(InstancePainter painter)
		{
			bool showState = painter.ShowState;
			Graphics g = painter.Graphics;
			painter.drawClock(CK, Direction.East);
			painter.drawBounds();
			painter.drawPort(CLR);
			painter.drawPort(RE);
			painter.drawPort(AVL);
			painter.drawPort(OUT);

			if (showState)
			{
				string str;
				int dispStart;
				int dispEnd;
				List<int> specials = new List<int>();
				FontMetrics fm = null;
				KeyboardData state = getKeyboardState(painter);
				lock (state)
				{
					str = state.ToString();
					for (int i = state.getNextSpecial(0); i >= 0; i = state.getNextSpecial(i + 1))
					{
						char c = state.getChar(i);
						specials.Add(Convert.ToInt32(c << 16 | i));
					}
					if (!state.DisplayValid)
					{
						fm = g.getFontMetrics(DEFAULT_FONT);
						state.updateDisplay(fm);
					}
					dispStart = state.DisplayStart;
					dispEnd = state.DisplayEnd;
				}

				if (str.Length > 0)
				{
					Bounds bds = painter.Bounds;
					drawBuffer(g, fm, str, dispStart, dispEnd, specials, bds);
				}
			}
			else
			{
				Bounds bds = painter.Bounds;
				int len = getBufferLength(painter.getAttributeValue(ATTR_BUFFER));
				string str = Strings.get("keybDesc", "" + len);
				FontMetrics fm = g.getFontMetrics();
				int x = bds.X + (WIDTH - fm.stringWidth(str)) / 2;
				int y = bds.Y + (HEIGHT + fm.getAscent()) / 2;
				g.drawString(str, x, y);
			}
		}

		private void drawDots(Graphics g, int x, int y, int width, int ascent)
		{
			int r = width / 10;
			if (r < 1)
			{
				r = 1;
			}
			int d = 2 * r;
			if (2 * r + 1 * d <= width)
			{
				g.fillOval(x + r, y - d, d, d);
			}
			if (3 * r + 2 * d <= width)
			{
				g.fillOval(x + 2 * r + d, y - d, d, d);
			}
			if (5 * r + 3 * d <= width)
			{
				g.fillOval(x + 3 * r + 2 * d, y - d, d, d);
			}
		}

		private void drawBuffer(Graphics g, FontMetrics fm, string str, int dispStart, int dispEnd, List<int> specials, Bounds bds)
		{
			int x = bds.X;
			int y = bds.Y;

			g.setFont(DEFAULT_FONT);
			if (fm == null)
			{
				fm = g.getFontMetrics();
			}
			int asc = fm.getAscent();
			int x0 = x + 8;
			int ys = y + (HEIGHT + asc) / 2;
			int dotsWidth = fm.stringWidth("m");
			int xs;
			if (dispStart > 0)
			{
				g.drawString(str.Substring(0, 1), x0, ys);
				xs = x0 + fm.stringWidth(str[0] + "m");
				drawDots(g, xs - dotsWidth, ys, dotsWidth, asc);
				string sub = str.Substring(dispStart, dispEnd - dispStart);
				g.drawString(sub, xs, ys);
				if (dispEnd < str.Length)
				{
					drawDots(g, xs + fm.stringWidth(sub), ys, dotsWidth, asc);
				}
			}
			else if (dispEnd < str.Length)
			{
				string sub = str.Substring(dispStart, dispEnd - dispStart);
				xs = x0;
				g.drawString(sub, xs, ys);
				drawDots(g, xs + fm.stringWidth(sub), ys, dotsWidth, asc);
			}
			else
			{
				xs = x0;
				g.drawString(str, xs, ys);
			}

			if (specials.Count > 0)
			{
				drawSpecials(specials, x0, xs, ys, asc, g, fm, str, dispStart, dispEnd);
			}
		}

		private void drawSpecials(List<int> specials, int x0, int xs, int ys, int asc, Graphics g, FontMetrics fm, string str, int dispStart, int dispEnd)
		{
			int[] px = new int[3];
			int[] py = new int[3];
			foreach (int? special in specials)
			{
				int code = special.Value;
				int pos = code & 0xFF;
				int w0;
				int w1;
				if (pos == 0)
				{
					w0 = x0;
					w1 = x0 + fm.stringWidth(str.Substring(0, 1));
				}
				else if (pos >= dispStart && pos < dispEnd)
				{
					w0 = xs + fm.stringWidth(str.Substring(dispStart, pos - dispStart));
					w1 = xs + fm.stringWidth(str.Substring(dispStart, (pos + 1) - dispStart));
				}
				else
				{
					continue; // this character is not in current view
				}
				w0++;
				w1--;

				int key = code >> 16;
				if (key == '\b')
				{
					int y1 = ys - asc / 2;
					g.drawLine(w0, y1, w1, y1);
					px[0] = w0 + 3;
					py[0] = y1 - 3;
					px[1] = w0;
					py[1] = y1;
					px[2] = w0 + 3;
					py[2] = y1 + 3;
					g.drawPolyline(px, py, 3);
				}
				else if (key == '\n')
				{
					int y1 = ys - 3;
					px[0] = w1;
					py[0] = ys - asc;
					px[1] = w1;
					py[1] = y1;
					px[2] = w0;
					py[2] = y1;
					g.drawPolyline(px, py, 3);
					px[0] = w0 + 3;
					py[0] = y1 - 3;
					px[1] = w0;
					py[1] = y1;
					px[2] = w0 + 3;
					py[2] = y1 + 3;
					g.drawPolyline(px, py, 3);
				}
				else if ((char)key == FORM_FEED)
				{
					g.drawRect(w0, ys - asc, w1 - w0, asc);
				}
			}
		}

		private static int getBufferLength(object bufferAttr)
		{
			if (bufferAttr is int?)
			{
				return ((int?) bufferAttr).Value;
			}
			else
			{
				return 32;
			}
		}

		private static KeyboardData getKeyboardState(InstanceState state)
		{
			int bufLen = getBufferLength(state.getAttributeValue(ATTR_BUFFER));
			KeyboardData ret = (KeyboardData) state.Data;
			if (ret == null)
			{
				ret = new KeyboardData(bufLen);
				state.Data = ret;
			}
			else
			{
				ret.updateBufferLength(bufLen);
			}
			return ret;
		}

		public static void addToBuffer(InstanceState state, char[] newChars)
		{
			KeyboardData keyboardData = getKeyboardState(state);
			for (int i = 0; i < newChars.Length; i++)
			{
				keyboardData.insert(newChars[i]);
			}
		}

		public class Poker : InstancePoker
		{
			public override void keyPressed(InstanceState state, KeyEvent e)
			{
				KeyboardData data = getKeyboardState(state);
				bool changed = false;
				bool used = true;
				lock (data)
				{
					switch (e.getKeyCode())
					{
					case KeyEvent.VK_DELETE:
						changed = data.delete();
						break;
					case KeyEvent.VK_LEFT:
						data.moveCursorBy(-1);
						break;
					case KeyEvent.VK_RIGHT:
						data.moveCursorBy(1);
						break;
					case KeyEvent.VK_HOME:
						data.Cursor = 0;
						break;
					case KeyEvent.VK_END:
						data.Cursor = int.MaxValue;
						break;
					default:
						used = false;
					break;
					}
				}
				if (used)
				{
					e.consume();
				}
				if (changed)
				{
					state.Instance.fireInvalidated();
				}
			}

			public override void keyTyped(InstanceState state, KeyEvent e)
			{
				KeyboardData data = getKeyboardState(state);
				char ch = e.getKeyChar();
				bool changed = false;
				if (ch != KeyEvent.CHAR_UNDEFINED)
				{
					if (!char.IsControl(ch) || ch == '\b' || ch == '\n' || ch == FORM_FEED)
					{
						lock (data)
						{
							changed = data.insert(ch);
						}
						e.consume();
					}
				}
				if (changed)
				{
					state.Instance.fireInvalidated();
				}
			}

			public virtual void draw(InstancePainter painter)
			{
				KeyboardData data = getKeyboardState(painter);
				Bounds bds = painter.getInstance().Bounds;
				Graphics g = painter.Graphics;
				FontMetrics fm = g.getFontMetrics(DEFAULT_FONT);

				string str;
				int cursor;
				int dispStart;
				lock (data)
				{
					str = data.ToString();
					cursor = data.CursorPosition;
					if (!data.DisplayValid)
					{
						data.updateDisplay(fm);
					}
					dispStart = data.DisplayStart;
				}

				int asc = fm.getAscent();
				int x = bds.X + 8;
				if (dispStart > 0)
				{
					x += fm.stringWidth(str[0] + "m");
					x += fm.stringWidth(str.Substring(dispStart, cursor - dispStart));
				}
				else if (cursor >= str.Length)
				{
					x += fm.stringWidth(str);
				}
				else
				{
					x += fm.stringWidth(str.Substring(0, cursor));
				}
				int y = bds.Y + (bds.Height + asc) / 2;
				g.drawLine(x, y - asc, x, y);
			}
		}
	}

}
