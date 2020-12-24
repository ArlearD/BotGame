﻿using Assets.Scripts.Economy.Data;
using Assets.Scripts.Interfaces;
using GameControl;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ServerPart.Scripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Assets.Scripts.Bot
{
    public class BotController : MonoBehaviour, IBot
    {
        private Camera _mainCamera;
        private NavMeshAgent _agent;
        private MethodInfo _playersUpdate;
        private object _pleyerCodeClassObject;
        public int Health { get; private set; }
        private float Reload;
        private float Tick;
        private bool _botWithCode;
        private string asemblyName;

        public bool IHaveArmor;
        public bool IHaveWeapon;
        public bool IHaveBoots;

        public Animation Animation;
        public GameObject Exploson;

        public PlayerDataFieldsInfo playerDataFields;
        public HealthBar healthBar;
        public float reloadTime;
        public MapController mapController;
        public Text nickName;
        public bool IsDead;
        public int Damage;
        public string Code;

        private bool _isServer = true;
        
        public void InitClientBot(string botName)
        {
            nickName.text = botName;
            _botWithCode = false;
            _isServer = false;
        }

        public void UpdateFromServer(ClientData clientData)
        {
            oldPos = transform.position;
            newPos = clientData.Position;
            _lerpTime = 0;
        }

        public void InitUserBot(string code, string botName, PlayerDataFieldsInfo playerDataFields)
        {
            Code = code;
            this.playerDataFields = playerDataFields;
            nickName.text = botName;
            _isServer = true;


            if (playerDataFields.Equipment.Armour != null)
            {
                IHaveArmor = true;
            }
            if (playerDataFields.Equipment.Boots != null)
            {
                IHaveWeapon = true;
            }
            if (playerDataFields.Equipment.Weapon != null)
            {
                IHaveBoots = true;
            }


            code = @"
using System;
using Assets.Scripts.Bot;
using GameControl;
using UnityEngine;
public class BotBrain
{
    private BotController _bot;

    public BotBrain(BotController bot)
    {
        _bot = bot;
    }
        public void Do()
        {"
          + code +
      @"}
}";

            var assembly = CompileExecutable(code);

            _botWithCode = true;
            Type magicType = assembly.GetType("BotBrain");

            ConstructorInfo[] magicConstructor = magicType.GetConstructors();
            _pleyerCodeClassObject = magicConstructor[0].Invoke(new object[] { this });

            _playersUpdate = magicType.GetMethod("Do");


            Assembly CompileExecutable(string codes)
            {
                asemblyName = botName + DateTime.Now.Ticks;

                CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>()
        { { "CompilerVersion", "v4.8" } });

                CompilerParameters parameters = new CompilerParameters()
                {
                    GenerateExecutable = false,

                    OutputAssembly = asemblyName,

                    GenerateInMemory = true,

                    TreatWarningsAsErrors = false
                };


                parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);

                parameters.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(BotController)).Location);

                parameters.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(MonoBehaviour)).Location);

                CompilerResults cr = provider.CompileAssemblyFromSource(parameters, codes);

                Debug.Log($"Количество ошибок: {cr.Errors.Count}");

                foreach (var item in cr.Errors)
                {
                    Debug.Log(item.ToString());
                }

                return cr.CompiledAssembly;
            }
        }


        void Start()
        {
            Health = IHaveArmor? 200: 100;
            Damage = IHaveWeapon? 20 : 10;
            Reload = reloadTime;
            if (healthBar != null)
                healthBar.SetMaxHealth(Health);


            _mainCamera = Camera.main;
            _agent = GetComponent<NavMeshAgent>();
            _agent.acceleration = float.MaxValue;
            _agent.angularSpeed = float.MaxValue;
            _agent.speed = IHaveBoots? 10: 5;
        }

        private void OnDestroy()
        {
            File.Delete(Directory.GetCurrentDirectory() + @"\" + asemblyName);
        }

        void OnDrawGizmos()
        {
            var angle = gameObject.transform.rotation.eulerAngles.y * Math.PI / 180;


            var botViewDirection = new Vector3(
                gameObject.transform.position.x + (float)Math.Sin(angle),
                gameObject.transform.position.y,
                gameObject.transform.position.z + (float)Math.Cos(angle));

            Gizmos.DrawSphere(botViewDirection, 0.4f);
        }

        public void Rotate(GameObject target)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;
        }

        private Vector3 oldPos;
        private Vector3 newPos;
        private float _lerpTime;
        
        void Update()
        {
            if (!IsDead && !mapController.GameIsStopped)
            {
                if (Health <= 0)
                {
                    IsDead = true;
                    gameObject.GetComponent<Collider>().enabled = false;
                    gameObject.GetComponent<NavMeshAgent>().enabled = false;
                    Animation.Stop();
                    Animation.Play("Death");
                    enabled = false;
                }

                if (_isServer)
                {
                    Tick += Time.deltaTime;
                    if (Tick >= 1 / 2f)
                    {
                        Tick = 0;
                        try
                        {
                            var _playerCodeValue = _playersUpdate.Invoke(_pleyerCodeClassObject, new object[] { });
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (Reload > 0)
                    {
                        Reload -= Time.deltaTime;
                    }
                }
                else
                {
                    transform.position = Vector3.Lerp(oldPos, newPos, _lerpTime);
                    _lerpTime += Time.deltaTime / Time.fixedDeltaTime;
                }
                

                //if (Input.GetMouseButtonDown(0) && !_botWithCode)
                //{
                //    RaycastHit hit;
                //    if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
                //    {
                //        _agent.SetDestination(hit.point);
                //    }
                //}
            }
        }

        /// <summary>
        /// Бот говорит том, где он сейчас находится.
        /// </summary>
        /// <returns>Текущие координаты в формате Vector2</returns>
        public Vector2 GetPosition()
        {
            var vector = new Vector2(transform.position.x - 460, transform.position.z - 460);
            return vector;
        }

        /// <summary>
        /// Бот идет в заданную точку.
        /// </summary>
        /// <param name="x">Координата x</param>
        /// <param name="y">Координата y</param>
        public void GoToPosition(float x, float y)
        {
            if (Health <= 0) return;

            if (x < 0 || y < 0 || x > 60 || y > 60)
                return;

            //Поправка на размер карты
            x = x + 460;
            y = y + 460;

            _agent.SetDestination(new Vector3(x, transform.position.y, y));
        }

        /// <summary>
        /// Бот бьет себя.
        /// </summary>
        /// <param name="damage">Количество урона</param>
        public void TakeDamage(int damage)
        {
            Health -= damage;
            healthBar.SetHealth(Health);
        }

        /// <summary>
        /// Бот совершает самоподрыв, теряет все здоровье и наносит такой же урон всем ботам в радиусе 5.
        /// </summary>
        public void Suicide()
        {
            var enemys = Physics.OverlapSphere(transform.position, 5f)
                .Where(x => x.gameObject.tag == "Bot" && x.gameObject != gameObject);

            foreach (var enemy in enemys)
            {
                enemy.gameObject.GetComponent<BotController>().TakeDamage(Health);
            }

            TakeDamage(Health);
            Instantiate(Exploson, transform);
        }

        /// <summary>
        /// Бот атакует область прямо перед собой и наносит урон первому попавшемуся противнику.
        /// </summary>
        public void Attack()
        {
            var viewAngle = gameObject.transform.rotation.eulerAngles.y * Math.PI / 180;

            var botViewDirection = new Vector3(
                gameObject.transform.position.x + (float)Math.Sin(viewAngle),
                gameObject.transform.position.y,
                gameObject.transform.position.z + (float)Math.Cos(viewAngle));


            var enemy = Physics.OverlapSphere(botViewDirection, 0.4f)
                .Where(x => x.gameObject.tag == "Bot" && x.gameObject != gameObject)
                .FirstOrDefault()?.gameObject;

            if (enemy != null && Reload <= 0)
            {
                enemy.GetComponent<BotController>().TakeDamage(Damage);
                Animation.Stop("Attack");
                Animation.Play("Attack");
                Reload = reloadTime;
            }
        }

        /// <summary>
        /// Бот мгновенно поворачивается в сторону заданной цели.
        /// </summary>
        /// <param name="target"></param>
        public void Rotate(Vector2 target)
        {
            Vector3 direction = (new Vector3(target.x + 460, 0, target.y + 460) - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;
        }

        /// <summary>
        /// Бот говорит о том, в каком направлении он сейчас повернут.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetRotation()
        {
            return new Vector2(transform.rotation.x, transform.rotation.z);
        }

        /// <summary>
        /// Бот говорит о оставшихся живых противниках.
        /// </summary>
        /// <returns>Список координат живых противников</returns>
        public List<Vector2> Vizor()
        {
            return mapController.Vizor(nickName.text);
        }
    }
}
