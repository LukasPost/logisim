// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{


	using AnalyzerModel = logisim.analyze.model.AnalyzerModel;
	using VariableList = logisim.analyze.model.VariableList;
	using VariableListEvent = logisim.analyze.model.VariableListEvent;
	using VariableListListener = logisim.analyze.model.VariableListListener;

	internal class OutputSelector
	{
		private class Model : AbstractListModel<string>, ComboBoxModel<string>, VariableListListener
		{
			private readonly OutputSelector outerInstance;

			public Model(OutputSelector outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal string selected;

			public virtual object SelectedItem
			{
				set
				{
					selected = (string)value;
				}
				get
				{
					return selected;
				}
			}


			public virtual int Size
			{
				get
				{
					return outerInstance.source.size();
				}
			}

			public virtual string getElementAt(int index)
			{
				return outerInstance.source.get(index);
			}

			public virtual void listChanged(VariableListEvent @event)
			{
				int index;
				string variable;
				object selection;
				switch (@event.Type)
				{
				case VariableListEvent.ALL_REPLACED:
					outerInstance.computePrototypeValue();
					fireContentsChanged(this, 0, Size);
					if (outerInstance.source.Empty)
					{
						outerInstance.select.setSelectedItem(null);
					}
					else
					{
						outerInstance.select.setSelectedItem(outerInstance.source.get(0));
					}
					break;
				case VariableListEvent.ADD:
					variable = @event.Variable;
					if (string.ReferenceEquals(outerInstance.prototypeValue, null) || variable.Length > outerInstance.prototypeValue.Length)
					{
						outerInstance.computePrototypeValue();
					}

					index = outerInstance.source.indexOf(variable);
					fireIntervalAdded(this, index, index);
					if (outerInstance.select.getSelectedItem() == null)
					{
						outerInstance.select.setSelectedItem(variable);
					}
					break;
				case VariableListEvent.REMOVE:
					variable = @event.Variable;
					if (variable.Equals(outerInstance.prototypeValue))
					{
						outerInstance.computePrototypeValue();
					}
					index = ((int?) @event.Data).Value;
					fireIntervalRemoved(this, index, index);
					selection = outerInstance.select.getSelectedItem();
					if (selection != null && selection.Equals(variable))
					{
						selection = outerInstance.source.Empty ? null : outerInstance.source.get(0);
						outerInstance.select.setSelectedItem(selection);
					}
					break;
				case VariableListEvent.MOVE:
					fireContentsChanged(this, 0, Size);
					break;
				case VariableListEvent.REPLACE:
					variable = @event.Variable;
					if (variable.Equals(outerInstance.prototypeValue))
					{
						outerInstance.computePrototypeValue();
					}
					index = ((int?) @event.Data).Value;
					fireContentsChanged(this, index, index);
					selection = outerInstance.select.getSelectedItem();
					if (selection != null && selection.Equals(variable))
					{
						outerInstance.select.setSelectedItem(@event.Source.get(index));
					}
					break;
				}
			}
		}

		private VariableList source;
		private JLabel label = new JLabel();
		private JComboBox<string> select = new JComboBox<string>();
		private string prototypeValue = null;

		public OutputSelector(AnalyzerModel model)
		{
			this.source = model.Outputs;

			Model listModel = new Model(this);
			select.setModel(listModel);
			source.addVariableListListener(listModel);
		}

		public virtual JPanel createPanel()
		{
			JPanel ret = new JPanel();
			ret.add(label);
			ret.add(select);
			return ret;
		}

		public virtual JLabel Label
		{
			get
			{
				return label;
			}
		}

		public virtual JComboBox<string> ComboBox
		{
			get
			{
				return select;
			}
		}

		internal virtual void localeChanged()
		{
			label.setText(Strings.get("outputSelectLabel"));
		}

		public virtual void addItemListener(ItemListener l)
		{
			select.addItemListener(l);
		}

		public virtual void removeItemListener(ItemListener l)
		{
			select.removeItemListener(l);
		}

		public virtual string SelectedOutput
		{
			get
			{
				string value = (string) select.getSelectedItem();
				if (!string.ReferenceEquals(value, null) && !source.contains(value))
				{
					if (source.Empty)
					{
						value = null;
					}
					else
					{
						value = source.get(0);
					}
					select.setSelectedItem(value);
				}
				return value;
			}
		}

		private void computePrototypeValue()
		{
			string newValue;
			if (source.Empty)
			{
				newValue = "xx";
			}
			else
			{
				newValue = "xx";
				for (int i = 0, n = source.size(); i < n; i++)
				{
					string candidate = source.get(i);
					if (candidate.Length > newValue.Length)
					{
						newValue = candidate;
					}
				}
			}
			if (string.ReferenceEquals(prototypeValue, null) || newValue.Length != prototypeValue.Length)
			{
				prototypeValue = newValue;
				select.setPrototypeDisplayValue(prototypeValue + "xx");
				select.revalidate();
			}
		}
	}

}
