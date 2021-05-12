using System;
using System.Collections.Generic;

namespace ClassicChess 
{
    public class Castle : Piece
    {
        public bool castling;

        public Castle(string _colour, string _position, string _kingPosition, string _oppKingPosition)
        {
            colour = _colour;
            if (colour == "White")
            {
                name = "WC";
            } else {
                name = "BC";
            }
            type = "Castle";
            position = _position;
            kingPosition = _kingPosition;
            oppKingPosition = _oppKingPosition;
            pointsWorth = 5;
            castling = true;
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

                if (selectedFile == targetFile) {
                    if (selectedRank < targetRank) {
                        for (int i = selectedRank + 1; i < targetRank; i++)
                        {
                            coordinate = fileConvert(Convert.ToString(selectedFile)) + Convert.ToString(i);
                            if (pieces.TryGetValue(coordinate, out Piece obstruct)) {
                                isLegal = false;
                                break;
                            }
                        }
                    } else {
                        for (int i = selectedRank - 1; i > targetRank; i--)
                        {
                            coordinate = fileConvert(Convert.ToString(selectedFile)) + Convert.ToString(i);
                            if (pieces.TryGetValue(coordinate, out Piece obstruct)) {
                                isLegal = false;
                                break;
                            }
                        }
                    }
                    
                } else if (selectedRank == targetRank) {
                    if (selectedFile < targetFile) {
                        for (int i = selectedFile + 1; i < targetFile; i++)
                        {
                            coordinate = fileConvert(Convert.ToString(i)) + Convert.ToString(selectedRank);
                            if (pieces.TryGetValue(coordinate, out Piece obstruct)) {
                                isLegal = false;
                                break;
                            }
                        }
                    } else {
                        for (int i = selectedFile - 1; i > targetFile; i--)
                        {
                            coordinate = fileConvert(Convert.ToString(i)) + Convert.ToString(selectedRank);
                            if (pieces.TryGetValue(coordinate, out Piece obstruct)) {
                                isLegal = false;
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

            if (numberOfMoves > 0) {
                castling = false;
            }
        }
    }
}