// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.proj
{

	public class JoinedAction : Action
	{
		internal Action[] todo;

		internal JoinedAction(params Action[] actions)
		{
			todo = actions;
		}

		public virtual Action FirstAction
		{
			get
			{
				return todo[0];
			}
		}

		public virtual Action LastAction
		{
			get
			{
				return todo[todo.Length - 1];
			}
		}

		public virtual IList<Action> Actions
		{
			get
			{
				return new List<Action> {todo};
			}
		}

		public override bool Modification
		{
			get
			{
				foreach (Action act in todo)
				{
					if (act.Modification)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override string Name
		{
			get
			{
				return todo[0].Name;
			}
		}

		public override void doIt(Project proj)
		{
			foreach (Action act in todo)
			{
				act.doIt(proj);
			}
		}

		public override void undo(Project proj)
		{
			for (int i = todo.Length - 1; i >= 0; i--)
			{
				todo[i].undo(proj);
			}
		}

		public override Action append(Action other)
		{
			int oldLen = todo.Length;
			Action[] newToDo = new Action[oldLen + 1];
			Array.Copy(todo, 0, newToDo, 0, oldLen);
			newToDo[oldLen] = other;
			todo = newToDo;
			return this;
		}
	}
}
