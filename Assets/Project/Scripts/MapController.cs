using Assets.Project.CodeNameData;
using Assets.Scripts.Bot;
using Assets.Scripts.Economy.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameControl
{
    public class MapController : MonoBehaviour
    {
        private static List<GameObject> bots;

        public GameObject botPrefab;

        private float Tick;
        private float Tick2;

        [SerializeField]
        private Text timeInfo;

        [SerializeField]
        private Text timeOutInfo;

        public bool GameIsStopped;

        public async void Init(float y, Vector2[] positions, Vector2 middlePos)
        {
            Tick = 0f;
            timeOutInfo.enabled = false;

            bots = new List<GameObject>();

            string bot1Code = @"
            var botPostitions = _bot.Vizor();
            var currentPosition = _bot.GetPosition();
            foreach (var position in botPostitions)
            {
                if (position.x != currentPosition.x && position.y != currentPosition.y)
                {
                    if (Math.Abs(position.x - currentPosition.x) < 2 && Math.Abs(position.y - currentPosition.y) < 2)
                    {
                        _bot.GoToPosition(position.x - 1, position.y - 1);
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
";
            //var a = _bot.Vizor();
            //_bot.GoToPosition(a[0].x, a[0].y);
            //_bot.Attack();

            //var botPostitions = _bot.Vizor();
            //var currentPosition = _bot.GetPosition();
            //foreach (var position in botPostitions)
            //{
            //    if (position.x != currentPosition.x && position.y != currentPosition.y)
            //    {
            //        _bot.GoToPosition(position.x - 1, position.y - 1);
            //        _bot.Rotate(position);
            //        _bot.Attack();
            //    }
            //}



            //InitializeBot(new Vector3(positions[0].x, y, positions[0].y), bot1Code, "Arleard");
            //InitializeBot(new Vector3(490, y, 490));

            if (!string.IsNullOrEmpty(Data.Player1Code))
                InitializeBot(new Vector3(positions[0].x, y, positions[0].y), Data.Player1Code, Data.Player1Name, PlayersData.Leonardo);
            if (!string.IsNullOrEmpty(Data.Player2Code))
                InitializeBot(new Vector3(positions[1].x, y, positions[1].y), Data.Player2Code, Data.Player2Name, PlayersData.Raphael);
            if (!string.IsNullOrEmpty(Data.Player3Code))
                InitializeBot(new Vector3(positions[2].x, y, positions[2].y), Data.Player3Code, Data.Player3Name, PlayersData.Donatello);
            if (!string.IsNullOrEmpty(Data.Player4Code))
                InitializeBot(new Vector3(positions[3].x, y, positions[3].y), Data.Player4Code, Data.Player4Name, PlayersData.Michelangelo);
        }

        private void InitializeBot(Vector3 position, string code = null, string name = null, PlayerDataFieldsInfo playerDataFieldsInfo = null)
        {
            var bot = Instantiate(botPrefab, position, Quaternion.identity);
            try
            {
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(name))
                    bot.GetComponent<BotController>().InitUserBot(code, name, playerDataFieldsInfo);
            }
            catch (System.Exception)
            {
                Debug.LogError($"Ошибка создания бота: {name}");
            }

            bots.Add(bot);
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P) && Tick < 60)
            {
                Tick = 60;
            }

            Tick += Time.deltaTime;

            timeInfo.text = Tick.ToString();

            if (Tick > 60 || bots.Where(x => !x.GetComponent<BotController>().IsDead).Count() == 1)
            {
                Tick2 += Time.deltaTime;
                GameIsStopped = true;
                timeOutInfo.enabled = true;
                if (Tick > 65 || Tick2 > 5)
                {
                    GameIsStopped = true;
                    var botControllers = bots.Select(x => x.GetComponent<BotController>());
                    foreach (var botController in botControllers)
                    {
                        if (botController.IsDead)
                        {
                            botController.playerDataFields.LoseBattle();
                        }
                        else
                        {
                            botController.playerDataFields.WinBattle();
                        }
                    }

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    SceneManager.LoadScene(0);
                }
            }
        }


        public List<Vector2> Vizor(string watcherName)
        {
            var positions = new List<Vector2>();
            var notDeadBots = bots
                .Where(x => !x.GetComponent<BotController>().IsDead 
                && x.GetComponent<BotController>().nickName.text != watcherName);
            foreach (var bot in notDeadBots)
                positions.Add(new Vector2(bot.transform.position.x - 460, bot.transform.position.z - 460));

            return positions;
        }
    }
}
