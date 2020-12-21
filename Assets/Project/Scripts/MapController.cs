using Assets.Project.CodeNameData;
using Assets.Scripts.Bot;
using Assets.Scripts.Economy.Data;
using System;
using System.Collections.Generic;
using System.IO;
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

        [SerializeField]
        private GameObject Player;

        private float Tick;
        private float Tick2;

        [SerializeField]
        private Text timeInfo;

        [SerializeField]
        private Text timeOutInfo;

        public bool GameIsStopped;

        public async void Init(float y, Vector2[] positions, Vector2 middlePos)
        {
            Player.transform.position = new Vector3(middlePos.x, y + 10, middlePos.y);
            Tick = 0f;
            timeOutInfo.enabled = false;

            bots = new List<GameObject>();

            string bot1Code = @"
            var botPostitions = _bot.Vizor();
            var currentPosition = _bot.GetPosition();
            foreach (var position in botPostitions)
            {
                    if (Math.Abs(position.x - currentPosition.x) < 2 && Math.Abs(position.y - currentPosition.y) < 2)
                    {
                        _bot.GoToPosition(position.x - 1, position.y - 1);
                        _bot.Rotate(position);
                        _bot.Attack();
                    }
                    else
                    {
                        _bot.GoToPosition(position.x, position.y);
                        _bot.Attack();
                    }
            }
";

            string bot2Code = @"
            var a = _bot.Vizor();
            _bot.GoToPosition(a[0].x, a[0].y);
            _bot.Attack();
";

            string bot3Code = @"
            var a = _bot.Vizor();
            _bot.GoToPosition(a[0].x, a[0].y);
            var currentPosition = _bot.GetPosition();
            _bot.Attack();
            if (Math.Abs(a[0].x - currentPosition.x) < 4 && Math.Abs(a[0].y - currentPosition.y) < 4)
            {
                _bot.Suicide();
            }
";

            string bot4Code = @"
            var a = _bot.Vizor();
            _bot.GoToPosition(a[0].x, a[0].y);
            var currentPosition = _bot.GetPosition();
            _bot.Attack();
            if (Math.Abs(a[0].x - currentPosition.x) < 4 && Math.Abs(a[0].y - currentPosition.y) < 4 && _bot.Health < 60)
            {
                _bot.Suicide();
            }
";

            string bot5Code = @"
            var botPostitions = _bot.Vizor();
            var currentPosition = _bot.GetPosition();
            if (botPostitions.Count > 1)
            {
            var isClose = false;
            foreach (var position in botPostitions)
            {
                    if (Math.Abs(position.x - currentPosition.x) < 8 && Math.Abs(position.y - currentPosition.y) < 8)
                    {
                        isClose = true;
                    }
                    else isClose = false;
            }
            float xDir = 0;
            float yDir = 0;
            if (isClose && Math.Abs(currentPosition.x) < 4) 
                xDir = currentPosition.x + 2;
                else xDir = currentPosition.x + 2;
            
            if (isClose && Math.Abs(currentPosition.y) < 4) 
                yDir = currentPosition.y + 2;
                else yDir = currentPosition.y + 2;


            _bot.GoToPosition(xDir, yDir);
            }
            else
            foreach (var position in botPostitions)
            {
                    if (Math.Abs(position.x - currentPosition.x) < 2 && Math.Abs(position.y - currentPosition.y) < 2)
                    {
                        _bot.GoToPosition(position.x - 1, position.y - 1);
                        _bot.Rotate(position);
                        _bot.Attack();
                    }
                    else
                    {
                        _bot.GoToPosition(position.x, position.y);
                        _bot.Attack();
                    }
            }
";



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

            if (Tick > 60 || bots.Where(x => !x.GetComponent<BotController>().IsDead)?.Count() <= 1)
            {
                Tick2 += Time.deltaTime;
                GameIsStopped = true;
                timeOutInfo.enabled = true;
                if (Tick > 65 || Tick2 > 5)
                {
                    GameIsStopped = true;
                    var botControllers = bots.Select(x => x.GetComponent<BotController>());
                    using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + @"\gameResults.json", true))
                    {
                        foreach (var botController in botControllers)
                        {
                            if (botController.IsDead)
                            {
                                sw.WriteLine(JsonUtility.ToJson(new GameResult(
                                    botController.Code,
                                    "0",
                                    botController.IHaveArmor ? "1" : "0",
                                    botController.IHaveWeapon ? "1" : "0",
                                    botController.IHaveBoots ? "1" : "0")
                                    ));
                                botController.playerDataFields.LoseBattle();
                            }
                            else
                            {
                                sw.WriteLine(JsonUtility.ToJson(new GameResult(
                                    botController.Code,
                                    "1",
                                    botController.IHaveArmor ? "1" : "0",
                                    botController.IHaveWeapon ? "1" : "0",
                                    botController.IHaveBoots ? "1" : "0")
                                    ));
                                botController.playerDataFields.WinBattle();
                            }


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

    [Serializable]
    public class GameResult
    {
        public string code;
        public string IsWinner;
        public string HaveArmor;
        public string HaveWeapon;
        public string HaveBoots;

        public GameResult(string code, string isWinner, string haveArmor, string haveWeapon, string haveBoots)
        {
            this.code = code;
            IsWinner = isWinner;
            HaveArmor = haveArmor;
            HaveWeapon = haveWeapon;
            HaveBoots = haveBoots;
        }
    }
}
