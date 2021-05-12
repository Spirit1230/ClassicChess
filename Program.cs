using System;
using System.Collections.Generic; //allows for the use of dictionaries

namespace ClassicChess
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Piece> pieceDict = new Dictionary<string, Piece>();
            string turnColour = "White";
            bool continueGame = true;
            int turn = 0;

            BoardSetUp.initialSetup(pieceDict);            
            BoardSetUp.CreateBoard(pieceDict);

            while (continueGame) //loops turns until the game ends
            {
                turn++;

                switch (turn % 2)
                { //changes turn from black to white and vice versa
                    case 1:
                        turnColour = "White";
                        break;
                    
                    default:
                        turnColour = "Black";
                        break;

                }

                //runs each turn
                Console.WriteLine("{0}'s move.", turnColour);
                PlayGame.movePiece(pieceDict, turnColour);
                BoardSetUp.CreateBoard(pieceDict);

                //checks whether the game has ended
                if (PlayGame.numLegalMoves(pieceDict, turnColour) == 0) {
                    foreach (KeyValuePair<string, Piece> findKing in pieceDict)
                    {
                        if (findKing.Value.type == "King" && findKing.Value.colour != turnColour) {
                            King oppKing = (King) findKing.Value;
                            if (oppKing.isInCheck) {
                                Console.WriteLine("Checkmate! {0} wins!",turnColour);
                            } else {
                                Console.WriteLine("Stalemate!");
                            }
                            continueGame = false;
                            break;
                        }
                    }
                }
            }

            //Prevents the terminal auto closing
            Console.ReadKey();
        }   
    }
}
