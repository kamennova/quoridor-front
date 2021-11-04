using System;
using System.Collections.Generic;
using System.Linq;

namespace quoridor_webAPI.Data.Models
{
    public class BaseMinimax
    {
        public static bool doLog = true;
        private static void log(String s)
        {
            if (doLog) { Console.WriteLine(s); }
        }

        public static Dictionary<Move, int> getPossibleMoves(GameState state, int turn)
        {
            Dictionary<Move, int> steps = getPossibleStepMoves(state, turn);

            //    public static PriorityQueue < Move, int > getPossibleMoves(Player player, Board state, List < Player > players) {
            //      PriorityQueue < Move, int > steps = getPossibleStepMoves(player, state, players);

      Dictionary < Move, int > walls = getPossibleWallMoves(state, turn);
//      log("steps " + steps.Count + " walls " + walls.Count); // todo investigate wall moves
//      PriorityQueue < Move, int > walls = getPossibleWallMoves(player, state, players);
        int temp = 0;
      foreach (var wall in walls) {
//      if (temp < 80) {
        steps.Add(wall.Key, wall.Value);
        temp++;
//        }
      }

            return steps;
        }

        private static List<Coordinate> generateWalls()
        {
            List<Coordinate> walls = new List<Coordinate>();
            for (int i = 0; i < 8; i++)
            {
                for (int a = 0; a < 8; a++)
                {
                    walls.Add(new Coordinate(i, a));
                }
            }

            return walls;
        }

        private static Dictionary<Move, int> getPossibleStepMoves(GameState state, int turn)
        {
            PlayerState player = state.getPlayer(turn);

            //    private static PriorityQueue < Move, int > getPossibleStepMoves(Player player, Board state, List < Player > players) {

            Coordinate c = player.coordinate;
            Coordinate c2 = state.getOpponent(turn).coordinate;
            //      PriorityQueue < Move, int > moves = new PriorityQueue < Move, int > ();
            Dictionary<Move, int> moves = new Dictionary<Move, int>();

            MoveValidator.getPossibleSimpleStepMoves(c, state).ForEach(m =>
            {
                if (!c2.Equals(m.coordinate))
                {
                    moves.Add(m, 0);
                }
            });

      List<Coordinate> hW =  state.getHorizontalWalls();
      List<Coordinate> vW = state.getVerticalWalls();

      if (MoveValidator.isOpponentNear(state, c, turn)) {
        Coordinate vectorToOpponent = new Coordinate(c2.x-c.x, c2.y-c.y);

        if (vectorToOpponent.x == 0 && vectorToOpponent.y == 1) { // opponent on top
          if (!MoveValidator.checkWallsToTheTop(c, hW)) { // can jump at least 1 up

          if (!MoveValidator.checkWallsToTheTop(c2, hW)) {
              moves.Add(new Move("Move", null, new Coordinate(c.x, c.y + 2)), 0);
          }
            if (!MoveValidator.checkWallsToTheLeft(c2, state.getVerticalWalls())) {
              moves.Add(new Move("Move", null, new Coordinate(c.x - 1, c.y + 1)), 0);
            }

            if (!MoveValidator.checkWallsToTheRight(c2, state.getVerticalWalls()))
            {
              moves.Add(new Move("Move", null, new Coordinate(c.x + 1, c.y + 1)), 0);
            }
          }
        } else if (vectorToOpponent.x == 0 && vectorToOpponent.y == -1) {//opponent from below
        if(!MoveValidator.checkWallsToTheBottom(c, hW)){
          if(!MoveValidator.checkWallsToTheBottom(c2, state.getHorizontalWalls())){//jump ?
              moves.Add(new Move("Move", null, new Coordinate(c.x, c.y - 2)), 0);
          }
            if(!MoveValidator.checkWallsToTheLeft(c2, vW))
            {
              moves.Add(new Move("Move", null, new Coordinate(c.x - 1, c.y - 1)), 0);
            }
            if(!MoveValidator.checkWallsToTheRight(c2, state.getVerticalWalls()))
            {
              moves.Add(new Move("Move", null, new Coordinate(c.x + 1, c.y - 1)), 0);
            }
          }
        } else if (vectorToOpponent.x == 1 && vectorToOpponent.y == 0){ // opponent on the right
        if(!MoveValidator.checkWallsToTheRight(c, vW)){
          if(!MoveValidator.checkWallsToTheRight(c2, vW)){
              moves.Add(new Move("Move", null, new Coordinate(c.x + 2, c.y)), 0);
          }
            if(!MoveValidator.checkWallsToTheTop(c2, hW)){
              moves.Add(new Move("Move", null, new Coordinate(c.x+1, c.y + 1)), 0);
            }
//            if(!MoveValidator.checkWallsToTheBottom(c2, hW)) {
//              moves.Add(new Move("Move", null, new Coordinate(c.x + 1, c.y - 1)), 0);
//            }
          }
        } else if (vectorToOpponent.x == -1 && vectorToOpponent.y == 0){ // opponent on the left
        if(!MoveValidator.checkWallsToTheLeft(c, vW)){
          if(!MoveValidator.checkWallsToTheLeft(c2, vW)){//jump ?
              moves.Add(new Move("Move", null, new Coordinate(c.x - 2, c.y)), 0);
          }

            if(!MoveValidator.checkWallsToTheTop(c2, state.getHorizontalWalls())) {
              moves.Add(new Move("Move", null, new Coordinate(c.x - 1, c.y + 1)), 0);
            }

            if(!MoveValidator.checkWallsToTheBottom(c2, state.getHorizontalWalls())) {
              moves.Add(new Move("Move", null, new Coordinate(c.x - 1, c.y -1 )), 0);
            }
          }
        }
      }

            return moves;
        }

        private static int evaluateWallMove(GameState state, int turn)
        {
            PlayerState opponent = state.getOpponent(turn);
            PlayerState player = state.getPlayer(turn);

            int playerGoal = player.color == "white" ? 8 : 0;
            int opponentGoal = opponent.color == "white" ? 8 : 0;

            int distancePlayer = AStar.search(state, player.coordinate, playerGoal);

            if (distancePlayer < 0)
            {
                return -1;
            }

            int distanceOpponent = AStar.search(state, opponent.coordinate, opponentGoal);

            if (distanceOpponent < 0)
            {
                return -1;
            }

            return distanceOpponent - distancePlayer;
        }

        private static bool touchesBoard(Coordinate w)
        {
            return w.x == 0 || w.y == 0 || w.x == 7 || w.y == 7;
        }

        private static int verticalTouchesWallsNum(Coordinate w, GameState state)
        {
            int counter = 0;
            if (touchesBoard(w)) { counter += 1; }

            counter += state.getVerticalWalls().Where(vOld => vOld.x == w.x && (vOld.y == w.y - 2 || vOld.y == w.y + 2)).Count();
            if (counter < 2)
            {
                counter += state.getHorizontalWalls().Where(hOld => hOld.y == w.y && (hOld.x == w.x - 2 || hOld.x == w.x + 2)).Count();
            }

            return counter;
        }

        private static int horizontalTouchesWallsNum(Coordinate w, GameState state)
        {
            int counter = 0;
            if (touchesBoard(w)) { counter += 1; }

            counter += state.getVerticalWalls().Where(vOld => vOld.x == w.x && (vOld.y == w.y - 2 || vOld.y == w.y + 2)).Count();
            if (counter < 2)
            {
                counter += state.getHorizontalWalls().Where(hOld => hOld.y == w.y && (hOld.x == w.x - 2 || hOld.x == w.x + 2)).Count();
            }

            return counter;
        }

        private static Dictionary<Move, int> getPossibleWallMoves(GameState state, int turn)
        {
            Dictionary<Move, int> moves = new Dictionary<Move, int>();

            //    private static PriorityQueue < Move, int > getPossibleWallMoves(Player player, Board state, List < Player > players) {
            //      PriorityQueue < Move, int > moves = new PriorityQueue < Move, int > ();

            if (state.getPlayer(turn).amountOfWalls > 0)
            {
                List<Coordinate> vW = state.getVerticalWalls();
                List<Coordinate> hW = state.getHorizontalWalls();

                List<Coordinate> vMoveW = generateWalls();
                List<Coordinate> hMoveW = generateWalls();

        vW.ForEach((w) => {
          vMoveW.Remove(w);
          vMoveW.Remove(new Coordinate(w.x, w.y + 1));
          hMoveW.Remove(new Coordinate(w.x, w.y));
        });

        hW.ForEach((w) => {
          hMoveW.Remove(w);
          hMoveW.Remove(new Coordinate(w.x + 1, w.y));
          vMoveW.Remove(new Coordinate(w.x, w.y));
        });

                vMoveW.ForEach(w =>
                {
                    int rate = 0;

                    // if touches other wall, check if can be passable with a star
                    if (verticalTouchesWallsNum(w, state) >= 2)
                    {
                        rate = evaluateWallMove(state, turn);
                        if (rate < 0)
                        {
                            return;
                        }
                    }
                    else
                    {
                        // todo rate quick?
                    }
                    //          moves.Enqueue(new Move("PutWall", "vertical", w), rate);
                    moves.Add(new Move("PutWall", "vertical", w), rate);
                });

                hMoveW.ForEach(w =>
                {
                    int rate = 0;
                    if (horizontalTouchesWallsNum(w, state) >= 2)
                    {
                        rate = evaluateWallMove(state, turn);
                        if (rate < 0)
                        {
                            return;
                        }
                    }
                    else
                    {
                        // todo rate quick
                    }
                    //          moves.Enqueue(new Move("PutWall", "horizontal", w), rate);
                    moves.Add(new Move("PutWall", "horizontal", w), rate);
                });
            }

            return moves;
        }

        private static int fullEvaluate(Move move, GameState state, int turn, int distanceOpponent, int distancePlayer)
        {
            return distanceOpponent - distancePlayer - 1; // -1 because next move is opponent's
        }

        int EvaluateMove(Move move)
        {
            if (move.type == "PutWall")
            {

            }
            else
            {

            }
            //ehristic algorithm?
            return 0;
        }

        private static void log(string text, int tab)
        {
            log(string.Concat(Enumerable.Repeat("   ", tab)) + tab + ") " + text);
        }

        private static void buildTree(Node node, GameState state, int turn, int depth)
        {
            // log("=============== depth " + depth + " ====, move: " + node.move.type + " " + node.move.coordinate + (turn == 0 ? "white" : "black") + " eval " + node.rate, 2 - depth);

            if (depth == 0)
            {
                return;
            }

            Dictionary<Move, int> possibleMoves = getPossibleMoves(state, turn);

            PlayerState opponent = state.getOpponent(turn);
            PlayerState player = state.getPlayer(turn);

            int playerGoal = player.color == "white" ? 8 : 0;
            int opponentGoal = opponent.color == "white" ? 8 : 0;

            int distancePlayer = AStar.search(state, player.coordinate, playerGoal);
            int distanceOpponent = AStar.search(state, opponent.coordinate, opponentGoal);

            foreach (var move in possibleMoves)
            {
                GameState newState = state.applyMoveToNew(move.Key, turn);

                int distancePlayer2 = AStar.search(newState, newState.getPlayer(turn).coordinate, playerGoal);
                int distanceOpponent2 = distanceOpponent;
                //       log(" move test " + move.Key + " dist2 " + distancePlayer2 + " " + distanceOpponent2, 2 - depth);

                // opponent's distance to goal may change
                if (move.Key.type == "PutWall" || MoveValidator.isOpponentNear(state, move.Key.coordinate, turn))
                {
                    distanceOpponent2 = AStar.search(state, opponent.coordinate, opponentGoal);
                }

                int rateFull = fullEvaluate(move.Key, newState, turn, distanceOpponent2, distancePlayer2);

                Node child = new Node(move.Key, rateFull);
                node.Insert(child);
                buildTree(child, newState, turn == 0 ? 1 : 0, depth - 1);
            }
        }

        private static Node selectNode(Node node, bool isMe, bool isRoot)
        {
            Node max = node;
            if (node.children.Count > 0)
            {
                max = node.children[0];
                node.children.ForEach(child =>
                {
                    Node last = selectNode(child, !isMe, false);
                    if (last.rate > max.rate)
                    {
                        max = child;
                    }
                });

                node.rate = isMe ? max.rate : -max.rate;
            }

            return isRoot ? max : node;
        }

        public static Move ChooseMove(GameState state, bool isWhite)
        {
            int turn = isWhite ? 0 : 1;
            Move zeroMove = new Move("null", "null", new Coordinate(0, 0));
            Node root = new Node(zeroMove, 0);
            buildTree(root, state, turn, 2);
            Node best = selectNode(root, true, true);
            log("best " + best.move.type + " " + best.move.coordinate + " " + best.rate);

            return best.move;
        }
    }
}