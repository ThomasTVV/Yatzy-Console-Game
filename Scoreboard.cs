using System;
using System.Linq; //bruges for .Contains();    for at undgå duplicates
using System.Collections.Generic; //for at lave en dictionary

namespace YatzyProgram
{
    internal class Scoreboard
    {
        internal Dictionary<string, int> Scores { get; set; }
        internal Dictionary<string, int> SingleNumberNumeric { get; }

        private Dictionary<string, int> Pairs { get; }

        private Dictionary<string, int> OfAKind { get; }

        private Dictionary<string, int> Straight { get; }

        internal List<string> PossibleOutcomes { get; set; }

        internal List<string> RemovedOutcomes { get; set; }

        internal Scoreboard()
        {
            Scores = new Dictionary<string, int>
            {
            {"enere", 0},
            {"toere", 0},
            {"treere", 0 },
            {"firere", 0 },
            {"femmere", 0 },
            {"seksere", 0 },
            {"1 par", 0 },
            {"2 par", 0 },
            //{"3 par", 0 },
            {"3 ens", 0 },
            {"4 ens", 0 },
            //{"2*3 ens", 0 },
            {"lille straight", 0 },
            {"store straight", 0 },
            //{"fuld straight", 0 },
            {"fuldt hus", 0 },
            {"chance", 0 },
            {"yatzy", 0 }
            };

            PossibleOutcomes = new List<string>();
            RemovedOutcomes = new List<string>(); //Holder styr på hvilke muligheder der er "droppet"

            SingleNumberNumeric = new Dictionary<string, int>
            {
            {"enere", 1},
            {"toere", 2},
            {"treere", 3 },
            {"firere", 4 },
            {"femmere", 5 },
            {"seksere", 6 }
            };

            Pairs = new Dictionary<string, int>
            {
            {"1 par", 1},
            {"2 par", 2},
            //{"3 par", 3 }
            };

            OfAKind = new Dictionary<string, int>
            {
            {"3 ens", 3},
            {"4 ens", 4},
            //{"2*3 ens", 6 }
            };

            Straight = new Dictionary<string, int>
            {
            {"lille straight", 1},
            {"store straight", 2},
            //{"fuld straight", 3}
            };
        }


        internal void PrintScoreboard(Yatzy spil)
        {
            PossibleOutcomes.Clear();
            Dictionary<string, int> tempdict = Scores;
            int upperScore = 0;
            int upperBonus = 0;
            int totalScore = 0;
            int loopCounter = 0;
            foreach (KeyValuePair<string, int> kvp in tempdict)
            {
                WriteToScoreboard(kvp.Key, spil, true); //tanker possibleOutcomes op.
                if (loopCounter < 6)
                {
                    upperScore += kvp.Value;
                }
                else if (loopCounter == 6)
                {
                    Console.WriteLine($"\tSum: {upperScore}");
                    if (upperScore > 62)
                    {
                        upperBonus = 50;
                        totalScore += upperBonus;
                    }
                    Console.WriteLine($"\tBonus: {upperBonus}");
                }


                totalScore += kvp.Value;
                string scoreboardText;

                if (RemovedOutcomes.Contains(kvp.Key))
                {
                    scoreboardText = $"\t{kvp.Key}: X";
                }
                else
                {
                    scoreboardText = $"\t{kvp.Key}: {kvp.Value}";
                }


                if ((PossibleOutcomes.Contains(kvp.Key)) && (kvp.Value == 0) && (!(RemovedOutcomes.Contains(kvp.Key))))
                {
                    scoreboardText += " <--";
                }

                Console.WriteLine(scoreboardText);
                loopCounter++;
            }
            Console.WriteLine($"\tSum: {totalScore}");
            Console.WriteLine("-------------------------------------\n");
        }

        internal void WriteToScoreboard(string userInput, Yatzy spil, bool AddToPossibleOutcomesInstead = false)
        {
            int[] countOfEachDice = spil.CountOfEachDice();

            if (spil.RoundCounter < 6) //altså når man spiller i upper section.
            {
                if (SingleNumberNumeric.ContainsKey(userInput))
                {
                    AddUpperToScoreboard(userInput, countOfEachDice, AddToPossibleOutcomesInstead);
                }
            }

            else //lower section
            {//ikke et switch statement fordi fx addPairToScoreboard er nemmere at samle som enkelt i stedet for flere forskellige cases i et swtich.
             //man kan nemlig ikke sige  case 1,2,3:    man kan kun sige case 1:    case 2:   osv.       Så det er for at undgå at gentage kode.
                if (Pairs.ContainsKey(userInput))
                {
                    AddPairToScoreboard(userInput, countOfEachDice, AddToPossibleOutcomesInstead);
                }

                else if (OfAKind.ContainsKey(userInput))
                {
                    AddOfAKindToScoreboard(userInput, countOfEachDice, AddToPossibleOutcomesInstead);
                }

                else if (Straight.ContainsKey(userInput))
                {
                    AddStraightToScoreboard(userInput, countOfEachDice, AddToPossibleOutcomesInstead);
                }

                else if (userInput == "fuldt hus")
                {
                    AddFuldHusToScoreboard(userInput, countOfEachDice, AddToPossibleOutcomesInstead);
                }

                else if (userInput == "yatzy")
                {
                    AddYatzyToScoreboard(userInput, countOfEachDice, AddToPossibleOutcomesInstead);
                }

                else if (userInput == "chance")
                {
                    AddChanceToScoreboard(userInput, countOfEachDice, AddToPossibleOutcomesInstead);
                }
            }
        }





        private void AddUpperToScoreboard(string userInput, int[] countOfEachDice, bool AddToPossibleOutcomesInstead = false)
        {
            int number = SingleNumberNumeric[userInput];
            int numberCount = countOfEachDice[number - 1]; //tæller antallet. Fx hvor mange 2'ere der er.
            if (numberCount > 0)
            {
                int score = number * numberCount; //Hvis der er 4 2'ere ganger vi 4 med 2 for at få 8 point. 
                AddToScoreboardOrPossibleOutcomes(userInput, score, AddToPossibleOutcomesInstead);
            }

        }

        private void AddPairToScoreboard(string userInput, int[] countOfEachDice, bool AddToPossibleOutcomesInstead = false)
        {
            int numberOfPairsToFind = Pairs[userInput];
            List<int> pairs = new List<int>();

            int[] countOfEachPair = new int[6];

            for (int i = 0; i < 6; i++)
            {
                countOfEachPair[i] = countOfEachDice[i] / 2; //Antal par af 1'ere, 2'ere osv. Int dividering, så får floor værdi. Man kan jo ikke have et halvt par. 
            }

            for (int i = 5; i > -1; i--) //starter bag fra i par arrayet for at få de største par først. Dvs. prioritering fra 6,5,4,3,2,1.
            {
                if (countOfEachPair[i] > 0) //dvs. hvis der er et eller flere par.
                {
                    pairs.Add(i + 1); //fx ved par i index nummer 5, er det et par 6'ere som tilføjes til listen.
                                      //bemærk: fx 5555 tæller ikke som 2 par https://en.wikipedia.org/wiki/Yatzy 
                                      //derfor er det ligemeget om countOfEachPair[i] er 1 eller 2 eller 3. Der bliver kun tilføjet et par. 

                }
            }

            int score = 0;
            for (int i = 0; i < numberOfPairsToFind; i++) //Hvis vi skal finde 2 par, tager vi de første to par i den sorterede liste med par.
            {
                if (pairs.Count >= numberOfPairsToFind)
                {
                    score += pairs[i] * 2;  //og ganger med 2 for at få scoren for hvert par.
                }
            }


            AddToScoreboardOrPossibleOutcomes(userInput, score, AddToPossibleOutcomesInstead);

        }

        private void AddOfAKindToScoreboard(string userInput, int[] countOfEachDice, bool AddToPossibleOutcomesInstead = false)
        {
            int numberOfKindToFind = OfAKind[userInput];
            List<int> ofAKind = new List<int>();
            int score = 0;

            if (numberOfKindToFind < 6)  //altså hvis det er 3 ens eller 4 ens vi leder efter. (6 betyder 2*3 ens)
            {
                for (int i = 5; i > -1; i--)
                {
                    if (countOfEachDice[i] >= numberOfKindToFind) //fx hvis der er 3 ens ved femmere, og vi leder efter 3 ens, så kør.
                    {
                        ofAKind.Add(i + 1);
                    }

                }

                if (ofAKind.Count > 0) //undgå outofindex exception.
                {
                    score = ofAKind[0] * numberOfKindToFind; //fx ved 3 femmere, er scoren at gange det samme. 
                }
            }

            //else //hvis vi leder efter 2*3 ens.
            //{
            //    for (int i = 5; i > -1; i--)
            //    {
            //        if (countOfEachDice[i] == 3)
            //        {
            //            ofAKind.Add(i + 1);
            //        }

            //        else if (countOfEachDice[i] == 6)
            //        {
            //            ofAKind.Add(i + 1);
            //            ofAKind.Add(i + 1);
            //        }
            //    }

            //    if (ofAKind.Count > 1) //undgå outofindex exception.
            //    {
            //        score = (ofAKind[0] * 3) + (ofAKind[1] * 3); //fx ved 3 toere og 3 femmere er scoren 3*2 + 3*5 = 21.
            //    }

            //}


            AddToScoreboardOrPossibleOutcomes(userInput, score, AddToPossibleOutcomesInstead);
        }

        private void AddStraightToScoreboard(string userInput, int[] countOfEachDice, bool AddToPossibleOutcomesInstead = false)
        {
            int straightToFind = Straight[userInput];

            switch (straightToFind)
            {
                case 1: //1 = lille straight   //måske ret det her så case "lille straight":
                    bool smallStraightExists = true;

                    for (int i = 0; i < 5; i++)
                    { //altså hvis ikke der er slået 1,2,3,4,5 mindst en gang, er der ikke lille straight (false).
                        if (!(countOfEachDice[i] > 0))
                        {
                            smallStraightExists = false;
                        }
                    }

                    if (smallStraightExists)
                    {
                        int score = 15;
                        AddToScoreboardOrPossibleOutcomes(userInput, score, AddToPossibleOutcomesInstead);
                    }
                    break;

                case 2: //2 = store straight
                    bool bigStraightExists = true;

                    for (int i = 1; i < 6; i++)
                    {
                        if (!(countOfEachDice[i] > 0))
                        {
                            bigStraightExists = false;
                        }
                    }

                    if (bigStraightExists)
                    {
                        int score = 20;
                        AddToScoreboardOrPossibleOutcomes(userInput, score, AddToPossibleOutcomesInstead);
                    }
                    break;

                    //case 3: //fuld straight
                    //    if (countOfEachDice.Max() == 1) //altså hvis der højest er en af hver, må det betyde at der er fuld straight. Idet vi spiller med 6 terninger.
                    //    {
                    //        if (AddToPossibleOutcomesInstead)
                    //        {
                    //            PossibleOutcomes.Add(userInput);
                    //        }
                    //        else
                    //        {
                    //            spil.Score.Scores[userInput] = 21;
                    //        }
                    //    }
                    //    break;
            }

        }

        private void AddFuldHusToScoreboard(string userInput, int[] countOfEachDice, bool AddToPossibleOutcomesInstead = false)
        {
            int score = 0;
            List<int> threeOfAKindAndPair = new List<int>(); //[0] er 3 ens og [1] er 1 par. 


            for (int i = 5; i > -1; i--) //tilføjer alle de udfald der er 3 af. Fx hvis der er 3 6'ere bliver 6 føjet til listen.
            {
                if (countOfEachDice[i] >= 3)
                {
                    threeOfAKindAndPair.Add(i + 1); //plus 1 fordi index [5] jo er 6.
                }
            }

            if (threeOfAKindAndPair.Count > 0)
            {
                for (int i = 5; i > -1; i--) //tilføjer alle de udfald der indeholder et par, hvor parret ikke er lig med de 3 ens der i forvejen er i listen.
                {
                    if ((countOfEachDice[i] == 2) && !(threeOfAKindAndPair.Contains(i + 1)))
                    {
                        threeOfAKindAndPair.Add(i + 1);
                        score = threeOfAKindAndPair[0] * 3 + threeOfAKindAndPair[1] * 2; //nu har vi 2 i arrayet og kan derfor udregne scoren.
                    }
                }

            }


            AddToScoreboardOrPossibleOutcomes(userInput, score, AddToPossibleOutcomesInstead);
        }

        private void AddChanceToScoreboard(string userInput, int[] countOfEachDice, bool AddToPossibleOutcomesInstead = false)
        {//selvom man altid kan få chance skal vi stadig tilføje den til possibleoutcomes (addtopossibleoutcomesinstead) pga metoden .Clear()
            int score = 0;

            for (int i = 0; i < 6; i++)
            {
                score += countOfEachDice[i] * (i + 1); //altså hvis der er 4 toere er regnestykket: 4 * (1 +1) = 4 * 2 = 8
            }

            AddToScoreboardOrPossibleOutcomes(userInput, score, AddToPossibleOutcomesInstead);
        }

        private void AddYatzyToScoreboard(string userInput, int[] countOfEachDice, bool AddToPossibleOutcomesInstead = false)
        {
            if (countOfEachDice.Max() == 6)
            {
                int score = 50;

                AddToScoreboardOrPossibleOutcomes(userInput, score, AddToPossibleOutcomesInstead);
            }
        }


        private void AddToScoreboardOrPossibleOutcomes(string userInput, int score, bool AddToPossibleOutcomesInstead)
        {
            if (AddToPossibleOutcomesInstead) //altså hvis de blot skal tilføjes til listen over mulige udfald, så skal selve scoren jo ikke ændres.
            {
                if (score > 0)
                {
                    PossibleOutcomes.Add(userInput);
                }
            }
            else
            {
                Scores[userInput] = score;  //selve scoren på scoreboardet.
            }
        }


    }
}
