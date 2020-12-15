using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IBot
    {
        void TakeDamage(int damage);

        void GoToPossition(float x, float y);

        void Attack(GameObject enemy);

        void Rotate(float angle);
    }
}
