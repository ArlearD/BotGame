using Assets.Scripts.Bot;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameControl
{
    public class MapController : MonoBehaviour
    {
        private List<Vector2> _botPositions;

        private static List<GameObject> bots;

        public GameObject prefab;

        public void Init(float y, Vector2[] positions, Vector2 middlePos)
        {
            bots = new List<GameObject>();
            //foreach (var position in positions)
            //{
            //    var bot = Instantiate(prefab, new Vector3(position.x, y, position.y), Quaternion.identity);
            //    bot.transform.LookAt(new Vector3(middlePos.x, y, middlePos.y));
            //}



            //var bot1 = Instantiate(prefab, new Vector3(500f, y + 0.5f, 500), Quaternion.identity);

            //var controller = bot1.GetComponent<BotController>();


            string code = @"
        public void Do()
        {
            var botPostitions = _map.Vizor();
            var currentPosition = _bot.GetCurrentPosition();
            foreach (var position in botPostitions)
            {
                if (position.x != currentPosition.x && position.y != currentPosition.y)
                    {
                        _bot.GoToPossition(position.x, position.y);
                    }
            }
            _bot.Attack();
        }";

            InitializeBot(new Vector3(positions[0].x, y, positions[0].y), code);
            InitializeBot(new Vector3(490, y, 490));
        }

        private void InitializeBot(Vector3 position, string code = null)
        {
            var bot = Instantiate(prefab, position, Quaternion.identity);
            try
            {
                if (!string.IsNullOrEmpty(code))
                    bot.GetComponent<BotController>().SetUserCode(code);
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
