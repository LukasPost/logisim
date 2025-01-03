// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;
using System.Collections.Generic;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{
	using Circuit = logisim.circuit.Circuit;
	using CircuitState = logisim.circuit.CircuitState;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using Bounds = logisim.data.Bounds;
	using Loader = logisim.file.Loader;
	using Project = logisim.proj.Project;
	using GifEncoder = logisim.util.GifEncoder;
	using StringGetter = logisim.util.StringGetter;



	internal class ExportImage
	{
		private const int SLIDER_DIVISIONS = 6;

		private const int FORMAT_GIF = 0;
		private const int FORMAT_PNG = 1;
		private const int FORMAT_JPG = 2;

		private const int BORDER_SIZE = 5;

		private ExportImage()
		{
		}

		internal static void doExport(Project proj)
		{
			// First display circuit/parameter selection dialog
			Frame frame = proj.Frame;
			CircuitJList list = new CircuitJList(proj, true);
			if (list.getModel().getSize() == 0)
			{
				JOptionPane.showMessageDialog(proj.Frame, Strings.get("exportEmptyCircuitsMessage"), Strings.get("exportEmptyCircuitsTitle"), JOptionPane.YES_NO_OPTION);
				return;
			}
			OptionsPanel options = new OptionsPanel(list);
			int action = JOptionPane.showConfirmDialog(frame, options, Strings.get("exportImageSelect"), JOptionPane.OK_CANCEL_OPTION, JOptionPane.QUESTION_MESSAGE);
			if (action != JOptionPane.OK_OPTION)
			{
				return;
			}
			List<Circuit> circuits = list.SelectedCircuits;
			double scale = options.Scale;
			bool printerView = options.PrinterView;
			if (circuits.Count == 0)
			{
				return;
			}

			ImageFileFilter filter;
			int fmt = options.ImageFormat;
			switch (options.ImageFormat)
			{
			case FORMAT_GIF:
				filter = new ImageFileFilter(fmt, Strings.getter("exportGifFilter"), new string[] {"gif"});
				break;
			case FORMAT_PNG:
				filter = new ImageFileFilter(fmt, Strings.getter("exportPngFilter"), new string[] {"png"});
				break;
			case FORMAT_JPG:
				filter = new ImageFileFilter(fmt, Strings.getter("exportJpgFilter"), new string[] {"jpg", "jpeg", "jpe", "jfi", "jfif", "jfi"});
				break;
			default:
				Console.Error.WriteLine("unexpected format; aborted"); // OK
				return;
			}

			// Then display file chooser
			Loader loader = proj.LogisimFile.Loader;
			JFileChooser chooser = loader.createChooser();
			if (circuits.Count > 1)
			{
				chooser.setFileSelectionMode(JFileChooser.DIRECTORIES_ONLY);
				chooser.setDialogTitle(Strings.get("exportImageDirectorySelect"));
			}
			else
			{
				chooser.setFileFilter(filter);
				chooser.setDialogTitle(Strings.get("exportImageFileSelect"));
			}
			int returnVal = chooser.showDialog(frame, Strings.get("exportImageButton"));
			if (returnVal != JFileChooser.APPROVE_OPTION)
			{
				return;
			}

			// Determine whether destination is valid
			File dest = chooser.getSelectedFile();
			chooser.setCurrentDirectory(dest.isDirectory() ? dest : dest.getParentFile());
			if (dest.exists())
			{
				if (!dest.isDirectory())
				{
					int confirm = JOptionPane.showConfirmDialog(proj.Frame, Strings.get("confirmOverwriteMessage"), Strings.get("confirmOverwriteTitle"), JOptionPane.YES_NO_OPTION);
					if (confirm != JOptionPane.YES_OPTION)
					{
						return;
					}
				}
			}
			else
			{
				if (circuits.Count > 1)
				{
					bool created = dest.mkdir();
					if (!created)
					{
						JOptionPane.showMessageDialog(proj.Frame, Strings.get("exportNewDirectoryErrorMessage"), Strings.get("exportNewDirectoryErrorTitle"), JOptionPane.YES_NO_OPTION);
						return;
					}
				}
			}

			// Create the progress monitor
			ProgressMonitor monitor = new ProgressMonitor(frame, Strings.get("exportImageProgress"), null, 0, 10000);
			monitor.setMillisToDecideToPopup(100);
			monitor.setMillisToPopup(200);
			monitor.setProgress(0);

			// And start a thread to actually perform the operation
			// (This is run in a thread so that Swing will update the
			// monitor.)
			(new ExportThread(frame, frame.Canvas, dest, filter, circuits, scale, printerView, monitor)).Start();

		}

		private class OptionsPanel : JPanel, ChangeListener
		{
			internal JSlider slider;
			internal JLabel curScale;
			internal JCheckBox printerView;
			internal JRadioButton formatPng;
			internal JRadioButton formatGif;
			internal JRadioButton formatJpg;
			internal GridBagLayout gridbag;
			internal GridBagConstraints gbc;
			internal Size curScaleDim;

// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
// ORIGINAL LINE: OptionsPanel(javax.swing.JList<?> list)
			internal OptionsPanel(JList<T1> list)
			{
				// set up components
				formatPng = new JRadioButton("PNG");
				formatGif = new JRadioButton("GIF");
				formatJpg = new JRadioButton("JPEG");
				ButtonGroup bgroup = new ButtonGroup();
				bgroup.add(formatPng);
				bgroup.add(formatGif);
				bgroup.add(formatJpg);
				formatPng.setSelected(true);

				slider = new JSlider(JSlider.HORIZONTAL, -3 * SLIDER_DIVISIONS, 3 * SLIDER_DIVISIONS, 0);
				slider.setMajorTickSpacing(10);
				slider.addChangeListener(this);
				curScale = new JLabel("222%");
				curScale.setHorizontalAlignment(SwingConstants.RIGHT);
				curScale.setVerticalAlignment(SwingConstants.CENTER);
				curScaleDim = new Size(curScale.getPreferredSize());
				curScaledim.Height = Math.Max(curScaledim.Height, slider.getPreferredSize().height);
				stateChanged(null);

				printerView = new JCheckBox();
				printerView.setSelected(true);

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
				addGb(new JLabel(Strings.get("labelImageFormat") + " "));
				Box formatsPanel = new Box(BoxLayout.Y_AXIS);
				formatsPanel.add(formatPng);
				formatsPanel.add(formatGif);
				formatsPanel.add(formatJpg);
				addGb(formatsPanel);

				gbc.gridy++;
				addGb(new JLabel(Strings.get("labelScale") + " "));
				addGb(slider);
				addGb(curScale);

				gbc.gridy++;
				addGb(new JLabel(Strings.get("labelPrinterView") + " "));
				addGb(printerView);
			}

			internal virtual void addGb(JComponent comp)
			{
				gridbag.setConstraints(comp, gbc);
				add(comp);
			}

			internal virtual double Scale
			{
				get
				{
					return Math.Pow(2.0, (double) slider.getValue() / SLIDER_DIVISIONS);
				}
			}

			internal virtual bool PrinterView
			{
				get
				{
					return printerView.isSelected();
				}
			}

			internal virtual int ImageFormat
			{
				get
				{
					if (formatGif.isSelected())
					{
						return FORMAT_GIF;
					}
					if (formatJpg.isSelected())
					{
						return FORMAT_JPG;
					}
					return FORMAT_PNG;
				}
			}

			public virtual void stateChanged(ChangeEvent e)
			{
				double scale = Scale;
				curScale.setText((int) (long)Math.Round(100.0 * scale, MidpointRounding.AwayFromZero) + "%");
				if (curScaleDim != null)
				{
					curScale.setPreferredSize(curScaleDim);
				}
			}
		}

		private class ImageFileFilter : FileFilter
		{
			internal int type;
			internal string[] extensions;
			internal StringGetter desc;

			internal ImageFileFilter(int type, StringGetter desc, string[] exts)
			{
				this.type = type;
				this.desc = desc;
				extensions = new string[exts.Length];
				for (int i = 0; i < exts.Length; i++)
				{
					extensions[i] = "." + exts[i].ToLower();
				}
			}

			public override bool accept(File f)
			{
				string name = f.getName().ToLower();
				for (int i = 0; i < extensions.Length; i++)
				{
					if (name.EndsWith(extensions[i], StringComparison.Ordinal))
					{
						return true;
					}
				}
				return f.isDirectory();
			}

			public override string Description
			{
				get
				{
					return desc.get();
				}
			}
		}

		private class ExportThread : Thread
		{
			internal Frame frame;
			internal Canvas canvas;
			internal File dest;
			internal ImageFileFilter filter;
			internal List<Circuit> circuits;
			internal double scale;
			internal bool printerView;
			internal ProgressMonitor monitor;

			internal ExportThread(Frame frame, Canvas canvas, File dest, ImageFileFilter f, List<Circuit> circuits, double scale, bool printerView, ProgressMonitor monitor)
			{
				this.frame = frame;
				this.canvas = canvas;
				this.dest = dest;
				this.filter = f;
				this.circuits = circuits;
				this.scale = scale;
				this.printerView = printerView;
				this.monitor = monitor;
			}

			public override void run()
			{
				foreach (Circuit circ in circuits)
				{
					export(circ);
				}
			}

			internal virtual void export(Circuit circuit)
			{
				Bounds bds = circuit.getBounds(canvas.getGraphics()).expand(BORDER_SIZE);
				int width = (int) (long)Math.Round(bds.Width * scale, MidpointRounding.AwayFromZero);
				int height = (int) (long)Math.Round(bds.Height * scale, MidpointRounding.AwayFromZero);
				BufferedImage img = new BufferedImage(width, height, BufferedImage.TYPE_INT_RGB);
				JGraphics @base = img.getJGraphics();
				JGraphics g = @base.create();
				g.setColor(Color.White);
				g.fillRect(0, 0, width, height);
				g.setColor(Color.Black);
				g.scale(scale, scale);
				g.translate(-bds.X, -bds.Y);

				CircuitState circuitState = canvas.Project.getCircuitState(circuit);
				ComponentDrawContext context = new ComponentDrawContext(canvas, circuit, circuitState, @base, g, printerView);
				circuit.draw(context, null);

				File where;
				if (dest.isDirectory())
				{
					where = new File(dest, circuit.Name + filter.extensions[0]);
				}
				else if (filter.accept(dest))
				{
					where = dest;
				}
				else
				{
					string newName = dest.getName() + filter.extensions[0];
					where = new File(dest.getParentFile(), newName);
				}
				try
				{
					switch (filter.type)
					{
					case FORMAT_GIF:
						GifEncoder.toFile(img, where, monitor);
						break;
					case FORMAT_PNG:
						ImageIO.write(img, "PNG", where);
						break;
					case FORMAT_JPG:
						ImageIO.write(img, "JPEG", where);
						break;
					}
				}
				catch (Exception)
				{
					JOptionPane.showMessageDialog(frame, Strings.get("couldNotCreateFile"));
					monitor.close();
					return;
				}
				g.dispose();
				monitor.close();
			}
		}
	}

}
