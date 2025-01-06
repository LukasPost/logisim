/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.base;

import java.util.Arrays;
import java.util.List;

import logisim.tools.Library;
import logisim.tools.MenuTool;
import logisim.tools.PokeTool;
import logisim.tools.SelectTool;
import logisim.tools.TextTool;
import logisim.tools.AddTool;
import logisim.tools.EditTool;
import logisim.tools.Tool;
import logisim.tools.WiringTool;

public class Base extends Library {
	private List<Tool> tools;

	public Base() {
		SelectTool select = new SelectTool();
		WiringTool wiring = new WiringTool();

		tools = Arrays.asList(new PokeTool(), new EditTool(select, wiring), select, wiring, new TextTool(),
				new MenuTool(), new AddTool(Text.FACTORY));
	}

	@Override
	public String getName() {
		return "Base";
	}

	@Override
	public String getDisplayName() {
		return Strings.get("baseLibrary");
	}

	@Override
	public List<Tool> getTools() {
		return tools;
	}
}
