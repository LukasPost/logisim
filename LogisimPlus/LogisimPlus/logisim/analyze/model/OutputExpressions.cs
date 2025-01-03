// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{

	public class OutputExpressions
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private class OutputData
		{
			private readonly OutputExpressions outerInstance;

			internal string output;
			internal int format;
			internal Expression expr = null;
			internal string exprString = null;
			internal List<Implicant> minimalImplicants = null;
			internal Expression minimalExpr = null;

			internal OutputData(OutputExpressions outerInstance, string output)
			{
				this.outerInstance = outerInstance;
				this.output = output;
				invalidate(true, false);
			}

			internal virtual bool ExpressionMinimal
			{
				get
				{
					return expr == minimalExpr;
				}
			}

			internal virtual Expression Expression
			{
				get
				{
					return expr;
				}
			}

			internal virtual string ExpressionString
			{
				get
				{
					if (string.ReferenceEquals(exprString, null))
					{
						if (expr == null)
						{
							invalidate(false, false);
						}
						exprString = expr == null ? "" : expr.ToString();
					}
					return exprString;
				}
			}

			internal virtual Expression MinimalExpression
			{
				get
				{
					if (minimalExpr == null)
					{
						invalidate(false, false);
					}
					return minimalExpr;
				}
			}

			internal virtual List<Implicant> MinimalImplicants
			{
				get
				{
					return minimalImplicants;
				}
			}

			internal virtual int MinimizedFormat
			{
				get
				{
					return format;
				}
				set
				{
					if (format != value)
					{
						format = value;
						this.invalidate(false, true);
					}
				}
			}


			internal virtual void setExpression(Expression newExpr, string newExprString)
			{
				expr = newExpr;
				exprString = newExprString;

				if (expr != minimalExpr)
				{ // for efficiency to avoid recomputation
					Entry[] values = computeColumn(outerInstance.model.TruthTable, expr);
					int outputColumn = outerInstance.model.Outputs.indexOf(output);
					outerInstance.updatingTable = true;
					try
					{
						outerInstance.model.TruthTable.setOutputColumn(outputColumn, values);
					}
					finally
					{
						outerInstance.updatingTable = false;
					}
				}

				outerInstance.fireModelChanged(OutputExpressionsEvent.OUTPUT_EXPRESSION, output, Expression);
			}

			internal virtual void removeInput(string input)
			{
				Expression oldMinExpr = minimalExpr;
				minimalImplicants = null;
				minimalExpr = null;

				if (!string.ReferenceEquals(exprString, null))
				{
					exprString = null; // invalidate it so it recomputes
				}
				if (expr != null)
				{
					Expression oldExpr = expr;
					Expression newExpr;
					if (oldExpr == oldMinExpr)
					{
						newExpr = MinimalExpression;
						expr = newExpr;
					}
					else
					{
						newExpr = expr.removeVariable(input);
					}
					if (newExpr == null || !newExpr.Equals(oldExpr))
					{
						expr = newExpr;
						outerInstance.fireModelChanged(OutputExpressionsEvent.OUTPUT_EXPRESSION, output, expr);
					}
				}
				outerInstance.fireModelChanged(OutputExpressionsEvent.OUTPUT_MINIMAL, output, minimalExpr);
			}

			internal virtual void replaceInput(string input, string newName)
			{
				minimalExpr = null;

				if (!string.ReferenceEquals(exprString, null))
				{
					exprString = Parser.replaceVariable(exprString, input, newName);
				}
				if (expr != null)
				{
					Expression newExpr = expr.replaceVariable(input, newName);
					if (!newExpr.Equals(expr))
					{
						expr = newExpr;
						outerInstance.fireModelChanged(OutputExpressionsEvent.OUTPUT_EXPRESSION, output);
					}
				}
				else
				{
					outerInstance.fireModelChanged(OutputExpressionsEvent.OUTPUT_EXPRESSION, output);
				}
				outerInstance.fireModelChanged(OutputExpressionsEvent.OUTPUT_MINIMAL, output);
			}

			internal bool invalidating = false;

			internal virtual void invalidate(bool initializing, bool formatChanged)
			{
				if (invalidating)
				{
					return;
				}
				invalidating = true;
				try
				{
					List<Implicant> oldImplicants = minimalImplicants;
					Expression oldMinExpr = minimalExpr;
					minimalImplicants = Implicant.computeMinimal(format, outerInstance.model, output);
					minimalExpr = Implicant.toExpression(format, outerInstance.model, minimalImplicants);
					bool minChanged = !implicantsSame(oldImplicants, minimalImplicants);

					if (!outerInstance.updatingTable)
					{
						// see whether the expression is still consistent with the truth table
						TruthTable table = outerInstance.model.TruthTable;
						Entry[] outputColumn = computeColumn(outerInstance.model.TruthTable, expr);
						int outputIndex = outerInstance.model.Outputs.indexOf(output);

						Entry[] currentColumn = table.getOutputColumn(outputIndex);
						if (!columnsMatch(currentColumn, outputColumn) || isAllUndefined(outputColumn) || formatChanged)
						{
							// if not, then we need to change the expression to maintain consistency
							bool exprChanged = expr != oldMinExpr || minChanged;
							expr = minimalExpr;
							if (exprChanged)
							{
								exprString = null;
								if (!initializing)
								{
									outerInstance.fireModelChanged(OutputExpressionsEvent.OUTPUT_EXPRESSION, output);
								}
							}
						}
					}

					if (!initializing && minChanged)
					{
						outerInstance.fireModelChanged(OutputExpressionsEvent.OUTPUT_MINIMAL, output);
					}
				}
				finally
				{
					invalidating = false;
				}
			}
		}

		private class MyListener : VariableListListener, TruthTableListener
		{
			private readonly OutputExpressions outerInstance;

			public MyListener(OutputExpressions outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void listChanged(VariableListEvent @event)
			{
				if (@event.Source == outerInstance.model.Inputs)
				{
					inputsChanged(@event);
				}
				else
				{
					outputsChanged(@event);
				}
			}

			internal virtual void inputsChanged(VariableListEvent @event)
			{
				int type = @event.Type;
				if (type == VariableListEvent.ALL_REPLACED && outerInstance.outputData.Count > 0)
				{
					outerInstance.outputData.Clear();
					outerInstance.fireModelChanged(OutputExpressionsEvent.ALL_VARIABLES_REPLACED);
				}
				else if (type == VariableListEvent.REMOVE)
				{
					string input = @event.Variable;
					foreach (string output in outerInstance.outputData.Keys)
					{
						OutputData data = outerInstance.getOutputData(output, false);
						if (data != null)
						{
							data.removeInput(input);
						}
					}
				}
				else if (type == VariableListEvent.REPLACE)
				{
					string input = @event.Variable;
					int inputIndex = ((int?) @event.Data).Value;
					string newName = @event.Source.get(inputIndex);
					foreach (string output in outerInstance.outputData.Keys)
					{
						OutputData data = outerInstance.getOutputData(output, false);
						if (data != null)
						{
							data.replaceInput(input, newName);
						}
					}
				}
				else if (type == VariableListEvent.MOVE || type == VariableListEvent.ADD)
				{
					foreach (string output in outerInstance.outputData.Keys)
					{
						OutputData data = outerInstance.getOutputData(output, false);
						if (data != null)
						{
							data.invalidate(false, false);
						}
					}
				}
			}

			internal virtual void outputsChanged(VariableListEvent @event)
			{
				int type = @event.Type;
				if (type == VariableListEvent.ALL_REPLACED && outerInstance.outputData.Count > 0)
				{
					outerInstance.outputData.Clear();
					outerInstance.fireModelChanged(OutputExpressionsEvent.ALL_VARIABLES_REPLACED);
				}
				else if (type == VariableListEvent.REMOVE)
				{
					outerInstance.outputData.Remove(@event.Variable);
				}
				else if (type == VariableListEvent.REPLACE)
				{
					string oldName = @event.Variable;
					if (outerInstance.outputData.ContainsKey(oldName))
					{
						OutputData toMove = outerInstance.outputData.Remove(oldName);
						int inputIndex = ((int?) @event.Data).Value;
						string newName = @event.Source.get(inputIndex);
						toMove.output = newName;
						outerInstance.outputData[newName] = toMove;
					}
				}
			}

			public virtual void cellsChanged(TruthTableEvent @event)
			{
				string output = outerInstance.model.Outputs.get(@event.Column);
				outerInstance.invalidate(output, false);
			}

			public virtual void structureChanged(TruthTableEvent @event)
			{
			}
		}

		private MyListener myListener;
		private AnalyzerModel model;
		private Dictionary<string, OutputData> outputData = new Dictionary<string, OutputData>();
		private List<OutputExpressionsListener> listeners = new List<OutputExpressionsListener>();
		private bool updatingTable = false;

		public OutputExpressions(AnalyzerModel model)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.model = model;
			model.Inputs.addVariableListListener(myListener);
			model.Outputs.addVariableListListener(myListener);
			model.TruthTable.addTruthTableListener(myListener);
		}

		//
		// listener methods
		//
		public virtual void addOutputExpressionsListener(OutputExpressionsListener l)
		{
			listeners.Add(l);
		}

		public virtual void removeOutputExpressionsListener(OutputExpressionsListener l)
		{
			listeners.Remove(l);
		}

		private void fireModelChanged(int type)
		{
			fireModelChanged(type, null, null);
		}

		private void fireModelChanged(int type, string variable)
		{
			fireModelChanged(type, variable, null);
		}

		private void fireModelChanged(int type, string variable, object data)
		{
			OutputExpressionsEvent @event = new OutputExpressionsEvent(model, type, variable, data);
			foreach (OutputExpressionsListener l in listeners)
			{
				l.expressionChanged(@event);
			}
		}

		//
		// access methods
		//
		public virtual Expression getExpression(string output)
		{
			if (string.ReferenceEquals(output, null))
			{
				return null;
			}
			return getOutputData(output, true).Expression;
		}

		public virtual string getExpressionString(string output)
		{
			if (string.ReferenceEquals(output, null))
			{
				return "";
			}
			return getOutputData(output, true).ExpressionString;
		}

		public virtual bool isExpressionMinimal(string output)
		{
			OutputData data = getOutputData(output, false);
			return data == null ? true : data.ExpressionMinimal;
		}

		public virtual Expression getMinimalExpression(string output)
		{
			if (string.ReferenceEquals(output, null))
			{
				return Expressions.constant(0);
			}
			return getOutputData(output, true).MinimalExpression;
		}

		public virtual List<Implicant> getMinimalImplicants(string output)
		{
			if (string.ReferenceEquals(output, null))
			{
				return Implicant.MINIMAL_LIST;
			}
			return getOutputData(output, true).MinimalImplicants;
		}

		public virtual int getMinimizedFormat(string output)
		{
			if (string.ReferenceEquals(output, null))
			{
				return AnalyzerModel.FORMAT_SUM_OF_PRODUCTS;
			}
			return getOutputData(output, true).MinimizedFormat;
		}

		//
		// modifier methods
		//
		public virtual void setMinimizedFormat(string output, int format)
		{
			int oldFormat = getMinimizedFormat(output);
			if (format != oldFormat)
			{
				getOutputData(output, true).MinimizedFormat = format;
				invalidate(output, true);
			}
		}

		public virtual void setExpression(string output, Expression expr)
		{
			setExpression(output, expr, null);
		}

		public virtual void setExpression(string output, Expression expr, string exprString)
		{
			if (string.ReferenceEquals(output, null))
			{
				return;
			}
			getOutputData(output, true).setExpression(expr, exprString);
		}

		private void invalidate(string output, bool formatChanged)
		{
			OutputData data = getOutputData(output, false);
			if (data != null)
			{
				data.invalidate(false, false);
			}
		}

		private OutputData getOutputData(string output, bool create)
		{
			if (string.ReferenceEquals(output, null))
			{
				throw new System.ArgumentException("null output name");
			}
			OutputData ret = outputData[output];
			if (ret == null && create)
			{
				if (model.Outputs.IndexOf(output) < 0)
				{
					throw new System.ArgumentException("unrecognized output " + output);
				}
				ret = new OutputData(this, output);
				outputData[output] = ret;
			}
			return ret;
		}

		private static Entry[] computeColumn(TruthTable table, Expression expr)
		{
			int rows = table.RowCount;
			int cols = table.InputColumnCount;
			Entry[] values = new Entry[rows];
			if (expr == null)
			{
				Arrays.Fill(values, Entry.DONT_CARE);
			}
			else
			{
				Assignments assn = new Assignments();
				for (int i = 0; i < rows; i++)
				{
					for (int j = 0; j < cols; j++)
					{
						assn.put(table.getInputHeader(j), TruthTable.isInputSet(i, j, cols));
					}
					values[i] = expr.evaluate(assn) ? Entry.ONE : Entry.ZERO;
				}
			}
			return values;
		}

		private static bool columnsMatch(Entry[] a, Entry[] b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					bool bothDefined = (a[i] == Entry.ZERO || a[i] == Entry.ONE) && (b[i] == Entry.ZERO || b[i] == Entry.ONE);
					if (bothDefined)
					{
						return false;
					}
				}
			}
			return true;
		}

		private static bool isAllUndefined(Entry[] a)
		{
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] == Entry.ZERO || a[i] == Entry.ONE)
				{
					return false;
				}
			}
			return true;
		}

		private static bool implicantsSame(List<Implicant> a, List<Implicant> b)
		{
			if (a == null)
			{
				return b == null || b.Count == 0;
			}
			else if (b == null)
			{
				return a == null || a.Count == 0;
			}
			else if (a.Count != b.Count)
			{
				return false;
			}
			else
			{
				IEnumerator<Implicant> ait = a.GetEnumerator();
				foreach (Implicant bi in b)
				{
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					if (!ait.hasNext())
					{
						return false; // should never happen
					}
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Implicant ai = ait.next();
					if (!ai.Equals(bi))
					{
						return false;
					}
				}
				return true;
			}
		}
	}

}
