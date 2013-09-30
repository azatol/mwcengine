using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public class MovementAction : Action
    {
        private int currentCoordinates;
        private int desiredCoordinates;
        private int actingColor;

        public MovementAction(int currentCoordinate, int desiredCoordinate, int actingColor)
        {
            this.currentCoordinates = currentCoordinate;
            this.desiredCoordinates = desiredCoordinate;
            this.actingColor = actingColor;
        }

        public bool isLegal(Position p)
        {
            int currentRow = Position.getRowIndex(currentCoordinates);
            int desiredRow = Position.getRowIndex(desiredCoordinates);
            int currentColumn = Position.getColumnIndex(currentCoordinates);
            int desiredColumn = Position.getColumnIndex(desiredCoordinates);

            // The acting color must be the one with priority
            if (p.getPriorityColor() != actingColor)
            {
                return false;
            }

            // coordinate values are outside the bounds of 0-63
            if (currentCoordinates < 0 || currentCoordinates > 63 || desiredCoordinates < 0 || desiredCoordinates > 63)
            {
                return false;
            }

            // You cannot move more than one row or column across.
            if (Math.Abs(currentRow - desiredRow) > 1 || Math.Abs(currentColumn - desiredColumn) > 1)
            {
                return false;
            }

            // You cannot stay where you started as part of a movement action.
            if (currentRow == desiredRow && currentColumn == desiredColumn)
            {
                return false;
            }

            // actingColor piece must be present at current coordinates
            if (!p.isOccupiedByColor(currentCoordinates, actingColor))
            {
                return false;
            }

            // opposingColor piece must not be present at desired coordinates
            if (p.isOccupiedByColor(desiredCoordinates, Position.getOpposingColor(actingColor)))
            {
                return false;
            }

            return true;
        }

        public Position transform(Position p)
        {
            Position n = p.moveTo(currentCoordinates, desiredCoordinates);
            n.advanceParity();
            n.advanceQuietTime();

            return n;
        }

        public void LMtransform(Position p, Position basePosition)
        {
            p.LMmoveTo(currentCoordinates, desiredCoordinates, basePosition);
            p.advanceParity();
            p.advanceQuietTime();
        }

        public string showNotation(Position p)
        {
            string currAlg = StUtility.getAlgebraicCoords(currentCoordinates);
            string desAlg = StUtility.getAlgebraicCoords(desiredCoordinates);

            string res = currAlg + "-" + desAlg;

            return res;
        }

        public static List<Action> getLegalMovementActions(Position p)
        {
            int color = p.getPriorityColor();

            List<Action> mActions = new List<Action>();
            List<int> ourPieces = p.getOccupiedByColorCoordList(color);
            List<int> dest;

            for (int i = 0; i < ourPieces.Count; i++)
            {
                dest = p.getLegalMoveDestinations(ourPieces[i]);

                mActions.AddRange(dest.ConvertAll(x => new MovementAction(ourPieces[i], x, color)));
            }

            return mActions;
        }

        public static int LMgetMovementActions(Position p, Action[] actions)
        {
            int color = p.getPriorityColor();

            int index = 64;
            MovementAction m;

            for (int i = 0; i < 64; i++)
            {
                if (!p.isOccupiedByColor(i, color))
                {
                    continue;
                }

                List<int> neigh = p.getNeighbors(i);

                for (int j = 0; j < neigh.Count; j++)
                {
                    if (p.isSquareOccupied(j))
                    {
                        continue;
                    }

                    m = (MovementAction)actions[index];
                    m.currentCoordinates = i;
                    m.desiredCoordinates = j;
                    m.actingColor = color;

                    index++;
                }
            }

            return index;
        }
    }
}
