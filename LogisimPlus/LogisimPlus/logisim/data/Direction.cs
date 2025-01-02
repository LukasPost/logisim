// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{
	public sealed class Direction : AttributeOptionInterface
	{

		public static readonly Direction East = new Direction("East", InnerEnum.East, 0);
		public static readonly Direction North = new Direction("North", InnerEnum.North, 1);
		public static readonly Direction West = new Direction("West", InnerEnum.West, 2);
		public static readonly Direction South = new Direction("South", InnerEnum.South, 3);

		private static readonly List<Direction> valueList = new List<Direction>();

		static Direction()
		{
			valueList.Add(East);
			valueList.Add(North);
			valueList.Add(West);
			valueList.Add(South);
		}

		public enum InnerEnum
		{
			East,
			North,
			West,
			South
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;

		private Direction(string name, InnerEnum innerEnum, int id)
		{
			this.id = id;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// @deprecated Please use valueOf instead 
		[Obsolete("Please use valueOf instead")]
		public static Direction parse(string str)
		{
			string cap = str.Substring(0, 1).ToUpper() + str.Substring(1);
			return Direction.valueOf(cap);
		}

		public static readonly Direction[] cardinals = new Direction[] {East, North, West, South};

		public static Direction[] Cardinals
		{
			get
			{
				return cardinals;
			}
		}

		public static Direction fromInt(int x)
		{
			return cardinals[x];
		}

		private static string[] displayStrings = new string[] {Strings.get("directionEastOption"), Strings.get("directionNorthOption"), Strings.get("directionWestOption"), Strings.get("directionSouthOption")};

		public string toDisplayString()
		{
			return displayStrings[id];
		}

		private static string[] displayStringsVertical = new string[] {Strings.get("directionEastVertical"), Strings.get("directionNorthVertical"), Strings.get("directionWestVertical"), Strings.get("directionSouthVertical")};

		public string toVerticalDisplayString()
		{
			return displayStringsVertical[id];
		}

		public double toRadians()
		{
			return id * Math.PI / 2.0;
		}

		public int toDegrees()
		{
			return id * 90;
		}

		public Direction reverse()
		{
			return fromInt((id + 2) & 3);
		}

		public Direction rotateCW()
		{
			return fromInt((id - 1) & 3);
		}

		public Direction rotateCCW()
		{
			return fromInt((id + 1) & 3);
		}

		// for AttributeOptionInterface
		public object Value
		{
			get
			{
				return this;
			}
		}

		public static Direction[] values()
		{
			return valueList.ToArray();
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static Direction valueOf(string name)
		{
			foreach (Direction enumInstance in Direction.valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}
