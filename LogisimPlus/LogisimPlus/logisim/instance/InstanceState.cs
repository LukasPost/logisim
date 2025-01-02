// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Value = logisim.data.Value;
	using Project = logisim.proj.Project;

	public interface InstanceState
	{
		Instance Instance {get;}

		InstanceFactory Factory {get;}

		Project Project {get;}

		AttributeSet AttributeSet {get;}

		E getAttributeValue<E>(Attribute<E> attr);

		Value getPort(int portIndex);

		bool isPortConnected(int portIndex);

		void setPort(int portIndex, Value value, int delay);

		InstanceData Data {get;set;}


		void fireInvalidated();

		bool CircuitRoot {get;}

		long TickCount {get;}
	}

}
