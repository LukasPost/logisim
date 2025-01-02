// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 2011, Carl Burch.
 * 
 * This file is part of the Logisim source code. The latest
 * version is available at http://www.cburch.com/logisim/.
 *
 * Logisim is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * Logisim is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Logisim; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA
 */

namespace logisim
{
	using Startup = logisim.gui.start.Startup;

	public class Main
	{
		public static readonly LogisimVersion VERSION = LogisimVersion.get(2, 7, 1);
		public static readonly string VERSION_NAME = VERSION.ToString();
		public const int COPYRIGHT_YEAR = 2011;

		public static void Main(string[] args)
		{
			Startup startup = Startup.parseArgs(args);
			if (startup == null)
			{
				Environment.Exit(0);
			}
			else
			{
				startup.run();
			}
		}
	}

}
