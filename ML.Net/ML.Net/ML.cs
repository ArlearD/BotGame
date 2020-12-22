using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace ML.Net
{
    public class ML
    {
        // входные параметры, определяем их
        public class GameData
        {
            [LoadColumn(0)]
            public float FirstAt;

            [LoadColumn(1)]
            public float SecondAt;

            [LoadColumn(2)]
            public float ThirdAt;

            [LoadColumn(3)]
            public float FourthAt;

            [LoadColumn(4)]
            public float HaveArmor;

            [LoadColumn(5)]
            public float HaveWeapon;

            [LoadColumn(6)]
            public float HaveBoots;

            [LoadColumn(7)]
            public string Label;
        }

        // IrisPrediction является результатом, возвращенным из операций прогнозирования
        public class Prediction
        {
            [ColumnName("PredictedLabel")]
            public string PredictedLabels;
        }

        private static List<float> CreateNewData(List<string> list)
        {
            var newList = new List<float>();
            var number1 = ParseForCreate(list[0]);
            var number2 = ParseForCreate(list[1]);
            var number3 = ParseForCreate(list[2]);
            var number4 = ParseForCreate(list[3]);
            newList.Add(number1);
            newList.Add(number2);
            newList.Add(number3);
            newList.Add(number4);
            return newList;
        }

        private static List<float> ParseClothes(GameResult gameRes)
        {
            var list = new List<float>();
            list.Add(float.Parse(gameRes.HaveArmor, CultureInfo.InvariantCulture.NumberFormat));
            list.Add(float.Parse(gameRes.HaveWeapon, CultureInfo.InvariantCulture.NumberFormat));
            list.Add(float.Parse(gameRes.HaveBoots, CultureInfo.InvariantCulture.NumberFormat));
            return list;
        }

        private static float ParseForCreate(string text)
        {
            string str = "";
            if (text == "zero")
                return 0f;
            var newText = text.Substring(5);
            string[] words = newText.Split(new char[] { '(' });
            str = GetCommandDictionary()[words[0]];
            if (words[1].Length == 1)
                return float.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
            if (str == "3")
                return float.Parse(str + "." + words[1].Substring(0, words[1].Length - 1),
                    CultureInfo.InvariantCulture.NumberFormat);
            if (str == "5")
            {
                char c = ')';
                var index = words[1].IndexOf(c);
                var s = words[1].Substring(0, index);
                str = str + "." + GetParamDictionary()[s];
                return float.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
            }
            str = str + ".";
            return GetAfterDotPart(ref str, words);
        }

        private static float GetAfterDotPart(ref string str, string[] words)
        {
            var mas = words[1].Split(' ');
            foreach (var s in mas)
            {
                if (s.Contains("a[") && s.Contains("x"))
                {
                    str = str + "002";
                    continue;
                }
                if (s.Contains("a[") && s.Contains("y"))
                {
                    str = str + "002";
                    continue;
                }
                char ch = ',';
                int indexOfChar = s.IndexOf(ch);
                if (indexOfChar == -1)
                {
                    ch = ')';
                    indexOfChar = s.IndexOf(ch);
                    if (indexOfChar == -1)
                        str = str + GetParamDictionary()[s];
                    else
                        str = str + s.Substring(0, indexOfChar);
                }
                else
                    str = str + s.Substring(0, indexOfChar);
            }
            return float.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        }

        private static Dictionary<string, string> GetCommandDictionary()
        {
            return new Dictionary<string, string>
                {
                    {"GetPosition", "1"},
                    {"GoToPosition", "2"},
                    {"TakeDamage", "3"},
                    {"Attack", "4"},
                    {"Rotate", "5"},
                    {"Suicide", "6"},
                    {"GetRotation", "7"},
                    {"Visor", "8"}
                };
        }

        private static Dictionary<string, string> GetAverseCommandDictionary()
        {
            return new Dictionary<string, string>
                {
                    {"1", "GetPosition"},
                    {"2", "GoToPosition"},
                    {"3", "TakeDamage"},
                    {"4", "Attack"},
                    {"5", "Rotate"},
                    {"6", "Suicide"},
                    {"7", "GetRotation"},
                    {"8", "Visor"}
                };
        }

        private static Dictionary<string, string> GetParamDictionary()
        {
            return new Dictionary<string, string>
                {
                    {"position", "001"},
                    {"position.x", "002"},
                    {"position.y", "003"},
                    {"-", "004"},
                    {"+", "005"},
                    {"*", "006"},
                    {"/", "007"},
                    {",", "008"},
                    {")", "009"}
                };
        }

        private static Dictionary<string, string> GetAverseParamDictionary()
        {
            return new Dictionary<string, string>
                {
                    {"001", "position"},
                    {"002", "position.x"},
                    {"003", "position.y"},
                    {"004", "-"},
                    {"005", "+"},
                    {"006", "*"},
                    {"007", "/"},
                    {"008", ","},
                    {"009", ")"}
                };
        }

        private static List<string> CreateClothes(GameData game)
        {
            var list = new List<string>();
            list.Add(game.HaveArmor.ToString());
            list.Add(game.HaveWeapon.ToString());
            list.Add(game.HaveBoots.ToString());
            return list;
        }

        private static string CreateCode(GameData res)
        {
            var str = @"
var a = _bot.Vizor();
var currentPosition = _bot.GetPosition();
foreach (var position in botPostitions)
{";
            str = str + TransformToCommand(res.FirstAt) + TransformToCommand(res.SecondAt) +
                TransformToCommand(res.ThirdAt) + TransformToCommand(res.FourthAt) + "}";
            return str;
        }

        private static string TransformToCommand(float number)
        {
            string text = "";
            if (number == 0f)
                return "";
            string str = number.ToString();
            string[] words = str.Split(new char[] { ',' });
            text = "_bot." + GetAverseCommandDictionary()[words[0]] + "(";
            if (words[0] == "4" || words[0] == "6" || words[0] == "7" || words[0] == "8")
                return text + ");\n";
            if (words[0] == "2")
                return text + "a[" + (new Random().Next(0, 10) / 1).ToString() + "].x, a[" + (new Random().Next(0, 10) / 1).ToString()
                    + "].y);\n";
            int i = 0;
            while (i < words[1].Length)
            {
                if (words[1][i] == '0' && words[1][i + 2] != '0')
                {
                    var key = words[1][i].ToString() + words[1][i + 1].ToString() + words[1][i + 2].ToString();
                    text = text + GetAverseParamDictionary()[key];
                    i = i + 3;
                }
                else
                {
                    text = text + words[1][i].ToString();
                    i = i + 1;
                }
            }
            return text + ");\n";
        }

        private static List<GameResult> DesirializeJson(string input)
        {
            var list = new List<GameResult>();
            var text = input.Split('\n');
            foreach (var e in text)
            {
                GameResult res = JsonSerializer.Deserialize<GameResult>(e);
                list.Add(res);
            }
            return list;
        }

        public static string Generator(string json)
        {
            var games = DesirializeJson(json);
            var data = new List<GameData>();
            foreach (var e in games)
            {
                var comList = new List<float>();
                var list = ParseFromDataBase(e.code);
                if (list[0] == "_bot.Vizor()")
                    list.Remove(list[0]);
                if (list[0] == "_bot.GetPosition()")
                    list.Remove(list[0]);
                if (list.Count() >= 4)
                    comList = CreateNewData(list.GetRange(0, 4));
                else
                {
                    while (list.Count() != 4)
                        list.Add("zero");
                    comList = CreateNewData(list);
                }
                var clothList = ParseClothes(e);
                data.Add(new GameData
                {
                    FirstAt = comList[0],
                    SecondAt = comList[1],
                    ThirdAt = comList[2],
                    FourthAt = comList[3],
                    HaveArmor = clothList[0],
                    HaveWeapon = clothList[1],
                    HaveBoots = clothList[2],
                    Label = e.IsWinner
                });
            }
            // Создание среды ML.NET 
            var mlContext = new MLContext();
            IDataView trainingDataView = mlContext.Data.LoadFromEnumerable<GameData>(data);
            // собираем модель для обучения
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label")
                .Append(mlContext.Transforms.Concatenate("Features", "FirstAt", "SecondAt", "ThirdAt", "FourthAt", "HaveArmor", "HaveWeapon", "HaveBoots"))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaNonCalibrated(labelColumnName: "Label", featureColumnName: "Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // Обучение модели
            //var model = pipeline.Fit(trainingDataView);

            var generate = new List<GameData>();
            var wins = data.Where(e => e.Label == "1").ToList();
            GameData gameData = new GameData
            {
                FirstAt = 0f,
                SecondAt = 0f,
                ThirdAt = 0f,
                FourthAt = 0f,
                HaveArmor = 0f,
                HaveWeapon = 0f,
                HaveBoots = 0f
            };
            int n = 0;
            while (n < 10)
            {
                gameData = CreateAttributes(wins, gameData);
                var date = new GameData
                {
                    FirstAt = gameData.FirstAt,
                    SecondAt = gameData.SecondAt,
                    ThirdAt = gameData.ThirdAt,
                    FourthAt = gameData.FourthAt,
                    HaveArmor = new Random().Next(0, 10) / 10f,
                    HaveWeapon = new Random().Next(0, 10) / 10f,
                    HaveBoots = new Random().Next(0, 10) / 10f
                };
                if (date.HaveArmor >= 0.5)
                    date.HaveArmor = 1f;
                else
                    date.HaveArmor = 0f;
                if (date.HaveWeapon >= 0.5)
                    date.HaveWeapon = 1f;
                else
                    date.HaveWeapon = 0f;
                if (date.HaveBoots >= 0.5)
                    date.HaveBoots = 1f;
                else
                    date.HaveBoots = 0f;
                generate.Add(date);
                n++;
                // Используя свои параметры, предсказываем
                //var prediction = mlContext.Model.CreatePredictionEngine<GameData, Prediction>(model)
                //    .Predict(date);
                //n++;
                //if (prediction.PredictedLabels == "1")
                //{
                //    generate.Add(date);
                //}
            }
            string answer = "";
            if (generate.Count() > 0)
            {
                foreach (var e in generate)
                {
                    var code = CreateCode(e);
                    var cloth = CreateClothes(e);
                    var gameRes = new OutputResult
                    {
                        code = code,
                        HaveArmor = cloth[0],
                        HaveWeapon = cloth[1],
                        HaveBoots = cloth[2]
                    };
                    answer = answer + JsonSerializer.Serialize<OutputResult>(gameRes) + "\n";
                }
            }
            else
            {
                var code = CreateCode(wins[0]);
                var cloth = CreateClothes(wins[0]);
                var gameRes = new OutputResult
                {
                    code = code,
                    HaveArmor = cloth[0],
                    HaveWeapon = cloth[1],
                    HaveBoots = cloth[2]
                };
                answer = answer + JsonSerializer.Serialize<OutputResult>(gameRes) + "\n";
            }

            return answer;
        }

        private static List<string> ParseFromDataBase(string str)
        {
            string word = "_bot";
            var list = new List<string>();
            int count = 0;
            string c = "";
            string[] mas = str.Split('\n');
            foreach (string s in mas)
            {
                if (s.Contains(word) && (s.Contains("if") == false))
                {
                    char chb = '_';
                    int indexofchb = s.IndexOf(chb);
                    var st = s.Remove(0, indexofchb);
                    count++;
                    char ch = ';';
                    int indexOfChar = st.IndexOf(ch);
                    c = st.Substring(0, indexOfChar);
                    list.Add(c);
                }
            }
            return list;
        }

        private static GameData CreateAttributes(List<GameData> wins, GameData gamePred)
        {
            int n = new Random().Next(0, 10);
            if (wins.Count() >= 4)
            {
                switch (n)
                {
                    case 0:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[2].ThirdAt;
                        gamePred.FourthAt = wins[3].FourthAt;
                        return gamePred;
                    case 1:
                        gamePred.FirstAt = wins[1].FirstAt;
                        gamePred.SecondAt = wins[0].SecondAt;
                        gamePred.ThirdAt = wins[3].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 2:
                        gamePred.FirstAt = wins[2].FirstAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[2].ThirdAt;
                        gamePred.FourthAt = wins[3].FourthAt;
                        return gamePred;
                    case 3:
                        gamePred.FirstAt = wins[0].SecondAt;
                        gamePred.SecondAt = wins[1].FirstAt;
                        gamePred.ThirdAt = wins[2].FourthAt;
                        gamePred.FourthAt = wins[3].ThirdAt;
                        return gamePred;
                    case 4:
                        gamePred.FirstAt = wins[3].FirstAt;
                        gamePred.SecondAt = wins[2].SecondAt;
                        gamePred.ThirdAt = wins[1].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 5:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[3].SecondAt;
                        gamePred.ThirdAt = wins[2].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 6:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[1].FirstAt;
                        gamePred.ThirdAt = wins[2].FourthAt;
                        gamePred.FourthAt = wins[3].SecondAt;
                        return gamePred;
                    case 7:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[2].ThirdAt;
                        gamePred.FourthAt = wins[3].FourthAt;
                        return gamePred;
                    case 8:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[2].SecondAt;
                        gamePred.ThirdAt = wins[3].ThirdAt;
                        gamePred.FourthAt = wins[1].FourthAt;
                        return gamePred;
                    case 9:
                        gamePred.FirstAt = wins[0].FourthAt;
                        gamePred.SecondAt = wins[3].SecondAt;
                        gamePred.ThirdAt = wins[1].FirstAt;
                        gamePred.FourthAt = wins[3].FourthAt;
                        return gamePred;
                    case 10:
                        gamePred.FirstAt = wins[0].SecondAt;
                        gamePred.SecondAt = wins[2].FourthAt;
                        gamePred.ThirdAt = wins[1].FirstAt;
                        gamePred.FourthAt = wins[3].FourthAt;
                        return gamePred;
                }
            }
            if (wins.Count() == 3)
            {
                switch (n)
                {
                    case 0:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[2].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 1:
                        gamePred.FirstAt = wins[1].FirstAt;
                        gamePred.SecondAt = wins[0].SecondAt;
                        gamePred.ThirdAt = wins[2].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 2:
                        gamePred.FirstAt = wins[2].FirstAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[2].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 3:
                        gamePred.FirstAt = wins[0].SecondAt;
                        gamePred.SecondAt = wins[1].FirstAt;
                        gamePred.ThirdAt = wins[2].FourthAt;
                        gamePred.FourthAt = wins[1].ThirdAt;
                        return gamePred;
                    case 4:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[2].SecondAt;
                        gamePred.ThirdAt = wins[1].ThirdAt;
                        gamePred.FourthAt = wins[2].FourthAt;
                        return gamePred;
                    case 5:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[2].SecondAt;
                        gamePred.ThirdAt = wins[2].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 6:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[1].FirstAt;
                        gamePred.ThirdAt = wins[1].FourthAt;
                        gamePred.FourthAt = wins[2].SecondAt;
                        return gamePred;
                    case 7:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[2].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 8:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[2].SecondAt;
                        gamePred.ThirdAt = wins[1].ThirdAt;
                        gamePred.FourthAt = wins[1].FourthAt;
                        return gamePred;
                    case 9:
                        gamePred.FirstAt = wins[0].FourthAt;
                        gamePred.SecondAt = wins[2].SecondAt;
                        gamePred.ThirdAt = wins[1].FirstAt;
                        gamePred.FourthAt = wins[2].FourthAt;
                        return gamePred;
                    case 10:
                        gamePred.FirstAt = wins[0].SecondAt;
                        gamePred.SecondAt = wins[2].FourthAt;
                        gamePred.ThirdAt = wins[1].FirstAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                }
            }
            if (wins.Count() == 2)
            {
                switch (n)
                {
                    case 0:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[1].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 1:
                        gamePred.FirstAt = wins[1].FirstAt;
                        gamePred.SecondAt = wins[0].SecondAt;
                        gamePred.ThirdAt = wins[1].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 2:
                        gamePred.FirstAt = wins[1].FirstAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[1].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 3:
                        gamePred.FirstAt = wins[0].SecondAt;
                        gamePred.SecondAt = wins[1].FirstAt;
                        gamePred.ThirdAt = wins[0].FourthAt;
                        gamePred.FourthAt = wins[1].ThirdAt;
                        return gamePred;
                    case 4:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[1].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 5:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[0].ThirdAt;
                        gamePred.FourthAt = wins[1].FourthAt;
                        return gamePred;
                    case 6:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[1].FirstAt;
                        gamePred.ThirdAt = wins[1].FourthAt;
                        gamePred.FourthAt = wins[0].SecondAt;
                        return gamePred;
                    case 7:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[0].ThirdAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 8:
                        gamePred.FirstAt = wins[0].FirstAt;
                        gamePred.SecondAt = wins[0].SecondAt;
                        gamePred.ThirdAt = wins[1].ThirdAt;
                        gamePred.FourthAt = wins[1].FourthAt;
                        return gamePred;
                    case 9:
                        gamePred.FirstAt = wins[0].FourthAt;
                        gamePred.SecondAt = wins[1].SecondAt;
                        gamePred.ThirdAt = wins[1].FirstAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                    case 10:
                        gamePred.FirstAt = wins[1].SecondAt;
                        gamePred.SecondAt = wins[0].FourthAt;
                        gamePred.ThirdAt = wins[1].FirstAt;
                        gamePred.FourthAt = wins[0].FourthAt;
                        return gamePred;
                }
            }
            if (wins.Count() == 1)
            {
                gamePred.FirstAt = wins[0].FirstAt;
                gamePred.SecondAt = wins[0].SecondAt;
                gamePred.ThirdAt = wins[0].ThirdAt;
                gamePred.FourthAt = wins[0].FourthAt;
            }

            return gamePred;
        }
    }
}
