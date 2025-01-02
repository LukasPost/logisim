// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.gates
{

	using Expression = logisim.analyze.model.Expression;
	using logisim.analyze.model;
	using ComponentFactory = logisim.comp.ComponentFactory;

	/// <summary>
	/// This represents the actual gate selection used corresponding to an expression, without any correspondence to how they
	/// would be laid down in a circuit. This intermediate representation permits easy manipulation of an expression's
	/// translation.
	/// </summary>
	internal abstract class CircuitDetermination
	{
		/// <summary>
		/// Ensures that all gates have only two inputs. </summary>
		internal virtual void convertToTwoInputs()
		{
		}

		/// <summary>
		/// Converts all gates to NANDs. Note that this will fail with an exception if any XOR/XNOR gates are used.
		/// </summary>
		internal virtual void convertToNands()
		{
		}

		/// <summary>
		/// Repairs two errors that may have cropped up in creating the circuit. First, if there are gates with more inputs
		/// than their capacity, we repair them. Second, any XOR/XNOR gates with more than 2 inputs should really be Odd/Even
		/// Parity gates.
		/// </summary>
		internal virtual void repair()
		{
		}

		/// <summary>
		/// A utility method for determining whether this fits the pattern of a NAND representing a NOT.
		/// </summary>
		internal virtual bool NandNot
		{
			get
			{
				return false;
			}
		}

		//
		// static members
		//
		internal class Gate : CircuitDetermination
		{
			internal ComponentFactory factory;
			internal List<CircuitDetermination> inputs = new List<CircuitDetermination>();

			internal Gate(ComponentFactory factory)
			{
				this.factory = factory;
			}

			internal virtual ComponentFactory Factory
			{
				get
				{
					return factory;
				}
			}

			internal virtual List<CircuitDetermination> Inputs
			{
				get
				{
					return inputs;
				}
			}

			internal override void convertToTwoInputs()
			{
				if (inputs.Count <= 2)
				{
					foreach (CircuitDetermination a in inputs)
					{
						a.convertToTwoInputs();
					}
				}
				else
				{
					ComponentFactory subFactory;
					if (factory == NorGate.FACTORY)
					{
						subFactory = OrGate.FACTORY;
					}
					else if (factory == NandGate.FACTORY)
					{
						subFactory = AndGate.FACTORY;
					}
					else
					{
						subFactory = factory;
					}

					int split = (inputs.Count + 1) / 2;
					CircuitDetermination a = convertToTwoInputsSub(0, split, subFactory);
					CircuitDetermination b = convertToTwoInputsSub(split, inputs.Count, subFactory);
					inputs.Clear();
					inputs.Add(a);
					inputs.Add(b);
				}
			}

			internal virtual CircuitDetermination convertToTwoInputsSub(int start, int stop, ComponentFactory subFactory)
			{
				if (stop - start == 1)
				{
					CircuitDetermination a = inputs[start];
					a.convertToTwoInputs();
					return a;
				}
				else
				{
					int split = (start + stop + 1) / 2;
					CircuitDetermination a = convertToTwoInputsSub(start, split, subFactory);
					CircuitDetermination b = convertToTwoInputsSub(split, stop, subFactory);
					Gate ret = new Gate(subFactory);
					ret.inputs.Add(a);
					ret.inputs.Add(b);
					return ret;
				}
			}

			internal override void convertToNands()
			{
				// first recurse to clean up any children
				foreach (CircuitDetermination sub in inputs)
				{
					sub.convertToNands();
				}

				// repair large XOR/XNORs to odd/even parity gates
				if (factory == NotGate.FACTORY)
				{
					inputs.Add(inputs[0]);
				}
				else if (factory == AndGate.FACTORY)
				{
					notOutput();
				}
				else if (factory == OrGate.FACTORY)
				{
					notAllInputs();
				}
				else if (factory == NorGate.FACTORY)
				{
					notAllInputs(); // the order of these two lines is significant
					notOutput();
				}
				else if (factory == NandGate.FACTORY)
				{
					;
				}
				else
				{
					throw new System.ArgumentException("Cannot handle " + factory.DisplayName);
				}
				factory = NandGate.FACTORY;
			}

			internal virtual void notOutput()
			{
				Gate sub = new Gate(NandGate.FACTORY);
				sub.inputs = this.inputs;
				this.inputs = new List<CircuitDetermination>();
				inputs.Add(sub);
				inputs.Add(sub);
			}

			internal virtual void notAllInputs()
			{
				for (int i = 0; i < inputs.Count; i++)
				{
					CircuitDetermination old = inputs[i];
					if (old.NandNot)
					{
						inputs[i] = ((Gate) old).inputs[0];
					}
					else
					{
						Gate now = new Gate(NandGate.FACTORY);
						now.inputs.Add(old);
						now.inputs.Add(old);
						inputs[i] = now;
					}
				}
			}

			internal override bool NandNot
			{
				get
				{
					return factory == NandGate.FACTORY && inputs.Count == 2 && inputs[0] == inputs[1];
				}
			}

			internal override void repair()
			{
				// check whether we need to split ourself up.
				int num = inputs.Count;
				if (num > GateAttributes.MAX_INPUTS)
				{
					int newNum = (num + GateAttributes.MAX_INPUTS - 1) / GateAttributes.MAX_INPUTS;
					List<CircuitDetermination> oldInputs = inputs;
					inputs = new List<CircuitDetermination>();

					ComponentFactory subFactory = factory;
					if (subFactory == NandGate.FACTORY)
					{
						subFactory = AndGate.FACTORY;
					}
					if (subFactory == NorGate.FACTORY)
					{
						subFactory = OrGate.FACTORY;
					}

					int per = num / newNum;
					int numExtra = num - per * newNum;
					int k = 0;
					for (int i = 0; i < newNum; i++)
					{
						Gate sub = new Gate(subFactory);
						int subCount = per + (i < numExtra ? 1 : 0);
						for (int j = 0; j < subCount; j++)
						{
							sub.inputs.Add(oldInputs[k]);
							k++;
						}
						inputs.Add(sub);
					}
				}

				// repair large XOR/XNORs to odd/even parity gates
				if (inputs.Count > 2)
				{
					if (factory == XorGate.FACTORY)
					{
						factory = OddParityGate.FACTORY;
					}
					else if (factory == XnorGate.FACTORY)
					{
						factory = EvenParityGate.FACTORY;
					}
				}

				// finally, recurse to clean up any children
				foreach (CircuitDetermination sub in inputs)
				{
					sub.repair();
				}
			}
		}

		internal class Input : CircuitDetermination
		{
			internal string name;

			internal Input(string name)
			{
				this.name = name;
			}

			internal virtual string Name
			{
				get
				{
					return name;
				}
			}
		}

		internal class Value : CircuitDetermination
		{
			internal int value;

			internal Value(int value)
			{
				this.value = value;
			}

			internal virtual int Value
			{
				get
				{
					return value;
				}
			}
		}

		internal static CircuitDetermination create(Expression expr)
		{
			if (expr == null)
			{
				return null;
			}
			return expr.visit(new Determine());
		}

		private class Determine : ExpressionVisitor<CircuitDetermination>
		{
			public virtual CircuitDetermination visitAnd(Expression a, Expression b)
			{
				return binary(a.visit(this), b.visit(this), AndGate.FACTORY);
			}

			public virtual CircuitDetermination visitOr(Expression a, Expression b)
			{
				return binary(a.visit(this), b.visit(this), OrGate.FACTORY);
			}

			public virtual CircuitDetermination visitXor(Expression a, Expression b)
			{
				return binary(a.visit(this), b.visit(this), XorGate.FACTORY);
			}

			internal virtual Gate binary(CircuitDetermination aret, CircuitDetermination bret, ComponentFactory factory)
			{
				if (aret is Gate)
				{
					Gate a = (Gate) aret;
					if (a.factory == factory)
					{
						if (bret is Gate)
						{
							Gate b = (Gate) bret;
							if (b.factory == factory)
							{
								a.inputs.AddRange(b.inputs);
								return a;
							}
						}
						a.inputs.Add(bret);
						return a;
					}
				}

				if (bret is Gate)
				{
					Gate b = (Gate) bret;
					if (b.factory == factory)
					{
						b.inputs.Add(aret);
						return b;
					}
				}

				Gate ret = new Gate(factory);
				ret.inputs.Add(aret);
				ret.inputs.Add(bret);
				return ret;
			}

			public virtual CircuitDetermination visitNot(Expression aBase)
			{
				CircuitDetermination aret = aBase.visit(this);
				if (aret is Gate)
				{
					Gate a = (Gate) aret;
					if (a.factory == AndGate.FACTORY)
					{
						a.factory = NandGate.FACTORY;
						return a;
					}
					else if (a.factory == OrGate.FACTORY)
					{
						a.factory = NorGate.FACTORY;
						return a;
					}
					else if (a.factory == XorGate.FACTORY)
					{
						a.factory = XnorGate.FACTORY;
						return a;
					}
				}

				Gate ret = new Gate(NotGate.FACTORY);
				ret.inputs.Add(aret);
				return ret;
			}

			public virtual CircuitDetermination visitVariable(string name)
			{
				return new Input(name);
			}

			public virtual CircuitDetermination visitConstant(int value)
			{
				return new Value(value);
			}
		}
	}

}
