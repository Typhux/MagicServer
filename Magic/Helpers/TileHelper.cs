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
                    land = "Plain";
                    break;

                case 1:
                    land = "Island";
                    break;

                case 2:
                    land = "Forest";
                    break;

                case 3:
                    land = "Mountain";
                    break;

                case 4:
                    land = "Swamp";
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
                    rarity = 0;
                }
                else if (rand <= 80)
                {
                    //Uncommon
                    rarity = 1;
                }
                else if (rand <= 95)
                {
                    //Rare
                    rarity = 2;
                }
                else
                {
                    //Mythic
                    rarity = 3;
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
    }
}