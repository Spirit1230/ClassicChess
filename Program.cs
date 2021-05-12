using System;
using System.Collections.Generic; //allows for the use of dictionaries

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
        private int numberOfMoves;

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
            Piece.King oppKing1 = (Piece.King) oppKing;

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

                                    Piece.Pawn enPassantTake2 = (Piece.Pawn) enPassantTake; //explicit conversion by cast

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
                        Piece tempBishop = new Piece.Bishop(colour,position,kingPosition,oppKingPosition);
                        if (tempBishop.LegalMove(pieces,moveTo)) {
                            isLegal = true;
                        }
                    } else if ((rankDiff == 0 && Math.Abs(fileDiff) > 0) || (Math.Abs(rankDiff) > 0 && fileDiff == 0)) { //Queen moving straight
                        Piece tempCastle = new Piece.Castle(colour,position,kingPosition,oppKingPosition);
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
                                    Piece.Castle castlePieceRight = (Piece.Castle) castlePiece;
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
                                    Piece.Castle castlePieceLeft = (Piece.Castle) castlePiece1;
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

    class Program
    {
        static void Main(string[] args)
        {

            Dictionary<string, Piece> pieceDict = new Dictionary<string, Piece>();
            string turnColour = "White";
            bool continueGame = true;
            int turn = 0;

            initialSetup(pieceDict);            
            CreateBoard(pieceDict);

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
                movePiece(pieceDict, turnColour);
                CreateBoard(pieceDict);

                //checks whether the game has ended
                if (numLegalMoves(pieceDict, turnColour) == 0) {
                    foreach (KeyValuePair<string, Piece> findKing in pieceDict)
                    {
                        if (findKing.Value.type == "King" && findKing.Value.colour != turnColour) {
                            Piece.King oppKing = (Piece.King) findKing.Value;
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

        static void CreateBoard(Dictionary<string, Piece> pieces)
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

        static void movePiece(Dictionary<string, Piece> pieces, string turn)
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
                                    Piece.Pawn selectPiece2 = (Piece.Pawn) selectPiece;
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
                                    Piece.King castleKing = (Piece.King) selectPiece;
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
                                Piece.Pawn selectPiece2 = (Piece.Pawn) selectPiece;
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
                                                Piece promoteQueen = new Piece.Queen(selectPiece.colour,selectPiece.position,selectPiece.kingPosition,selectPiece.oppKingPosition);
                                                pieces.Remove(selectPiece.position);
                                                pieces.Add(promoteQueen.position,promoteQueen);
                                                Promoting = false;
                                                break;
                                            case "Bishop":
                                                Piece promoteBishop = new Piece.Bishop(selectPiece.colour,selectPiece.position,selectPiece.kingPosition,selectPiece.oppKingPosition);
                                                pieces.Remove(selectPiece.position);
                                                pieces.Add(promoteBishop.position,promoteBishop);
                                                Promoting = false;
                                                break;
                                            case "Knight":
                                                Piece promoteKnight = new Piece.Knight(selectPiece.colour,selectPiece.position,selectPiece.kingPosition,selectPiece.oppKingPosition);
                                                pieces.Remove(selectPiece.position);
                                                pieces.Add(promoteKnight.position,promoteKnight);
                                                Promoting = false;
                                                break;
                                            case "Castle":
                                                Piece promoteCastle = new Piece.Castle(selectPiece.colour,selectPiece.position,selectPiece.kingPosition,selectPiece.oppKingPosition);
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

        static int numLegalMoves(Dictionary<string, Piece> pieces, string turn) 
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

        static void initialSetup(Dictionary<string, Piece> pieces)
        {
            //Creates all the piece and assigns them to the pieces dictionary 
            //referenced by their positions

            Piece WhiteKing = new Piece.King("White","e1","e1","e8");
            pieces.Add(WhiteKing.position,WhiteKing);
            Piece BlackKing = new Piece.King("Black","e8","e8","e1");
            pieces.Add(BlackKing.position,BlackKing);
            
            Piece pawn1 = new Piece.Pawn("Black","a7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn1.position,pawn1);
            Piece pawn2 = new Piece.Pawn("White","a2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn2.position,pawn2);
            Piece pawn3 = new Piece.Pawn("Black","b7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn3.position,pawn3);
            Piece pawn4 = new Piece.Pawn("White","b2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn4.position,pawn4);
            Piece pawn5 = new Piece.Pawn("Black","c7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn5.position,pawn5);
            Piece pawn6 = new Piece.Pawn("White","c2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn6.position,pawn6);
            Piece pawn7 = new Piece.Pawn("Black","d7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn7.position,pawn7);
            Piece pawn8 = new Piece.Pawn("White","d2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn8.position,pawn8);
            Piece pawn9 = new Piece.Pawn("Black","e7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn9.position,pawn9);
            Piece pawn10 = new Piece.Pawn("White","e2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn10.position,pawn10);
            Piece pawn11 = new Piece.Pawn("Black","f7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn11.position,pawn11);
            Piece pawn12 = new Piece.Pawn("White","f2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn12.position,pawn12);
            Piece pawn13 = new Piece.Pawn("Black","g7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn13.position,pawn13);
            Piece pawn14 = new Piece.Pawn("White","g2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn14.position,pawn14);
            Piece pawn15 = new Piece.Pawn("Black","h7",BlackKing.position,WhiteKing.position);
            pieces.Add(pawn15.position,pawn15);
            Piece pawn16 = new Piece.Pawn("White","h2",WhiteKing.position,BlackKing.position);
            pieces.Add(pawn16.position,pawn16);

            Piece castle1 = new Piece.Castle("White","a1",WhiteKing.position,BlackKing.position);
            pieces.Add(castle1.position,castle1);
            Piece castle2 = new Piece.Castle("White","h1",WhiteKing.position,BlackKing.position);
            pieces.Add(castle2.position,castle2);
            Piece castle3 = new Piece.Castle("Black","a8",BlackKing.position,WhiteKing.position);
            pieces.Add(castle3.position,castle3);
            Piece castle4 = new Piece.Castle("Black","h8",BlackKing.position,WhiteKing.position);
            pieces.Add(castle4.position,castle4);

            Piece Knight1 = new Piece.Knight("White","b1",WhiteKing.position,BlackKing.position);
            pieces.Add(Knight1.position,Knight1);
            Piece Knight2 = new Piece.Knight("White","g1",WhiteKing.position,BlackKing.position);
            pieces.Add(Knight2.position,Knight2);
            Piece Knight3 = new Piece.Knight("Black","b8",BlackKing.position,WhiteKing.position);
            pieces.Add(Knight3.position,Knight3);
            Piece Knight4 = new Piece.Knight("Black","g8",BlackKing.position,WhiteKing.position);
            pieces.Add(Knight4.position,Knight4);

            Piece Bishop1 = new Piece.Bishop("White","c1",WhiteKing.position,BlackKing.position);
            pieces.Add(Bishop1.position,Bishop1);
            Piece Bishop2 = new Piece.Bishop("White","f1",WhiteKing.position,BlackKing.position);
            pieces.Add(Bishop2.position,Bishop2);
            Piece Bishop3 = new Piece.Bishop("Black","c8",BlackKing.position,WhiteKing.position);
            pieces.Add(Bishop3.position,Bishop3);
            Piece Bishop4 = new Piece.Bishop("Black","f8",BlackKing.position,WhiteKing.position);
            pieces.Add(Bishop4.position,Bishop4);

            Piece WhiteQueen = new Piece.Queen("White","d1",WhiteKing.position,BlackKing.position);
            pieces.Add(WhiteQueen.position,WhiteQueen);
            Piece BlackQueen = new Piece.Queen("Black","d8",BlackKing.position,WhiteKing.position);
            pieces.Add(BlackQueen.position,BlackQueen);

        }
    }
}
