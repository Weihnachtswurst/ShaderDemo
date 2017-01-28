using System;

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
