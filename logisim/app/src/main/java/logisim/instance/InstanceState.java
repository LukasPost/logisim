/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.instance;

import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.Value;
import logisim.proj.Project;

public interface InstanceState {
	Instance getInstance();

	InstanceFactory getFactory();

	Project getProject();

	AttributeSet getAttributeSet();

	<E> E getAttributeValue(Attribute<E> attr);

	Value getPort(int portIndex);

	boolean isPortConnected(int portIndex);

	void setPort(int portIndex, Value value, int delay);

	InstanceData getData();

	void setData(InstanceData value);

	void fireInvalidated();

	boolean isCircuitRoot();

	long getTickCount();
}
