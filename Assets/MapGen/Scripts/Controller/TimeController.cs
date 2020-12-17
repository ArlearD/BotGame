using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float startTime;
    private float current;
    private float previous = 0;
    private float buffer = 0;
    private int K;
    [SerializeField] private GameObject TimeControllerObject;

    void Start()
    {
        Ruller ruller = TimeControllerObject.GetComponent<Ruller>();
        K = ruller.K;
        startTime = ruller.StartTime + 60000;
    }

    void Update()
    {
        startTime += Time.deltaTime * K;
        current = startTime % 86400 / 240;
        gameObject.transform.Rotate(buffer, 0, 0);
        buffer = current - previous;
        previous = current;
    }
}