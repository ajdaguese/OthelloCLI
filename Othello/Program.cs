using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    class Program
    {
        static void Main(string[] args)
        {
            //the board -1 means p1 is occupying, 0 means it is unoccupied and 1 means p2 is occupying
            int[,] board = new int[8,8];
            //initilize the board to empty
            board = setUpBoard(board);
            //a boolean for looping input cycles until the users' input is in the correct format
            Boolean inputBool = false;
            //indicates if the game is done, 0 if game is in progress, 2 if the result was a tie otherwise it is the playernum of the winning player
            int gameFinished = 0;
            //set to -1 if the player is first and 1 if the player is second
            int playerNum = 0;
            //keeps track of whose turn it is, -1 always goes first
            int currentTurn = -1;
            int difficulty = 0;
            while (!inputBool)
            {
                Console.WriteLine("Please enter difficulty level 1-10:");
                String diff = Console.ReadLine();
                try
                {
                    difficulty = Int32.Parse(diff);
                }
                catch(Exception e)
                {

                }
                if(difficulty > 0 && difficulty < 11)
                {
                    inputBool = true;
                }
                else
                {
                    Console.WriteLine("Incorrect difficulty input");
                }
            }
            inputBool = false;
            while(!inputBool)
            {
                Console.WriteLine("Do you want to go First or Second? Input f or s");
                String fors = Console.ReadLine();
                if (fors[0] == 'f')
                {
                    playerNum = -1;
                    inputBool = true;
                }
                else if (fors[0] == 's')
                {
                    playerNum = 1;
                    inputBool = true;
                }
                else
                {
                    Console.WriteLine("Please enter f or s");
                }
                while(gameFinished == 0)
                {
                    //starts inputBool as false at the beginning of the loop, it is needed for user input
                    inputBool = false;
                    if (currentTurn == playerNum)
                    {
                        //if there are no possible moves, skip players turn
                        if (possibleMoves(board, playerNum).Count == 0)
                        {
                            Console.WriteLine("You have no possible moves, skipping turn");
                            inputBool = true;

                        }
                        while (!inputBool)
                        {
                            writeBoard(board);
                            Console.WriteLine("please enter move as x and y corrdinates in the format \"x y\"");
                            
                            //the try catches any exceptions with reading values in and converting them to ints
                            try
                            {
                                String userMove = Console.ReadLine();
                                int x = (int)(Char.GetNumericValue(userMove[2]));
                                int y = (int)(Char.GetNumericValue(userMove[0]));
                                //if one of the input numbers is out of bounds...
                                if (x < 1 || x > 8 || y < 1 || y > 8)
                                {
                                    Console.WriteLine("That move is out of bounds, please enter only numbers 1-8");
                                }
                                else
                                {
                                    Tuple<int, int> move = new Tuple<int, int>(x, y);
                                    if (moveValid(board, move, playerNum))
                                    {
                                        board = executeMove(board, move, playerNum);
                                        inputBool = true;
                                        gameFinished = checkWin(board, playerNum);
                                    }
                                    else
                                    {
                                        Console.WriteLine("This move is invalid, please try another");
                                    }
                                }
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("There was an error with your input, please use the format \"X Y\" with numbers 1-8");
                            }
                        }
                        Console.WriteLine("Move made, current board: ");
                        writeBoard(board);
                    }
                    else
                    {
                        //The player num for the AI should be the currentTurn number or else this method will not be called
                        if (possibleMoves(board, currentTurn).Count > 0)
                        {
                            board = executeMove(board, gameAI(board, difficulty, currentTurn, -500, 500).Item1, currentTurn);
                            writeBoard(board);
                            gameFinished = checkWin(board, currentTurn);
                        }
                        else
                        {
                            Console.WriteLine("Opponent could not make a move");
                        }
                    }
                    //changes turn
                    currentTurn *= -1;
                }
                if(gameFinished == playerNum)
                {
                    Console.WriteLine("You Win!");
                }
                else
                {
                    Console.WriteLine("Your opponent won");
                }
                Console.ReadKey();
            }
        }
        private static int[,] setUpBoard(int[,] b)
        {
            //initilize the board to empty
            for (int i = 0; i < b.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    b[i, j] = 0;
                }
            }
            //set up the b like the example
            b[3, 3] = -1;
            b[3, 4] = 1;
            b[4, 3] = 1;
            b[4, 4] = -1;
            return b;
        }
        private static void writeBoard(int[,] b)
        {
            for (int i = 0; i < b.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    //the switch statement writes the character corresponding to the player (or lack thereof) occupying the space at i, j
                    switch (b[i, j])
                    {
                        case 1:
                            Console.Write("X");
                            break;
                        case 0:
                            Console.Write(".");
                            break;
                        case -1:
                            Console.Write("O");
                            break;
                    }
                }
                Console.Write("\n");
            }
            //makes it easier to distinguish two boards printed next to each other
            Console.Write("\n\n\n\n");
        }
        /**
         * Checks if a given move is valid given the board, attempted move and the playerNum (i.e 1 or -1)
        **/ 
        private static Boolean moveValid(int[,] b, Tuple<int, int> move, int playerNum)
        {
            Boolean valid = false;
            //If the space is already taken, the move is invalid
            if (b[move.Item1 - 1, move.Item2 - 1] != 0)
            {
                valid = false;
            }
            else
            {
                //use the move helper to check each of the 8 directions for a valid move, if just one is valid this move is valid
                valid = (moveValidHelper(b, move, playerNum, 0, 1) || moveValidHelper(b, move, playerNum, 1, 0)
                    || moveValidHelper(b, move, playerNum, 1, 1) || moveValidHelper(b, move, playerNum, -1, 0)
                    || moveValidHelper(b, move, playerNum, 0, -1) || moveValidHelper(b, move, playerNum, -1, 1)
                    || moveValidHelper(b, move, playerNum, 1, -1) || moveValidHelper(b, move, playerNum, -1, -1));
            }
            return valid;
        }
        /**
         * Checks the validity of one of 8 directions from the attempted move, if along this line there is a tile belonging to you after a straight line of tiles belonging
         * to your opponent, this direction is valid and true is returned
        **/
        private static Boolean moveValidHelper(int[,] b, Tuple<int, int> move, int playerNum, int iIter, int jIter)
        {
            Boolean directionValid = false;
            //i and j are initiated to the location of the move (0-7 for us, but 1-8 for the user hence the -1)
            int i = move.Item1 - 1;
            int j = move.Item2 - 1;
            i = i + iIter;
            j = j + jIter;
            //checks if we are out of bounds, if so the direction cannot be valid
            if(!(i < 8 && j < 8 && i >= 0 && j >= 0))
            {
                directionValid = false;
            }
            //if the piece in this direction is an opponents piece
            else if (b[i, j] != playerNum * -1)
            {
                directionValid = false;
            }
            else
            {
                i = i + iIter;
                j = j + jIter;
                //while i and j are not out of bounds
                while (i < 8 && j < 8 && i >= 0 && j >= 0)
                {
                    //if the next spot in the direction is 0, set the valid flag to false and break, if it is a spot for your player set valid flag to true then break
                    //if it's not one of those two, it's your opponent's piece and the loop continues
                    if(b[i, j] == 0)
                    {
                        directionValid = false;
                        break;
                    }
                    else if(b[i, j] == playerNum)
                    {
                        directionValid = true;
                        break;
                    }
                    i = i + iIter;
                    j = j + jIter;
                }
            }
            return directionValid;
        }

        private static int[,] executeMove(int[,] b, Tuple<int, int> move, int playerNum)
        {
            //loops through all possible directions (skipping 0, 0 since it is not a direction) and checks if a move is valid, if yes it updates the board with the move
            for(int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {
                    if (moveValidHelper(b, move, playerNum, i, j) && (i != 0 || j != 0))
                    {
                        b = moveExecuteHelper(b, move, playerNum, i, j);
                    }
                }
            }
            b[move.Item1 - 1, move.Item2 - 1] = playerNum;
            return b;
        }

        private static int[,] moveExecuteHelper(int[,] b, Tuple<int, int> move, int playerNum, int iIter, int jIter)
        {
            //i and j are initiated to the location of the move (0-7 for us, but 1-8 for the user hence the -1)
            int i = move.Item1 - 1;
            int j = move.Item2 - 1;
            i = i + iIter;
            j = j + jIter;
            //while i and j are not out of bounds
            while (i < 8 && j < 8 && i >= 0 && j >= 0)
            {
                //it has already been established at this point that the move is valid in this direction, so anything that is not your token must be your opponents token
                //replace opponents tokens with your own until you find your own token
               if(b[i, j] != playerNum)
                {
                    b[i, j] *= -1;
                }
               else
                {
                    break;
                }
                i += iIter;
                j += jIter;
            }
            return b;
        }

        private static LinkedList<Tuple<int, int>> possibleMoves(int[,] b, int playerNum)
        {
            LinkedList<Tuple<int, int>> moveList = new LinkedList<Tuple<int, int>>();
            for(int i = 0; i < b.GetLength(0); i++)
            {
                for(int j = 0; j < b.GetLength(1); j++)
                {
                    //if a position is not empty a valid move cannot be made in that position
                    if(b[i, j] != 0)
                    {
                        continue;
                    }
                    //add one to i and j to make them representative of the player's input
                    Tuple<int, int> move = new Tuple<int, int>(i+1, j+1);
                    //if there is a valid move at this location
                    if(moveValid(b, move, playerNum))
                    {
                        moveList.AddLast(move);
                    }
                }
            }
            return moveList;
        }

        private static int checkWin(int[,] b, int playerNum)
        {
            //sum is 0 if no one has won or the playerNum of the winning player. In the event of a tie, it will be 2
            int sum = 0;
            if(possibleMoves(b, playerNum).Count == 0 && possibleMoves(b, playerNum*-1).Count == 0)
            {
                sum = boardTally(b);
                //if the sum is 0, there was a tie if not dividing sum by the absolute value of the sum will give the playerNum of the winner
                if(sum == 0)
                {
                    sum = 2;
                }
                else
                {
                    sum = sum / Math.Abs(sum);
                }
            }
            return sum;
        }
        //Executes the minimax algorithm and returns a tuple containing a move and the "score" for recursion
        private static Tuple<Tuple<int, int>, int> gameAI(int[,] b, int depth, int player, int alpha, int beta)
        {
            Tuple<Tuple<int, int>, int> move = null;
            Tuple<int, int> bestMove = null;
            //I've arbitraily chosen 500 or -500 as the initial score, in this case it will be the same as assigning it to infinity
            //player 1 is looking for a max and player -1 is looking for a min, so we multiply the 500 by -player to ensure it is the worst score
            int score = 500 * player * -1;
            LinkedList<Tuple<int, int>> possible = possibleMoves(b, player);
            //if there are no possible moves, we are at the bottom of the tree if the depth is 0 or below, we treat this node as the bottom of the tree
            if (possible.Count == 0 || depth < 1)
            {
                //we set move to null here, it is not needed until we get out of recursion so this should be fine as long as I check that the AI has possible moves before
                //running the algorithm
                move = new Tuple<Tuple<int, int>, int>(null, boardTally(b));
            }
            else
            {
                foreach (Tuple<int, int> m in possibleMoves(b, player))
                {
                    if (player > 0)
                    {
                        int[,] newBoard = b.Clone() as int[,];
                        newBoard = executeMove(newBoard, m, player);
                        int nextScore = gameAI(newBoard, depth - 1, player * -1, alpha, beta).Item2;
                        if (nextScore > score)
                        {
                            bestMove = m;
                            score = nextScore;
                            
                        }
                        alpha = Math.Max(alpha, nextScore);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                    else
                    {
                        int[,] newBoard = b.Clone() as int[,];
                        newBoard = executeMove(newBoard, m, player);
                        int nextScore = gameAI(newBoard, depth - 1, player * -1, alpha, beta).Item2;
                        if (nextScore < score)
                        {
                            bestMove = m;
                            score = nextScore;      
                        }
                        beta = Math.Min(beta, nextScore);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
                move = new Tuple<Tuple<int, int>, int>(bestMove, score);
            }
            return move;
        }

        private static int boardTally(int[,] b)
        {
            int sum = 0;
            for (int i = 0; i < b.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    sum += b[i, j];
                }
            }
            return sum;
        }

    }
}
