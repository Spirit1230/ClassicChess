using System;
using System.Collections.Generic;

namespace ClassicChess 
{
    public class Knight : Piece 
    {
        public Knight(string _colour, string _position, string _kingPosition, string _oppKingPosition) 
        {
            colour = _colour;
            if (colour == "White")
            {
                name = "WN";
            } else {
                name = "BN";
            }
            type = "Knight";
            position = _position;
            kingPosition = _kingPosition;
            oppKingPosition = _oppKingPosition;
            pointsWorth = 3;
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

                int rankDiff = Math.Abs(targetRank - selectedRank);
                int fileDiff = Math.Abs(targetFile - selectedFile);

                bool moveLegal = (rankDiff == 2 && fileDiff == 1) || (rankDiff == 1 && fileDiff ==2);
                
                if (moveLegal) {
                    isLegal = true;
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