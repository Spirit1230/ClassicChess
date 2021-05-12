using System;
using System.Collections.Generic;

namespace ClassicChess 
{
    public class Piece 
    {
        public string colour;
        public string name;
        public string type;
        public string position;
        public string kingPosition;
        public string oppKingPosition;
        public int pointsWorth;
        protected int numberOfMoves;

        public bool generalLegalMove(Piece movingPiece, string turn, string moveTo, Dictionary<string, Piece> pieces)
        {
            //does a general check to see if a move is legal which is univerasal for all pieces on the board
            //e.g. ensures white pieces are moved during white's turn and cannot take ally pieces and likewise for black

            bool isLegal = true;
            bool targetPresent = pieces.TryGetValue(moveTo, out Piece targetPiece);

            switch (targetPresent)
            {   case false:
                    //no piece present at target coordinate
                    if(movingPiece.colour != turn) {
                        isLegal = false;
                    }
                    break;
                                  
                default:
                    //piece present at target coordinate
                    if (movingPiece.colour != turn || targetPiece.colour == turn) {
                        isLegal = false;
                    }  
                    break;
            }

            if (isLegal) { //checks whether a move put the ally king in check

                string originalPosition = position;

                switch (targetPresent) 
                {
                    case false:

                        movingPiece.position = moveTo;
                        pieces.Add(moveTo,movingPiece);
                        pieces.Remove(originalPosition);
                        
                        if(movingPiece.type == "King") {
                            updateKingPosition(pieces,moveTo,colour);
                        }

                        if (pieceCheck(pieces, kingPosition, colour)) {
                            isLegal = false;
                        }

                        movingPiece.position = originalPosition;
                        pieces.Add(originalPosition,movingPiece);
                        pieces.Remove(moveTo);

                        if(movingPiece.type == "King") {
                            updateKingPosition(pieces,originalPosition,colour);
                        }

                        break;
                  
                    default:

                        movingPiece.position = moveTo;
                        pieces.Remove(moveTo);
                        pieces.Add(moveTo,movingPiece);
                        pieces.Remove(originalPosition);
                        
                        if(movingPiece.type == "King") {
                            updateKingPosition(pieces,moveTo,colour);
                        }

                        if (pieceCheck(pieces, kingPosition, colour)) {
                            isLegal = false;
                        }

                        movingPiece.position = originalPosition;
                        pieces.Add(originalPosition,movingPiece);
                        pieces.Remove(moveTo);
                        pieces.Add(targetPiece.position,targetPiece);

                        if(movingPiece.type == "King") {
                            updateKingPosition(pieces,originalPosition,colour);
                        }

                        break;

                }
                
            }

            return isLegal;
        }

        public bool pieceCheck(Dictionary<string, Piece> pieces, string positionCheck, string turn) 
        {
            //checks to see if any opposing piece threatens a specified position

            bool inCheck = false;

            foreach (KeyValuePair<string, Piece> threatPiece in pieces)
            {
                if (threatPiece.Value.colour != turn) {
                    if (threatPiece.Value.LegalMove(pieces, positionCheck)) {
                        inCheck = true;
                        break;
                    }
                }
            }

            return inCheck;
        }

        public virtual bool LegalMove(Dictionary<string, Piece> pieces, string moveTo)
        {
            //checks whether a move is legal with regards to specific rules for each piece
            //DON'T USE pieceCheck() METHOD HERE (this causes a stack overflow exception)

            bool isLegal = true;

            return isLegal;
        }

        public virtual void SuccesfullMove(Dictionary<string, Piece> pieces, string turn)
        {
            //performs certain general house cleaning jobs after a move has been made
            //more specific jobs are handled within the child classes

            numberOfMoves++;

            bool findKing = pieces.TryGetValue(oppKingPosition, out Piece oppKing);
            King oppKing1 = (King) oppKing;

            if (pieceCheck(pieces,oppKingPosition,oppKing.colour)) {
                oppKing1.isInCheck = true;
                Console.WriteLine("{0} King is in check!",oppKing.colour);
            } else {
                oppKing1.isInCheck = false;
            }

        }

        public void updateKingPosition(Dictionary<string, Piece> pieces, string newKingPosition, string turn) 
        {
            foreach (KeyValuePair<string, Piece> piece in pieces)
            {
                if (piece.Value.colour == turn) {
                    piece.Value.kingPosition = newKingPosition;
                } else {
                    piece.Value.oppKingPosition = newKingPosition;
                }
            }
        }

        public static string fileConvert(string toConvert)
        {
            //Converts the file labeling between letters and numbers

            switch(toConvert)
            {
                case "a":
                    toConvert = "1";
                    break;
                case "b":
                    toConvert = "2";
                    break;
                case "c":
                    toConvert = "3";
                    break;
                case "d":
                    toConvert = "4";
                    break;
                case "e":
                    toConvert = "5";
                    break;
                case "f":
                    toConvert = "6";
                    break;
                case "g":
                    toConvert = "7";
                    break;
                case "h":
                    toConvert = "8";
                    break;
                case "1":
                    toConvert = "a";
                    break;
                case "2":
                    toConvert = "b";
                    break;
                case "3":
                    toConvert = "c";
                    break;
                case "4":
                    toConvert = "d";
                    break;
                case "5":
                    toConvert = "e";
                    break;
                case "6":
                    toConvert = "f";
                    break;
                case "7":
                    toConvert = "g";
                    break;
                case "8":
                    toConvert = "h";
                    break;
                default:
                    break;
            }
            return toConvert;
        }
    }
}