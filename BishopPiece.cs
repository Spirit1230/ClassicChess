using System;
using System.Collections.Generic;

namespace ClassicChess
{
    public class Bishop : Piece
    {
        public Bishop(string _colour, string _position, string _kingPosition, string _oppKingPosition)
        {
            colour = _colour;
            if (colour == "White")
            {
                name = "WB";
            } else {
                name = "BB";
            }
            type = "Bishop";
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
                string coordinate ;
                isLegal = true;

                //splitting up and converting rank and file for logic and maths
                selectedRank = Convert.ToInt16(Convert.ToString(position.ToCharArray()[1]));
                selectedFile = Convert.ToInt16(fileConvert(Convert.ToString(position.ToCharArray()[0])));
                targetRank = Convert.ToInt16(Convert.ToString(moveTo.ToCharArray()[1]));
                targetFile = Convert.ToInt16(fileConvert(Convert.ToString(moveTo.ToCharArray()[0])));

                int rankDiff = targetRank - selectedRank;
                int fileDiff = targetFile - selectedFile;

                if (Math.Abs(rankDiff) == Math.Abs(fileDiff)) {
                    if (rankDiff > 0) {
                        int file = selectedFile;
                        for (int rank = selectedRank + 1; rank < targetRank; rank++)
                        {
                            switch (fileDiff/Math.Abs(fileDiff))
                            {
                                case 1:
                                    file++;
                                    coordinate = fileConvert(Convert.ToString(file)) + Convert.ToString(rank);
                                    if (pieces.TryGetValue(coordinate, out Piece obstruct)) {
                                        isLegal = false;
                                        break;
                                    }
                                    break;
                                
                                case -1:
                                    file--;
                                    coordinate = fileConvert(Convert.ToString(file)) + Convert.ToString(rank);
                                    if (pieces.TryGetValue(coordinate, out Piece obstruct1)) {
                                        isLegal = false;
                                        break;
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    } else {
                        int file = selectedFile;
                        for (int rank = selectedRank - 1; rank > targetRank; rank--)
                        {
                            switch (fileDiff/Math.Abs(fileDiff))
                            {
                                case 1:
                                    file++;
                                    coordinate = fileConvert(Convert.ToString(file)) + Convert.ToString(rank);
                                    if (pieces.TryGetValue(coordinate, out Piece obstruct)) {
                                        isLegal = false;
                                        break;
                                    }
                                    break;
                                
                                case -1:
                                    file--;
                                    coordinate = fileConvert(Convert.ToString(file)) + Convert.ToString(rank);
                                    if (pieces.TryGetValue(coordinate, out Piece obstruct1)) {
                                        isLegal = false;
                                        break;
                                    }
                                    break;

                                default:
                                    break;
                            }
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
            base.SuccesfullMove(pieces, turn);
        }
    }
}