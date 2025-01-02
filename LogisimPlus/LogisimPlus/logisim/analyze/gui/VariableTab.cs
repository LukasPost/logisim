// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{


	using VariableList = logisim.analyze.model.VariableList;
	using VariableListEvent = logisim.analyze.model.VariableListEvent;
	using VariableListListener = logisim.analyze.model.VariableListListener;
	using StringUtil = logisim.util.StringUtil;

	internal class VariableTab : AnalyzerTab, TabInterface
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private class VariableListModel : AbstractListModel<string>, VariableListListener
		{
			internal VariableList list;
			internal string[] listCopy;

			public VariableListModel(VariableList list)
			{
				this.list = list;
				updateCopy();
				list.addVariableListListener(this);
			}

			internal virtual void updateCopy()
			{
				listCopy = list.toArray(new string[list.size()]);
			}

			public virtual int Size
			{
				get
				{
					return listCopy.Length;
				}
			}

			public virtual string getElementAt(int index)
			{
				return index >= 0 && index < listCopy.Length ? listCopy[index] : null;
			}

			internal virtual void update()
			{
				string[] oldCopy = listCopy;
				updateCopy();
				fireContentsChanged(this, 0, oldCopy.Length);
			}

			public virtual void listChanged(VariableListEvent @event)
			{
				string[] oldCopy = listCopy;
				updateCopy();
				int index;
				switch (@event.Type)
				{
				case VariableListEvent.ALL_REPLACED:
					fireContentsChanged(this, 0, oldCopy.Length);
					return;
				case VariableListEvent.ADD:
					index = list.indexOf(@event.Variable);
					fireIntervalAdded(this, index, index);
					return;
				case VariableListEvent.REMOVE:
					index = ((int?) @event.Data).Value;
					fireIntervalRemoved(this, index, index);
					return;
				case VariableListEvent.MOVE:
					fireContentsChanged(this, 0, Size);
					return;
				case VariableListEvent.REPLACE:
					index = ((int?) @event.Data).Value;
					fireContentsChanged(this, index, index);
					return;
				}
			}
		}

		private class MyListener : ActionListener, DocumentListener, ListSelectionListener
		{
			private readonly VariableTab outerInstance;

			public MyListener(VariableTab outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				if ((src == outerInstance.add || src == outerInstance.field) && outerInstance.add.isEnabled())
				{
					string name = outerInstance.field.getText().Trim();
					if (!name.Equals(""))
					{
						outerInstance.data.add(name);
						if (outerInstance.data.contains(name))
						{
							outerInstance.list.setSelectedValue(name, true);
						}
						outerInstance.field.setText("");
						outerInstance.field.grabFocus();
					}
				}
				else if (src == outerInstance.rename)
				{
					string oldName = (string) outerInstance.list.getSelectedValue();
					string newName = outerInstance.field.getText().Trim();
					if (!string.ReferenceEquals(oldName, null) && !newName.Equals(""))
					{
						outerInstance.data.replace(oldName, newName);
						outerInstance.field.setText("");
						outerInstance.field.grabFocus();
					}
				}
				else if (src == outerInstance.remove)
				{
					string name = (string) outerInstance.list.getSelectedValue();
					if (!string.ReferenceEquals(name, null))
					{
						outerInstance.data.remove(name);
					}
				}
				else if (src == outerInstance.moveUp)
				{
					string name = (string) outerInstance.list.getSelectedValue();
					if (!string.ReferenceEquals(name, null))
					{
						outerInstance.data.move(name, -1);
						outerInstance.list.setSelectedValue(name, true);
					}
				}
				else if (src == outerInstance.moveDown)
				{
					string name = (string) outerInstance.list.getSelectedValue();
					if (!string.ReferenceEquals(name, null))
					{
						outerInstance.data.move(name, 1);
						outerInstance.list.setSelectedValue(name, true);
					}
				}
			}

			public virtual void insertUpdate(DocumentEvent @event)
			{
				outerInstance.computeEnabled();
			}

			public virtual void removeUpdate(DocumentEvent @event)
			{
				insertUpdate(@event);
			}

			public virtual void changedUpdate(DocumentEvent @event)
			{
				insertUpdate(@event);
			}

			public virtual void valueChanged(ListSelectionEvent @event)
			{
				outerInstance.computeEnabled();
			}
		}

		private VariableList data;
		private MyListener myListener;

		private JList<string> list = new JList<string>();
		private JTextField field = new JTextField();
		private JButton remove = new JButton();
		private JButton moveUp = new JButton();
		private JButton moveDown = new JButton();
		private JButton add = new JButton();
		private JButton rename = new JButton();
		private JLabel error = new JLabel(" ");

		internal VariableTab(VariableList data)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.data = data;

			list.setModel(new VariableListModel(data));
			list.setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
			list.addListSelectionListener(myListener);
			remove.addActionListener(myListener);
			moveUp.addActionListener(myListener);
			moveDown.addActionListener(myListener);
			add.addActionListener(myListener);
			rename.addActionListener(myListener);
			field.addActionListener(myListener);
			field.getDocument().addDocumentListener(myListener);

			JScrollPane listPane = new JScrollPane(list, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
			listPane.setPreferredSize(new Size(100, 100));

			JPanel topPanel = new JPanel(new GridLayout(3, 1));
			topPanel.add(remove);
			topPanel.add(moveUp);
			topPanel.add(moveDown);

			JPanel fieldPanel = new JPanel();
			fieldPanel.add(rename);
			fieldPanel.add(add);

			GridBagLayout gb = new GridBagLayout();
			GridBagConstraints gc = new GridBagConstraints();
			setLayout(gb);
			Insets oldInsets = gc.insets;

			gc.insets = new Insets(10, 10, 0, 0);
			gc.fill = GridBagConstraints.BOTH;
			gc.weightx = 1.0;
			gb.setConstraints(listPane, gc);
			add(listPane);

			gc.fill = GridBagConstraints.NONE;
			gc.anchor = GridBagConstraints.PAGE_START;
			gc.weightx = 0.0;
			gb.setConstraints(topPanel, gc);
			add(topPanel);

			gc.insets = new Insets(10, 10, 0, 10);
			gc.gridwidth = GridBagConstraints.REMAINDER;
			gc.gridx = 0;
			gc.gridy = GridBagConstraints.RELATIVE;
			gc.fill = GridBagConstraints.HORIZONTAL;
			gb.setConstraints(field, gc);
			add(field);

			gc.insets = oldInsets;
			gc.fill = GridBagConstraints.NONE;
			gc.anchor = GridBagConstraints.LINE_END;
			gb.setConstraints(fieldPanel, gc);
			add(fieldPanel);

			gc.fill = GridBagConstraints.HORIZONTAL;
			gb.setConstraints(error, gc);
			add(error);

			if (!data.Empty)
			{
				list.setSelectedValue(data.get(0), true);
			}
			computeEnabled();
		}

		internal override void localeChanged()
		{
			remove.setText(Strings.get("variableRemoveButton"));
			moveUp.setText(Strings.get("variableMoveUpButton"));
			moveDown.setText(Strings.get("variableMoveDownButton"));
			add.setText(Strings.get("variableAddButton"));
			rename.setText(Strings.get("variableRenameButton"));
			validateInput();
		}

		internal override void updateTab()
		{
			VariableListModel model = (VariableListModel) list.getModel();
			model.update();
		}

		internal virtual void registerDefaultButtons(DefaultRegistry registry)
		{
			registry.registerDefaultButton(field, add);
		}

		private void computeEnabled()
		{
			int index = list.getSelectedIndex();
			int max = list.getModel().getSize();
			bool selected = index >= 0 && index < max;
			remove.setEnabled(selected);
			moveUp.setEnabled(selected && index > 0);
			moveDown.setEnabled(selected && index < max);

			bool ok = validateInput();
			add.setEnabled(ok && data.size() < data.MaximumSize);
			rename.setEnabled(ok && selected);
		}

		private bool validateInput()
		{
			string text = field.getText().Trim();
			bool ok = true;
			bool errorShown = true;
			if (text.Length == 0)
			{
				errorShown = false;
				ok = false;
			}
			else if (!Character.isJavaIdentifierStart(text[0]))
			{
				error.setText(Strings.get("variableStartError"));
				ok = false;
			}
			else
			{
				for (int i = 1; i < text.Length && ok; i++)
				{
					char c = text[i];
					if (!Character.isJavaIdentifierPart(c))
					{
						error.setText(StringUtil.format(Strings.get("variablePartError"), "" + c));
						ok = false;
					}
				}
			}
			if (ok)
			{
				for (int i = 0, n = data.size(); i < n && ok; i++)
				{
					string other = data.get(i);
					if (text.Equals(other))
					{
						error.setText(Strings.get("variableDuplicateError"));
						ok = false;
					}
				}
			}
			if (ok || !errorShown)
			{
				if (data.size() >= data.MaximumSize)
				{
					error.setText(StringUtil.format(Strings.get("variableMaximumError"), "" + data.MaximumSize));
				}
				else
				{
					error.setText(" ");
				}
			}
			return ok;
		}

		public virtual void copy()
		{
			field.requestFocus();
			field.copy();
		}

		public virtual void paste()
		{
			field.requestFocus();
			field.paste();
		}

		public virtual void delete()
		{
			field.requestFocus();
			field.replaceSelection("");
		}

		public virtual void selectAll()
		{
			field.requestFocus();
			field.selectAll();
		}
	}

}
