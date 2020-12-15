using UnityEngine;
using System.Collections;
 
public class ParticleSystemAutoDestroy : MonoBehaviour 
{
	[SerializeField]
	private float Destr;
	private ParticleSystem PS;
	
	void Start()
	{
		PS=gameObject.GetComponent<ParticleSystem>();
		Destr=PS.main.duration;
	}

	void Update()
	{
		Destr -= Time.deltaTime/60;
		if (Destr<0)
		{
			Destroy(gameObject);
		}
	}
}