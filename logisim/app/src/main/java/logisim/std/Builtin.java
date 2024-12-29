/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std;

import java.util.Arrays;
import java.util.Collections;
import java.util.List;

import logisim.std.arith.Arithmetic;
import logisim.std.base.Base;
import logisim.std.gates.Gates;
import logisim.std.io.Io;
import logisim.std.memory.Memory;
import logisim.std.plexers.Plexers;
import logisim.std.wiring.Wiring;
import logisim.tools.Library;
import logisim.tools.Tool;

public class Builtin extends Library {
	private List<Library> libraries = null;

	public Builtin() {
		libraries = Arrays.asList(new Library[] {
			new Base(),
			new Gates(),
			new Wiring(),
			new Plexers(),
			new Arithmetic(),
			new Memory(),
			new Io(),
		});
	}

	@Override
	public String getName() { return "Builtin"; }

	@Override
	public String getDisplayName() { return Strings.get("builtinLibrary"); }

	@Override
	public List<Tool> getTools() { return Collections.emptyList(); }
	
	@Override
	public List<Library> getLibraries() {
		return libraries;
	}
}
