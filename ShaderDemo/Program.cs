using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Window window = new Window(640, 480);

            window.Run(200);
        }

        void someMethod()
        {
            Console.Out.WriteLine("Method called");
        }
    }
}
