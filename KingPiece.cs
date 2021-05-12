using System;
using System.Collections.Generic;

namespace ClassicChess 
{
    public class King : Piece 
    {
        public bool isInCheck;
        public bool castling;
        public bool succesfullCastle;

        public King(string _colour, string _position, string _kingPosition, string _oppKingPosition)
        {
            colour = _colour;
            if (colour == "White")
            {
                name = "WK";
            } else {
                name = "BK";
            }
            type = "King";
            position = _position;
            kingPosition = _kingPosition;
            oppKingPosition = _oppKingPosition;
            isInCheck = false;
            castling = true;
            succesfullCastle = false;
            numberOfMoves = 0;
        }

        public override bool LegalMove(Dictionary<string, Piece> pieces, string moveTo) 
        {
            int selectedRank;
            int selectedFile;
            int targetRank;
            int targetFile;
            string coordinate;
            bool isLegal = false;

            //splitting up and converting rank and file for logic and maths
            selectedRank = Convert.ToInt16(Convert.ToString(position.ToCharArray()[1]));
            selectedFile = Convert.ToInt16(fileConvert(Convert.ToString(position.ToCharArray()[0])));
            targetRank = Convert.ToInt16(Convert.ToString(moveTo.ToCharArray()[1]));
            targetFile = Convert.ToInt16(fileConvert(Convert.ToString(moveTo.ToCharArray()[0])));

            int rankDiff = targetRank - selectedRank;
            int fileDiff = targetFile - selectedFile;

            
            if (Math.Abs(rankDiff) <= 1 && Math.Abs(fileDiff) <= 1) {
                isLegal = true;
            } else if (rankDiff == 0 && Math.Abs(fileDiff) == 2 && castling) { //handles castling 

                switch (fileDiff/Math.Abs(fileDiff))
                {
                    case 1:
                        if (pieces.TryGetValue("h" + Convert.ToString(selectedRank), out Piece castlePiece)) {
                            if (castlePiece.type == "Castle") {
                                Castle castlePieceRight = (Castle) castlePiece;
                                if (castlePieceRight.castling) {
                                    isLegal = true;
                                    succesfullCastle = true;
                                    for (int file = selectedFile + 1; file < 8; file++)
                                    {
                                        coordinate = fileConvert(Convert.ToString(file)) + Convert.ToString(selectedRank);
                                        if (pieces.TryGetValue(coordinate, out Piece obstruct)) {
                                            isLegal = false;
                                            succesfullCastle = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }                            
                        break;
                    case -1:
                        if (pieces.TryGetValue("a" + Convert.ToString(selectedRank), out Piece castlePiece1)) {
                            if (castlePiece1.type == "Castle") {
                                Castle castlePieceLeft = (Castle) castlePiece1;
                                if (castlePieceLeft.castling) {
                                    isLegal = true;
                                    succesfullCastle = true;
                                    for (int file = selectedFile - 1; file > 1; file--)
                                    {
                                        coordinate = fileConvert(Convert.ToString(file)) + Convert.ToString(selectedRank);
                                        if (pieces.TryGetValue(coordinate, out Piece obstruct)) {
                                            isLegal = false;
                                            succesfullCastle = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }                            
                        break;
                    default:
                        break;
                }
            }

            return isLegal;
        }

        public override void SuccesfullMove(Dictionary<string, Piece> pieces, string turn)
        {
            base.SuccesfullMove(pieces, turn);

            updateKingPosition(pieces, position, colour);
            
            if (numberOfMoves > 0) {
                castling = false;
            }

            if (succesfullCastle) {
                succesfullCastle = false;
            }
        }

    }
}