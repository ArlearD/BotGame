using System;

namespace Economy
{
    [Serializable]
    public class Resources
    {
        public int credits;
        public int ore;
        public int oil;
        public int wood;
        public int goods;

        public Resources(int credits = 0, int ore = 0, int oil = 0, int wood = 0, int goods = 0)
        {
            this.credits = credits;
            this.ore = ore;
            this.oil = oil;
            this.wood = wood;
            this.goods = goods;
        }

        public static Resources operator +(Resources a, Resources b) =>
            new Resources(
                a.credits + b.credits,
                a.ore + b.ore,
                a.oil + b.oil,
                a.wood + b.wood,
                a.goods + b.goods);

        public static Resources operator -(Resources a, Resources b) =>
            new Resources(
                a.credits - b.credits,
                a.ore - b.ore,
                a.oil - b.oil,
                a.wood - b.wood,
                a.goods - b.goods);

        public static bool operator>(Resources a, Resources b) =>
            a.credits > b.credits || a.ore > b.ore || a.oil > b.oil || a.wood > b.wood || a.goods > b.goods;

        public static bool operator <(Resources a, Resources b) =>
            b > a;

        public static Resources operator *(Resources a, double b)
        {
            return new Resources(
                (int)(a.credits * b),
                (int)(a.ore * b),
                (int)(a.oil * b),
                (int)(a.wood * b),
                (int)(a.goods * b));
        }
    }
}
