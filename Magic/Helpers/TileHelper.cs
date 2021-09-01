using Newtonsoft.Json;
using Magic.Entities;
using Magic.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Magic.Helpers
{
    public class TileHelper
    {

        // Part avec le drawEngine
        private CardHelper card = new CardHelper();
        private Random random = new Random();

        #region DrawEngine
        public string RandomLand()
        {
            string land = string.Empty;
            switch (random.Next(0, 5))
            {
                case 0:
                    land = Land.Plain.ToString();
                    break;

                case 1:
                    land = Land.Island.ToString();
                    break;

                case 2:
                    land = Land.Forest.ToString();
                    break;

                case 3:
                    land = Land.Mountain.ToString();
                    break;

                case 4:
                    land = Land.Swamp.ToString();
                    break;
            }

            return land;
        }

        public ResponseCard RandomCard(string land, bool isBeginning = false)
        {
            int rarity;

            if (isBeginning)
            {
                rarity = 0;
            }
            else
            {
                var rand = random.Next(1, 101);

                //Commom 60%
                //Uncommon 20%
                //Rare 15%
                //Mythic 5%


                if (rand <= 60)
                {
                    //Commom
                    rarity = (int)RarityCard.Common;
                }
                else if (rand <= 80)
                {
                    //Uncommon
                    rarity = (int)RarityCard.Uncommon;
                }
                else if (rand <= 95)
                {
                    //Rare
                    rarity = (int)RarityCard.Rare;
                }
                else
                {
                    //Mythic
                    rarity = (int)RarityCard.Mythic;
                }
            }

            var cardLand = card.GetCardForLand(rarity, land);

            if (cardLand == null)
            {
                cardLand = RandomCard(land, true);
            }

            return cardLand;
        }

        #endregion

        public Tile GetStartTile(List<Tile> tiles)
        {
            return tiles.Where(t => t.IsStart).FirstOrDefault();
        }

        public Tile CreateTile(int latitude, int longitude, bool isBeginning = false, bool isStart = false)
        {
            var tile = new Tile
            {
                Guid = Guid.NewGuid().ToString(),
                Land = RandomLand(),
                Latitude = latitude,
                Longitude = longitude,
                IsStart = isStart,
                IsActual = isStart,
                IsExplored = true,
                Event = new List<ResponseCard>()
            };

            if (!isStart)
            {
                tile.Event.Add(RandomCard(tile.Land, true));
                tile.IsExplored = false;
            }

            return tile;
        }

        public List<Tile> GetResponseTiles(List<Tile> tiles)
        {
            var listTile = new List<Tile>();

            var actualTile = tiles.Find(t => t.IsActual);

            for (var i = actualTile.Latitude - 1; i <= actualTile.Latitude + 1; i++)
            {
                for (var j = actualTile.Longitude - 1; j <= actualTile.Longitude + 1; j++)
                {
                    listTile.Add(tiles.Find(t => t.Latitude == i && t.Longitude == j));
                }
            }

            return listTile;
        }
    }
}