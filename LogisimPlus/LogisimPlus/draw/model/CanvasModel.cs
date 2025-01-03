// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.model
{

	using Selection = draw.canvas.Selection;
	using Text = draw.shapes.Text;
	using Bounds = logisim.data.Bounds;

	public interface CanvasModel
	{
		// listener methods
		void addCanvasModelListener(CanvasModelListener l);

		void removeCanvasModelListener(CanvasModelListener l);

		// methods that don't change any data in the model
		void paint(JGraphics g, Selection selection);

		List<CanvasObject> ObjectsFromTop {get;}

		List<CanvasObject> ObjectsFromBottom {get;}

		ICollection<CanvasObject> getObjectsIn(Bounds bds);

		ICollection<CanvasObject> getObjectsOverlapping(CanvasObject shape);

		// methods that alter the model
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public void addObjects(int index, java.util.Collection<? extends CanvasObject> shapes);
		void addObjects<T1>(int index, ICollection<T1> shapes);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public void addObjects(java.util.Map<? extends CanvasObject, int> shapes);
		void addObjects<T1>(Dictionary<T1> shapes);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public void removeObjects(java.util.Collection<? extends CanvasObject> shapes);
		void removeObjects<T1>(ICollection<T1> shapes);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public void translateObjects(java.util.Collection<? extends CanvasObject> shapes, int dx, int dy);
		void translateObjects<T1>(ICollection<T1> shapes, int dx, int dy);

		void reorderObjects(List<ReorderRequest> requests);

		Handle moveHandle(HandleGesture gesture);

		void insertHandle(Handle desired, Handle previous);

		Handle deleteHandle(Handle handle);

		Dictionary<AttributeMapKey, object> AttributeValues {set;}

		void setText(Text text, string value);
	}

}
