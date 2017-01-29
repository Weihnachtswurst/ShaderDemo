using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDemo
{
    public static class Picker
    {
        private static Dictionary<int, Action> map = new Dictionary<int, Action>();
        private static Queue<int> RecycledIndices = new Queue<int>();
        private static int NextIndex = 1;

        public static int register(Action action) {
            int index;

            if (RecycledIndices.Count != 0)
                index = RecycledIndices.Dequeue();
            else
                index = NextIndex++;

            map.Add(index, action);
            return index;
        }

        public static void deregister(int index)
        {
            map.Remove(index);
        }

        public static void invoke(int index)
        {
            //if (index != 0)
                //map[index]();
        }
    }
}
