using System;
using System.Collections.Generic;

namespace ClassicChess 
{
    public class Queen : Piece 
    {
        public Queen(string _colour, string _position, string _kingPosition, string _oppKingPosition)
        {
            colour = _colour;
            if (colour == "White")
            {
                name = "WQ";
            } else {
                name = "BQ";
            }
            type = "Queen";
            position = _position;
            kingPosition = _kingPosition;
            oppKingPosition = _oppKingPosition;
            pointsWorth = 9;
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

                //splitting up and converting rank and file for logic and maths
                selectedRank = Convert.ToInt16(Convert.ToString(position.ToCharArray()[1]));
                selectedFile = Convert.ToInt16(fileConvert(Convert.ToString(position.ToCharArray()[0])));
                targetRank = Convert.ToInt16(Convert.ToString(moveTo.ToCharArray()[1]));
                targetFile = Convert.ToInt16(fileConvert(Convert.ToString(moveTo.ToCharArray()[0])));

                int rankDiff = targetRank - selectedRank;
                int fileDiff = targetFile - selectedFile;

                if (Math.Abs(rankDiff) == Math.Abs(fileDiff)) { //Queen moving diagonally
                    Piece tempBishop = new Bishop(colour,position,kingPosition,oppKingPosition);
                    if (tempBishop.LegalMove(pieces,moveTo)) {
                        isLegal = true;
                    }
                } else if ((rankDiff == 0 && Math.Abs(fileDiff) > 0) || (Math.Abs(rankDiff) > 0 && fileDiff == 0)) { //Queen moving straight
                    Piece tempCastle = new Castle(colour,position,kingPosition,oppKingPosition);
                    if (tempCastle.LegalMove(pieces,moveTo)) {
                        isLegal = true;
                    }
                }         
            }

            return isLegal;
        }

        public override void SuccesfullMove(Dictionary<string, Piece> pieces, string turn)
        {
            base.SuccesfullMove(pieces, turn);
        }
    }
}