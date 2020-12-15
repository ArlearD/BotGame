using UnityEngine;

public class Ruller : MonoBehaviour
{
    public float StartTime;
    public int K;
    public GameObject Player;
    public ParticleSystem Rain;
    [SerializeField] private float RainDur;
    [SerializeField] private float TimeUntilRain;
    public GameObject Star;
    private float Radius = 2500f;
    public GameObject Sun;
    private float Current;
    private float Previous;
    private float Buffer;

    void Start()
    {
        TimeUntilRain = Random.Range(600f, 1200f); //Время до первого дождя (сек)
        StarSpawn();
    }

    public void RainStart()
    {
        var rain = Rain.GetComponent<ParticleSystem>();
        var main = rain.main;
        main.duration = Random.Range(240f, 480f) / 60; //Длительность дождя
        TimeUntilRain += main.duration * 60 + Random.Range(600f, 1200f); //Время до следующего дождя 
        RainDur = main.duration * 60;

        float xPos = Player.transform.position.x + Random.Range(-20f, 20f);
        float yPos = Player.transform.position.y + 50f + Random.Range(-10f, 10f);
        float zPos = Player.transform.position.z + Random.Range(-20f, 20f);
        Instantiate(Rain, new Vector3(xPos, yPos, zPos), Quaternion.identity);
    }

    public void StarSpawn()
    {
        float xPos = Player.transform.position.x;
        float yPos = Player.transform.position.y;
        float zPos = Player.transform.position.z;
        
        for (int i = 0; i < Random.Range(100, 200); i++) //Спавн звезд
        {
            Instantiate(Star, new Vector3(xPos + Random.Range(-2000f, 2000f), 
                yPos + 400f + Random.Range(-50f, 50f), 
                zPos + Random.Range(-2000f, 2000f)), Quaternion.identity);
        }
    }


    void Update()
    {
        StartTime += Time.deltaTime * K;
        Current = StartTime % 86400 / 240;
        Sun.transform.Rotate(Buffer, 0, 0);
        Buffer = Current - Previous;
        Previous = Current;
    }
}