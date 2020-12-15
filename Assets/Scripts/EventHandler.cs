using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventHandler : MonoBehaviour
{
    [SerializeField] private EventType _eventType;

    private void Start()
    {
        var lastEnum = Enum
            .GetValues(typeof(EventType))
            .Cast<EventType>()
            .Max();
        
        _eventType = (EventType)Random.Range(0, (int)lastEnum + 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (_eventType)
            {
                case EventType.HighTeleport:
                    TeleportPlayerToHigh(other);
                    break;
                case EventType.Teleport:
                    TeleportPlayer(other);
                    break;
                case EventType.ChangeEnvironment:
                    ChangeBackground();
                    break;
                case EventType.BuildSomething:
                    BuildSomething(other);
                    break;
            }
            Destroy(this);
        }
    }

    private void TeleportPlayerToHigh(Collider other)
    {
        Debug.Log("teleport player to high");
        var characterController = other.GetComponent<CharacterController>();
        characterController.Move(new Vector3(0, 
            Random.Range(50, 500f), 0));
    }
    
    private void TeleportPlayer(Collider other)
    {
        Debug.Log("teleport player");
        var characterController = other.GetComponent<CharacterController>();
        characterController.Move(new Vector3(Random.Range(-10, 10f), 0, Random.Range(-10, 10f)));
    }
    
    private void ChangeBackground()
    {
        Debug.Log("change background");
        var generator = GameObject.FindWithTag("GameController");
        var levelGenerator = generator.GetComponent<LevelGenerator>();
        levelGenerator.ApplyRandomSkybox();
    }
    
    private void BuildSomething(Collider other)
    {
        var playerPosition = other.transform.position;
        Debug.Log("build sth");
        var generator = GameObject.FindWithTag("GameController");
        var levelGenerator = generator.GetComponent<LevelGenerator>();
        var changedPlayerPosition = new Vector3(playerPosition.x + Random.Range(-10, 10), 
            playerPosition.y + Random.Range(1, 10), playerPosition.z + Random.Range(-10, 10));
        Build(0, changedPlayerPosition, levelGenerator.EventObject);
    }

    private void Build(int currentLevel, Vector3 previousPosition, GameObject prefab)
    {
        currentLevel += 1;
        if (currentLevel > 25)
            return;
        var newPositionX = new Vector3(previousPosition.x + 1, previousPosition.y, previousPosition.z);
        var rnd = Random.value;
        if (rnd > 0.7f)
        {
            Instantiate(prefab, newPositionX, Quaternion.identity);
            Build(currentLevel, newPositionX, prefab);
        }

        var newPositionZ = new Vector3(previousPosition.x, previousPosition.y, previousPosition.z + 1);
        rnd = Random.value;
        if (rnd > 0.7f)
        {
            Instantiate(prefab, newPositionZ, Quaternion.identity);
            Build(currentLevel, newPositionZ, prefab);
        }
        
        var newPositionY = new Vector3(previousPosition.x, previousPosition.y + 1, previousPosition.z);
        rnd = Random.value;
        if (rnd > 0.7f)
        {
            Instantiate(prefab, newPositionY, Quaternion.identity);
            Build(currentLevel, newPositionY, prefab);
        }
    }
}

public enum EventType
{
    HighTeleport,
    Teleport,
    ChangeEnvironment,
    BuildSomething
}