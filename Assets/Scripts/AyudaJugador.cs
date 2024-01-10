using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AyudaJugador : MonoBehaviour
{
    [SerializeField] private GameObject[] objetosAyudasJugador;
    private BoardManager boardManager;

    private void Start()
    {
        boardManager = GameManager.instance.GetComponent<BoardManager>();
    }
    public void SpawnAlrededorJugador()
    {
        GameObject toInstantiate;
        GameObject instance;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                int ayudaX = x + (int)gameObject.transform.position.x - 1;
                int ayudaY = y + (int)gameObject.transform.position.y - 1;
                Vector3 VectorInstance = new Vector3(ayudaX, ayudaY, 0);
                if (boardManager.dungeonGridPositions.ContainsKey(VectorInstance) && VectorInstance != Player.instance.transform.position && VectorInstance !=gameObject.transform.position)
                {
                    toInstantiate = objetosAyudasJugador[Random.Range(0, objetosAyudasJugador.Length)];
                    instance = Instantiate(toInstantiate, VectorInstance, Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform.parent);
                    boardManager.dungeonGridPositions.Remove(VectorInstance);
                }
            }
        }
        Destroy(gameObject);
    }
}
