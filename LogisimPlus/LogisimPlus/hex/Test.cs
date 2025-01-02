// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace hex
{


	public class Test
	{
		private class Model : HexModel
		{
			internal List<HexModelListener> listeners = new List<HexModelListener>();
			internal int[] data = new int[924];

			public virtual void addHexModelListener(HexModelListener l)
			{
				listeners.Add(l);
			}

			public virtual void removeHexModelListener(HexModelListener l)
			{
				listeners.Remove(l);
			}

			public virtual long FirstOffset
			{
				get
				{
					return 11111;
				}
			}

			public virtual long LastOffset
			{
				get
				{
					return data.Length + 11110;
				}
			}

			public virtual int ValueWidth
			{
				get
				{
					return 9;
				}
			}

			public virtual int get(long address)
			{
				return data[(int)(address - 11111)];
			}

			public virtual void set(long address, int value)
			{
				int[] oldValues = new int[] {data[(int)(address - 11111)]};
				data[(int)(address - 11111)] = value & 0x1FF;
				foreach (HexModelListener l in listeners)
				{
					l.bytesChanged(this, address, 1, oldValues);
				}
			}

			public virtual void set(long start, int[] values)
			{
				int[] oldValues = new int[values.Length];
				Array.Copy(data, (int)(start - 11111), oldValues, 0, values.Length);
				Array.Copy(values, 0, data, (int)(start - 11111), values.Length);
				foreach (HexModelListener l in listeners)
				{
					l.bytesChanged(this, start, values.Length, oldValues);
				}
			}

			public virtual void fill(long start, long len, int value)
			{
				int[] oldValues = new int[(int) len];
				Array.Copy(data, (int)(start - 11111), oldValues, 0, (int) len);
				Arrays.Fill(data, (int)(start - 11111), (int) len, value);
				foreach (HexModelListener l in listeners)
				{
					l.bytesChanged(this, start, len, oldValues);
				}
			}
		}

		public static void Main(string[] args)
		{
			JFrame frame = new JFrame();
			HexModel model = new Model();
			HexEditor editor = new HexEditor(model);
			frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
			frame.getContentPane().add(new JScrollPane(editor));
			frame.pack();
			frame.setVisible(true);
		}
	}

}
