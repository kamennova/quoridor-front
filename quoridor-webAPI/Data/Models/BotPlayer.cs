using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quoridor_webAPI.Data.Models
{
    public class BotPlayer : Player
    {

        public BotPlayer(int id) : base(id) {
        }

        public Move getMove(Board board, List<Player> players) {
                return generateRandomMove(board, players);
        }

        private Move generateRandomMove(Board board, List<Player> players){
//            List<Coordinate> possible = getPossibleSteps(board, players);

            return new Move("Step", null, new Coordinate(0, 0));
        }
    }
}