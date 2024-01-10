using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Wall : MonoBehaviour {
	public Sprite dmgSprite;
	public int hp = 3;
	public GameObject[] foodTiles;

	private SpriteRenderer spriteRenderer;
    [SerializeField] private float ExperienciaDeDerrota;

    void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	public void DamageWall (int loss) {

            spriteRenderer.sprite = dmgSprite;

		hp -= loss;

		if (hp <= 0) {
            Transform target;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            target.GetComponent<Player>().SubirDeNivel(ExperienciaDeDerrota);
            if (Random.Range (0,5) == 1) {
				GameObject toInstantiate = foodTiles [Random.Range (0, foodTiles.Length)];
				GameObject instance = Instantiate (toInstantiate, new Vector3 (transform.position.x, transform.position.y, 0f), Quaternion.identity) as GameObject;
                
                instance.transform.SetParent (transform.parent);
            }
            BoardManager boardManager = GameManager.instance.GetComponent<BoardManager>();
            boardManager.wallPositions.Remove(gameObject.transform.position);
            gameObject.SetActive (false);
		}
	}
}