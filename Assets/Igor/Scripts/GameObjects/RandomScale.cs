using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScale : MonoBehaviour
{
    void Start()
    {
        float scale = Random.Range(0.1f, 1);
        transform.localScale = new Vector3(scale, scale, scale);
    }
}