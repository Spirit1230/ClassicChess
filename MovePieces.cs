using System;
using System.Collections.Generic;

namespace ClassicChess 
{
    class PlayGame 
    {
        public static void movePiece(Dictionary<string, Piece> pieces, string turn)
        {
            //moves a selected piece to a target coordinate

            string toMove = "";
            string moveTo = "";
            bool noMove = true;

            while (noMove)
            {
                Console.Write("Input the coordinate of the piece to move: ");
                toMove = Console.ReadLine();
                Console.Write("Input the target coordinate: ");
                moveTo = Console.ReadLine();

                //determines prescence of pieces on both coordinates selected
                bool toMovePiece = pieces.TryGetValue(toMove, out Piece selectPiece);
                bool moveToPiece = pieces.TryGetValue(moveTo, out Piece targetPiece);

                if (toMovePiece) { //piece present at selected coordinate
                    if (selectPiece.generalLegalMove(selectPiece, turn, moveTo, pieces)) {
                        if(selectPiece.LegalMove(pieces, moveTo)) {
                            if (moveToPiece) { //selected piece taking an opposing piece directly
                                pieces.Remove(moveTo);
                                selectPiece.position = moveTo;
                                pieces.Add(moveTo, selectPiece);
                                pieces.Remove(toMove);
                            } else { //standard move to empty position
                                if (selectPiece.type == "Pawn") { //special move for handling en passant
                                    Pawn selectPiece2 = (Pawn) selectPiece;
                                    if (selectPiece2.takeEnPassant == true) {
                                        int rank = Convert.ToInt16(Convert.ToString(moveTo.ToCharArray()[1]));
                                        string toRemove = "";
                                        switch (selectPiece2.colour) 
                                        {
                                            case "White":
                                                toRemove = Convert.ToString(moveTo.ToCharArray()[0]) +  Convert.ToString(rank - 1);
                                                break;
                                            case "Black":
                                                toRemove = Convert.ToString(moveTo.ToCharArray()[0]) +  Convert.ToString(rank + 1);
                                                break;
                                            default:
                                                break;

                                        } //when taking en passant the piece being taken is behind the moved pawn so taken inderectly 
                                        pieces.Remove(toRemove);
                                        selectPiece2.takeEnPassant = false;
                                    }
                                } else if (selectPiece.type == "King") { //special move for handling castling
                                    King castleKing = (King) selectPiece;
                                    if (castleKing.succesfullCastle) {
                                        char file = moveTo.ToCharArray()[0];
                                        switch (file) {
                                            case 'c':
                                                pieces.TryGetValue("a" + Convert.ToString(moveTo.ToCharArray()[1]), out Piece leftCastle);
                                                leftCastle.position = "d" + Convert.ToString(moveTo.ToCharArray()[1]);
                                                pieces.Add(leftCastle.position,leftCastle);
                                                pieces.Remove("a" + Convert.ToString(moveTo.ToCharArray()[1]));
                                                castleKing.succesfullCastle = false;
                                                break;
                                            case 'g':
                                                pieces.TryGetValue("h" + Convert.ToString(moveTo.ToCharArray()[1]), out Piece rightCastle);
                                                rightCastle.position = "f" + Convert.ToString(moveTo.ToCharArray()[1]);
                                                pieces.Add(rightCastle.position,rightCastle);
                                                pieces.Remove("h" + Convert.ToString(moveTo.ToCharArray()[1]));
                                                castleKing.succesfullCastle = false;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                selectPiece.position = moveTo;
                                pieces.Add(moveTo, selectPiece);
                                pieces.Remove(toMove);
                            }
                            noMove = false; //piece succesfully moved
                            selectPiece.SuccesfullMove(pieces, turn);
                            if (selectPiece.type == "Pawn") { //handles pawn promotion
                                Pawn selectPiece2 = (Pawn) selectPiece;
                                if (selectPiece2.promote) {
                                    bool Promoting = true;
                                    Console.WriteLine("Pawn has reached the final rank and can promote!");
                                    while (Promoting)
                                    {
                                        Console.Write("Please select either Queen, Bishop, Knight or Castle: ");
                                        string promoteTo = Console.ReadLine();
                                        switch (promoteTo)
                                        { //can only handle promotion to each type once, could probaly be more successfully managed with a dictionary
                                            case "Queen":
                                                Piece promoteQueen = new Queen(selectPiece.colour,selectPiece.position,selectPiece.kingPosition,selectPiece.oppKingPosition);
                                                pieces.Remove(selectPiece.position);
                                                pieces.Add(promoteQueen.position,promoteQueen);
                                                Promoting = false;
                                                break;
                                            case "Bishop":
                                                Piece promoteBishop = new Bishop(selectPiece.colour,selectPiece.position,selectPiece.kingPosition,selectPiece.oppKingPosition);
                                                pieces.Remove(selectPiece.position);
                                                pieces.Add(promoteBishop.position,promoteBishop);
                                                Promoting = false;
                                                break;
                                            case "Knight":
                                                Piece promoteKnight = new Knight(selectPiece.colour,selectPiece.position,selectPiece.kingPosition,selectPiece.oppKingPosition);
                                                pieces.Remove(selectPiece.position);
                                                pieces.Add(promoteKnight.position,promoteKnight);
                                                Promoting = false;
                                                break;
                                            case "Castle":
                                                Piece promoteCastle = new Castle(selectPiece.colour,selectPiece.position,selectPiece.kingPosition,selectPiece.oppKingPosition);
                                                pieces.Remove(selectPiece.position);
                                                pieces.Add(promoteCastle.position,promoteCastle);
                                                Promoting = false;
                                                break;
                                            default:
                                                Console.WriteLine("Invalid choice!");
                                                break;
                                        }
                                    }
                                }
                            }
                        } else {
                            Console.WriteLine("Illegal Move!");
                        }
                    } else {
                        Console.WriteLine("Illegal Move!");
                    }
                } else {
                    Console.WriteLine("No piece selected");
                }
            }
        }

        public static int numLegalMoves(Dictionary<string, Piece> pieces, string turn) 
        {
            //counts the number of legal moves that can be made by the opposing team
            //if this is equal to zero then the opposing King is either in checkmate or stalemate 

            int numMoves = 0;
            string coordinate;
            string coordinateSearch;

            for (int rankSearch = 1; rankSearch <= 8 ; rankSearch++)
                {
                    for (int fileSearch = 1; fileSearch <= 8 ; fileSearch++) 
                    {
                        coordinateSearch = Piece.fileConvert(Convert.ToString(fileSearch)) + Convert.ToString(rankSearch);
                        if (pieces.TryGetValue(coordinateSearch, out Piece potPiece)) {
                            if (potPiece.colour != turn) {
                                for (int rank = 1; rank <= 8 ; rank++)
                                {
                                    for (int file = 1; file <= 8 ; file++) 
                                    {
                                        coordinate = Piece.fileConvert(Convert.ToString(file)) + Convert.ToString(rank);
                                        if (coordinate != potPiece.position) {
                                            if (potPiece.generalLegalMove(potPiece, potPiece.colour, coordinate, pieces)) {
                                                if (potPiece.LegalMove(pieces, coordinate)) {
                                                    numMoves++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            return numMoves;
        }  
    }
}