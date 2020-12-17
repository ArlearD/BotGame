using Assets.Scripts.Bot;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameControl
{
    public class MapController : MonoBehaviour
    {
        private static List<GameObject> bots;

        public GameObject botPrefab;

        public void Init(float y, Vector2[] positions, Vector2 middlePos)
        {
            bots = new List<GameObject>();

            string bot1Code = @"
        public void Do()
        {
            var botPostitions = _map.Vizor();
            var currentPosition = _bot.GetCurrentPosition();
            foreach (var position in botPostitions)
            {
                if (position.x != currentPosition.x && position.y != currentPosition.y)
                {
                    if (Math.Abs(position.x - currentPosition.x) < 2 && Math.Abs(position.y - currentPosition.y) < 2)
                    {
                        _bot.Rotate(position);
                        _bot.Attack();
                    }
                    else
                    {
                        _bot.GoToPossition(position.x - 1, position.y - 1);
                        _bot.Attack();
                    }
                }
            }
        }";

            InitializeBot(new Vector3(positions[0].x, y, positions[0].y), bot1Code, "Arleard");
            InitializeBot(new Vector3(490, y, 490));
        }

        private void InitializeBot(Vector3 position, string code = null, string name = null)
        {
            var bot = Instantiate(botPrefab, position, Quaternion.identity);
            try
            {
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(name))
                    bot.GetComponent<BotController>().InitUserBot(code, name);
            }
            catch (System.Exception)
            {
            }

            bots.Add(bot);
        }


        void Update()
        {
            
        }

        public List<Vector2> Vizor()
        {
            var positions = new List<Vector2>();

            foreach (var bot in bots)
                positions.Add(new Vector2(bot.transform.position.x - 460, bot.transform.position.z - 460));

            return positions;
        }
    }
}
