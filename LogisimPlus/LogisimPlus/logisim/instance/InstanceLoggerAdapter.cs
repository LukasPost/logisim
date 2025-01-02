// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{
	using CircuitState = logisim.circuit.CircuitState;
	using Value = logisim.data.Value;
	using Loggable = logisim.gui.log.Loggable;

	internal class InstanceLoggerAdapter : Loggable
	{
		private InstanceComponent comp;
		private InstanceLogger logger;
		private InstanceStateImpl state;

		public InstanceLoggerAdapter(InstanceComponent comp, Type loggerClass)
		{
			try
			{
				this.comp = comp;
				this.logger = loggerClass.GetConstructor().newInstance();
				this.state = new InstanceStateImpl(null, comp);
			}
			catch (Exception t)
			{
				handleError(t, loggerClass);
				logger = null;
			}
		}

		private void handleError(Exception t, Type loggerClass)
		{
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = loggerClass.FullName;
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Console.Error.WriteLine("error while instantiating logger " + className + ": " + t.GetType().FullName);
			string msg = t.Message;
			if (!string.ReferenceEquals(msg, null))
			{
				Console.Error.WriteLine("  (" + msg + ")"); // OK
			}
		}

		public virtual object[] getLogOptions(CircuitState circState)
		{
			if (logger != null)
			{
				updateState(circState);
				return logger.getLogOptions(state);
			}
			else
			{
				return null;
			}
		}

		public virtual string getLogName(object option)
		{
			if (logger != null)
			{
				return logger.getLogName(state, option);
			}
			else
			{
				return null;
			}
		}

		public virtual Value getLogValue(CircuitState circuitState, object option)
		{
			if (logger != null)
			{
				updateState(circuitState);
				return logger.getLogValue(state, option);
			}
			else
			{
				return Value.UNKNOWN;
			}
		}

		private void updateState(CircuitState circuitState)
		{
			if (state.CircuitState != circuitState)
			{
				state.repurpose(circuitState, comp);
			}
		}
	}

}
