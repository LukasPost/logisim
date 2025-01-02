// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{


	using Circuit = logisim.circuit.Circuit;
	using CircuitState = logisim.circuit.CircuitState;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using Bounds = logisim.data.Bounds;
	using Project = logisim.proj.Project;
	using StringUtil = logisim.util.StringUtil;

	public class Print
	{
		private Print()
		{
		}

		public static void doPrint(Project proj)
		{
			CircuitJList list = new CircuitJList(proj, true);
			Frame frame = proj.Frame;
			if (list.getModel().getSize() == 0)
			{
				JOptionPane.showMessageDialog(proj.Frame, Strings.get("printEmptyCircuitsMessage"), Strings.get("printEmptyCircuitsTitle"), JOptionPane.YES_NO_OPTION);
				return;
			}
			ParmsPanel parmsPanel = new ParmsPanel(list);
			int action = JOptionPane.showConfirmDialog(frame, parmsPanel, Strings.get("printParmsTitle"), JOptionPane.OK_CANCEL_OPTION, JOptionPane.QUESTION_MESSAGE);
			if (action != JOptionPane.OK_OPTION)
			{
				return;
			}
			IList<Circuit> circuits = list.SelectedCircuits;
			if (circuits.Count == 0)
			{
				return;
			}

			PageFormat format = new PageFormat();
			Printable print = new MyPrintable(proj, circuits, parmsPanel.Header, parmsPanel.RotateToFit, parmsPanel.PrinterView);

			PrinterJob job = PrinterJob.getPrinterJob();
			job.setPrintable(print, format);
			if (job.printDialog() == false)
			{
				return;
			}
			try
			{
				job.print();
			}
			catch (PrinterException e)
			{
				JOptionPane.showMessageDialog(proj.Frame, StringUtil.format(Strings.get("printError"), e.ToString()), Strings.get("printErrorTitle"), JOptionPane.ERROR_MESSAGE);
			}
		}

		private class ParmsPanel : JPanel
		{
			internal JCheckBox rotateToFit;
			internal JCheckBox printerView;
			internal JTextField header;
			internal GridBagLayout gridbag;
			internal GridBagConstraints gbc;

// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
// ORIGINAL LINE: ParmsPanel(javax.swing.JList<?> list)
			internal ParmsPanel(JList<T1> list)
			{
				// set up components
				rotateToFit = new JCheckBox();
				rotateToFit.setSelected(true);
				printerView = new JCheckBox();
				printerView.setSelected(true);
				header = new JTextField(20);
				header.setText("%n (%p of %P)");

				// set up panel
				gridbag = new GridBagLayout();
				gbc = new GridBagConstraints();
				setLayout(gridbag);

				// now add components into panel
				gbc.gridy = 0;
				gbc.gridx = GridBagConstraints.RELATIVE;
				gbc.anchor = GridBagConstraints.NORTHWEST;
				gbc.insets = new Insets(5, 0, 5, 0);
				gbc.fill = GridBagConstraints.NONE;
				addGb(new JLabel(Strings.get("labelCircuits") + " "));
				gbc.fill = GridBagConstraints.HORIZONTAL;
				addGb(new JScrollPane(list));
				gbc.fill = GridBagConstraints.NONE;

				gbc.gridy++;
				addGb(new JLabel(Strings.get("labelHeader") + " "));
				addGb(header);

				gbc.gridy++;
				addGb(new JLabel(Strings.get("labelRotateToFit") + " "));
				addGb(rotateToFit);

				gbc.gridy++;
				addGb(new JLabel(Strings.get("labelPrinterView") + " "));
				addGb(printerView);
			}

			internal virtual void addGb(JComponent comp)
			{
				gridbag.setConstraints(comp, gbc);
				add(comp);
			}

			internal virtual bool RotateToFit
			{
				get
				{
					return rotateToFit.isSelected();
				}
			}

			internal virtual bool PrinterView
			{
				get
				{
					return printerView.isSelected();
				}
			}

			internal virtual string Header
			{
				get
				{
					return header.getText();
				}
			}
		}

		private class MyPrintable : Printable
		{
			internal Project proj;
			internal IList<Circuit> circuits;
			internal string header;
			internal bool rotateToFit;
			internal bool printerView;

			internal MyPrintable(Project proj, IList<Circuit> circuits, string header, bool rotateToFit, bool printerView)
			{
				this.proj = proj;
				this.circuits = circuits;
				this.header = header;
				this.rotateToFit = rotateToFit;
				this.printerView = printerView;
			}

			public virtual int print(Graphics @base, PageFormat format, int pageIndex)
			{
				if (pageIndex >= circuits.Count)
				{
					return Printable.NO_SUCH_PAGE;
				}

				Circuit circ = circuits[pageIndex];
				CircuitState circState = proj.getCircuitState(circ);
				Graphics g = @base.create();
				Graphics2D g2 = g is Graphics2D ? (Graphics2D) g : null;
				FontMetrics fm = g.getFontMetrics();
				string head = (!string.ReferenceEquals(header, null) && !header.Equals("")) ? Print.format(header, pageIndex + 1, circuits.Count, circ.Name) : null;
				int headHeight = (string.ReferenceEquals(head, null) ? 0 : fm.getHeight());

				// Compute image size
				double imWidth = format.getImageableWidth();
				double imHeight = format.getImageableHeight();

				// Correct coordinate system for page, including
				// translation and possible rotation.
				Bounds bds = circ.getBounds(g).expand(4);
				double scale = Math.Min(imWidth / bds.Width, (imHeight - headHeight) / bds.Height);
				if (g2 != null)
				{
					g2.translate(format.getImageableX(), format.getImageableY());
					if (rotateToFit && scale < 1.0 / 1.1)
					{
						double scale2 = Math.Min(imHeight / bds.Width, (imWidth - headHeight) / bds.Height);
						if (scale2 >= scale * 1.1)
						{ // will rotate
							scale = scale2;
							if (imHeight > imWidth)
							{ // portrait -> landscape
								g2.translate(0, imHeight);
								g2.rotate(-Math.PI / 2);
							}
							else
							{ // landscape -> portrait
								g2.translate(imWidth, 0);
								g2.rotate(Math.PI / 2);
							}
							double t = imHeight;
							imHeight = imWidth;
							imWidth = t;
						}
					}
				}

				// Draw the header line if appropriate
				if (!string.ReferenceEquals(head, null))
				{
					g.drawString(head, (int) (long)Math.Round((imWidth - fm.stringWidth(head)) / 2, MidpointRounding.AwayFromZero), fm.getAscent());
					if (g2 != null)
					{
						imHeight -= headHeight;
						g2.translate(0, headHeight);
					}
				}

				// Now change coordinate system for circuit, including
				// translation and possible scaling
				if (g2 != null)
				{
					if (scale < 1.0)
					{
						g2.scale(scale, scale);
						imWidth /= scale;
						imHeight /= scale;
					}
					double dx = Math.Max(0.0, (imWidth - bds.Width) / 2);
					g2.translate(-bds.X + dx, -bds.Y);
				}

				// Ensure that the circuit is eligible to be drawn
				Rectangle clip = g.getClipBounds();
				clip.add(bds.X, bds.Y);
				clip.add(bds.X + bds.Width, bds.Y + bds.Height);
				g.setClip(clip);

				// And finally draw the circuit onto the page
				ComponentDrawContext context = new ComponentDrawContext(proj.Frame.getCanvas(), circ, circState, @base, g, printerView);
				ICollection<Component> noComps = Collections.emptySet();
				circ.draw(context, noComps);
				g.dispose();
				return Printable.PAGE_EXISTS;
			}
		}

		private static string format(string header, int index, int max, string circName)
		{
			int mark = header.IndexOf('%');
			if (mark < 0)
			{
				return header;
			}
			StringBuilder ret = new StringBuilder();
			int start = 0;
			for (; mark >= 0 && mark + 1 < header.Length; start = mark + 2, mark = header.IndexOf('%', start))
			{
				ret.Append(header.Substring(start, mark - start));
				switch (header[mark + 1])
				{
				case 'n':
					ret.Append(circName);
					break;
				case 'p':
					ret.Append("" + index);
					break;
				case 'P':
					ret.Append("" + max);
					break;
				case '%':
					ret.Append("%");
					break;
				default:
					ret.Append("%" + header[mark + 1]);
				break;
				}
			}
			if (start < header.Length)
			{
				ret.Append(header.Substring(start));
			}
			return ret.ToString();
		}
	}

}
