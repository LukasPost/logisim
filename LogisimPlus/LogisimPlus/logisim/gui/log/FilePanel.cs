// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.IO;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{


	using Value = logisim.data.Value;
	using JFileChoosers = logisim.util.JFileChoosers;
	using StringUtil = logisim.util.StringUtil;

	internal class FilePanel : LogPanel
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			listener = new Listener(this);
		}

		private class Listener : ActionListener, ModelListener
		{
			private readonly FilePanel outerInstance;

			public Listener(FilePanel outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void selectionChanged(ModelEvent @event)
			{
			}

			public virtual void entryAdded(ModelEvent @event, Value[] values)
			{
			}

			public virtual void filePropertyChanged(ModelEvent @event)
			{
				Model model = outerInstance.Model;
				computeEnableItems(model);

				File file = model.File;
				outerInstance.fileField.setText(file == null ? "" : file.getPath());
				outerInstance.enableButton.setEnabled(file != null);

				outerInstance.headerCheckBox.setSelected(model.FileHeader);
			}

			internal virtual void computeEnableItems(Model model)
			{
				if (model.FileEnabled)
				{
					outerInstance.enableLabel.setText(Strings.get("fileEnabled"));
					outerInstance.enableButton.setText(Strings.get("fileDisableButton"));
				}
				else
				{
					outerInstance.enableLabel.setText(Strings.get("fileDisabled"));
					outerInstance.enableButton.setText(Strings.get("fileEnableButton"));
				}
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				if (src == outerInstance.enableButton)
				{
					outerInstance.Model.FileEnabled = !outerInstance.Model.FileEnabled;
				}
				else if (src == outerInstance.selectButton)
				{
					int result = outerInstance.chooser.showSaveDialog(outerInstance.LogFrame);
					if (result != JFileChooser.APPROVE_OPTION)
					{
						return;
					}
					File file = outerInstance.chooser.getSelectedFile();
					if (file.exists() && (!file.canWrite() || file.isDirectory()))
					{
						JOptionPane.showMessageDialog(outerInstance.LogFrame, StringUtil.format(Strings.get("fileCannotWriteMessage"), file.getName()), Strings.get("fileCannotWriteTitle"), JOptionPane.OK_OPTION);
						return;
					}
					if (file.exists() && file.length() > 0)
					{
						string[] options = new string[] {Strings.get("fileOverwriteOption"), Strings.get("fileAppendOption"), Strings.get("fileCancelOption")};
						int option = JOptionPane.showOptionDialog(outerInstance.LogFrame, StringUtil.format(Strings.get("fileExistsMessage"), file.getName()), Strings.get("fileExistsTitle"), 0, JOptionPane.QUESTION_MESSAGE, null, options, options[0]);
						if (option == 0)
						{
							try
							{
								StreamWriter delete = new StreamWriter(file);
								delete.Close();
							}
							catch (IOException)
							{
							}
						}
						else if (option == 1)
						{
							// do nothing
						}
						else
						{
							return;
						}
					}
					outerInstance.Model.File = file;
				}
				else if (src == outerInstance.headerCheckBox)
				{
					outerInstance.Model.FileHeader = outerInstance.headerCheckBox.isSelected();
				}
			}
		}

		private Listener listener;
		private JLabel enableLabel = new JLabel();
		private JButton enableButton = new JButton();
		private JLabel fileLabel = new JLabel();
		private JTextField fileField = new JTextField();
		private JButton selectButton = new JButton();
		private JCheckBox headerCheckBox = new JCheckBox();
		private JFileChooser chooser = JFileChoosers.create();

		public FilePanel(LogFrame frame) : base(frame)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}

			JPanel filePanel = new JPanel(new GridBagLayout());
			GridBagLayout gb = (GridBagLayout) filePanel.getLayout();
			GridBagConstraints gc = new GridBagConstraints();
			gc.fill = GridBagConstraints.HORIZONTAL;
			gb.setConstraints(fileLabel, gc);
			filePanel.add(fileLabel);
			gc.weightx = 1.0;
			gb.setConstraints(fileField, gc);
			filePanel.add(fileField);
			gc.weightx = 0.0;
			gb.setConstraints(selectButton, gc);
			filePanel.add(selectButton);
			fileField.setEditable(false);
			fileField.setEnabled(false);

			setLayout(new GridBagLayout());
			gb = (GridBagLayout) getLayout();
			gc = new GridBagConstraints();
			gc.gridx = 0;
			gc.weightx = 1.0;
			gc.gridy = GridBagConstraints.RELATIVE;
			JComponent glue;
			glue = new JPanel();
			gc.weighty = 1.0;
			gb.setConstraints(glue, gc);
			add(glue);
			gc.weighty = 0.0;
			gb.setConstraints(enableLabel, gc);
			add(enableLabel);
			gb.setConstraints(enableButton, gc);
			add(enableButton);
			glue = new JPanel();
			gc.weighty = 1.0;
			gb.setConstraints(glue, gc);
			add(glue);
			gc.weighty = 0.0;
			gc.fill = GridBagConstraints.HORIZONTAL;
			gb.setConstraints(filePanel, gc);
			add(filePanel);
			gc.fill = GridBagConstraints.NONE;
			glue = new JPanel();
			gc.weighty = 1.0;
			gb.setConstraints(glue, gc);
			add(glue);
			gc.weighty = 0.0;
			gb.setConstraints(headerCheckBox, gc);
			add(headerCheckBox);
			glue = new JPanel();
			gc.weighty = 1.0;
			gb.setConstraints(glue, gc);
			add(glue);
			gc.weighty = 0.0;

			enableButton.addActionListener(listener);
			selectButton.addActionListener(listener);
			headerCheckBox.addActionListener(listener);
			modelChanged(null, Model);
			localeChanged();
		}

		public override string Title
		{
			get
			{
				return Strings.get("fileTab");
			}
		}

		public override string HelpText
		{
			get
			{
				return Strings.get("fileHelp");
			}
		}

		public override void localeChanged()
		{
			listener.computeEnableItems(Model);
			fileLabel.setText(Strings.get("fileLabel") + " ");
			selectButton.setText(Strings.get("fileSelectButton"));
			headerCheckBox.setText(Strings.get("fileHeaderCheck"));
		}

		public override void modelChanged(Model oldModel, Model newModel)
		{
			if (oldModel != null)
			{
				oldModel.removeModelListener(listener);
			}
			if (newModel != null)
			{
				newModel.addModelListener(listener);
				listener.filePropertyChanged(null);
			}
		}

	}

}
