using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFixing : MonoBehaviour
{
    void Start () {
        Application.runInBackground = true;
    }
}
