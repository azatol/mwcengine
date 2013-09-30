using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public class CannonAction : Action
    {

        // n/a  - pawn - knight - bishop - rook - queen - king
        public static bool[] allowedCannon = new bool[] { false, false, false, false, true, false, true };

        public static int[] rays = new int[] { -9, -8, -7, -1, 1, 7, 8, 9 };

        public int cannonCoordinates;
        public int targetCoordinates;
        public int actingColor;

        public int cannonRow;
        public int cannonColumn;
        public int targetRow;
        public int targetColumn;

        public CannonAction(int cannonCoordinates, int targetCoordinates, int actingColor)
        {
            this.cannonCoordinates = cannonCoordinates;
            this.targetCoordinates = targetCoordinates;
            this.actingColor = actingColor;

            this.cannonRow = Position.getRowIndex(cannonCoordinates);
            this.cannonColumn = Position.getColumnIndex(cannonCoordinates);
            this.targetRow = Position.getRowIndex(targetCoordinates);
            this.targetColumn = Position.getColumnIndex(targetCoordinates);
        }

        public bool isLegal(Position p)
        {
            //  cannon is legal if:

            // The ray starting at target going should only have one other piece occupying it, the target enemy.
            // cannon must be acting color, target must be opposing color

            if (!isTargetEnemyOccupied(p))
            {
                return false;
            }

            if (!doesFriendlyCannonExist(p))
            {
                return false;
            }

            if (!isLegalRay())
            {
                return false;
            }

            if (isCannonTargetProtected(p))
            {
                return false;
            }

            return true;
        }

        public Position transform(Position p)
        {
            Position n;

            n = p.removeAt(targetCoordinates);
            n.advanceParity();
            n.resetQuietTime();

            return n;
        }

        public void LMtransform(Position p, Position basePosition)
        {
            p.LMremoveAt(targetCoordinates, basePosition);
            p.advanceParity();
            p.resetQuietTime();
        }

        public string showNotation(Position p)
        {
            string targetAlg = StUtility.getAlgebraicCoords(targetCoordinates);
            string cannonAlg = StUtility.getAlgebraicCoords(cannonCoordinates);

            string notation = cannonAlg + " cannon " + targetAlg;

            return notation;
        }

        public static List<Action> getLegalCannonActions(Position p)
        {
            int color = p.getPriorityColor();

            List<int> cannons = p.getCannons(color);

            int pieceCount = 0;

            int foundAt = -1;

            int a;

            List<Action> m = new List<Action>();
            CannonAction temp;

            for (int i = 0; i < cannons.Count; i++)
            {
                for (int j = 0; j < rays.Length; j++)
                {
                    pieceCount = 0;
                    a = cannons[i];

                    while ((a = advanceAlongRay(a, rays[j])) != -1)
                    {
                        if (p.isSquareOccupied(a))
                        {
                            if (p.isOccupiedByColor(a, color))
                            {
                                pieceCount = 100;
                                break;
                            }

                            foundAt = a;
                            pieceCount++;

                        }
                    }

                    if (pieceCount == 1)
                    {
                        temp = new CannonAction(cannons[i], foundAt, color);
                        m.Add(temp);
                    }
                }
            }

            return m;
        }

        // ends up oob : -1
        public static int advanceAlongRay(int coor, int ray)
        {
            int a;

            int file = coor % 8;
            int raySide =  ((ray + 9) % 8) - 1;

            if (file == 0 && raySide == -1 || file == 7 && raySide == 1)
            {
                return -1;
            }

            a = coor + ray;

            if (a < 0 || a > 63)
            {
                return -1;
            }

            return a;
        }

        private bool isTargetEnemyOccupied(Position p)
        {
            bool res = p.isOccupiedByColor(targetCoordinates, 8 - actingColor);

            return res;
        }

        private bool doesFriendlyCannonExist(Position p)
        {
            bool res = p.isOccupiedByColor(cannonCoordinates, actingColor);

            return res;
        }

        private RayDirection getRayDirection()
        {
            int lr = Math.Sign(targetColumn - cannonColumn);
            int ud = Math.Sign(targetRow - cannonRow);

            int d = lr + 8 * ud;
            RayDirection dir = (RayDirection)d;

            if (dir == RayDirection.Down || dir == RayDirection.Up || dir == RayDirection.Left || dir == RayDirection.Right)
            {
                return dir;
            }
            else
            {
                // all diagonals involve +/- 1 x to +/- 1 y, so when taking the abs, their slope should be 1.
                int rowDiff = Math.Abs(cannonRow - targetRow);
                int columnDiff = Math.Abs(cannonColumn - targetColumn);

                if (rowDiff == columnDiff)
                {
                    return dir;
                }
                else
                {
                    return RayDirection.None;
                }
            }

        }

        private bool isLegalRay()
        {
            if(cannonCoordinates == targetCoordinates)
            {
                return false;
            }

            RayDirection dir = getRayDirection();

            return (dir != RayDirection.None);
        }

        private bool isCannonTargetProtected(Position p)
        {
            RayDirection dir = getRayDirection();

            // Consider illegal rays to be protected.
            if (dir == RayDirection.None)
            {
                return true;
            }

            int d = (int)dir;

            int udWalk;
            int lrWalk;

            decomposeRay(d, out udWalk, out lrWalk);
            
            int r = cannonRow + udWalk; 
            int c = cannonColumn + lrWalk;

            int contacts = 0;

            while (Position.onBoard(r, c))
            {
                if (p.isSquareOccupied(c + 8 * r))
                {
                    contacts++;        
                }

                if (contacts == 2)
                {
                    return true;
                }

                r += udWalk;
                c += lrWalk;
            }

            return (contacts != 1);
        }

        public static void decomposeRay(int ray, out int ud, out int lr)
        {
            int r = Math.Abs(ray);
            int s = Math.Sign(ray);

            int udTemp = 0;
            int lrTemp = 0;

            if (r > 4)
            {
                r -= 8;
                udTemp++;
            }

            lrTemp = r;

            udTemp *= s;
            lrTemp *= s;

            lr = lrTemp;
            ud = udTemp;
        }

        public static int LMgetCannonActions(Position p, Action[] actions)
        {
            int index = 16;
            int color = p.getPriorityColor();

            List<int> cannons = p.getCannons(color);

            int pieceCount;
            int a;
            int foundAt = 0;

            CannonAction ca;

            for (int i = 0; i < cannons.Count; i++)
            {
                for (int j = 0; j < rays.Length; j++)
                {
                    pieceCount = 0;
                    a = cannons[i];

                    while ((a = advanceAlongRay(a, rays[j])) != -1)
                    {
                        if (p.isSquareOccupied(a))
                        {
                            if (p.isOccupiedByColor(a, color))
                            {
                                pieceCount = 100;
                                break;
                            }

                            foundAt = a;
                            pieceCount++;
                        }
                    }

                    if (pieceCount == 1)
                    {
                        ca = (CannonAction)actions[index];
                        ca.actingColor = color;
                        ca.cannonCoordinates = cannons[i];
                        ca.targetCoordinates = foundAt;

                        index++;
                    }
                }
            }

            return index;
        }
    }

    // left-right axis : -1, 0, 1
    // down-up axis : -8, 0, 8
    public enum RayDirection
    {
        DownLeft = -9,
        Down = -8,
        DownRight = -7,
        Left = -1,
        None = 0,
        Right = 1,
        UpLeft = 7,
        Up = 8,
        UpRight = 9
    }
}
