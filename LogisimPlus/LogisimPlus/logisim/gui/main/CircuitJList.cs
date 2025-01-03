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
	using Bounds = logisim.data.Bounds;
	using LogisimFile = logisim.file.LogisimFile;
	using Project = logisim.proj.Project;

	internal class CircuitJList : JList<Circuit>
	{
		public CircuitJList(Project proj, bool includeEmpty)
		{
			LogisimFile file = proj.LogisimFile;
			Circuit current = proj.CurrentCircuit;
			List<Circuit> options = new List<Circuit>();
			bool currentFound = false;
			foreach (Circuit circ in file.Circuits)
			{
				if (!includeEmpty || circ.Bounds != Bounds.EMPTY_BOUNDS)
				{
					if (circ == current)
					{
						currentFound = true;
					}
					options.Add(circ);
				}
			}

			setListData(options);
			if (currentFound)
			{
				setSelectedValue(current, true);
			}
			setVisibleRowCount(Math.Min(6, options.Count));
		}

		public virtual List<Circuit> SelectedCircuits
		{
			get
			{
				List<Circuit> selected = getSelectedValuesList();
				if (selected != null)
				{
					return selected;
				}
				else
				{
					return [];
				}
			}
		}

	}

}
