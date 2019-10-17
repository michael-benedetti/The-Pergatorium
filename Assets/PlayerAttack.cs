using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject explosion;
    public float radius = 5.0F;
    public float power = 50.0F;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
                    GameObject newExplosion = (GameObject) Instantiate(explosion, hit.point, Quaternion.identity);
                    Collider[] colliders = Physics.OverlapSphere(hit.point, radius);
                    foreach (Collider collide in colliders)
                    {
                        Rigidbody rb = collide.GetComponent<Rigidbody>();
                        if (rb != null)
                         rb.AddExplosionForce(power, hit.point, radius, 3.0F, ForceMode.Impulse);
                    }
                    Destroy(newExplosion, 1);
                }
            }
    }
}
