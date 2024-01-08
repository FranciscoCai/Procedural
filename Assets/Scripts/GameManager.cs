using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public float turnDelay = 0.1f;
	public int healthPoints = 100;
    public int playerLevel = 100;
    public static GameManager instance = null;
	[HideInInspector] public bool playersTurn = true;
	
	// Configuracion de los enemigos
	public bool enemiesFaster = false;
	public bool enemiesSmarter = false;
	public int enemySpawnRatio = 20;
	
	private BoardManager boardScript;

	private DungeonManager dungeonScript;
	private Player playerScript;
	// Lista de enemigos
	private List<Enemy> enemies;
	private bool enemiesMoving;
	
	private bool playerInDungeon;
	public bool CoseguirArma = false;
	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);	

		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemy>();
		enemiesFaster = false;
		enemiesSmarter = false;

		boardScript = GetComponent<BoardManager> ();

		dungeonScript = GetComponent<DungeonManager> ();
		playerScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		InitGame();
	}

	void OnLevelWasLoaded(int index) {
		InitGame();
	}

	void InitGame() {
		enemies.Clear();

		boardScript.BoardSetup();

		playerInDungeon = false;
	}

	void Update() {
		if(playersTurn || enemiesMoving)
			return;
		// Cuando no sea el turno del jugador, movemos a los enemigos
		StartCoroutine (MoveEnemies ());
	}
	
	public void AddEnemyToList(Enemy script ) {
		enemies.Add(script);
	}

	public void RemoveEnemyFromList(Enemy script) {
		enemies.Remove(script);
	}

	public void GameOver() {
		enabled = false;
	}
	
	// Mueve a todos los enemigos
	IEnumerator MoveEnemies() {
		// Indicamos que estamos moviendo a los enemigos
		enemiesMoving = true;
		// Esperamos un poco a que el jugador se haya movido
		yield return new WaitForSeconds(turnDelay);
		// Si no hay ningun enemigo, esperamos solo un poco
		if (enemies.Count == 0) {
			yield return new WaitForSeconds(turnDelay);
		}
		// Aqui guardamos si algun enemigo hay que borrarlo del mapa
		List<Enemy> enemiesToDestroy = new List<Enemy>();
		// Para cada enemigo
		for (int i = 0; i < enemies.Count; i++)
		{
			// Si el enemigo no es visible en la mazmorra, no lo movemos
			if (playerInDungeon) {
				if ((!enemies[i].getSpriteRenderer().isVisible)) {
					if (i == enemies.Count - 1)
						yield return new WaitForSeconds(enemies[i].moveTime);
					continue;
				}
			// Si un enemigo se sale de las casillas del mundo abierto, lo borramos
			} else {
				if ((!enemies[i].getSpriteRenderer().isVisible) || (!boardScript.checkValidTile (enemies[i].transform.position))) {
					enemiesToDestroy.Add(enemies[i]);
					continue;
				}
			}
			// Movemos al enemigo
			enemies[i].MoveEnemy ();
			// Esperamos a que el enemigo se mueva
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		// Indicamos que vuelve a ser el turno del jugador
		playersTurn = true;
		enemiesMoving = false;
		// Borramos los enemigos que se han salido del tablero
		for (int i = 0; i < enemiesToDestroy.Count; i++) {
			enemies.Remove(enemiesToDestroy[i]);
			Destroy(enemiesToDestroy[i].gameObject);
		}
		enemiesToDestroy.Clear ();
	}

	public void updateBoard (int horizantal, int vertical) {
		boardScript.addToBoard(horizantal, vertical);
	}

	public void enterDungeon () {
		dungeonScript.StartDungeon ();
		boardScript.SetDungeonBoard (dungeonScript.gridPositions, dungeonScript.maxBound, dungeonScript.endPos);
		playerScript.dungeonTransition = false;

		playerInDungeon = true;
		// Destruimos todos los enemigos del mundo abierto
		for (int i = 0; i < enemies.Count; i++) {
			Destroy(enemies[i].gameObject);
		}
		enemies.Clear ();
	}

	public void exitDungeon () {
		boardScript.SetWorldBoard ();
		playerScript.dungeonTransition = false;

		playerInDungeon = false;

		enemies.Clear ();
	}
}