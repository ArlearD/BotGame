using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	private Transform cam;

    private void Start()
    {
        cam = GameObject.FindGameObjectsWithTag("MainCamera").FirstOrDefault().transform;
    }

    void LateUpdate()
    {
		transform.LookAt(transform.position + cam.forward);
    }
}
