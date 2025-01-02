// ----------------------------------------------------------------------------------------
// Copyright © 2007 - 2024 Tangible Software Solutions, Inc.
// This class can be used by anyone provided that the copyright notice remains intact.
//
// This class includes methods to convert Java rectangular arrays (jagged arrays
// with inner arrays of the same length).
// ----------------------------------------------------------------------------------------
internal static class RectangularArrays
{
    public static string[][] RectangularStringArray(int size1, int size2)
    {
        string[][] newArray = new string[size1][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new string[size2];
        }

        return newArray;
    }

    public static Entry[][] RectangularEntryArray(int size1, int size2)
    {
        Entry[][] newArray = new Entry[size1][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new Entry[size2];
        }

        return newArray;
    }

    public static sbyte[][] RectangularSbyteArray(int size1, int size2)
    {
        sbyte[][] newArray = new sbyte[size1][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new sbyte[size2];
        }

        return newArray;
    }
}
