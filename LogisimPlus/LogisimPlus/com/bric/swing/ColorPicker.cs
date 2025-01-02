// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Threading;

/*
* @(#)ColorPicker.java  1.0  2008-03-01
*
* Copyright (c) 2008 Jeremy Wood
* E-mail: mickleness@gmail.com
* All rights reserved.
*
* The copyright of this software is owned by Jeremy Wood.
* You may not use, copy or modify this software, except in
* accordance with the license agreement you entered into with
* Jeremy Wood. For details see accompanying license terms.
*/

namespace com.bric.swing
{


	/// <summary>
	/// This is a panel that offers a robust set of controls to pick a color.
	/// <P>
	/// This was originally intended to replace the <code>JColorChooser</code>. To use this class to create a color choosing
	/// dialog, simply call: <BR>
	/// <code>ColorPicker.showDialog(frame, originalColor);</code>
	/// <P>
	/// However this panel is also resizable, and it can exist in other contexts. For example, you might try the following
	/// panel: <BR>
	/// <code>ColorPicker picker = new ColorPicker(false, false);</code> <BR>
	/// <code>picker.setPreferredSize(new Dimension(200,160));</code> <BR>
	/// <code>picker.setMode(ColorPicker.HUE);</code>
	/// <P>
	/// This will create a miniature color picker that still lets the user choose from every available color, but it does not
	/// include all the buttons and numeric controls on the right side of the panel. This might be ideal if you are working
	/// with limited space, or non-power-users who don't need the RGB values of a color. The <code>main()</code> method of
	/// this class demonstrates possible ways you can customize a <code>ColorPicker</code> component.
	/// <P>
	/// To listen to color changes to this panel, you can add a <code>PropertyChangeListener</code> listening for changes to
	/// the <code>SELECTED_COLOR_PROPERTY</code>. This will be triggered only when the RGB value of the selected color
	/// changes.
	/// <P>
	/// To listen to opacity changes to this panel, use a <code>PropertyChangeListener</code> listening for changes to the
	/// <code>OPACITY_PROPERTY</code>.
	/// 
	/// @version 1.2
	/// @author Jeremy Wood
	/// </summary>
	public class ColorPicker : JPanel
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			hexDocListener = new HexDocumentListener(this);
			alpha = new Option(this, strings.getObject("alphaLabel").ToString(), 255);
			hue = new Option(this, strings.getObject("hueLabel").ToString(), 360);
			sat = new Option(this, strings.getObject("saturationLabel").ToString(), 100);
			bri = new Option(this, strings.getObject("brightnessLabel").ToString(), 100);
			red = new Option(this, strings.getObject("redLabel").ToString(), 255);
			green = new Option(this, strings.getObject("greenLabel").ToString(), 255);
			blue = new Option(this, strings.getObject("blueLabel").ToString(), 255);
		}

		private const long serialVersionUID = 3L;

		/// <summary>
		/// The localized strings used in this (and related) panel(s). </summary>
		protected internal static ResourceBundle strings = ResourceBundle.getBundle("bric.ColorPicker");

		/// <summary>
		/// This demonstrates how to customize a small <code>ColorPicker</code> component.
		/// </summary>
		public static void Main(string[] args)
		{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final JFrame demo = new JFrame("Demo");
			JFrame demo = new JFrame("Demo");
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final JWindow palette = new JWindow(demo);
			JWindow palette = new JWindow(demo);
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final ColorPicker picker = new ColorPicker(true, false);
			ColorPicker picker = new ColorPicker(true, false);

// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final JComboBox<String> comboBox = new JComboBox<>();
			JComboBox<string> comboBox = new JComboBox<string>();
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final JCheckBox alphaCheckbox = new JCheckBox("Include Alpha");
			JCheckBox alphaCheckbox = new JCheckBox("Include Alpha");
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final JCheckBox hsbCheckbox = new JCheckBox("Include HSB Values");
			JCheckBox hsbCheckbox = new JCheckBox("Include HSB Values");
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final JCheckBox rgbCheckbox = new JCheckBox("Include RGB Values");
			JCheckBox rgbCheckbox = new JCheckBox("Include RGB Values");
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final JCheckBox modeCheckbox = new JCheckBox("Include Mode Controls", true);
			JCheckBox modeCheckbox = new JCheckBox("Include Mode Controls", true);
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final JButton button = new JButton("Show Dialog");
			JButton button = new JButton("Show Dialog");

			demo.getContentPane().setLayout(new GridBagLayout());
			palette.getContentPane().setLayout(new GridBagLayout());

			GridBagConstraints c = new GridBagConstraints();
			c.gridx = 0;
			c.gridy = 0;
			c.weightx = 1;
			c.weighty = 0;
			c.insets = new Insets(5, 5, 5, 5);
			c.anchor = GridBagConstraints.WEST;
			palette.getContentPane().add(comboBox, c);
			c.gridy++;
			palette.getContentPane().add(alphaCheckbox, c);
			c.gridy++;
			palette.getContentPane().add(hsbCheckbox, c);
			c.gridy++;
			palette.getContentPane().add(rgbCheckbox, c);
			c.gridy++;
			palette.getContentPane().add(modeCheckbox, c);

			c.gridy = 0;
			c.weighty = 1;
			c.fill = GridBagConstraints.BOTH;
			picker.setPreferredSize(new Dimension(220, 200));
			demo.getContentPane().add(picker, c);
			c.gridy++;
			c.weighty = 0;
			demo.getContentPane().add(picker.ExpertControls, c);
			c.gridy++;
			c.fill = GridBagConstraints.NONE;
			demo.getContentPane().add(button, c);

			comboBox.addItem("Hue");
			comboBox.addItem("Saturation");
			comboBox.addItem("Brightness");
			comboBox.addItem("Red");
			comboBox.addItem("Green");
			comboBox.addItem("Blue");

			ActionListener checkboxListener = new ActionListenerAnonymousInnerClass(demo, picker, alphaCheckbox, hsbCheckbox, rgbCheckbox, modeCheckbox);
			picker.OpacityVisible = false;
			picker.HSBControlsVisible = false;
			picker.RGBControlsVisible = false;
			picker.HexControlsVisible = false;
			picker.PreviewSwatchVisible = false;

			picker.addPropertyChangeListener(MODE_PROPERTY, new PropertyChangeListenerAnonymousInnerClass(picker, comboBox));

			alphaCheckbox.addActionListener(checkboxListener);
			hsbCheckbox.addActionListener(checkboxListener);
			rgbCheckbox.addActionListener(checkboxListener);
			modeCheckbox.addActionListener(checkboxListener);
			button.addActionListener(new ActionListenerAnonymousInnerClass2(demo, picker));

			comboBox.addActionListener(new ActionListenerAnonymousInnerClass3(picker));
			comboBox.setSelectedIndex(2);

			palette.pack();
			palette.setLocationRelativeTo(null);

			demo.addComponentListener(new ComponentAdapterAnonymousInnerClass(demo, palette));
			demo.pack();
			demo.setLocationRelativeTo(null);
			demo.setVisible(true);
			palette.setVisible(true);

			demo.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		}

		private class ActionListenerAnonymousInnerClass : ActionListener
		{
			private JFrame demo;
			private com.bric.swing.ColorPicker picker;
			private JCheckBox alphaCheckbox;
			private JCheckBox hsbCheckbox;
			private JCheckBox rgbCheckbox;
			private JCheckBox modeCheckbox;

			public ActionListenerAnonymousInnerClass(JFrame demo, com.bric.swing.ColorPicker picker, JCheckBox alphaCheckbox, JCheckBox hsbCheckbox, JCheckBox rgbCheckbox, JCheckBox modeCheckbox)
			{
				this.demo = demo;
				this.picker = picker;
				this.alphaCheckbox = alphaCheckbox;
				this.hsbCheckbox = hsbCheckbox;
				this.rgbCheckbox = rgbCheckbox;
				this.modeCheckbox = modeCheckbox;
			}

			public void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				if (src == alphaCheckbox)
				{
					picker.OpacityVisible = alphaCheckbox.isSelected();
				}
				else if (src == hsbCheckbox)
				{
					picker.HSBControlsVisible = hsbCheckbox.isSelected();
				}
				else if (src == rgbCheckbox)
				{
					picker.RGBControlsVisible = rgbCheckbox.isSelected();
				}
				else if (src == modeCheckbox)
				{
					picker.ModeControlsVisible = modeCheckbox.isSelected();
				}
				demo.pack();
			}
		}

		private class PropertyChangeListenerAnonymousInnerClass : PropertyChangeListener
		{
			private com.bric.swing.ColorPicker picker;
			private JComboBox<string> comboBox;

			public PropertyChangeListenerAnonymousInnerClass(com.bric.swing.ColorPicker picker, JComboBox<string> comboBox)
			{
				this.picker = picker;
				this.comboBox = comboBox;
			}

			public void propertyChange(PropertyChangeEvent evt)
			{
				int m = picker.Mode;
				if (m == HUE)
				{
					comboBox.setSelectedIndex(0);
				}
				else if (m == SAT)
				{
					comboBox.setSelectedIndex(1);
				}
				else if (m == BRI)
				{
					comboBox.setSelectedIndex(2);
				}
				else if (m == RED)
				{
					comboBox.setSelectedIndex(3);
				}
				else if (m == GREEN)
				{
					comboBox.setSelectedIndex(4);
				}
				else if (m == BLUE)
				{
					comboBox.setSelectedIndex(5);
				}
			}
		}

		private class ActionListenerAnonymousInnerClass2 : ActionListener
		{
			private JFrame demo;
			private com.bric.swing.ColorPicker picker;

			public ActionListenerAnonymousInnerClass2(JFrame demo, com.bric.swing.ColorPicker picker)
			{
				this.demo = demo;
				this.picker = picker;
			}

			public void actionPerformed(ActionEvent e)
			{
				Color color = picker.Color;
				color = ColorPicker.showDialog(demo, color, true);
				if (color != null)
				{
					picker.Color = color;
				}
			}
		}

		private class ActionListenerAnonymousInnerClass3 : ActionListener
		{
			private com.bric.swing.ColorPicker picker;

			public ActionListenerAnonymousInnerClass3(com.bric.swing.ColorPicker picker)
			{
				this.picker = picker;
			}

			public void actionPerformed(ActionEvent e)
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: int i = ((JComboBox<?>) e.getSource()).getSelectedIndex();
				int i = ((JComboBox<object>) e.getSource()).getSelectedIndex();
				if (i == 0)
				{
					picker.Mode = ColorPicker.HUE;
				}
				else if (i == 1)
				{
					picker.Mode = ColorPicker.SAT;
				}
				else if (i == 2)
				{
					picker.Mode = ColorPicker.BRI;
				}
				else if (i == 3)
				{
					picker.Mode = ColorPicker.RED;
				}
				else if (i == 4)
				{
					picker.Mode = ColorPicker.GREEN;
				}
				else if (i == 5)
				{
					picker.Mode = ColorPicker.BLUE;
				}
			}
		}

		private class ComponentAdapterAnonymousInnerClass : ComponentAdapter
		{
			private JFrame demo;
			private JWindow palette;

			public ComponentAdapterAnonymousInnerClass(JFrame demo, JWindow palette)
			{
				this.demo = demo;
				this.palette = palette;
			}

			public void componentMoved(ComponentEvent e)
			{
				Point p = demo.getLocation();
				palette.setLocation(new Point(p.x - palette.getWidth() - 10, p.y));
			}
		}

		public static Color showDialog(Container owner, Color originalColor)
		{
			if (owner is Window)
			{
				return showDialog((Window) owner, originalColor);
			}
			else
			{
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Logger.getLogger(typeof(ColorPicker).FullName).log(Level.SEVERE, "Not a Window subclass: " + owner);
				Toolkit.getDefaultToolkit().beep();
			}
			return null;
		}

		/// <summary>
		/// This creates a modal dialog prompting the user to select a color.
		/// <P>
		/// This uses a generic dialog title: "Choose a Color", and does not include opacity.
		/// </summary>
		/// <param name="owner">         the dialog this new dialog belongs to. This must be a Frame or a Dialog. Java 1.6 supports
		///                      Windows here, but this package is designed/compiled to work in Java 1.4, so an
		///                      <code>IllegalArgumentException</code> will be thrown if this component is a
		///                      <code>Window</code>. </param>
		/// <param name="originalColor"> the color the <code>ColorPicker</code> initially points to. </param>
		/// <returns> the <code>Color</code> the user chooses, or <code>null</code> if the user cancels the dialog. </returns>
		public static Color showDialog(Window owner, Color originalColor)
		{
			return showDialog(owner, null, originalColor, false);
		}

		/// <summary>
		/// This creates a modal dialog prompting the user to select a color.
		/// <P>
		/// This uses a generic dialog title: "Choose a Color".
		/// </summary>
		/// <param name="owner">          the dialog this new dialog belongs to. This must be a Frame or a Dialog. Java 1.6 supports
		///                       Windows here, but this package is designed/compiled to work in Java 1.4, so an
		///                       <code>IllegalArgumentException</code> will be thrown if this component is a
		///                       <code>Window</code>. </param>
		/// <param name="originalColor">  the color the <code>ColorPicker</code> initially points to. </param>
		/// <param name="includeOpacity"> whether to add a control for the opacity of the color. </param>
		/// <returns> the <code>Color</code> the user chooses, or <code>null</code> if the user cancels the dialog. </returns>
		public static Color showDialog(Window owner, Color originalColor, bool includeOpacity)
		{
			return showDialog(owner, null, originalColor, includeOpacity);
		}

		/// <summary>
		/// This creates a modal dialog prompting the user to select a color.
		/// </summary>
		/// <param name="owner">          the dialog this new dialog belongs to. This must be a Frame or a Dialog. Java 1.6 supports
		///                       Windows here, but this package is designed/compiled to work in Java 1.4, so an
		///                       <code>IllegalArgumentException</code> will be thrown if this component is a
		///                       <code>Window</code>. </param>
		/// <param name="title">          the title for the dialog. </param>
		/// <param name="originalColor">  the color the <code>ColorPicker</code> initially points to. </param>
		/// <param name="includeOpacity"> whether to add a control for the opacity of the color. </param>
		/// <returns> the <code>Color</code> the user chooses, or <code>null</code> if the user cancels the dialog. </returns>
		public static Color showDialog(Window owner, string title, Color originalColor, bool includeOpacity)
		{
			ColorPickerDialog d;
			if (owner is Frame || owner == null)
			{
				d = new ColorPickerDialog((Frame) owner, originalColor, includeOpacity);
			}
			else if (owner is Dialog)
			{
				d = new ColorPickerDialog((Dialog) owner, originalColor, includeOpacity);
			}
			else
			{
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new System.ArgumentException("the owner (" + owner.GetType().FullName + ") must be a java.awt.Frame or a java.awt.Dialog");
			}

			d.setTitle(string.ReferenceEquals(title, null) ? strings.getObject("ColorPickerDialogTitle").ToString() : title);
			d.pack();
			d.setVisible(true);
			return d.Color;
		}

		/// <summary>
		/// <code>PropertyChangeEvents</code> will be triggered for this property when the selected color changes.
		/// <P>
		/// (Events are only created when then RGB values of the color change. This means, for example, that the change from
		/// HSB(0,0,0) to HSB(.4,0,0) will <i>not</i> generate events, because when the brightness stays zero the RGB color
		/// remains (0,0,0). So although the hue moved around, the color is still black, so no events are created.)
		/// 
		/// </summary>
		public const string SELECTED_COLOR_PROPERTY = "selected color";

		/// <summary>
		/// <code>PropertyChangeEvents</code> will be triggered for this property when <code>setModeControlsVisible()</code>
		/// is called.
		/// </summary>
		public const string MODE_CONTROLS_VISIBLE_PROPERTY = "mode controls visible";

		/// <summary>
		/// <code>PropertyChangeEvents</code> will be triggered when the opacity value is adjusted.
		/// </summary>
		public const string OPACITY_PROPERTY = "opacity";

		/// <summary>
		/// <code>PropertyChangeEvents</code> will be triggered when the mode changes. (That is, when the wheel switches from
		/// HUE, SAT, BRI, RED, GREEN, or BLUE modes.)
		/// </summary>
		public const string MODE_PROPERTY = "mode";

		/// <summary>
		/// Used to indicate when we're in "hue mode". </summary>
		protected internal const int HUE = 0;
		/// <summary>
		/// Used to indicate when we're in "brightness mode". </summary>
		protected internal const int BRI = 1;
		/// <summary>
		/// Used to indicate when we're in "saturation mode". </summary>
		protected internal const int SAT = 2;
		/// <summary>
		/// Used to indicate when we're in "red mode". </summary>
		protected internal const int RED = 3;
		/// <summary>
		/// Used to indicate when we're in "green mode". </summary>
		protected internal const int GREEN = 4;
		/// <summary>
		/// Used to indicate when we're in "blue mode". </summary>
		protected internal const int BLUE = 5;

		/// <summary>
		/// The vertical slider </summary>
		private JSlider slider = new JSlider(JSlider.VERTICAL, 0, 100, 0);

		internal ChangeListener changeListener = new ChangeListenerAnonymousInnerClass();

		private class ChangeListenerAnonymousInnerClass : ChangeListener
		{
			public void stateChanged(ChangeEvent e)
			{
				object src = e.getSource();

				if (outerInstance.hue.contains(src) || outerInstance.sat.contains(src) || outerInstance.bri.contains(src))
				{
					if (outerInstance.adjustingSpinners > 0)
					{
						return;
					}

					outerInstance.setHSB(outerInstance.hue.getFloatValue() / 360f, outerInstance.sat.getFloatValue() / 100f, outerInstance.bri.getFloatValue() / 100f);
				}
				else if (outerInstance.red.contains(src) || outerInstance.green.contains(src) || outerInstance.blue.contains(src))
				{
					if (outerInstance.adjustingSpinners > 0)
					{
						return;
					}

					outerInstance.setRGB(outerInstance.red.getIntValue(), outerInstance.green.getIntValue(), outerInstance.blue.getIntValue());
				}
				else if (src == outerInstance.colorPanel)
				{
					if (outerInstance.adjustingColorPanel > 0)
					{
						return;
					}

					int mode = outerInstance.Mode;
					if (mode == HUE || mode == BRI || mode == SAT)
					{
						float[] hsb = outerInstance.colorPanel.getHSB();
						outerInstance.setHSB(hsb[0], hsb[1], hsb[2]);
					}
					else
					{
						int[] rgb = outerInstance.colorPanel.getRGB();
						outerInstance.setRGB(rgb[0], rgb[1], rgb[2]);
					}
				}
				else if (src == outerInstance.slider)
				{
					if (outerInstance.adjustingSlider > 0)
					{
						return;
					}

					int v = outerInstance.slider.getValue();
					Option option = outerInstance.SelectedOption;
					option.Value = v;
				}
				else if (outerInstance.alpha.contains(src))
				{
					if (outerInstance.adjustingOpacity > 0)
					{
						return;
					}
					int v = outerInstance.alpha.getIntValue();
					outerInstance.Opacity = ((float) v) / 255f;
				}
				else if (src == outerInstance.opacitySlider)
				{
					if (outerInstance.adjustingOpacity > 0)
					{
						return;
					}

					float newValue = (((float) outerInstance.opacitySlider.getValue()) / 255f);
					outerInstance.Opacity = newValue;
				}
			}
		}

		internal ActionListener actionListener = new ActionListenerAnonymousInnerClass4();

		private class ActionListenerAnonymousInnerClass4 : ActionListener
		{
			public void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				if (src == outerInstance.hue.radioButton)
				{
					outerInstance.Mode = HUE;
				}
				else if (src == outerInstance.bri.radioButton)
				{
					outerInstance.Mode = BRI;
				}
				else if (src == outerInstance.sat.radioButton)
				{
					outerInstance.Mode = SAT;
				}
				else if (src == outerInstance.red.radioButton)
				{
					outerInstance.Mode = RED;
				}
				else if (src == outerInstance.green.radioButton)
				{
					outerInstance.Mode = GREEN;
				}
				else if (src == outerInstance.blue.radioButton)
				{
					outerInstance.Mode = BLUE;
				}
			}
		}

		/// <returns> the currently selected <code>Option</code> </returns>
		private Option SelectedOption
		{
			get
			{
				int mode = Mode;
				if (mode == HUE)
				{
					return hue;
				}
				else if (mode == SAT)
				{
					return sat;
				}
				else if (mode == BRI)
				{
					return bri;
				}
				else if (mode == RED)
				{
					return red;
				}
				else if (mode == GREEN)
				{
					return green;
				}
				else
				{
					return blue;
				}
			}
		}

		/// <summary>
		/// This thread will wait a second or two before committing the text in the hex TextField. This gives the user a
		/// chance to finish typing... but if the user is just waiting for something to happen, this makes sure after a
		/// second or two something happens.
		/// </summary>
		internal class HexUpdateThread : Thread
		{
			private readonly ColorPicker outerInstance;

			internal long myStamp;
			internal string text;

			public HexUpdateThread(ColorPicker outerInstance, long stamp, string s)
			{
				this.outerInstance = outerInstance;
				myStamp = stamp;
				text = s;
			}

			public virtual void run()
			{
				if (SwingUtilities.isEventDispatchThread() == false)
				{
					long WAIT = 1500;

					while (DateTimeHelper.CurrentUnixTimeMillis() - myStamp < WAIT)
					{
						try
						{
							long delay = WAIT - (DateTimeHelper.CurrentUnixTimeMillis() - myStamp);
							if (delay < 1)
							{
								delay = 1;
							}
							Thread.Sleep(delay);
						}
						catch (Exception)
						{
							Thread.yield();
						}
					}
					SwingUtilities.invokeLater(this);
					return;
				}

				if (myStamp != outerInstance.hexDocListener.lastTimeStamp)
				{
					// another event has come along and trumped this one
					return;
				}

				if (text.Length > 6)
				{
					text = text.Substring(0, 6);
				}
				while (text.Length < 6)
				{
					text = text + "0";
				}
				if (outerInstance.hexField.getText().Equals(text))
				{
					return;
				}

				int pos = outerInstance.hexField.getCaretPosition();
				outerInstance.hexField.setText(text);
				outerInstance.hexField.setCaretPosition(pos);
			}
		}

		internal HexDocumentListener hexDocListener;

		internal class HexDocumentListener : DocumentListener
		{
			private readonly ColorPicker outerInstance;

			public HexDocumentListener(ColorPicker outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal long lastTimeStamp;

			public virtual void changedUpdate(DocumentEvent e)
			{
				lastTimeStamp = DateTimeHelper.CurrentUnixTimeMillis();

				if (outerInstance.adjustingHexField > 0)
				{
					return;
				}

				string s = outerInstance.hexField.getText();
				s = stripToHex(s);
				if (s.Length == 6)
				{
					// the user typed 6 digits: we can work with this:
					try
					{
						int i = Convert.ToInt32(s, 16);
						outerInstance.setRGB(((i >> 16) & 0xff), ((i >> 8) & 0xff), ((i) & 0xff));
						return;
					}
					catch (System.FormatException e2)
					{
						// this shouldn't happen, since we already stripped out non-hex characters.
						Console.WriteLine(e2.ToString());
						Console.Write(e2.StackTrace);
					}
				}
				Thread thread = new HexUpdateThread(outerInstance, lastTimeStamp, s);
				thread.Start();
				while (DateTimeHelper.CurrentUnixTimeMillis() - lastTimeStamp == 0)
				{
					Thread.yield();
				}
			}

			/// <summary>
			/// Strips a string down to only uppercase hex-supported characters. </summary>
			internal virtual string stripToHex(string s)
			{
				s = s.ToUpper();
				string s2 = "";
				for (int a = 0; a < s.Length; a++)
				{
					char c = s[a];
					if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9' || c == '0' || c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F')
					{
						s2 = s2 + c;
					}
				}
				return s2;
			}

			public virtual void insertUpdate(DocumentEvent e)
			{
				changedUpdate(e);
			}

			public virtual void removeUpdate(DocumentEvent e)
			{
				changedUpdate(e);
			}
		}

		private Option alpha;
		private Option hue;
		private Option sat;
		private Option bri;
		private Option red;
		private Option green;
		private Option blue;
		private ColorSwatch preview = new ColorSwatch(50);
		private JLabel hexLabel = new JLabel(strings.getObject("hexLabel").ToString());
		private JTextField hexField = new JTextField("000000");

		/// <summary>
		/// Used to indicate when we're internally adjusting the value of the spinners. If this equals zero, then incoming
		/// events are triggered by the user and must be processed. If this is not equal to zero, then incoming events are
		/// triggered by another method that's already responding to the user's actions.
		/// </summary>
		private int adjustingSpinners = 0;

		/// <summary>
		/// Used to indicate when we're internally adjusting the value of the slider. If this equals zero, then incoming
		/// events are triggered by the user and must be processed. If this is not equal to zero, then incoming events are
		/// triggered by another method that's already responding to the user's actions.
		/// </summary>
		private int adjustingSlider = 0;

		/// <summary>
		/// Used to indicate when we're internally adjusting the selected color of the ColorPanel. If this equals zero, then
		/// incoming events are triggered by the user and must be processed. If this is not equal to zero, then incoming
		/// events are triggered by another method that's already responding to the user's actions.
		/// </summary>
		private int adjustingColorPanel = 0;

		/// <summary>
		/// Used to indicate when we're internally adjusting the value of the hex field. If this equals zero, then incoming
		/// events are triggered by the user and must be processed. If this is not equal to zero, then incoming events are
		/// triggered by another method that's already responding to the user's actions.
		/// </summary>
		private int adjustingHexField = 0;

		/// <summary>
		/// Used to indicate when we're internally adjusting the value of the opacity. If this equals zero, then incoming
		/// events are triggered by the user and must be processed. If this is not equal to zero, then incoming events are
		/// triggered by another method that's already responding to the user's actions.
		/// </summary>
		private int adjustingOpacity = 0;

		/// <summary>
		/// The "expert" controls are the controls on the right side of this panel: the labels/spinners/radio buttons.
		/// </summary>
		private JPanel expertControls = new JPanel(new GridBagLayout());

		private ColorPickerPanel colorPanel = new ColorPickerPanel();

		private JSlider opacitySlider = new JSlider(0, 255, 255);
		private JLabel opacityLabel = new JLabel(strings.getObject("opacityLabel").ToString());

		/// <summary>
		/// Create a new <code>ColorPicker</code> with all controls visible except opacity. </summary>
		public ColorPicker() : this(true, false)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Create a new <code>ColorPicker</code>.
		/// </summary>
		/// <param name="showExpertControls"> the labels/spinners/buttons on the right side of a <code>ColorPicker</code> are
		///                           optional. This boolean will control whether they are shown or not.
		///                           <P>
		///                           It may be that your users will never need or want numeric control when they choose
		///                           their colors, so hiding this may simplify your interface. </param>
		/// <param name="includeOpacity">     whether the opacity controls will be shown </param>
		public ColorPicker(bool showExpertControls, bool includeOpacity) : base(new GridBagLayout())
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			GridBagConstraints c = new GridBagConstraints();

			Insets normalInsets = new Insets(3, 3, 3, 3);

			JPanel options = new JPanel(new GridBagLayout());
			c.gridx = 0;
			c.gridy = 0;
			c.weightx = 1;
			c.weighty = 1;
			c.insets = normalInsets;
			ButtonGroup bg = new ButtonGroup();

			// put them in order
			Option[] optionsArray = new Option[] {hue, sat, bri, red, green, blue};

			for (int a = 0; a < optionsArray.Length; a++)
			{
				if (a == 3 || a == 6)
				{
					c.insets = new Insets(normalInsets.top + 10, normalInsets.left, normalInsets.bottom, normalInsets.right);
				}
				else
				{
					c.insets = normalInsets;
				}
				c.anchor = GridBagConstraints.EAST;
				c.fill = GridBagConstraints.NONE;
				options.add(optionsArray[a].label, c);
				c.gridx++;
				c.anchor = GridBagConstraints.WEST;
				c.fill = GridBagConstraints.HORIZONTAL;
				if (optionsArray[a].spinner != null)
				{
					options.add(optionsArray[a].spinner, c);
				}
				else
				{
					options.add(optionsArray[a].slider, c);
				}
				c.gridx++;
				c.fill = GridBagConstraints.NONE;
				options.add(optionsArray[a].radioButton, c);
				c.gridy++;
				c.gridx = 0;
				bg.add(optionsArray[a].radioButton);
			}
			c.insets = new Insets(normalInsets.top + 10, normalInsets.left, normalInsets.bottom, normalInsets.right);
			c.anchor = GridBagConstraints.EAST;
			c.fill = GridBagConstraints.NONE;
			options.add(hexLabel, c);
			c.gridx++;
			c.anchor = GridBagConstraints.WEST;
			c.fill = GridBagConstraints.HORIZONTAL;
			options.add(hexField, c);
			c.gridy++;
			c.gridx = 0;
			c.anchor = GridBagConstraints.EAST;
			c.fill = GridBagConstraints.NONE;
			options.add(alpha.label, c);
			c.gridx++;
			c.anchor = GridBagConstraints.WEST;
			c.fill = GridBagConstraints.HORIZONTAL;
			options.add(alpha.spinner, c);

			c.gridx = 0;
			c.gridy = 0;
			c.weightx = 1;
			c.weighty = 1;
			c.fill = GridBagConstraints.BOTH;
			c.anchor = GridBagConstraints.CENTER;
			c.insets = normalInsets;
			c.gridwidth = 2;
			add(colorPanel, c);

			c.gridwidth = 1;
			c.insets = normalInsets;
			c.gridx += 2;
			c.weighty = 1;
			c.gridwidth = 1;
			c.fill = GridBagConstraints.VERTICAL;
			c.weightx = 0;
			add(slider, c);

			c.gridx++;
			c.fill = GridBagConstraints.VERTICAL;
			c.gridheight = GridBagConstraints.REMAINDER;
			c.anchor = GridBagConstraints.CENTER;
			c.insets = new Insets(0, 0, 0, 0);
			add(expertControls, c);

			c.gridx = 0;
			c.gridheight = 1;
			c.gridy = 1;
			c.weightx = 0;
			c.weighty = 0;
			c.insets = normalInsets;
			c.anchor = GridBagConstraints.CENTER;
			add(opacityLabel, c);
			c.gridx++;
			c.gridwidth = 2;
			c.weightx = 1;
			c.fill = GridBagConstraints.HORIZONTAL;
			add(opacitySlider, c);

			c.gridx = 0;
			c.gridy = 0;
			c.gridheight = 1;
			c.gridwidth = 1;
			c.fill = GridBagConstraints.BOTH;
			c.weighty = 1;
			c.anchor = GridBagConstraints.CENTER;
			c.weightx = 1;
			c.insets = new Insets(normalInsets.top, normalInsets.left + 8, normalInsets.bottom + 10, normalInsets.right + 8);
			expertControls.add(preview, c);
			c.gridy++;
			c.weighty = 0;
			c.anchor = GridBagConstraints.CENTER;
			c.insets = new Insets(normalInsets.top, normalInsets.left, 0, normalInsets.right);
			expertControls.add(options, c);

			preview.setOpaque(true);
			colorPanel.setPreferredSize(new Dimension(expertControls.getPreferredSize().height, expertControls.getPreferredSize().height));

			slider.addChangeListener(changeListener);
			colorPanel.addChangeListener(changeListener);
			slider.setUI(new ColorPickerSliderUI(slider, this));
			hexField.getDocument().addDocumentListener(hexDocListener);
			Mode = BRI;

			ExpertControlsVisible = showExpertControls;

			OpacityVisible = includeOpacity;

			opacitySlider.addChangeListener(changeListener);

			Opacity = 1;
		}

		/// <summary>
		/// This controls whether the hex field (and label) are visible or not.
		/// <P>
		/// Note this lives inside the "expert controls", so if <code>setExpertControlsVisible(false)</code> has been called,
		/// then calling this method makes no difference: the hex controls will be hidden.
		/// </summary>
		public virtual bool HexControlsVisible
		{
			set
			{
				hexLabel.setVisible(value);
				hexField.setVisible(value);
			}
		}

		/// <summary>
		/// This controls whether the preview swatch visible or not.
		/// <P>
		/// Note this lives inside the "expert controls", so if <code>setExpertControlsVisible(false)</code> has been called,
		/// then calling this method makes no difference: the swatch will be hidden.
		/// </summary>
		public virtual bool PreviewSwatchVisible
		{
			set
			{
				preview.setVisible(value);
			}
		}

		/// <summary>
		/// The labels/spinners/buttons on the right side of a <code>ColorPicker</code> are optional. This method will
		/// control whether they are shown or not.
		/// <P>
		/// It may be that your users will never need or want numeric control when they choose their colors, so hiding this
		/// may simplify your interface.
		/// </summary>
		/// <param name="b"> whether to show or hide the expert controls. </param>
		public virtual bool ExpertControlsVisible
		{
			set
			{
				expertControls.setVisible(value);
			}
		}

		/// <returns> the current HSB coordinates of this <code>ColorPicker</code>. Each value is between [0,1].
		///  </returns>
		public virtual float[] HSB
		{
			get
			{
				return new float[] {hue.FloatValue / 360f, sat.FloatValue / 100f, bri.FloatValue / 100f};
			}
		}

		/// <returns> the current RGB coordinates of this <code>ColorPicker</code>. Each value is between [0,255].
		///  </returns>
		public virtual int[] RGB
		{
			get
			{
				return new int[] {red.IntValue, green.IntValue, blue.IntValue};
			}
		}

		/// <summary>
		/// Returns the currently selected opacity (a float between 0 and 1).
		/// </summary>
		/// <returns> the currently selected opacity (a float between 0 and 1). </returns>
		public virtual float Opacity
		{
			get
			{
				return ((float) opacitySlider.getValue()) / 255f;
			}
			set
			{
				if (value < 0 || value > 1)
				{
					throw new System.ArgumentException("The opacity (" + value + ") must be between 0 and 1.");
				}
				adjustingOpacity++;
				try
				{
					int i = (int)(255 * value);
					opacitySlider.setValue(i);
					alpha.spinner.setValue(i);
					if (lastOpacity != value)
					{
						firePropertyChange(OPACITY_PROPERTY, lastOpacity, i);
						Color c = preview.getForeground();
						preview.setForeground(new Color(c.getRed(), c.getGreen(), c.getBlue(), i));
					}
					lastOpacity = value;
				}
				finally
				{
					adjustingOpacity--;
				}
			}
		}

		private float lastOpacity = 1;


		/// <summary>
		/// Sets the mode of this <code>ColorPicker</code>. This is especially useful if this picker is in non-expert mode,
		/// so the radio buttons are not visible for the user to directly select.
		/// </summary>
		/// <param name="mode"> must be HUE, SAT, BRI, RED, GREEN or BLUE. </param>
		public virtual int Mode
		{
			set
			{
				if (!(value == HUE || value == SAT || value == BRI || value == RED || value == GREEN || value == BLUE))
				{
					throw new System.ArgumentException("mode must be HUE, SAT, BRI, REd, GREEN, or BLUE");
				}
				putClientProperty(MODE_PROPERTY, value);
				hue.radioButton.setSelected(value == HUE);
				sat.radioButton.setSelected(value == SAT);
				bri.radioButton.setSelected(value == BRI);
				red.radioButton.setSelected(value == RED);
				green.radioButton.setSelected(value == GREEN);
				blue.radioButton.setSelected(value == BLUE);
    
				colorPanel.Mode = value;
				adjustingSlider++;
				try
				{
					slider.setValue(0);
					Option option = SelectedOption;
					slider.setInverted(value == HUE);
					int max = option.Maximum;
					slider.setMaximum(max);
					slider.setValue(option.IntValue);
					slider.repaint();
    
					if (value == HUE || value == SAT || value == BRI)
					{
						setHSB(hue.FloatValue / 360f, sat.FloatValue / 100f, bri.FloatValue / 100f);
					}
					else
					{
						setRGB(red.IntValue, green.IntValue, blue.IntValue);
    
					}
				}
				finally
				{
					adjustingSlider--;
				}
			}
			get
			{
				int? i = (int?) getClientProperty(MODE_PROPERTY);
				if (i == null)
				{
					return -1;
				}
				return i.Value;
			}
		}

		/// <summary>
		/// This controls whether the radio buttons that adjust the mode are visible.
		/// <P>
		/// (These buttons appear next to the spinners in the expert controls.)
		/// <P>
		/// Note these live inside the "expert controls", so if <code>setExpertControlsVisible(false)</code> has been called,
		/// then these will never be visible.
		/// </summary>
		/// <param name="b"> </param>
		public virtual bool ModeControlsVisible
		{
			set
			{
				hue.radioButton.setVisible(value && hue.Visible);
				sat.radioButton.setVisible(value && sat.Visible);
				bri.radioButton.setVisible(value && bri.Visible);
				red.radioButton.setVisible(value && red.Visible);
				green.radioButton.setVisible(value && green.Visible);
				blue.radioButton.setVisible(value && blue.Visible);
				putClientProperty(MODE_CONTROLS_VISIBLE_PROPERTY, value);
			}
		}


		/// <summary>
		/// Sets the current color of this <code>ColorPicker</code>. This method simply calls <code>setRGB()</code> and
		/// <code>setOpacity()</code>.
		/// </summary>
		/// <param name="c"> the new color to use. </param>
		public virtual Color Color
		{
			set
			{
				setRGB(value.getRed(), value.getGreen(), value.getBlue());
				float opacity = ((float) value.getAlpha()) / 255f;
				Opacity = opacity;
			}
			get
			{
				int[] i = RGB;
				return new Color(i[0], i[1], i[2], opacitySlider.getValue());
			}
		}

		/// <summary>
		/// Sets the current color of this <code>ColorPicker</code>
		/// </summary>
		/// <param name="r"> the red value. Must be between [0,255]. </param>
		/// <param name="g"> the green value. Must be between [0,255]. </param>
		/// <param name="b"> the blue value. Must be between [0,255]. </param>
		public virtual void setRGB(int r, int g, int b)
		{
			if (r < 0 || r > 255)
			{
				throw new System.ArgumentException("The red value (" + r + ") must be between [0,255].");
			}
			if (g < 0 || g > 255)
			{
				throw new System.ArgumentException("The green value (" + g + ") must be between [0,255].");
			}
			if (b < 0 || b > 255)
			{
				throw new System.ArgumentException("The blue value (" + b + ") must be between [0,255].");
			}

			Color lastColor = Color;

			bool updateRGBSpinners = adjustingSpinners == 0;

			adjustingSpinners++;
			adjustingColorPanel++;
			int alpha = this.alpha.IntValue;
			try
			{
				if (updateRGBSpinners)
				{
					red.Value = r;
					green.Value = g;
					blue.Value = b;
				}
				preview.setForeground(new Color(r, g, b, alpha));
				float[] hsb = new float[3];
				Color.RGBtoHSB(r, g, b, hsb);
				hue.Value = (int)(hsb[0] * 360f + .49f);
				sat.Value = (int)(hsb[1] * 100f + .49f);
				bri.Value = (int)(hsb[2] * 100f + .49f);
				colorPanel.setRGB(r, g, b);
				updateHexField();
				updateSlider();
			}
			finally
			{
				adjustingSpinners--;
				adjustingColorPanel--;
			}
			Color newColor = Color;
			if (lastColor.Equals(newColor) == false)
			{
				firePropertyChange(SELECTED_COLOR_PROPERTY, lastColor, newColor);
			}
		}


		private void updateSlider()
		{
			adjustingSlider++;
			try
			{
				int mode = Mode;
				if (mode == HUE)
				{
					slider.setValue(hue.IntValue);
				}
				else if (mode == SAT)
				{
					slider.setValue(sat.IntValue);
				}
				else if (mode == BRI)
				{
					slider.setValue(bri.IntValue);
				}
				else if (mode == RED)
				{
					slider.setValue(red.IntValue);
				}
				else if (mode == GREEN)
				{
					slider.setValue(green.IntValue);
				}
				else if (mode == BLUE)
				{
					slider.setValue(blue.IntValue);
				}
			}
			finally
			{
				adjustingSlider--;
			}
			slider.repaint();
		}

		/// <summary>
		/// This returns the panel with several rows of spinner controls.
		/// <P>
		/// Note you can also call methods such as <code>setRGBControlsVisible()</code> to adjust which controls are showing.
		/// <P>
		/// (This returns the panel this <code>ColorPicker</code> uses, so if you put it in another container, it will be
		/// removed from this <code>ColorPicker</code>.)
		/// </summary>
		/// <returns> the panel with several rows of spinner controls. </returns>
		public virtual JPanel ExpertControls
		{
			get
			{
				return expertControls;
			}
		}

		/// <summary>
		/// This shows or hides the RGB spinner controls.
		/// <P>
		/// Note these live inside the "expert controls", so if <code>setExpertControlsVisible(false)</code> has been called,
		/// then calling this method makes no difference: the RGB controls will be hidden.
		/// </summary>
		/// <param name="b"> whether the controls should be visible or not. </param>
		public virtual bool RGBControlsVisible
		{
			set
			{
				red.Visible = value;
				green.Visible = value;
				blue.Visible = value;
			}
		}

		/// <summary>
		/// This shows or hides the HSB spinner controls.
		/// <P>
		/// Note these live inside the "expert controls", so if <code>setExpertControlsVisible(false)</code> has been called,
		/// then calling this method makes no difference: the HSB controls will be hidden.
		/// </summary>
		/// <param name="b"> whether the controls should be visible or not. </param>
		public virtual bool HSBControlsVisible
		{
			set
			{
				hue.Visible = value;
				sat.Visible = value;
				bri.Visible = value;
			}
		}

		/// <summary>
		/// This shows or hides the alpha controls.
		/// <P>
		/// Note the alpha spinner live inside the "expert controls", so if <code>setExpertControlsVisible(false)</code> has
		/// been called, then this method does not affect that spinner. However, the opacity slider is <i>not</i> affected by
		/// the visibility of the export controls.
		/// </summary>
		/// <param name="b"> </param>
		public virtual bool OpacityVisible
		{
			set
			{
				opacityLabel.setVisible(value);
				opacitySlider.setVisible(value);
				alpha.label.setVisible(value);
				alpha.spinner.setVisible(value);
			}
		}

		/// <returns> the <code>ColorPickerPanel</code> this <code>ColorPicker</code> displays. </returns>
		public virtual ColorPickerPanel ColorPanel
		{
			get
			{
				return colorPanel;
			}
		}

		/// <summary>
		/// Sets the current color of this <code>ColorPicker</code>
		/// </summary>
		/// <param name="h"> the hue value. </param>
		/// <param name="s"> the saturation value. Must be between [0,1]. </param>
		/// <param name="b"> the blue value. Must be between [0,1]. </param>
		public virtual void setHSB(float h, float s, float b)
		{
			if (float.IsInfinity(h) || float.IsNaN(h))
			{
				throw new System.ArgumentException("The hue value (" + h + ") is not a valid number.");
			}
			// hue is cyclic, so it can be any value:
			while (h < 0)
			{
				h++;
			}
			while (h > 1)
			{
				h--;
			}

			if (s < 0 || s > 1)
			{
				throw new System.ArgumentException("The saturation value (" + s + ") must be between [0,1]");
			}
			if (b < 0 || b > 1)
			{
				throw new System.ArgumentException("The brightness value (" + b + ") must be between [0,1]");
			}

			Color lastColor = Color;

			bool updateHSBSpinners = adjustingSpinners == 0;
			adjustingSpinners++;
			adjustingColorPanel++;
			try
			{
				if (updateHSBSpinners)
				{
					hue.Value = (int)(h * 360f + .49f);
					sat.Value = (int)(s * 100f + .49f);
					bri.Value = (int)(b * 100f + .49f);
				}

				Color c = new Color(Color.HSBtoRGB(h, s, b));
				int alpha = this.alpha.IntValue;
				c = new Color(c.getRed(), c.getGreen(), c.getBlue(), alpha);
				preview.setForeground(c);
				red.Value = c.getRed();
				green.Value = c.getGreen();
				blue.Value = c.getBlue();
				colorPanel.setHSB(h, s, b);
				updateHexField();
				updateSlider();
				slider.repaint();
			}
			finally
			{
				adjustingSpinners--;
				adjustingColorPanel--;
			}
			Color newColor = Color;
			if (lastColor.Equals(newColor) == false)
			{
				firePropertyChange(SELECTED_COLOR_PROPERTY, lastColor, newColor);
			}
		}

		private void updateHexField()
		{
			adjustingHexField++;
			try
			{
				int r = red.IntValue;
				int g = green.IntValue;
				int b = blue.IntValue;

				int i = (r << 16) + (g << 8) + b;
				string s = Convert.ToString(i, 16).ToUpper();
				while (s.Length < 6)
				{
					s = "0" + s;
				}
				if (hexField.getText().equalsIgnoreCase(s) == false)
				{
					hexField.setText(s);
				}
			}
			finally
			{
				adjustingHexField--;
			}
		}

		internal class Option
		{
			private readonly ColorPicker outerInstance;

			internal JRadioButton radioButton = new JRadioButton();
			internal JSpinner spinner;
			internal JSlider slider;
			internal JLabel label;

			public Option(ColorPicker outerInstance, string text, int max)
			{
				this.outerInstance = outerInstance;
				spinner = new JSpinner(new SpinnerNumberModel(0, 0, max, 5));
				spinner.addChangeListener(outerInstance.changeListener);

				/*
				 * this tries out Tim Boudreaux's new slider UI. It's a good UI, but I think for the ColorPicker the numeric
				 * controls are more useful. That is: users who want click-and-drag control to choose their colors don't
				 * need any of these Option objects at all; only power users who may have specific RGB values in mind will
				 * use these controls: and when they do limiting them to a slider is unnecessary. That's my current
				 * position... of course it may not be true in the real world... :)
				 */
				// slider = new JSlider(0,max);
				// slider.addChangeListener(changeListener);
				// slider.setUI(new org.netbeans.paint.api.components.PopupSliderUI());

				label = new JLabel(text);
				radioButton.addActionListener(outerInstance.actionListener);
			}

			public virtual int Value
			{
				set
				{
					if (slider != null)
					{
						slider.setValue(value);
					}
					if (spinner != null)
					{
						spinner.setValue(value);
					}
				}
			}

			public virtual int Maximum
			{
				get
				{
					if (slider != null)
					{
						return slider.getMaximum();
					}
					return (int)((Number)((SpinnerNumberModel) spinner.getModel()).getMaximum());
				}
			}

			public virtual bool contains(object src)
			{
				return (src == slider || src == spinner || src == radioButton || src == label);
			}

			public virtual float FloatValue
			{
				get
				{
					return IntValue;
				}
			}

			public virtual int IntValue
			{
				get
				{
					if (slider != null)
					{
						return slider.getValue();
					}
					return (int)((Number) spinner.getValue());
				}
			}

			public virtual bool Visible
			{
				get
				{
					return label.isVisible();
				}
				set
				{
					bool radioButtonsAllowed = true;
					bool? z = (bool?) getClientProperty(MODE_CONTROLS_VISIBLE_PROPERTY);
					if (z != null)
					{
						radioButtonsAllowed = z.Value;
					}
    
					radioButton.setVisible(value && radioButtonsAllowed);
					if (slider != null)
					{
						slider.setVisible(value);
					}
					if (spinner != null)
					{
						spinner.setVisible(value);
					}
					label.setVisible(value);
				}
			}

		}
	}

}
