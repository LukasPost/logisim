// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using Project = logisim.proj.Project;
	using StringGetter = logisim.util.StringGetter;

	internal class SplitterDistributeItem : JMenuItem, ActionListener
	{
		private Project proj;
		private Splitter splitter;
		private int order;

		public SplitterDistributeItem(Project proj, Splitter splitter, int order)
		{
			this.proj = proj;
			this.splitter = splitter;
			this.order = order;
			addActionListener(this);

			SplitterAttributes attrs = (SplitterAttributes) splitter.AttributeSet;
			sbyte[] actual = attrs.bit_end;
			sbyte[] desired = SplitterAttributes.computeDistribution(attrs.fanout, actual.Length, order);
			bool same = actual.Length == desired.Length;
			for (int i = 0; same && i < desired.Length; i++)
			{
				if (actual[i] != desired[i])
				{
					same = false;
				}
			}
			setEnabled(!same);
			setText(toGetter().get());
		}

		private StringGetter toGetter()
		{
			if (order > 0)
			{
				return Strings.getter("splitterDistributeAscending");
			}
			else
			{
				return Strings.getter("splitterDistributeDescending");
			}
		}

		public virtual void actionPerformed(ActionEvent e)
		{
			SplitterAttributes attrs = (SplitterAttributes) splitter.AttributeSet;
			sbyte[] actual = attrs.bit_end;
			sbyte[] desired = SplitterAttributes.computeDistribution(attrs.fanout, actual.Length, order);
			CircuitMutation xn = new CircuitMutation(proj.CircuitState.Circuit);
			for (int i = 0, n = Math.Min(actual.Length, desired.Length); i < n; i++)
			{
				if (actual[i] != desired[i])
				{
					xn.set(splitter, attrs.getBitOutAttribute(i), Convert.ToInt32(desired[i]));
				}
			}
			proj.doAction(xn.toAction(toGetter()));
		}
	}

}
