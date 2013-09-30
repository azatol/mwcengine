using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            // blank, white p, white knight, white bishop, white rook, white queen, white king, blank, blank, black p, black knight, black bishop, black rook, black queen, black king, blank,
            // Pawn neighbor bonus, Piece advanced rank bonus.

            // Pawn structure player 1
            Engine e = new Engine(new decimal[] { 0, 0.5m, 7m, 6m, 12m, 9m, 14m, 0m, 0m, -0.5m, -7m, -6m, -12m, -9m, -14m, 0m }, 0.5m, 0.1m);
            //ConsolePlayer p = new ConsolePlayer();

            // Rank advancement player
            Engine e2 = new Engine(new decimal[] { 0, 0.5m, 7m, 6m, 12m, 9m, 14m, 0m, 0m, -0.8m, -7m, -6m, -12m, -9m, -14m, 0m }, 0.05m, 0.2m);

            ConsoleReferee r = new ConsoleReferee(e2, e);
            EndState end = r.play();

            if (end == EndState.WhiteWins)
            {
                Console.WriteLine("White wins.");
            }
            else if(end == EndState.BlackWins)
            {
                Console.WriteLine("Black wins.");
            }
            else if (end == EndState.Draw)
            {
                Console.WriteLine("Draw.");
            }

            Console.WriteLine("Press enter to leave the game.");

            string inp = Console.ReadLine();

            return;
        }
    }
}
