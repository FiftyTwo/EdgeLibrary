﻿using EdgeLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckersGame
{
    //A player used by the computer
    public class ComputerPlayer : Player
    {
        private ComputerMoveChooser MoveChooser;
        private Move ChosenMove;

        private RandomTicker Ticker;

        public ComputerPlayer(string name, int difficulty = 1, int difficultyFluctuation = 1, float moveWait = 1000, float moveWaitFluctuation = 500) : base(name)
        {
            Ticker = new RandomTicker(moveWait - moveWaitFluctuation, moveWait + moveWaitFluctuation);

            MoveChooser = new ComputerMoveChooser(difficulty, difficultyFluctuation);

            Ticker.Enabled = false;
            Ticker.OnTick += Ticker_OnTick;
        }

        public override bool ReceivePreviousMove(Move move, Dictionary<Piece, List<Move>> possibleMoves)
        {
            if (!base.ReceivePreviousMove(move, possibleMoves))
            {
                return false;
            }

            List<Move> moves = new List<Move>();
            foreach(Piece piece in possibleMoves.Keys)
            {
                moves.AddRange(possibleMoves[piece]);
            }

            ChosenMove = MoveChooser.ChooseMove(moves, BoardManager.Instance.Board);
            Ticker.Enabled = true;

            return true;
        }

        void Ticker_OnTick(GameTime gameTime)
        {
            SendMove(ChosenMove);
            Ticker.Enabled = false;
        }

        public override void Draw(GameTime gameTime)
        {
        }

        public override void Update(GameTime gameTime)
        {
            Ticker.Update(gameTime);
        }
    }
}
