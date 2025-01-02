// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{
	using Component = logisim.comp.Component;
	using logisim.data;

	public interface CircuitMutator
	{
		void clear(Circuit circuit);

		void add(Circuit circuit, Component comp);

		void remove(Circuit circuit, Component comp);

		void replace(Circuit circuit, Component oldComponent, Component newComponent);

		void replace(Circuit circuit, ReplacementMap replacements);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public void set(Circuit circuit, logisim.comp.Component comp, logisim.data.Attribute<?> attr, Object value);
		void set<T1>(Circuit circuit, Component comp, Attribute<T1> attr, object value);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public void setForCircuit(Circuit circuit, logisim.data.Attribute<?> attr, Object value);
		void setForCircuit<T1>(Circuit circuit, Attribute<T1> attr, object value);
	}

}
