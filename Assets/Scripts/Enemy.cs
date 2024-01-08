using UnityEngine;
using System.Collections;
using NUnit;
using UnityEngine.UIElements;

public class Enemy : MovingObject
{
	public int playerDamage;
	public int hp = 20;

	private Animator animator;
	private Transform target;
	// Controla si se mueven cada dos turnos
	private bool skipMove;
	
	private SpriteRenderer spriteRenderer;
	[SerializeField] private GameObject[] Food;
    [SerializeField] private float ExperienciaDeDerrota;

	private BoardManager boardManager;

    protected override void Start () {
		GameManager.instance.AddEnemyToList (this);
		boardManager = GameManager.instance.gameObject.GetComponent<BoardManager> ();

		animator = GetComponent<Animator> ();

		target = GameObject.FindGameObjectWithTag ("Player").transform;

		spriteRenderer = GetComponent<SpriteRenderer> ();

		base.Start ();
	}

	protected override bool AttemptMove <T> (int xDir, int yDir) {
		// Si se mueven cada dos turnos o cada uno
		if(skipMove && !GameManager.instance.enemiesFaster) {
			skipMove = false;
			return false;
		}

		base.AttemptMove <T> (xDir, yDir);

		skipMove = true;
		return true;
	}

	// Mueve al enemigo en direccion al jugador
	public void MoveEnemy () {
		int xDir = 0;
		int yDir = 0;

		if (GameManager.instance.enemiesSmarter) {
			// Miramos en cuantas direcciones nos tenemos que mover
			int xHeading = (int)target.position.x - (int)transform.position.x;
			int yHeading = (int)target.position.y - (int)transform.position.y;
			bool moveOnX = false;

			if (Mathf.Abs(xHeading) >= Mathf.Abs(yHeading)) {
				moveOnX = true;
			}
			// Nos intentamos mover dos veces en dos direcciones
			for (int attempt = 0; attempt < 2; attempt++) {
				if (moveOnX == true && xHeading < 0) {
					xDir = -1; yDir = 0;
				}
				else if (moveOnX == true && xHeading > 0) {
					xDir = +1; yDir = 0;
				}
				else if (moveOnX == false && yHeading < 0) {
					yDir = -1; xDir = 0;
				}
				else if (moveOnX == false && yHeading > 0) {
					yDir = +1; xDir = 0;
				}

				Vector2 start = transform.position;
				Vector2 end = start + new Vector2 (xDir, yDir);
				base.boxCollider.enabled = false;
				RaycastHit2D hit = Physics2D.Linecast (start, end, base.blockingLayer); 
				base.boxCollider.enabled = true;
				if (hit.transform != null) 
				{
					if (hit.transform.gameObject.tag == "Wall" || hit.transform.gameObject.tag == "Chest") 
					{
						if (moveOnX == true)
							moveOnX = false;
						else 
							moveOnX = true;
					} 
					else 
					{
						if (boardManager.gridPositions.ContainsKey(end))
						{
							break;
						}
						else
						{
                            if (moveOnX == true)
                                moveOnX = false;
                            else
                                moveOnX = true;
                        }
					}
				}
			}

		} else {
			// Si estamos en el mismo eje X, nos movemos en vertical
			if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
				yDir = target.position.y > transform.position.y ? 1 : -1;
			// Sino, nos movemos en horizontal
			else
				xDir = target.position.x > transform.position.x ? 1 : -1;
		}
        if (boardManager.gridPositions.ContainsKey(new Vector2 (transform.position.x +xDir, transform.position.y +yDir)))
        {
            AttemptMove<Player>(xDir, yDir);
        }
	}

	protected override void OnCantMove <T> (T component) {
		Player hitPlayer = component as Player;

		hitPlayer.LoseHealth (playerDamage);

		animator.SetTrigger ("enemyAttack");
		
	}

	public SpriteRenderer getSpriteRenderer () {
		return spriteRenderer;
	}

	public void DamageEnemy (int loss) {
		hp -= loss;
		
		if (hp <= 0) {
			if (Random.Range(0, 2) == 0)
			{
				GameObject toinstance = Food[Random.Range(0, Food.Length)];
				Instantiate(toinstance, gameObject.transform.position, Quaternion.identity);
			}
			target.GetComponent<Player>().SubirDeNivel(ExperienciaDeDerrota);
			GameManager.instance.RemoveEnemyFromList (this);
			Destroy (gameObject);
		}
	}
}
