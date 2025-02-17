﻿// ---------------------------------------------------------------------------------------------------------
// Copyright © 2007 - 2024 Tangible Software Solutions, Inc.
// This class can be used by anyone provided that the copyright notice remains intact.
//
// This class is used to replace some calls to java.util.Arrays methods with the C# equivalent.
// ---------------------------------------------------------------------------------------------------------
using System;

internal static class Arrays
{
	public static T[] CopyOf<T>(T[] original, int newLength)
	{
		T[] dest = new T[newLength];
		Array.Copy(original, dest, Math.Min(original.Length, newLength));
		return dest;
	}

	public static T[] CopyOfRange<T>(T[] original, int fromIndex, int toIndex)
	{
		int length = toIndex - fromIndex;
		T[] dest = new T[length];
		Array.Copy(original, fromIndex, dest, 0, length);
		return dest;
	}

	public static void Fill<T>(T[] array, T value)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = value;
		}
	}

	public static void Fill<T>(T[] array, int fromIndex, int toIndex, T value)
	{
		for (int i = fromIndex; i < toIndex; i++)
		{
			array[i] = value;
		}
	}
}
