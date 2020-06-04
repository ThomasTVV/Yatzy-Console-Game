namespace YatzyProgram
{
    internal enum Biased { Positive, Negative };
    internal class BiasedDice : Dice
    {
        internal int BiasedPercent { get; set; }

        //BiasedState er enten Biased.Positive eller Biased.Negative.
        internal Biased BiasedState { get; set; }
        internal BiasedDice(int biasedPercent, Biased biasedState)
        {
            Current = 0;
            BiasedPercent = biasedPercent;
            BiasedState = biasedState;
        }

        internal override void Roll()
        {
            Current = rand.Next(1, 7);

            int randomPercent = rand.Next(1, 101);

            if (BiasedPercent >= randomPercent)
            {//jo højere BiasedPercent er, jo oftere vil den være højere end randomPercent - og dermed snyde.
                if ((Current <= 5) && (BiasedState == Biased.Positive))
                {
                    Current++;
                }

                else if ((Current >= 2) && (BiasedState == Biased.Negative))
                {
                    Current--;
                }
            }


        }
    }
}
