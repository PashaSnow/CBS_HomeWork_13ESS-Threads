using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// эксперементы

namespace JagetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int[][] x = new int[1][];
            x[0] =  new int[] { 5, 2 };

            Console.WriteLine(x.Length);
            Console.WriteLine(x.Rank);
            
            Console.WriteLine(Console.CursorLeft);
            Console.WriteLine(Console.CursorTop);
            Console.CursorVisible = true;
            Console.SetCursorPosition(5, 7);
            Console.Write(Console.CursorLeft);
            Console.Write(Console.CursorTop);
            Console.SetCursorPosition(5, 0);
            Console.Write(Console.CursorLeft);
            Console.Write(Console.CursorTop);
            Console.WriteLine(Console.CursorVisible);

        }
    }
}
