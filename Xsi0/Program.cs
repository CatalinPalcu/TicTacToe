using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xsi0
{ 
    public enum PlayMode : byte
    {
        TwoPlayers=1,
        OnePlayerBeginer,
        OnePlayerIntermediate,
        OnePlayerExpert
    }
    class Program
    {
        static void Main(string[] args)
        {
            Thread t = new Thread(() => Play());
            t.Start();

        }

        public static void Play()
        {
            char keepGoing;
            do
            {
                Game game1 = new Game();
                int mod = 0;
                do
                {
                    Console.WriteLine("Select the mode that you want to play:");
                    Console.WriteLine("\t1 for Two Players");
                    Console.WriteLine("\t2 for One Player - beginer");
                    Console.WriteLine("\t3 for One Player - intermediate");
                    Console.WriteLine("\t4 for One Player - expert");
                } while (!Int32.TryParse(Console.ReadLine(), out mod) || mod < 1 || mod > 4);

                game1.PlayGame((PlayMode)mod);

                Console.WriteLine("Press 'y' and enter to play another game");
                keepGoing = Console.ReadLine()[0];
            } while (keepGoing == 'y');
        }

    }

    public class Game
    {
        private static char[] values = { ' ', 'X', 'O' };
        private int[,] matrix = new int[3, 3];
        private int nrOfMoves;

        public Game()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    matrix[i, j] = 0;
            nrOfMoves = 0;
        }

        private bool ValidateMove(int i, int j, int player)
        {
            if (matrix[i,j]==0)
            {
                matrix[i, j] = player;
                nrOfMoves++;
                return true;
            }
            return false;
        }

        private bool CanContinue
        {
            get
            {
                return nrOfMoves < 9;
            }
        }

        private int EmptySpaceLine(int l)
        {
            int c = 0;
            while (c <3)
            {
                if (matrix[l, c] == 0)
                    return c;
                c++;
            }
            return -1;
        }

        private int EmptySpaceColumn(int c)
        {
            int l = 0;
            while (l<3)
            {
                if (matrix[l, c] == 0)
                    return l;
                l++;
            }
            return -1;
        }

        private int EmptySpacePD()
        {
            int i = 0;
            while (i<3)
            {
                if (matrix[i, i] == 0)
                    return i;
                i++;
            }
            return -1;
        }

        private int EmptySpaceSD()
        {
            int i = 0;
            while(i<3)
            {
                if (matrix[i, 2 - i] == 0)
                    return i;
                i++;
            }
            return -1;
        }
        private int WinInOneMove(int player)
        {
            int sum = 0;
            int i, j;
            // verific sansa de castig pe linie
            for (i = 0; i < 3; i++)
            {
                sum = 0;
                for (j = 0; j < 3; j++)
                    if (matrix[i, j] == player)
                        sum += player;
                if (sum == 2*player && EmptySpaceLine(i) != -1)
                    return i * 3 + EmptySpaceLine(i);
            }
            // verific sansa de castig pe coloana
            for (j = 0; j < 3; j++)
            {
                sum = 0;
                for (i = 0; i < 3; i++)
                    if (matrix[i, j] == player)
                        sum += player;
                if (sum == 2*player && EmptySpaceColumn(j) != -1)
                    return EmptySpaceColumn(j) * 3 + j;
            }
            //verific sansa de castig pe diagonala principala
            sum = 0;
            for (i = 0; i < 3; i++)
                if (matrix[i, i] == player)
                    sum += player;
            if (sum == 2*player && EmptySpacePD() != -1)
                return EmptySpacePD() * 4;

            //verific sansa de castig pe diagonala secundara
            sum = 0;
            for (i = 0; i < 3; i++)
                if (matrix[i, 2 - i] == player)
                    sum += player;
            if (sum == 2*player && EmptySpaceSD() != -1)
                return EmptySpaceSD() * 2 + 2;

            // player-ul nu poate castiga jocul dintr-o mutare

            return -1;
        }

        private int WinInTwoMoves(int player)
        {
            Random r = new Random();
            int poz,i,j;
            int oponent = player % 2 + 1;
            if (this.nrOfMoves > 6) // am nevoie de 2 mutari ale lui player si o mutare a concurentului
                return -1;
       
            if (this.nrOfMoves == 0)
            {
                return r.Next(0, 5) * 2;
            }
            if (this.nrOfMoves == 1)
            {
                if (matrix[1, 1] == 0)
                    return 4;
                else
                {
                    do
                    {
                        poz = r.Next(0, 5) * 2;
                    } while (poz == 4);
                    return poz;
                }
            }

            if (nrOfMoves == 2)
            {
                if (matrix[1,1] == 0)
                {
                    i = 0;
                    while (i<=2)
                    {
                        j = 0;
                        while (j<=2)
                        {
                            if (matrix[i, j] == 0 && matrix[i, 2 - j] == player && matrix[i, 1] == 0)
                                return 3 * i + j;
                            if (matrix[i, j] == 0 && matrix[2 - i, j] == player && matrix[1, j] == 0)
                                return 3 * i + j;
                            j += 2;
                        }
                        i += 2;
                    }
                } else if (matrix[1,1] == player)
                {
                    poz = ColtCompletat(player%2+1);
                    if (poz != -1)
                        return 8 - poz;
                    else
                    {
                        do
                        {
                            poz = r.Next(0, 5) * 2;
                        } while (poz == 4);
                        return poz;
                    }
                }
                else
                {
                    return 8 - ColtCompletat(player);
                }
            } else if (nrOfMoves == 3)
            {
                if (matrix[1, 1] == player)
                {
                    if ((matrix[0, 0] == oponent && matrix[2, 2] == oponent) || (matrix[2, 0] == oponent && matrix[0, 2] == oponent))
                    {
                        return r.Next(0, 4) * 2 + 1;
                    }
                    else if ((matrix[1, 0] == oponent && matrix[1, 2] == oponent) || (matrix[0, 1] == oponent && matrix[2, 1] == oponent))
                    {
                        do
                        {
                            poz = r.Next(0, 5) * 2;
                        } while (poz == 4);
                        return poz;
                    }
                    else
                    {
                        // cautam celulele completate de adversar
                        int l1=0, c1;
                        while (l1 <3)
                        {
                            c1 = (l1 + 1) % 2;
                            while (c1 < 3)
                            {
                                if (matrix[l1, c1] == oponent)
                                    break;
                                c1 += 2;
                            }
                            if (c1 < 3 && matrix[l1, c1] == oponent)
                                break;
                            l1++;
                        }

                        // de completat
                    }
                }
                else if (matrix[1, 1] == 0)
                    return 4;
                else
                {
                    do
                    {
                        i = r.Next(0, 2) * 2;
                        j = r.Next(0, 2) * 2;
                    } while (matrix[i, j] != 0);
                    return 3 * i + j;
                }
            }

            i = 0;
            while (i <= 2)
            {
                j = 0;
                while (j <= 2)
                {
                    if (matrix[i, j] == 0 && matrix[i, 2 - j] == player && matrix[i, 1] == 0)
                        return 3 * i + j;
                    if (matrix[i, j] == 0 && matrix[2 - i, j] == player && matrix[1, j] == 0)
                        return 3 * i + j;
                    j += 2;
                }
                i += 2;
            }

            if (matrix[1, 1] == 0)
                return 4;



            return -1;
        }

        public int ColtCompletat(int player)
        {
            int i = 0,
                j;

            while (i<=2)
            {
                j = 0;
                while (j<=2)
                {
                    if (matrix[i, j] == player)
                        return 3 * i + j;
                    j += 2;
                }
                i += 2;
            }
            return -1;
        }

        private int MakeAMove(PlayMode mod)
        {
            int poz;

            if (mod == PlayMode.OnePlayerIntermediate || mod == PlayMode.OnePlayerExpert)
            {
                // verificam daca calculatorul poate castiga dintr-o mutare
                poz = WinInOneMove(2);
                if (poz != -1)
                    return poz;
                // verificam daca player 1 poate castiga dintr-o mutare
                poz = WinInOneMove(1);
                if (poz != -1)
                    return poz;
            }

            if (mod == PlayMode.OnePlayerExpert)
            {
                poz = WinInTwoMoves(2);
                if (poz != -1)
                    return poz;
            }

            // alegem o pozitie random, neocupata
            Random r = new Random();
            int l, c;
            do
            {
                l = r.Next(0, 12) % 3 ;
                c = r.Next(0, 12) % 3 ;
            } while (matrix[l,c] != 0);

            return l*3+c;
        }

        private bool HaveWinner(int l, int c)
        {
            if (matrix[l, c] == 0)
                return false;
            if (matrix[l, 0] == matrix[l, 1] && matrix[l, 1] == matrix[l, 2])
                return true;
            if (matrix[0, c] == matrix[1, c] && matrix[1, c] == matrix[2, c])
                return true;
            if (l == c)
                if (matrix[0, 0] == matrix[1, 1] && matrix[1, 1] == matrix[2, 2])
                    return true;
            if (l + c == 2)
                if (matrix[0, 2] == matrix[1, 1] && matrix[1, 1] == matrix[2, 0])
                    return true;
            
            return false;
        }

        public void ShowTable()
        {
            Console.WriteLine();
            for (int i=0; i<2;i++)
            {
                Console.Write(" {0} | {1} | {2}",values[matrix[i,0]],values[matrix[i,1]],values[matrix[i,2]]);
                Console.WriteLine("\n - | - | -");
            }
            Console.Write(" {0} | {1} | {2}\n\n", values[matrix[2, 0]], values[matrix[2, 1]], values[matrix[2, 2]]);
        }



        public void PlayGame(PlayMode mod = PlayMode.TwoPlayers)
        {
            int poz;
            if (mod == PlayMode.TwoPlayers)
                Console.WriteLine("Character for player 1: {0} \tCharacter for player2: {1}",values[1],values[2]);
            else
                Console.WriteLine("You are player 1. Your character is:{0}",values[1]);
            Console.WriteLine("Insert the number of line, number of column to make a move");
            Random r = new Random();
            int player = r.Next(0,10);
            int l = 1,
                c = 1;
            while (this.CanContinue && !this.HaveWinner(l-1,c-1))
            {
                player = player % 2 + 1;
                ShowTable();
                if (mod == PlayMode.TwoPlayers || player == 1)
                {
                    do
                    {
                        Console.WriteLine("Player{0} make a valid move:", player);
                        do
                        {
                            Console.Write("Line:");
                        } while (!Int32.TryParse(Console.ReadLine(), out l) || l < 1 || l > 3);
                        do
                        {
                            Console.Write("Column:");
                        } while (!Int32.TryParse(Console.ReadLine(), out c) || c < 1 || c > 3);
                    } while (!this.ValidateMove(l - 1, c - 1, player));
                } else
                {
                    poz = MakeAMove(mod);
                    c = poz % 3 + 1;
                    l = poz / 3 + 1;
                    this.ValidateMove(l - 1, c - 1, player);
                }
            }

            ShowTable();

            if (this.HaveWinner(l - 1, c - 1))
            {   if (mod == PlayMode.TwoPlayers || player == 1)
                    Console.WriteLine("\n\n\t\tCONGRATULATIONS PLAYER{0}, you just win the game level:{1}", player,mod.ToString());
                else
                    Console.WriteLine("\n\n\t\tComputer just win the game :(");
            }
            else
                Console.WriteLine("\n\n\t\tGood game. This was a draw");
        }

        
    }
}
