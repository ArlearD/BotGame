using Economy;

namespace Assets.Scripts.Economy.Data
{
    public static class PlayersData
    {
        public static EconomyPlayerType CurrentPlayer { get; set; }

        public static PlayerDataFieldsInfo Leonardo { get; set; } = new PlayerDataFieldsInfo();

        public static PlayerDataFieldsInfo Raphael { get; set; } = new PlayerDataFieldsInfo();

        public static PlayerDataFieldsInfo Donatello { get; set; } = new PlayerDataFieldsInfo();

        public static PlayerDataFieldsInfo Michelangelo { get; set; } = new PlayerDataFieldsInfo();

    }
}
