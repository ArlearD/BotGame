using Economy.Buildings.Base;

namespace Economy.Buildings
{
    public class Shop : Building
    {
        public Shop(string name)
        {
            isOn = true;
            base.name = name;
            cost = new Resources(0, 300, 300, 300);
            input = new Resources(0, 0, 0, 0, 100);
            output = new Resources(1000);
        }

        public override string Type => "Shop";

        public void SellGoods(Player player, double coeff, out int payment)
        {
            payment = (int)(100 * coeff);
            var workersMoney = player.workersMoney;
            if (workersMoney < output.credits * coeff)
            {
                var coeff2 = workersMoney / (double)output.credits;
                if (player.RemoveResources(input * coeff2))
                {
                    player.workersMoney -= (int) (output.credits * coeff2);
                    player.AddResources(output * coeff2);
                    payment = (int) (100 * coeff2);
                }
                else
                    payment = 0;
            }
            else
            {
                if (player.RemoveResources(input * coeff))
                {
                    player.workersMoney -= (int) (output.credits * coeff);
                    player.AddResources(output * coeff);
                }
                else
                    payment = 0;
            }
        }
    }
}
