using System;
using System.Collections.Generic;

namespace ClassicChess 
{
    public class Pawn : Piece
    {
        public bool enPassant;
        public bool takeEnPassant;
        public bool promote;
        public bool twoMoves;

        public Pawn(string _colour, string _position, string _kingPosition, string _oppKingPosition)
        {
            colour = _colour;
            if (colour == "White")
            {
                name = "WP";
            } else {
                name = "BP";
            }
            type = "Pawn";
            position = _position;
            kingPosition = _kingPosition;
            oppKingPosition = _oppKingPosition;
            pointsWorth = 1;
            enPassant = false;
            takeEnPassant = false;
            promote = false;
            twoMoves = true;
            numberOfMoves = 0;
        }

        public override bool LegalMove(Dictionary<string, Piece> pieces, string moveTo)
        {
            bool isLegal = false;

            if (base.LegalMove(pieces, moveTo)) {

                int selectedRank;
                int selectedFile;
                int targetRank;
                int targetFile;
                string coordinate;
                isLegal = true;

                //splitting up and converting rank and file for logic and maths
                selectedRank = Convert.ToInt16(Convert.ToString(position.ToCharArray()[1]));
                selectedFile = Convert.ToInt16(fileConvert(Convert.ToString(position.ToCharArray()[0])));
                targetRank = Convert.ToInt16(Convert.ToString(moveTo.ToCharArray()[1]));
                targetFile = Convert.ToInt16(fileConvert(Convert.ToString(moveTo.ToCharArray()[0])));

                //logic rules for taking with a pawn
                bool whiteTake = Math.Abs(selectedFile -targetFile) == 1 && selectedRank - targetRank == -1 && colour == "White";
                bool blackTake = Math.Abs(selectedFile -targetFile) == 1 && selectedRank - targetRank == 1 && colour == "Black";
                bool take = whiteTake || blackTake;

                //logic rules for moving with a pawn
                bool whiteMove = selectedFile == targetFile && selectedRank - targetRank < 0 && colour == "White";
                bool blackMove = selectedFile == targetFile && selectedRank - targetRank > 0 && colour == "Black";
                bool move = whiteMove || blackMove;

                if (move) {
                    switch (Math.Abs(targetRank - selectedRank))
                    {
                        case 1:
                            if (pieces.TryGetValue(moveTo, out Piece obstruct)) { //checks if pawn is obstructed
                                isLegal = false;
                            }
                            break;
                        case 2:
                            if (twoMoves) {
                                if (selectedRank < targetRank) {
                                    for (int rank = selectedRank + 1; rank <= targetRank; rank++)
                                    {
                                        coordinate = fileConvert(Convert.ToString(targetFile)) + Convert.ToString(rank);
                                        if (pieces.TryGetValue(coordinate, out Piece obstruct1)) { //checks if pawn is obstructed
                                            isLegal = false;
                                            break;
                                        }
                                    }
                                } else {
                                    for (int rank = selectedRank - 1; rank >= targetRank; rank--)
                                    {
                                        coordinate = fileConvert(Convert.ToString(targetFile)) + Convert.ToString(rank);
                                        if (pieces.TryGetValue(coordinate, out Piece obstruct1)) { //checks if pawn is obstructed
                                            isLegal = false;
                                            break;
                                        }
                                    }
                                }
                            } else { //pawn can only make two moves from starting position
                                isLegal = false;
                            }
                            break;
                        default:
                            isLegal = false;
                            break;
                    }
                } else if (take) {

                    string enPassantCoordinate;

                    if (colour == "White") {
                        enPassantCoordinate = fileConvert(Convert.ToString(targetFile)) + Convert.ToString(targetRank - 1);
                    } else {
                        enPassantCoordinate = fileConvert(Convert.ToString(targetFile)) + Convert.ToString(targetRank + 1);        
                    }

                    if (pieces.TryGetValue(moveTo, out Piece obstruct2) == false) { //checks if piece is present for taking

                        if (pieces.TryGetValue(enPassantCoordinate, out Piece enPassantTake)) { //checks for en Passant take

                            if (enPassantTake.type == "Pawn") {

                                Pawn enPassantTake2 = (Pawn) enPassantTake; //explicit conversion by cast

                                if (enPassantTake2.enPassant == false){
                                    isLegal = false;
                                } else {
                                    takeEnPassant = true;
                                }
                            }

                        } else {
                            isLegal = false;
                        }
                    }
                } else {
                    isLegal = false;
                }
            }

            return isLegal;
        }

        public override void SuccesfullMove(Dictionary<string, Piece> pieces, string turn)
        {
            //Changes necessary values after a successful move

            base.SuccesfullMove(pieces, turn);

            //logic for deciding whether a pawn can promote
            bool canWhitePromote = position.ToCharArray()[1] == '8' && colour == "White";
            bool canBlackPromote = position.ToCharArray()[1] == '1' && colour == "Black";
            bool canPromote = canWhitePromote || canBlackPromote;

            //Logic for deciding whether a pawn can be taken be en passant
            bool canWhiteEnPassant = position.ToCharArray()[1] == '4' && numberOfMoves == 1 && colour == "White";
            bool canBlackEnPassant = position.ToCharArray()[1] == '5' && numberOfMoves == 1 && colour == "Black";
            bool canEnPassant = canWhiteEnPassant || canBlackEnPassant;

            if (takeEnPassant) {
                takeEnPassant = false;
            }
            if (canEnPassant && twoMoves) {
                enPassant = true;
            } else {
                enPassant = false;
            }
            if (numberOfMoves > 0) {
                twoMoves = false;
            }
            if (canPromote) {
                promote = true;
            }
        }
    }
}