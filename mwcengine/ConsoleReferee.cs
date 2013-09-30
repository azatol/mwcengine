using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public class ConsoleReferee
    {
        private Position canonical;
        private Agent[] agents;

        public ConsoleReferee(Agent white, Agent black)
        {
            agents = new Agent[2];
            agents[0] = white;
            agents[1] = black;

            canonical = new Position();
        }

        public EndState play()
        {
            Action next;
            Agent active;
            int index;
            EndState result;

            int turn = 1;
            string[] elip = new string[] { ".", "..." };

            while ((result = canonical.getGameStatus()) == EndState.None)
            {
                index = (canonical.getPriorityColor() >> 3);
                active = agents[index];
                next = active.chooseMove(canonical);

                if (next is MeleeAction)
                {
                    Console.WriteLine("Melee");
                }

                Console.WriteLine(turn.ToString() + elip[index] + " " + next.showNotation(canonical) + " " + DateTime.Now.ToLongTimeString());
                turn++;

                canonical = next.transform(canonical);
            }

            return result;
        }
    }
}
