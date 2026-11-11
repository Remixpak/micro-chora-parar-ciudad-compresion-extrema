using Unity.VisualScripting;
using UnityEngine;

public class MoveScenario : MonoBehaviour
{
    //[SerializeField] Transform camera;
    public int speed = 500;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - speed * Time.deltaTime);
        if(this.transform.position.z < - 58)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destructor"))
        {
            Destroy(this.gameObject);
        }
    }
}
