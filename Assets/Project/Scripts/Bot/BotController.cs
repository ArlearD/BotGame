using Assets.Scripts.Interfaces;
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Bot
{
    public class BotController : MonoBehaviour, IBot
    {
        private Camera _mainCamera;
        public HealthBar healthBar;
        private NavMeshAgent _agent;
        private MethodInfo _playersUpdate;
        private object _pleyerCodeClassObject;
        public float reloadTime = 1f;
        public int maxHealth = 100;

        public Collider attackZoneCollider;

        private int Health;
        private float Reload;



        public void SetUserCode(string code)
        {
            var assembly = CompileExecutable(code);


            Type magicType = assembly.GetType("TestClass");


            ConstructorInfo[] magicConstructor = magicType.GetConstructors();
            _pleyerCodeClassObject = magicConstructor[0].Invoke(new object[] { this });

            _playersUpdate = magicType.GetMethod("TestMethod");


            Assembly CompileExecutable(string codes)
            {

                CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider(new System.Collections.Generic.Dictionary<string, string>()
        { { "CompilerVersion", "v4.8" } });

                CompilerParameters parameters = new CompilerParameters()
                {
                    GenerateExecutable = false,

                    OutputAssembly = "Test",

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
            Health = maxHealth;
            Reload = reloadTime;
            if(healthBar!=null)
                healthBar.SetMaxHealth(maxHealth);


            _mainCamera = Camera.main;
            _agent = GetComponent<NavMeshAgent>();
            _agent.acceleration = float.MaxValue;
            _agent.angularSpeed = float.MaxValue;
            _agent.speed = 30;

            string code = @"
using Assets.Scripts.Bot;
public class TestClass 
{
    private BotController _bot;

    public TestClass(BotController bot)
    {
        _bot = bot;
    }

    public void TestMethod()
    {
        _bot.GoToPossition(13,76);
    }
}";

            SetUserCode(code);
        }

        void OnDrawGizmos()
        {
            var angle = gameObject.transform.rotation.eulerAngles.y * Math.PI / 180;


            Debug.Log(angle);


            var botViewDirection = new Vector3(
                gameObject.transform.position.x + (float)Math.Sin(angle),
                gameObject.transform.position.y,
                gameObject.transform.position.z + (float)Math.Cos(angle));

            Gizmos.DrawSphere(botViewDirection, 0.4f);
        }

        void Update()
        {
            if (Health <= 0) Destroy(gameObject);

            //if (Input.GetMouseButtonDown(0))
            //{
            //    var _playerCodeValue = _playersUpdate.Invoke(_pleyerCodeClassObject, new object[] { });
            //}


            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    _agent.SetDestination(hit.point);
                }
            }

            int angle = 0;

            if (angle >= 360)
            {
                angle = 0;
            }
            else
            {
                Rotate(angle);
                angle++;
            }

            var botViewDirection = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 1);

            var enemy = Physics.OverlapSphere(botViewDirection, 0.4f)
                .Where(x => x.gameObject.tag == "Bot" && x.gameObject != gameObject)
                .FirstOrDefault()?.gameObject;

            if (enemy != null && Reload <= 0)
            {
                enemy.GetComponent<BotController>().TakeDamage(10);
                Reload = reloadTime;
            }

            if (Reload > 0)
            {
                Reload -= Time.deltaTime;
            }
        }

        public void GoToPossition(float x, float y)
        {
            _agent.SetDestination(new Vector3(x, 0.5f, y));
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;

            healthBar.SetHealth(Health);
        }

        public void Attack(GameObject enemy)
        {

        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (gameObject != this && gameObject.tag == "Bot")
        //        enemysCollider = other;
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    if (enemysCollider == other)
        //        enemysCollider = null;
        //}

        public void Rotate(float angle)
        {
            transform.rotation = Quaternion.Euler(0, angle, 0);
            gameObject.transform.Rotate(0,angle,0);
        }
    }
}
