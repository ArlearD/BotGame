using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IBot
    {
        void TakeDamage(int damage);

        void GoToPossition(float x, float y);

        void Attack();

        void Rotate(GameObject target);
    }
}
