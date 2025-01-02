// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.IO;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.prefs
{


	using Loader = logisim.file.Loader;
	using LoaderException = logisim.file.LoaderException;
	using LogisimFile = logisim.file.LogisimFile;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Template = logisim.prefs.Template;
	using JFileChoosers = logisim.util.JFileChoosers;
	using StringUtil = logisim.util.StringUtil;

	internal class TemplateOptions : OptionsPanel
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private class MyListener : ActionListener, PropertyChangeListener
		{
			private readonly TemplateOptions outerInstance;

			public MyListener(TemplateOptions outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				if (src == outerInstance.templateButton)
				{
					JFileChooser chooser = JFileChoosers.create();
					chooser.setDialogTitle(Strings.get("selectDialogTitle"));
					chooser.setApproveButtonText(Strings.get("selectDialogButton"));
					int action = chooser.showOpenDialog(outerInstance.PreferencesFrame);
					if (action == JFileChooser.APPROVE_OPTION)
					{
						File file = chooser.getSelectedFile();
						FileStream reader = null;
						Stream reader2 = null;
						try
						{
							Loader loader = new Loader(outerInstance.PreferencesFrame);
							reader = new FileStream(file, FileMode.Open, FileAccess.Read);
							Template template = Template.create(reader);
							reader2 = template.createStream();
							LogisimFile.load(reader2, loader); // to see if OK
							AppPreferences.setTemplateFile(file, template);
							AppPreferences.TemplateType = AppPreferences.TEMPLATE_CUSTOM;
						}
						catch (LoaderException)
						{
						}
						catch (IOException ex)
						{
							JOptionPane.showMessageDialog(outerInstance.PreferencesFrame, StringUtil.format(Strings.get("templateErrorMessage"), ex.ToString()), Strings.get("templateErrorTitle"), JOptionPane.ERROR_MESSAGE);
						}
						finally
						{
							try
							{
								if (reader != null)
								{
									reader.Close();
								}
							}
							catch (IOException)
							{
							}
							try
							{
								if (reader2 != null)
								{
									reader2.Close();
								}
							}
							catch (IOException)
							{
							}
						}
					}
				}
				else
				{
					int value = AppPreferences.TEMPLATE_UNKNOWN;
					if (outerInstance.plain.isSelected())
					{
						value = AppPreferences.TEMPLATE_PLAIN;
					}
					else if (outerInstance.empty.isSelected())
					{
						value = AppPreferences.TEMPLATE_EMPTY;
					}
					else if (outerInstance.custom.isSelected())
					{
						value = AppPreferences.TEMPLATE_CUSTOM;
					}
					AppPreferences.TemplateType = value;
				}
				computeEnabled();
			}

			public virtual void propertyChange(PropertyChangeEvent @event)
			{
				string prop = @event.getPropertyName();
				if (prop.Equals(AppPreferences.TEMPLATE_TYPE))
				{
					int value = AppPreferences.TemplateType;
					outerInstance.plain.setSelected(value == AppPreferences.TEMPLATE_PLAIN);
					outerInstance.empty.setSelected(value == AppPreferences.TEMPLATE_EMPTY);
					outerInstance.custom.setSelected(value == AppPreferences.TEMPLATE_CUSTOM);
				}
				else if (prop.Equals(AppPreferences.TEMPLATE_FILE))
				{
					TemplateField = (File) @event.getNewValue();
				}
			}

			internal virtual File TemplateField
			{
				set
				{
					try
					{
						outerInstance.templateField.setText(value == null ? "" : value.getCanonicalPath());
					}
					catch (IOException)
					{
						outerInstance.templateField.setText(value.getName());
					}
					computeEnabled();
				}
			}

			internal virtual void computeEnabled()
			{
				outerInstance.custom.setEnabled(!outerInstance.templateField.getText().Equals(""));
				outerInstance.templateField.setEnabled(outerInstance.custom.isSelected());
			}
		}

		private MyListener myListener;

		private JRadioButton plain = new JRadioButton();
		private JRadioButton empty = new JRadioButton();
		private JRadioButton custom = new JRadioButton();
		private JTextField templateField = new JTextField(40);
		private JButton templateButton = new JButton();

		public TemplateOptions(PreferencesFrame window) : base(window)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}

			ButtonGroup bgroup = new ButtonGroup();
			bgroup.add(plain);
			bgroup.add(empty);
			bgroup.add(custom);

			plain.addActionListener(myListener);
			empty.addActionListener(myListener);
			custom.addActionListener(myListener);
			templateField.setEditable(false);
			templateButton.addActionListener(myListener);
			myListener.computeEnabled();

			GridBagLayout gridbag = new GridBagLayout();
			GridBagConstraints gbc = new GridBagConstraints();
			setLayout(gridbag);
			gbc.weightx = 1.0;
			gbc.gridx = 0;
			gbc.gridy = GridBagConstraints.RELATIVE;
			gbc.gridwidth = 3;
			gbc.anchor = GridBagConstraints.LINE_START;
			gridbag.setConstraints(plain, gbc);
			add(plain);
			gridbag.setConstraints(empty, gbc);
			add(empty);
			gridbag.setConstraints(custom, gbc);
			add(custom);
			gbc.fill = GridBagConstraints.HORIZONTAL;
			gbc.gridwidth = 1;
			gbc.gridy = 3;
			gbc.gridx = GridBagConstraints.RELATIVE;
			JPanel strut = new JPanel();
			strut.setMinimumSize(new Size(50, 1));
			strut.setPreferredSize(new Size(50, 1));
			gbc.weightx = 0.0;
			gridbag.setConstraints(strut, gbc);
			add(strut);
			gbc.weightx = 1.0;
			gridbag.setConstraints(templateField, gbc);
			add(templateField);
			gbc.weightx = 0.0;
			gridbag.setConstraints(templateButton, gbc);
			add(templateButton);

			AppPreferences.addPropertyChangeListener(AppPreferences.TEMPLATE_TYPE, myListener);
			AppPreferences.addPropertyChangeListener(AppPreferences.TEMPLATE_FILE, myListener);
			switch (AppPreferences.TemplateType)
			{
			case AppPreferences.TEMPLATE_PLAIN:
				plain.setSelected(true);
				break;
			case AppPreferences.TEMPLATE_EMPTY:
				empty.setSelected(true);
				break;
			case AppPreferences.TEMPLATE_CUSTOM:
				custom.setSelected(true);
				break;
			}
			myListener.TemplateField = AppPreferences.TemplateFile;
		}

		public override string Title
		{
			get
			{
				return Strings.get("templateTitle");
			}
		}

		public override string HelpText
		{
			get
			{
				return Strings.get("templateHelp");
			}
		}

		public override void localeChanged()
		{
			plain.setText(Strings.get("templatePlainOption"));
			empty.setText(Strings.get("templateEmptyOption"));
			custom.setText(Strings.get("templateCustomOption"));
			templateButton.setText(Strings.get("templateSelectButton"));
		}
	}

}
