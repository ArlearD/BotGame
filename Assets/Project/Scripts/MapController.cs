using Assets.Scripts.Bot;
using System.Collections.Generic;
using UnityEngine;

namespace GameControl
{
    public class MapController : MonoBehaviour
    {
        private List<Vector2> _botPositions;

        public List<BotController> bots;

        public GameObject prefab;

        public void Init(float y, Vector2[] positions, Vector2 middlePos)
        {
            foreach (var position in positions)
            {
                var bot = Instantiate(prefab, new Vector3(position.x, y, position.y), Quaternion.identity);
                bot.transform.LookAt(new Vector3(middlePos.x, y, middlePos.y));
            }

            //var bot1 = Instantiate(prefab, new Vector3(500f, y + 0.5f, 500), Quaternion.identity);

            //var controller = bot1.GetComponent<BotController>();
        }
        
        
        void Update()
        {
            
        }

        public List<Vector2> GetCurrentBotsPositions()
        {
            var positions = new List<Vector2>();

            foreach (var bot in bots)
                positions.Add(new Vector2(bot.transform.position.x, bot.transform.position.z));

            return positions;
        }
    }
}
