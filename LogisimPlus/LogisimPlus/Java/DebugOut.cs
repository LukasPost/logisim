using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisimPlus.Java;
public class DebugOut
{
    public static void printf(string format, params object[] args)
    {
        Console.WriteLine(format);
        foreach(object arg in args) 
            Console.WriteLine($"    |  {arg}");
    }
}
