using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;

    public void Spawn(Vector3 position)
    {
        Instantiate(itemPrefab, position, Quaternion.identity);
    }
}