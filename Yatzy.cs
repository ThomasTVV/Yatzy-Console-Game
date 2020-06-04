/*

Dette program er et singleplayer yatzy spil, hvor der spilles med 6 terninger. Programmet indeholder 4 klasser: Yatzy, Scoreboard, Dice og BiasedDice.

Yatzy
Denne klasse er selve kernen i spillet. Den har blandt andet en property der indeholder 6 terninger, og en property der indeholder et scoreboard. 
Metoden, der initierer og styrer runderne er StartGame(). Den mest avancerede funktion er UserActions(). Denne funktion registrerer, hvilke handlinger 
brugeren vil tage i brug (fx gemme, rulle, fjerne osv.). Metoden opererer primært ved hjælp af rekursion, hvor en int rollCount styrer antal runder der
er spillet. Hvis spilleren reroller kaldes UserActions(rollCount + 1). Hvis der fx sker en fejl kaldes UserAction(rollCount) for at prøve forfra. 
Rekursionen stopper når rollCount er større end 3 (som default). 
Ideen er at, hver gang man ruller, har man et antal valg som for eksempel at gemme, rulle, fjerne osv. Hvis man vælger at rulle igen, har man igen 
disse valg. Dette har jeg valgt at opfylde ved hjælp af rekursion.

Scoreboard
Scoreboard klassen har til hovedformål at lagre, hvilke scores der er blevet gemt, samt de respektive metoder til at gemme med. Den indeholder desuden
to lister, der hedder PossibleOutcomes og RemovedOutcomes. PossibleOutcomes får tilføjet elementer, hver gang der rulles med terningerne, og 
indeholder hvilke muligheder spilleren har i forhold til at gemme på scoreboardet. RemovedOutcomes indeholder alle de muligheder på scoreboardet der 
er fjernet. 

Dice
Dice klassen er meget kort og dens primære funktion er at rulle terningen. Den har en enkelt property der indeholder hvad terningen har slået. 

BiasedDice
BiasedDice er også kort og nedarver fra Dice klassen. Den indeholder 2 ekstra properties, der bestemmer hvor ofte der skal snydes, og om der skal 
snydes positivt eller negativt. 

Obs: I Scoreboard klassen er et par muligheder kommenteret ud som fx ”3 par”, idet jeg senere så at de ikke skulle være med. Jeg valgte bare at 
kommentere dem ud, så man har mulighed for nemt at spille med dem senere, hvis man vil.

*/



using System;
using System.Linq; //bruges for .Contains();
using System.Collections.Generic; //for at lave en dictionary


namespace YatzyProgram
{
    
    internal class Yatzy
    {

        private Dice[] Dices { get; set; }
        private BiasedDice[] BiasedDices { get; set; } //Bruges når vi vil bytte en normal terning ud med en snydeterning.
        private Scoreboard Score { get; set; }
        internal int RoundCounter { get; private set; }

        internal Yatzy()
        {
            Dices = new Dice[6];

            for (int i = 0; i < Dices.Length; i++)
            {
                Dices[i] = new Dice();
            }

            BiasedDices = new BiasedDice[6];

            Score = new Scoreboard();

            RoundCounter = 0;

        }


        // Main method
        public static void Main(string[] args)
        {
            Yatzy spil = new Yatzy();
            spil.StartGame();

        }

        private void StartGame() 
        {
            Intro();

            while (RoundCounter < Score.Scores.Count)
            { //antallet af runder der skal spilles er jo lig med antal mulige udfald på scoreboardet.
                WaitForPlayerToContinue("Tryk på en vilkårlig tast for at rulle med terningerne...");
                Roll();
                Score.PrintScoreboard(this);
                PrintRoll();
                UserActions();
                RoundCounter++;
            }


            Console.WriteLine("\n\n\nSpillet er nu færdigt! Dine scores er:");
            Score.PrintScoreboard(this);
            WaitForPlayerToContinue("Tryk på en vilkårlig tast for at kunne lukke programmet...");
        }


        private void Intro()
        {
            string text = "Velkommen til Yatzy!";
            text += "\n\nSådan spiller du:";
            text += "\n\tEfter et rul angiver du hvilke terninger du gerne vil gemme.";
            text += "\n\tHvis du vil gemme alle 5'er og 6'ere skriver du blot 56";
            text += "\n\thvis du vil gemme 2 4'ere og 1 5'er skriver du blot 4-2 5-1";
            text += "\n\tNår du vil gemme resultatet angiver du blot hvilket resultat du vil gemme. Fx 'yatzy'";
            text += "\n\tNår du vil fjerne en mulighed skriver du 'fjern' foran. Fx 'fjern yatzy'";
            text += "\n\tSkriv fx 'snyd 1 100 positive' for at snyde positivt med første terning 100 % af gangene";
            text += "\n\n\tSkriv 'hjælp' for at se denne vejledning igen.";
            Console.WriteLine(text);
        }

        /// <summary>
        /// Denne metode er kernen bag hele spillet. Den registrerer hvilke actions brugeren gerne vil tage i brug. 
        /// Dvs. om han vil reroll, fjerne en mulighed, gemme en mulighed osv.
        /// 
        /// Metoden opererer primært ved hjælp af rekursion.
        ///     Når man reroller kaldes UserActions(rollCount + 1).  (Dog inde i Reroll() metoden)
        ///     Når der sker en fejl og man skal tilbage til hvor man startede kaldes UserActions(rollCount)
        ///     Rekursionen stopper når man har rollet 3 gange i alt. Altså når rollCount == 3. (som default) 
        /// </summary>
        private void UserActions(int rollCount = 1)
        {
            int allowedRolls = 3;
            string userInputString = Console.ReadLine();

            //Hvis input kun er int
            if (Int32.TryParse(userInputString, out _))
            {
                HoldDices(userInputString, rollCount, allowedRolls);
            }

            //hvis input er en string
            else
            {
                if ((Score.PossibleOutcomes.Contains(userInputString)) && (Score.Scores[userInputString] == 0) && (!(Score.RemovedOutcomes.Contains(userInputString))))
                { //altså vi tilføjer til scoreboard hvis det vi skriver er en mulighed, og den ikke allerede er taget.
                    AddToScoreboard(userInputString);
                    Console.WriteLine($"\n\n{userInputString} er nu tilføjet til scoreboardet med en værdi på {Score.Scores[userInputString]}");
                }
                else if (userInputString.Contains("fjern "))
                {
                    RemoveOutcome(userInputString, rollCount);
                }
                else if (userInputString.Contains("hjælp"))
                {
                    Intro();
                    UserActions(rollCount);
                }
                else if (userInputString.Contains("-"))
                {
                    RerollSpecific(userInputString, rollCount, allowedRolls);
                }
                else if (userInputString.Contains("snyd"))
                {
                    Cheat(userInputString, rollCount);
                }
                else if ((string.IsNullOrEmpty(userInputString)) && (rollCount < allowedRolls))
                {
                    Reroll(0, rollCount); //0 fordi vi ikke vil gemme nogen terninger
                }
                else
                {
                    Console.WriteLine("Ikke en gyldig mulighed. Prøv igen.");
                    UserActions(rollCount);
                }
            }
        }

        private void AddToScoreboard(string userInput)
        {
            string allLowercase = userInput.ToLower();
            Score.WriteToScoreboard(allLowercase, this);
        }

        private void RemoveOutcome(string userInputString, int rollCount)
        {
            string[] inputSplit = userInputString.Split("fjern ");
            string OutcomeToRemove = inputSplit[1];
            if ((Score.Scores.ContainsKey(OutcomeToRemove)) && (Score.Scores[OutcomeToRemove] == 0) && (!Score.RemovedOutcomes.Contains(OutcomeToRemove)))
            {//altså hvis ordet er på scoreboardet og scoren for ordet er 0 og den ikke allerede er fjernet.

                if ((RoundCounter < 6) && (!Score.SingleNumberNumeric.ContainsKey(OutcomeToRemove)))
                {//Altså hvis man er i upper section og man prøver at fjerne en mulighed fra lower section. Det må man ikke.
                    Console.WriteLine("Du kan pt. kun fjerne muligheder fra upper section. Prøv igen.");
                    UserActions(rollCount);
                }
                else
                {
                    Score.RemovedOutcomes.Add(OutcomeToRemove);
                    Console.WriteLine($"{OutcomeToRemove} er nu fjernet.");
                }
            }

            else
            {
                Console.WriteLine("Ikke en gyldig mulighed. Prøv igen.");
                UserActions(rollCount);
            }
        }

        private void RerollSpecific(string userInputString, int rollCount, int allowedRolls)
        {
            if (rollCount < allowedRolls)
            {
                try
                {
                    string[] inputSplit = userInputString.Split(" ");
                    //array eks: 1-1, 2-1, 3-1. (Hvis man vil holde en ener, en toer og en treer)
                    int[] countOfEachDice = CountOfEachDice();
                    List<int> lockedDices = new List<int>();

                    for (int i = 0; i < inputSplit.Length; i++)
                    {
                        string[] DiceValueAndSaveAmount = inputSplit[i].Split("-");
                        int DiceValue = Convert.ToInt32(DiceValueAndSaveAmount[0]);
                        int SaveAmount = Convert.ToInt32(DiceValueAndSaveAmount[1]);

                        if (SaveAmount <= countOfEachDice[DiceValue - 1])
                        {//Altså hvis fx vi vil gemme 2 enere og der er 3 enere i alt. Så må vi gerne. 
                            for (int k = 0; k < SaveAmount; k++)
                            {
                                lockedDices.Add(DiceValue);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ikke en gyldig mulighed. Prøv igen. Hvis man vil fx vil gemme 1 4'er og 2 5'ere så skriv '4-1 5-2'");
                            UserActions(rollCount);
                            return; //vigtigt return. Uden den kører den nemlig også nedenstående for loop ved fejl, hvilket giver nogle sjove rekursion fejl.
                        }

                    }


                    for (int i = 0; i < lockedDices.Count(); i++) //herefter overfører vi blot værdierne til terningerne. 
                    {
                        Dices[i].Current = lockedDices[i];
                    }

                    Reroll(lockedDices.Count(), rollCount);

                }

                catch (Exception)
                {
                    Console.WriteLine("Ikke en gyldig mulighed. Prøv igen.");
                    UserActions(rollCount);
                }
            }
            else
            {
                Console.WriteLine("Du har ikke flere kast tilbage. Gem på scoreboardet eller fjern en mulighed. skriv 'hjælp' for instruktioner.");
                UserActions(rollCount);
            }


        }

        private void Cheat(string userInputString, int rollCount)
        {//Gør en dice til en dynamisk type af biasedDice
            try
            {
                string[] inputSplit = userInputString.Split(" ");

                int diceIndex = Convert.ToInt32(inputSplit[1]);
                int biasedPercent = Convert.ToInt32(inputSplit[2]);

                Biased biasedState = Biased.Positive; //skal declare den til en value, ellers brokker den sig
                if (inputSplit[3] == "positive")
                {
                    biasedState = Biased.Positive;
                }
                else if (inputSplit[3] == "negative")
                {
                    biasedState = Biased.Negative;
                }
                else
                {
                    Console.WriteLine("Ikke en gyldig mulighed. Prøv igen.\nEksempel input: 'snyd 1 100 positive' betyder 'første terning skal snyde positivt 100 % af gangene'");
                    UserActions(rollCount);
                    return;
                }

                if (BiasedDices[diceIndex - 1] == null) //hvis det ikke er en snydeterning i forvejen
                {
                    BiasedDices[diceIndex - 1] = new BiasedDice(biasedPercent, biasedState);
                    Dices[diceIndex - 1] = BiasedDices[diceIndex - 1];  //dynamisk type
                    Console.WriteLine($"Terning nummer {diceIndex} er nu en {biasedState} biased dice, og snyder {biasedPercent}% af gangene.");
                }
                else //hvis det ER en snydeterning i forvejen
                {
                    BiasedDices[diceIndex - 1].BiasedPercent = biasedPercent;
                    BiasedDices[diceIndex - 1].BiasedState = biasedState;
                    Console.WriteLine($"Terning nummer {diceIndex} er ændret til at være {biasedState} biased {biasedPercent}% af gangene.");
                }


                UserActions(rollCount);
            }
            catch (Exception)
            {
                Console.WriteLine("Ikke en gyldig mulighed. Prøv igen.\nEksempel input: 'snyd 1 100 positive' betyder 'første terning skal snyde positivt 100 % af gangene'");
                UserActions(rollCount);
            }
        }


        private void Reroll(int lockedDicesCount, int rollCount)
        {
            RollNotLockedDices(lockedDicesCount);
            Console.WriteLine("\n\n");
            Score.PrintScoreboard(this);
            PrintRollWithHyphen(rollCount, lockedDicesCount);  //efter runde 1 skal vi printe med den her. 
            UserActions(rollCount + 1); //rekursion 
        }

        //Ruller ved hjælp af hver dices egen roll method, så BiasedDice kan rulle biased.
        private void Roll()
        {
            foreach (Dice element in Dices)
            {
                element.Roll();
            }
        }


        private void PrintRoll()
        {
            Console.WriteLine(this);
        }





        private void HoldDices(string userInputString, int rollCount, int allowedRolls)
        {
            if (rollCount < allowedRolls) //man kan kun reroll 2 gange (default). 
            {
                int[] inputArrayInt = StringToIntArray(userInputString);
                int lockedDicesCount = LockDices(inputArrayInt);
                Reroll(lockedDicesCount, rollCount);
            }
            else
            {
                Console.WriteLine("Du har ikke flere kast tilbage. Gem på scoreboardet eller fjern en ubrugt mulighed.");
                UserActions(rollCount);
            }
        }

        private int LockDices(int[] inputArrayInt)
        {
            //looper gennem hvert enkelt gemt tal. Dvs. hvis alle 4'ere og 5'ere er gemt, looper vi 2 gange.
            //og hver gang vi looper tæller vi antallet (savedDicesCount). Dvs. hvis der er 3 4'ere og 1 5'er, så er de første 4 terninger låst til disse værdier. 

            int numberOfLockedDices = 0;
            int[] tempCurrent = new int[6]; //laver en temp beholder for at undgå at overwrite de nuværende terninger når vi gemmer. 
            for (int i = 0; i < inputArrayInt.Length; i++)
            {
                int savedDicesCount = NumberOf(inputArrayInt[i]);

                for (int k = 0; k < savedDicesCount; k++)
                {
                    tempCurrent[numberOfLockedDices] = inputArrayInt[i];
                    numberOfLockedDices++;
                }

            }

            for (int i = 0; i < 6; i++) //herefter overfører vi blot værdierne til terningerne. 
            {
                Dices[i].Current = tempCurrent[i];
            }

            return numberOfLockedDices;

            //obs: Konsekvenser af tilgangen til denne metode er at dices[5] oftere vil snyde end dices[0], idet det er værdien og ikke selve terninger der bliver låst.
            //Hvis man fx kun vil snyde i starten, kan man derfor bare lave biaseddices ved de første dices. 
        }

        private void RollNotLockedDices(int numberOfLockedDices)
        {
            for (int i = numberOfLockedDices; i < 6; i++)
            {
                Dices[i].Roll();
            }
        }

        private int[] StringToIntArray(string userInput)
        { //userinput vil fx være 35, hvor vi returnerer et array med
            char[] inputArray = userInput.ToCharArray();
            int[] inputArrayInt = new int[inputArray.Length];

            for (int i = 0; i < inputArray.Length; i++)
            {
                int inputNumeric = (int)Char.GetNumericValue(inputArray[i]);
                //undgår duplicates.
                if (!inputArrayInt.Contains(inputNumeric))
                {
                    inputArrayInt[i] = (int)Char.GetNumericValue(inputArray[i]);

                    //checker for korrekt værdi
                    if ((inputArrayInt[i] < 1) || (inputArrayInt[i] > 6))
                    {
                        Console.WriteLine("Ugyldige tal angivet.");
                    }
                }
            }

            return inputArrayInt;
        }



        //tæller antallet af hver terningudkast. Altså antal 1'ere, antal 2'ere osv. 
        internal int[] CountOfEachDice()
        {
            int[] rv = new int[6];

            for (int i = 0; i < rv.Length; i++)
            {
                rv[i] = NumberOf(i + 1);
            }
            return rv;
        }

        public override string ToString()
        {
            string rv = "Kast 1 terninge værdier: ";
            foreach (Dice element in Dices)
            {
                rv += element.Current + " ";
            }
            return rv;
        }

        //printer også en fin lille bindestreg mellem de gemte terninger og dem der er rullet. 
        private void PrintRollWithHyphen(int rollCount, int numberOfSavedDices = -1)
        {
            string rv = $"Kast {rollCount + 1} terninge værdier: ";

            for (int i = 0; i < Dices.Length; i++)
            {
                if (i == numberOfSavedDices)
                {
                    rv += "- ";
                }

                rv += Dices[i].Current + " ";
            }
            Console.WriteLine(rv);
        }

        private int NumberOf(int nummer)
        {
            int count = 0;
            foreach (Dice element in Dices)
            {
                if (element.Current == nummer)
                {
                    count++;
                }
            }
            return count;
        }

        internal void TestDiceFairness()
        {
            List<string> dices = new List<string> { "Normal dice", "Positive biased dice", "Negative biased dice" };
            int[] totalDiceValues = { 0, 0, 0 };
            Dice[] testDices = new Dice[3];

            testDices[0] = new Dice();
            testDices[1] = new BiasedDice(100, Biased.Positive);
            testDices[2] = new BiasedDice(100, Biased.Negative);

            int numberOfLoops = 1000;


            //lægger de samlede værdier for hver terning til totalDiceValues
            for (int i = 0; i < numberOfLoops; i++)
            {
                for (int diceType = 0; diceType < 3; diceType++)
                {
                    testDices[diceType].Roll();
                    totalDiceValues[diceType] += testDices[diceType].Current;
                }

            }

            //Udregner gennemsnit for hver terningtype og printer det.
            for (int i = 0; i < 3; i++)
            {
                float average = totalDiceValues[i] / (float)numberOfLoops;
                Console.WriteLine($"Gennemnit for {dices[i]}: {average}");
            }

        }

        private void WaitForPlayerToContinue(string waitText)
        {
            Console.WriteLine($"\n{waitText}");
            Console.ReadKey(true);
            Console.WriteLine("\n\n"); 
        }
    }
}
