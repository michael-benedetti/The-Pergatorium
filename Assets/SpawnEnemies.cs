using UnityEngine;
using System.Collections;
public class SpawnEnemies : MonoBehaviour 
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject myPrefab;

    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {
        StartCoroutine(Spawner());
    }

    IEnumerator Spawner() {
        while (true) {
            Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            yield return new WaitForSeconds(1);
        }
    }
}
