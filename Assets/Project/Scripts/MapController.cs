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

        void Start()
        {
            var bot1 = Instantiate(prefab);

            var controller = bot1.GetComponent<BotController>();
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
