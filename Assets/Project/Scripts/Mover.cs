using UnityEngine;

public class Mover : MonoBehaviour
{
    // Start is called before the first frame update

    private Rigidbody _rigidbody;

    [SerializeField]
    private float force = 10;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            _rigidbody.AddForce(Vector3.up * force, ForceMode.Force);
        }
    }

    void FixedUpdate()
    {
    }
}
