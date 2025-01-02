// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{


	using Circuit = logisim.circuit.Circuit;
	using FileStatistics = logisim.file.FileStatistics;
	using LogisimFile = logisim.file.LogisimFile;
	using Library = logisim.tools.Library;
	using TableSorter = logisim.util.TableSorter;

	public class StatisticsDialog : JDialog, ActionListener
	{
		public static void show(JFrame parent, LogisimFile file, Circuit circuit)
		{
			FileStatistics stats = FileStatistics.compute(file, circuit);
			StatisticsDialog dlog = new StatisticsDialog(parent, circuit.Name, new StatisticsTableModel(stats));
			dlog.setVisible(true);
		}

		private class StatisticsTableModel : AbstractTableModel
		{
			internal FileStatistics stats;

			internal StatisticsTableModel(FileStatistics stats)
			{
				this.stats = stats;
			}

			public virtual int ColumnCount
			{
				get
				{
					return 5;
				}
			}

			public virtual int RowCount
			{
				get
				{
					return stats.Counts.Count + 2;
				}
			}

			public override Type getColumnClass(int column)
			{
				return column < 2 ? typeof(string) : typeof(Integer);
			}

			public override string getColumnName(int column)
			{
				switch (column)
				{
				case 0:
					return Strings.get("statsComponentColumn");
				case 1:
					return Strings.get("statsLibraryColumn");
				case 2:
					return Strings.get("statsSimpleCountColumn");
				case 3:
					return Strings.get("statsUniqueCountColumn");
				case 4:
					return Strings.get("statsRecursiveCountColumn");
				default:
					return "??"; // should never happen
				}
			}

			public virtual object getValueAt(int row, int column)
			{
				IList<FileStatistics.Count> counts = stats.Counts;
				int countsLen = counts.Count;
				if (row < 0 || row >= countsLen + 2)
				{
					return "";
				}
				FileStatistics.Count count;
				if (row < countsLen)
				{
					count = counts[row];
				}
				else if (row == countsLen)
				{
					count = stats.TotalWithoutSubcircuits;
				}
				else
				{
					count = stats.TotalWithSubcircuits;
				}
				switch (column)
				{
				case 0:
					if (row < countsLen)
					{
						return count.Factory.DisplayName;
					}
					else if (row == countsLen)
					{
						return Strings.get("statsTotalWithout");
					}
					else
					{
						return Strings.get("statsTotalWith");
					}
				case 1:
					if (row < countsLen)
					{
						Library lib = count.Library;
						return lib == null ? "-" : lib.DisplayName;
					}
					else
					{
						return "";
					}
				case 2:
					return Convert.ToInt32(count.SimpleCount);
				case 3:
					return Convert.ToInt32(count.UniqueCount);
				case 4:
					return Convert.ToInt32(count.RecursiveCount);
				default:
					return ""; // should never happen
				}
			}
		}

		private class CompareString : IComparer<string>
		{
			internal string[] fixedAtBottom;

			public CompareString(params string[] fixedAtBottom)
			{
				this.fixedAtBottom = fixedAtBottom;
			}

			public virtual int Compare(string a, string b)
			{
				for (int i = fixedAtBottom.Length - 1; i >= 0; i--)
				{
					string s = fixedAtBottom[i];
					if (a.Equals(s))
					{
						return b.Equals(s) ? 0 : 1;
					}
					if (b.Equals(s))
					{
						return -1;
					}
				}
				return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
			}
		}

		private class StatisticsTable : JTable
		{
			public override void setBounds(int x, int y, int width, int height)
			{
				base.setBounds(x, y, width, height);
				PreferredColumnWidths = new double[] {0.45, 0.25, 0.1, 0.1, 0.1};
			}

			protected internal virtual double[] PreferredColumnWidths
			{
				set
				{
					Dimension tableDim = getPreferredSize();
    
					double total = 0;
					for (int i = 0; i < getColumnModel().getColumnCount(); i++)
					{
						total += value[i];
					}
    
					for (int i = 0; i < getColumnModel().getColumnCount(); i++)
					{
						TableColumn column = getColumnModel().getColumn(i);
						double width = tableDim.width * (value[i] / total);
						column.setPreferredWidth((int) width);
					}
				}
			}
		}

		private StatisticsDialog(JFrame parent, string circuitName, StatisticsTableModel model) : base(parent, true)
		{
			setDefaultCloseOperation(DISPOSE_ON_CLOSE);
			setTitle(Strings.get("statsDialogTitle", circuitName));

			JTable table = new StatisticsTable();
			TableSorter mySorter = new TableSorter(model, table.getTableHeader());
			IComparer<string> comp = new CompareString("", Strings.get("statsTotalWithout"), Strings.get("statsTotalWith"));
			mySorter.setColumnComparator(typeof(string), comp);
			table.setModel(mySorter);
			JScrollPane tablePane = new JScrollPane(table);

			JButton button = new JButton(Strings.get("statsCloseButton"));
			button.addActionListener(this);
			JPanel buttonPanel = new JPanel();
			buttonPanel.add(button);

			Container contents = this.getContentPane();
			contents.setLayout(new BorderLayout());
			contents.add(tablePane, BorderLayout.CENTER);
			contents.add(buttonPanel, BorderLayout.PAGE_END);
			this.pack();

			Dimension pref = contents.getPreferredSize();
			if (pref.width > 750 || pref.height > 550)
			{
				if (pref.width > 750)
				{
					pref.width = 750;
				}
				if (pref.height > 550)
				{
					pref.height = 550;
				}
				this.setSize(pref);
			}
		}

		public virtual void actionPerformed(ActionEvent e)
		{
			this.dispose();
		}
	}

}
