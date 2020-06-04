using System;

namespace YatzyProgram
{
    internal class Dice
    {
        internal int Current { get; set; }

        internal Dice()
        {
            Current = 0;

        }

        
        protected static Random rand = new Random();
        internal virtual void Roll()
        {
            Current = rand.Next(1, 7);
        }

        public override string ToString()
        {
            return "Roll: " + Current;
        }

        internal void PrintCurrent()
        {
            Console.WriteLine(Current);
        }
    }
}
