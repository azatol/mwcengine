using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public class Position
    {
        // 0,1,2 - White's 1st, 2nd, 3rd action
        // 3,4,5 - Black's 1st, 2nd, 3rd action
        // After Black's 3rd, parity resets to 0
        private int parity;

        private int quietTime;  // 120 means a draw (20 action sets each)

        // White: 0x0
        // Black: 0x8
        // Pawn: 0x1
        // Knight: 0x2
        // Bishop: 0x3
        // Rook: 0x4
        // Queen: 0x5
        // King: 0x6
        // Piece value = color + piece
        // Test color: value & 0x8
        // Value 0x0 : empty

        //black back row:  56 57 58 59 60 61 62 63
        //black front row: 48 49 50 51 52 53 54 55
        //white front row:  8  9 10 11 12 13 14 15
        //white back row:   0  1  2  3  4  5  6  7
        private List<int> board;

        public static ulong[] powers = new ulong[] {0x1UL, 0x2UL, 0x4UL, 0x8UL, 0x10UL, 0x20UL, 0x40UL, 0x80UL, 0x100UL, 0x200UL, 0x400UL, 0x800UL, 0x1000UL, 0x2000UL, 0x4000UL, 0x8000UL, // 15
                                                    0x10000UL, 0x20000UL, 0x40000UL, 0x80000UL, 0x100000UL, 0x200000UL, 0x400000UL, 0x800000UL, 0x1000000UL, 0x2000000UL, 0x4000000UL,      // 26
                                                    0x8000000UL, 0x10000000UL, 0x20000000UL, 0x40000000UL, 0x80000000UL, 0x100000000UL, 0x200000000UL, 0x400000000UL, 0x800000000UL,        // 35
                                                    0x1000000000UL, 0x2000000000UL, 0x4000000000UL, 0x8000000000UL, 0x10000000000UL, 0x20000000000UL, 0x40000000000UL, 0x80000000000UL,     // 43
                                                    0x100000000000UL, 0x200000000000UL, 0x400000000000UL, 0x800000000000UL, 0x1000000000000UL, 0x2000000000000UL, 0x4000000000000UL,        // 50
                                                    0x8000000000000UL, 0x10000000000000UL, 0x20000000000000UL, 0x40000000000000UL, 0x80000000000000UL, 0x100000000000000UL,                 // 56
                                                    0x200000000000000UL, 0x400000000000000UL, 0x800000000000000UL, 0x1000000000000000UL, 0x2000000000000000UL, 0x4000000000000000UL,        // 62
                                                    0x8000000000000000UL };                                                                                                                // 63

        const ulong notAFile = 0xfefefefefefefefeUL; // ~0x0101010101010101
        const ulong notHFile = 0x7f7f7f7f7f7f7f7fUL; // ~0x8080808080808080

        public Dictionary<int, List<int>> cannons;
        public HashSet<int> whitePawns;
        public HashSet<int> blackPawns;

        public static Dictionary<int, List<int>> neighbors;

        // Default constructor builds initial position
        public Position()
        {
            parity = 0;
            quietTime = 0;

            board = new List<int>(64)
            { 
                0x4, 0x2, 0x3, 0x5, 0x6, 0x3, 0x2, 0x4,
                0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1,
                0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
                0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
                0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
                0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
                0x9, 0x9, 0x9, 0x9, 0x9, 0x9, 0x9, 0x9,
                0xC, 0xA, 0xB, 0xD, 0xE, 0xB, 0xA, 0xC 
            };

            cannons = new Dictionary<int, List<int>>();
            cannons[0] = new List<int>() { 0, 4, 7 };
            cannons[8] = new List<int>() { 56, 60, 63 };

            whitePawns = new HashSet<int>() { 8, 9, 10, 11, 12, 13, 14, 15 };
            blackPawns = new HashSet<int>() { 48, 49, 50, 51, 52, 53, 54, 55 };

            neighbors = setupNeighborDictionary();
        }

        public Position(int parity, List<int> board, int quietTime, Dictionary<int, List<int>> cannons, HashSet<int> whitePawns, HashSet<int> blackPawns)
        {
            if (parity < 0 || parity > 5)
            {
                throw new ArgumentException("Parity error: " + parity);
            }

            if (board == null || board.Count != 64)
            {
                throw new ArgumentException("Board error");
            }

            this.parity = parity;
            this.board = new List<int>(board);
            this.quietTime = quietTime;

            this.cannons = new Dictionary<int, List<int>>();
            this.cannons[0] = new List<int>(cannons[0]);
            this.cannons[8] = new List<int>(cannons[8]);

            this.whitePawns = new HashSet<int>(whitePawns);
            this.blackPawns = new HashSet<int>(blackPawns);
        }

        public List<int> getCannons(int color)
        {
            return cannons[color];
        }

        public List<Position> generatePositions()
        {
            List<Action> actions = generateActions();
            List<Position> positions = new List<Position>();

            for (int i = 0; i < actions.Count; i++)
            {
                positions.Add(actions[i].transform(this));
            }

            return positions;
        }

        private decimal getPieceScore(IEngine e)
        {
            decimal score = 0.0m;

            for (int i = 0; i < 64; i++)
            {
                score += e.getPieceScore(board[i]);
            }

            return score;
        }

        private decimal getPositionalScore(IEngine e)
        {
            decimal score = 0.0m;

            List<int> whitePawnsList = whitePawns.ToList();
            List<int> blackPawnsList = blackPawns.ToList();

            List<int> pawnNeighbors;

            decimal pawnNWeight = e.getPawnNeighborWeight();

            for (int i = 0; i < whitePawnsList.Count; i++)
            {
                pawnNeighbors = Position.neighbors[whitePawnsList[i]];

                for (int j = 0; j < pawnNeighbors.Count; j++)
                {
                    if (whitePawns.Contains(pawnNeighbors[j]))
                    {
                        score += pawnNWeight;
                    }
                }
            }

            for (int i = 0; i < blackPawnsList.Count; i++)
            {
                pawnNeighbors = Position.neighbors[blackPawnsList[i]];

                for (int j = 0; j < pawnNeighbors.Count; j++)
                {
                    if (blackPawns.Contains(pawnNeighbors[j]))
                    {
                        score -= pawnNWeight;
                    }
                }
            }

            return score;
        }

        private decimal getAdvancedRankScore(IEngine e)
        {
            decimal score = 0.0m;
            int v;
            int rw;
            int c;

            decimal advWeight = e.getRankAdvancedWeight();

            for (int i = 0; i < 64; i++)
            {
                v = board[i];

                if (v == 0)
                {
                    continue;
                }

                rw = i / 8;
                c = v & 8;

                if (c == 0)
                {
                    score += rw * advWeight;
                }
                else if (c == 8)
                {
                    score += (rw - 7) * advWeight;
                }
            }

            return score;
        }

        public decimal evaluate(IEngine e)
        {
            decimal score = 0.0m;

            score += getPieceScore(e);

            score += getPositionalScore(e);

            score += getAdvancedRankScore(e);

            return score;
        }

        public List<Action> generateActions()
        {
            List<Action> actions = MovementAction.getLegalMovementActions(this);
            actions.AddRange(MeleeAction.getLegalMeleeActions(this));
            actions.AddRange(CannonAction.getLegalCannonActions(this));

            return actions;
        }

        public void LMgenerateActions(Action[] actionSet, out int movementEdge, out int meleeEdge, out int cannonEdge)
        {         
            movementEdge = MovementAction.LMgetMovementActions(this, actionSet);
            meleeEdge = MeleeAction.LMgetMeleeActions(this, actionSet);
            cannonEdge = CannonAction.LMgetCannonActions(this, actionSet);
        }

        public void advanceParity()
        {
            parity++;

            if (parity > 5)
            {
                parity = 0;
            }
        }

        public void resetQuietTime()
        {
            quietTime = 0;
        }

        public void advanceQuietTime()
        {
            quietTime++;
        }

        public EndState getGameStatus()
        {
            ulong whiteBoard = getOccupiedByColorBitBoard(0x0);
            ulong blackBoard = getOccupiedByColorBitBoard(0x8);

            if (quietTime > 120)
            {
                throw new Exception("Invalid position state: Quiet Time has advanced beyond the maximum allowed without a draw being declared");
            }

            if (whiteBoard == 0UL && blackBoard == 0UL)
            {
                throw new Exception("Invalid position state: Both boards are empty. No sequence of moves could lead to this state.");
            }

            if (quietTime == 120)
            {
                return EndState.Draw;
            }

            if (whiteBoard == 0UL)
            {
                return EndState.BlackWins;
            }

            if (blackBoard == 0UL)
            {
                return EndState.WhiteWins;
            }

            return EndState.None;
            
        }

        public int getPriorityColor()
        {
            if (parity >= 3)
            {
                return 0x8;
            }
            else
            {
                return 0x0;
            }
        }

        public int getColor(int coor)
        {
            return (board[coor] & 0x8);
        }

        public int getPiece(int coor)
        {
            return (board[coor] % 8);
        }

        public int getValue(int coor)
        {
            return (board[coor]);
        }

        public bool isSquareOccupied(int coor)
        {
            return (board[coor] != 0L);
        }

        public bool isOccupiedByColor(int coor, int color)
        {
            long v = board[coor];

            if (v == 0L)
            {
                return false;
            }

            long c = v & 0x8L;

            return (c == color);
        }

        // 0-7 (a-h traditional)
        public static int getColumnIndex(int coor)
        {
            return (coor % 8);
        }

        // 0-7 (1-8 traditional)
        public static int getRowIndex(int coor)
        {
            return (coor / 8);
        }

        public static int getOpposingColor(int color)
        {
            return (8 - color);
        }

        public Position removeAt(int coor)
        {
            Position n = new Position(parity, board, quietTime, cannons, whitePawns, blackPawns);

            n.cannons[0].Remove(coor);
            n.cannons[8].Remove(coor);

            n.whitePawns.Remove(coor);
            n.blackPawns.Remove(coor);

            n.board[coor] = 0x0;

            return n;
        }

        public void LMremoveAt(int coor, Position basePosition)
        {
            // How to handle "cannons", whitePawns, blackPawns in low memory environment?
            parity = basePosition.parity;
            quietTime = basePosition.quietTime;
            
            cannons = new Dictionary<int, List<int>>();
            cannons[0] = new List<int>(basePosition.cannons[0]);
            cannons[8] = new List<int>(basePosition.cannons[8]);

            whitePawns = new HashSet<int>(basePosition.whitePawns);
            blackPawns = new HashSet<int>(basePosition.blackPawns);

            for (int i = 0; i < 64; i++)
            {
                board[i] = basePosition.board[i];
            }

            board[coor] = 0;
            cannons[0].Remove(coor);
            cannons[8].Remove(coor);

            whitePawns.Remove(coor);
            blackPawns.Remove(coor);
        }

        public Position moveTo(int currentCoor, int desiredCoor)
        {
            int color = getPriorityColor();

            Position n = new Position(parity, board, quietTime, cannons, whitePawns, blackPawns);

            if (n.cannons[color].Contains(currentCoor))
            {
                n.cannons[color].Remove(currentCoor);
                n.cannons[color].Add(desiredCoor);
            }

            if (n.whitePawns.Contains(currentCoor))
            {
                n.whitePawns.Remove(currentCoor);
                n.whitePawns.Add(desiredCoor);
            }
            else if (n.blackPawns.Contains(currentCoor))
            {
                n.blackPawns.Remove(currentCoor);
                n.blackPawns.Add(desiredCoor);
            }

            n.board[desiredCoor] = n.board[currentCoor];
            n.board[currentCoor] = 0x0;

            return n;
        }

        public void LMmoveTo(int currentCoor, int desiredCoor, Position basePosition)
        {
            parity = basePosition.parity;
            quietTime = basePosition.quietTime;

            int color = getPriorityColor();

            cannons = new Dictionary<int, List<int>>();
            cannons[0] = new List<int>(basePosition.cannons[0]);
            cannons[8] = new List<int>(basePosition.cannons[8]);

            whitePawns = new HashSet<int>(basePosition.whitePawns);
            blackPawns = new HashSet<int>(basePosition.blackPawns);

            for (int i = 0; i < 64; i++)
            {
                board[i] = basePosition.board[i];
            }

            if (cannons[color].Contains(currentCoor))
            {
                cannons[color].Remove(currentCoor);
                cannons[color].Add(desiredCoor);
            }

            if (whitePawns.Contains(currentCoor))
            {
                whitePawns.Remove(currentCoor);
                whitePawns.Add(desiredCoor);
            }
            else if (blackPawns.Contains(currentCoor))
            {
                blackPawns.Remove(currentCoor);
                blackPawns.Add(desiredCoor);
            }

            board[desiredCoor] = board[currentCoor];
            board[currentCoor] = 0;
        }

        public Dictionary<int, List<int>> setupNeighborDictionary()
        {
            var neigh = new Dictionary<int, List<int>>();
            List<int> temp;

            for (int i = 0; i < 64; i++)
            {
                temp = generateNeighborCoords(i);
                neigh.Add(i, temp);
            }

            return neigh;
        }

        private List<int> generateNeighborCoords(int coor)
        {
            List<int> n = new List<int>();
            
            int p;

            for (int i = 0; i < CannonAction.rays.Length; i++)
            {
                p = CannonAction.advanceAlongRay(coor, CannonAction.rays[i]);

                if (p == -1)
                {
                    continue;
                }
                else
                {
                    n.Add(p);
                }
            }

            return n;
        }

        public List<int> getNeighbors(int coor)
        {
            return neighbors[coor];
        }

        // 1 in each square where a piece of "color" could move to.
        // 0 in any square they could not move to.
        public List<int> generatePotentialMovementList(int color)
        {
            ulong them = getOccupiedByColorBitBoard(8 - color);
            ulong m = getOccupiedByColorBitBoard(color);

            ulong any = m | them;

            ulong movement = kingAttacks(m);

            // remove movements that are already occupied by something else.

            ulong notany = ~any;

            movement = movement & notany;

            List<int> coords = getCoorListFromBitBoard(movement);
            return coords;
        }

        public List<int> getEnemiesInContact(int color)
        {
            int enemyColor = color ^ 0x8;

            ulong ourPieces = getOccupiedByColorBitBoard(color);
            ulong theirPieces = getOccupiedByColorBitBoard(enemyColor);

            ulong ourNeighbors = kingAttacks(ourPieces);

            ulong contacts = ourNeighbors & theirPieces;

            List<int> coords = getCoorListFromBitBoard(contacts);

            return coords;
        }

        public List<int> getOccupiedByColorCoordList(int color)
        {
            List<int> temp = new List<int>();

            for (int i = 0; i < 64; i++)
            {
                if (board[i] == 0)
                {
                    continue;
                }

                if ((board[i] & 8) == color)
                {
                    temp.Add(i);
                }
            }

            return temp;
        }

        // From coor to where?
        public List<int> getLegalMoveDestinations(int coor)
        {
            List<int> lm = new List<int>();

            int p;
            int v;

            List<int> n = neighbors[coor];

            for (int i = 0; i < n.Count; i++)
            {
                p = n[i];
                v = board[p];

                if (v == 0)
                {
                    lm.Add(p);
                }
            }

            return lm;
        }

        public ulong getOccupiedByColorBitBoard(int color)
        {
            ulong m = 0UL;

            int mask = 0x8;
            int val;

            for (int i = 0; i < 64; i++)
            {
                val = board[i];

                if (val == 0)
                {
                    continue;
                }

                val = board[i] & mask;

                if (val == color)
                {
                    m = m | powers[i];
                }
            }

            return m;
        }

        public List<int> getCoorListFromBitBoard(ulong bitBoard)
        {
            ulong val;

            List<int> coords = new List<int>();

            for(int i = 0; i < 64; i++)
            {
                val = bitBoard & powers[i];
    
                if(val != 0)
                {
                    coords.Add(i);
                }
            }

            return coords;
        }

        public static ulong kingAttacks(ulong kingSet) 
        {
           ulong attacks = eastOne(kingSet) | westOne(kingSet);
           kingSet    |= attacks;
           attacks    |= nortOne(kingSet) | soutOne(kingSet);
           return attacks;
        }

        public static ulong eastOne (ulong b) {return (b << 1) & notAFile;}
        public static ulong noEaOne (ulong b) {return (b << 9) & notAFile;}
        public static ulong soEaOne (ulong b) {return (b >> 7) & notAFile;}
        public static ulong westOne (ulong b) {return (b >> 1) & notHFile;}
        public static ulong soWeOne (ulong b) {return (b >> 9) & notHFile;}
        public static ulong noWeOne (ulong b) {return (b << 7) & notHFile;}

        public static ulong soutOne (ulong b) {return  b >> 8;}
        public static ulong nortOne (ulong b) {return  b << 8;}

        public int countFriendlyPawnNeighbors(int coor, int color)
        {
            List<int> neighborValues = neighbors[coor];

            int count = 0;
            int val;
            int ideal;

            for (int i = 0; i < neighborValues.Count; i++)
            {
                val = board[neighborValues[i]];

                ideal = color + 1;

                if (val == ideal)
                {
                    count++;
                }
            }

            return count;
        }

       

        public static bool onBoard(int row, int column)
        {
            if (row < 0 || row > 7)
            {
                return false;
            }

            if (column < 0 || column > 7)
            {
                return false;
            }

            return true;
        }

        public static bool atOtherLocation(int activeRow, int activeColumn, int otherRow, int otherColumn)
        {
            return (activeRow == otherRow && activeColumn == otherColumn);
        }

        public override string ToString()
        {
            string text = "";

            int color = getPriorityColor();
            int actionsLeft = 3 - (parity % 3);
            string colorName;

            if (color == 0)
            {
                colorName = "White's";
            }
            else
            {
                colorName = "Black's";
            }

            text += colorName + " turn. " + actionsLeft + " actions left. \n ";
            text += "Quiet Time (120 = draw): " + quietTime + "\n ";
            
            for(int r = 0; r < 8; r++)
            {
                for(int c = 0; c < 8; c++)
                {
                    text += " " + board[r * 8 + c].ToString();
                }

                text += "\n";
            }

            return text;
        }
    }
}
