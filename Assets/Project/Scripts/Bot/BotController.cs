using Assets.Scripts.Interfaces;
using GameControl;
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
        private NavMeshAgent _agent;
        private MethodInfo _playersUpdate;
        private object _pleyerCodeClassObject;
        private int Health;
        private float Reload;
        private float Tick;
        private bool _botWithCode;

        public HealthBar healthBar;
        public float reloadTime;
        public int maxHealth;

        public MapController mapController;

        public void SetUserCode(string code)
        {
            code = @"
using Assets.Scripts.Bot;
using GameControl;
using UnityEngine;
public class BotBrain
{
    private BotController _bot;
    private MapController _map;

    public BotBrain(BotController bot, MapController map)
    {
        _bot = bot;
        _map = map;
    }
" + code + "}";

            var assembly = CompileExecutable(code);

            _botWithCode = true;
            Type magicType = assembly.GetType("BotBrain");


            ConstructorInfo[] magicConstructor = magicType.GetConstructors();
            _pleyerCodeClassObject = magicConstructor[0].Invoke(new object[] { this, mapController });

            _playersUpdate = magicType.GetMethod("Do");


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

        void Update()
        {
            if (Health <= 0) Destroy(gameObject);

            Tick += Time.deltaTime;
            if (Tick >= 1)
            {
                Tick = 0;
                var _playerCodeValue = _playersUpdate.Invoke(_pleyerCodeClassObject, new object[] { });
            }
            if (Reload > 0)
            {
                Reload -= Time.deltaTime;
            }

            if (Input.GetMouseButtonDown(0) && !_botWithCode)
            {
                RaycastHit hit;
                if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    _agent.SetDestination(hit.point);
                }
            }

        }

        public Vector2 GetCurrentPosition()
        {
            var vector = new Vector2(transform.position.x - 460, transform.position.z - 460);
            return vector;
        }

        public void GoToPossition(float x, float y)
        {
            if (x < 0 || y < 0 || x > 60 || y > 60) 
                return;

            //Поправка на размер карты
            x = x + 460;
            y = y + 460;

            _agent.SetDestination(new Vector3(x, transform.position.y, y));
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            healthBar.SetHealth(Health);
        }

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
                enemy.GetComponent<BotController>().TakeDamage(10);
                Reload = reloadTime;
            }
        }

        public void Rotate(GameObject target)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;
        }
    }
}
