using System;
using System.Collections.Generic;

namespace ClassicChess 
{
    class BoardSetUp 
    {
        public static void CreateBoard(Dictionary<string, Piece> pieces)
        {
            //Prints out the chess board and places pieces

            string coordinates;
            string toWrite;
            string result;
            
            for (int rank = 8; rank >= 1; rank--) //Loops through the ranks 1-8
            {
                toWrite = Convert.ToString(rank) + " |"; //Places rank number on left
                Console.WriteLine("  -------------------------");
                for (int file = 1; file <= 8; file++) //Loops through the files a-h
                {
                    coordinates = Piece.fileConvert(Convert.ToString(file)) + Convert.ToString(rank);
                    
                    if (pieces.TryGetValue(coordinates, out Piece selectPiece)) {
                        result = selectPiece.name + "|";
                    } else {
                        result = "  |";
                    }
                    toWrite = toWrite + result;
                }
                Console.WriteLine(toWrite);
            }
            
            Console.WriteLine("  -------------------------");
            Console.WriteLine("   a  b  c  d  e  f  g  h "); //Places file letter at bottom
        }

        public static void initialSetup(Dictionary<string, Piece> pieces)
        {
            //Creates all the piece and assigns them to the pieces dictionary 
            //referenced by their positions

            Piece WhiteKing = new King("White","e1","e1","e8");
            pieces.Add(WhiteKing.position,WhiteKing);
            Piece BlackKing = new King("Black","e8","e8","e1");
            pieces.Add(BlackKing.position,BlackKing);
            
            Piece pawn1 = new Pawn("Black","a7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn1.position,pawn1);
            Piece pawn2 = new Pawn("White","a2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn2.position,pawn2);
            Piece pawn3 = new Pawn("Black","b7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn3.position,pawn3);
            Piece pawn4 = new Pawn("White","b2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn4.position,pawn4);
            Piece pawn5 = new Pawn("Black","c7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn5.position,pawn5);
            Piece pawn6 = new Pawn("White","c2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn6.position,pawn6);
            Piece pawn7 = new Pawn("Black","d7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn7.position,pawn7);
            Piece pawn8 = new Pawn("White","d2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn8.position,pawn8);
            Piece pawn9 = new Pawn("Black","e7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn9.position,pawn9);
            Piece pawn10 = new Pawn("White","e2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn10.position,pawn10);
            Piece pawn11 = new Pawn("Black","f7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn11.position,pawn11);
            Piece pawn12 = new Pawn("White","f2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn12.position,pawn12);
            Piece pawn13 = new Pawn("Black","g7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn13.position,pawn13);
            Piece pawn14 = new Pawn("White","g2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn14.position,pawn14);
            Piece pawn15 = new Pawn("Black","h7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn15.position,pawn15);
            Piece pawn16 = new Pawn("White","h2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn16.position,pawn16);

            Piece castle1 = new Castle("White","a1",WhiteKing.position,BlackKing.position);
            pieces.Add(castle1.position,castle1);
            Piece castle2 = new Castle("White","h1",WhiteKing.position,BlackKing.position);
            pieces.Add(castle2.position,castle2);
            Piece castle3 = new Castle("Black","a8",BlackKing.position,WhiteKing.position);
            pieces.Add(castle3.position,castle3);
            Piece castle4 = new Castle("Black","h8",BlackKing.position,WhiteKing.position);
            pieces.Add(castle4.position,castle4);

            Piece Knight1 = new Knight("White","b1",WhiteKing.position,BlackKing.position);
            pieces.Add(Knight1.position,Knight1);
            Piece Knight2 = new Knight("White","g1",WhiteKing.position,BlackKing.position);
            pieces.Add(Knight2.position,Knight2);
            Piece Knight3 = new Knight("Black","b8",BlackKing.position,WhiteKing.position);
            pieces.Add(Knight3.position,Knight3);
            Piece Knight4 = new Knight("Black","g8",BlackKing.position,WhiteKing.position);
            pieces.Add(Knight4.position,Knight4);

            Piece Bishop1 = new Bishop("White","c1",WhiteKing.position,BlackKing.position);
            pieces.Add(Bishop1.position,Bishop1);
            Piece Bishop2 = new Bishop("White","f1",WhiteKing.position,BlackKing.position);
            pieces.Add(Bishop2.position,Bishop2);
            Piece Bishop3 = new Bishop("Black","c8",BlackKing.position,WhiteKing.position);
            pieces.Add(Bishop3.position,Bishop3);
            Piece Bishop4 = new Bishop("Black","f8",BlackKing.position,WhiteKing.position);
            pieces.Add(Bishop4.position,Bishop4);

            Piece WhiteQueen = new Queen("White","d1",WhiteKing.position,BlackKing.position);
            pieces.Add(WhiteQueen.position,WhiteQueen);
            Piece BlackQueen = new Queen("Black","d8",BlackKing.position,WhiteKing.position);
            pieces.Add(BlackQueen.position,BlackQueen);

        }
    }
}