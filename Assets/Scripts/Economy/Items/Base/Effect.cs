namespace Economy.Items.Base
{
    public class Effect
    {
        public readonly int Health;
        public readonly int Damage;
        public readonly int Speed;

        public Effect(int health, int damage, int speed)
        {
            Health = health;
            Damage = damage;
            Speed = speed;
        }

        public static Effect operator +(Effect a, Effect b)
        {
            return new Effect(a.Health + b.Health, a.Damage + b .Damage, a.Speed + b.Speed);
        }
    }
}
