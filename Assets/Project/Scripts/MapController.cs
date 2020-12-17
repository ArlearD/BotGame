using Assets.Scripts.Bot;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Project.CodeNameData;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace GameControl
{
    public class MapController : MonoBehaviour
    {
        private static List<GameObject> bots;

        public GameObject botPrefab;

        public async void Init(float y, Vector2[] positions, Vector2 middlePos)
        {
            var a = new HttpClient();

            var b = await a.GetAsync(@"https://www.google.ru/");


            var c = await b.Content.ReadAsStringAsync();


            bots = new List<GameObject>();

            string bot1Code = @"
        public void Do()
        {
            var botPostitions = _map.Vizor();
            var currentPosition = _bot.GetPosition();
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
                        _bot.GoToPosition(position.x - 1, position.y - 1);
                        _bot.Attack();
                    }
                }
            }
        }";

            //InitializeBot(new Vector3(positions[0].x, y, positions[0].y), bot1Code, "Arleard");
            InitializeBot(new Vector3(490, y, 490));

            if (!string.IsNullOrEmpty(Data.Player1Name))
                InitializeBot(new Vector3(positions[0].x, y, positions[0].y), Data.Player1Code, Data.Player1Name);
            if (!string.IsNullOrEmpty(Data.Player2Name))
                InitializeBot(new Vector3(positions[1].x, y, positions[1].y), Data.Player2Code, Data.Player2Name);
            if (!string.IsNullOrEmpty(Data.Player3Name))
                InitializeBot(new Vector3(positions[2].x, y, positions[2].y), Data.Player3Code, Data.Player3Name);
            if (!string.IsNullOrEmpty(Data.Player4Name))
                InitializeBot(new Vector3(positions[3].x, y, positions[3].y), Data.Player4Code, Data.Player4Name);
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

        //Возвращает местоположение всех живых ботов на карте
        public List<Vector2> Vizor()
        {
            var positions = new List<Vector2>();
            var notNullBots = bots.Where(x => x != null);
            foreach (var bot in notNullBots)
                positions.Add(new Vector2(bot.transform.position.x - 460, bot.transform.position.z - 460));

            return positions;
        }
    }
}
