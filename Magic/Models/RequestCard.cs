namespace Magic.Models
{
    public class RequestCard
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public string SubType { get; set; }
        public int BlueMana { get; set; }
        public int GreenMana { get; set; }
        public int WhiteMana { get; set; }
        public int BlackMana { get; set; }
        public int RedMana { get; set; }
        public int NeutralMana { get; set; }
        public int Rarity { get; set; }
        public string Mechanic { get; set; }
        public string CodeName { get; set; }
        public int Power { get; set; }
        public int Defense { get; set; }
        public int EditionId { get; set; }
        public string Commentary { get; set; }
        public string UrlImage { get; set; }
    }
}