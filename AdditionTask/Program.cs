using System;
using System.Threading;

namespace AdditionTask
{
    class Program
    {
        static int i = 0;
        static void Recartion(object n)
        {
            int m = (int)n;
            Console.WriteLine(new string(' ', i) + $"{m}");
            --m;
            ++i;
            if (m > 0)
            {
                Thread.Sleep(100);
                Recartion(m);
            }
        }
        static void Main(string[] args)
        {
            ParameterizedThreadStart x = new ParameterizedThreadStart(Recartion);
            Thread y = new Thread(x);
            y.Start(15);
        }
    }
}
