﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quoridor_webAPI.Data.Models;
using quoridor_webAPI.Data.ViewModels;

namespace quoridor_webAPI.Data.Services
{
    public class PlayerService
    {
        private Player _player;
        public void AddPlayer(PlayerVM player)
        {
            _player = new Player(1)
            {
                Id = 1,
                coordinate = player.Coordinate,
                amountOfWalls = player.amountOfWalls
            };

        }

        public void UpdatePlayer(PlayerVM player)
        {
            _player.coordinate = player.Coordinate;
            _player.amountOfWalls = player.amountOfWalls;
        }

        public Coordinate GetValidPosition()
        {
            //A* algorithm

            return new Coordinate(0, 0);
        }
    }
}
