using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyType
{
    scape, attack, elite, diferentLevel
}
public class Enemy : MovingObject
{
    public int playerDamage;
    public int hp = 20;
    [SerializeField] private int[] RangoDeHP;
    private Animator animator;
    private Transform target;
    // Controla si se mueven cada dos turnos
    private bool skipMove;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject[] Food;
    [SerializeField] private float ExperienciaDeDerrota;

    private BoardManager boardManager;
    [SerializeField] private GameObject Weapon;
    public EnemyType type;
    public Color level;
    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        spriteRenderer = GetComponent<SpriteRenderer>();
        switch (type)
        {
            case EnemyType.diferentLevel:
                NivelDependiendoDeLaVida();
                break;
                default:
                hp = Random.Range(RangoDeHP[0], RangoDeHP[1] + 1);
                break;
        }
        boardManager = GameManager.instance.gameObject.GetComponent<BoardManager>();

        animator = GetComponent<Animator>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

        

        if(GameManager.instance.playerInDungeon)
        {
            GameManager.instance.recolocarDungeon += enemigoRecolocar;
        }
        base.Start();
    }
    private void NivelDependiendoDeLaVida()
    {
        int maximoIntanciaEnemigo = Convert.ToInt32(Math.Min(GameManager.instance.playerLevel, 60f));
        int randomLevel = Random.Range(maximoIntanciaEnemigo, 100);
        if (randomLevel >= 0 && randomLevel < 50)
        {
            spriteRenderer.color = level = Color.blue;
            hp = Random.Range(RangoDeHP[0], RangoDeHP[1]);
        }
        else if (randomLevel >= 50 && randomLevel < 75)
        {
            spriteRenderer.color = level = Color.green;
            hp = Random.Range(RangoDeHP[0], RangoDeHP[1] )+ 10;
        }
        else if (randomLevel >= 75 && randomLevel < 90)
        {
            spriteRenderer.color = level = Color.yellow;
            hp = Random.Range(RangoDeHP[0], RangoDeHP[1] )+ 15;
        }
        else
        {
            spriteRenderer.color = level = Color.black;
            hp = Random.Range(RangoDeHP[0], RangoDeHP[1] )+ 20;
        }
    }
    public void enemigoRecolocar()
    {
        List<Vector2> keys = new List<Vector2>(boardManager.dungeonGridPositions.Keys);
        if (keys.Count > 0)
        {

            int randomIndex = Random.Range(0, keys.Count);

            Vector2 randomVector = keys[randomIndex];

            gameObject.transform.position = randomVector;
        }
    }
    private void OnDestroy()
    {
        if (GameManager.instance.playerInDungeon)
        {
            GameManager.instance.recolocarDungeon -= enemigoRecolocar;
        }
    }
    private void OnDisable()
    {
        if (GameManager.instance.playerInDungeon)
        {
            GameManager.instance.recolocarDungeon -= enemigoRecolocar;
        }
    }
    protected override bool AttemptMove<T>(int xDir, int yDir)
    {
        // Si se mueven cada dos turnos o cada uno
        if (skipMove && !GameManager.instance.enemiesFaster)
        {
            skipMove = false;
            return false;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
        return true;
    }

    // Mueve al enemigo en direccion al jugador
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (GameManager.instance.enemiesSmarter)
        {
            // Miramos en cuantas direcciones nos tenemos que mover
            int xHeading = (int)target.position.x - (int)transform.position.x;
            int yHeading = (int)target.position.y - (int)transform.position.y;
            bool moveOnX = false;

            if (Mathf.Abs(xHeading) >= Mathf.Abs(yHeading))
            {
                moveOnX = true;
            }

            // Nos intentamos mover dos veces en dos direcciones
            for (int attempt = 0; attempt < 2; attempt++)
            {
                switch (type)
                {
                    case EnemyType.scape:
                        if (moveOnX == true && xHeading < 0)
                        {
                            xDir = +1; yDir = 0;
                        }
                        else if (moveOnX == true && xHeading > 0)
                        {
                            xDir = -1; yDir = 0;
                        }
                        else if (moveOnX == false && yHeading < 0)
                        {
                            yDir = +1; xDir = 0;
                        }
                        else if (moveOnX == false && yHeading > 0)
                        {
                            yDir = -1; xDir = 0;
                        }
                        break;
                    default:
                        if (moveOnX == true && xHeading < 0)
                        {
                            xDir = -1; yDir = 0;
                        }
                        else if (moveOnX == true && xHeading > 0)
                        {
                            xDir = +1; yDir = 0;
                        }
                        else if (moveOnX == false && yHeading < 0)
                        {
                            yDir = -1; xDir = 0;
                        }
                        else if (moveOnX == false && yHeading > 0)
                        {
                            yDir = +1; xDir = 0;
                        }
                        break;
                }

                Vector2 start = transform.position;
                Vector2 end = start + new Vector2(xDir, yDir);
                base.boxCollider.enabled = false;
                RaycastHit2D hit = Physics2D.Linecast(start, end, base.blockingLayer);
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
                        if (boardManager.gridPositions.ContainsKey(end) || !Player.instance.onWorldBoard)
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

        }
        else
        {
            // Si estamos en el mismo eje X, nos movemos en vertical
            if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
                yDir = target.position.y > transform.position.y ? 1 : -1;
            // Sino, nos movemos en horizontal
            else
                xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        if (!GameManager.instance.playerInDungeon)
        {
            switch (type)
            {
                case EnemyType.elite:
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            int enemyX = x + (int)gameObject.transform.position.x - 1;
                            int enemyY = y + (int)gameObject.transform.position.y - 1;
                            Vector3 VectorInstance = new Vector3(enemyX, enemyY, 0);
                            /*(if (boardManager.gridPositions.ContainsKey(VectorInstance) && VectorInstance != gameObject.transform.position && !boardManager.wallPositions.ContainsKey(VectorInstance) && gameObject.transform.position!= Player.instance.gameObject.transform.position)
                            {
                                GameObject toInstantiate = boardManager.wallTiles[Random.Range(0, boardManager.wallTiles.Length)];
                                GameObject instance = Instantiate(toInstantiate, VectorInstance, Quaternion.identity) as GameObject;
                                instance.transform.SetParent(transform.parent);
                                boardManager.wallPositions.Add(VectorInstance, VectorInstance);
                            }
                            else */
                          
                            if (boardManager.wallPositions.TryGetValue(VectorInstance, out GameObject wallObject))
                            {
                                Wall wallComponent = wallObject.GetComponent<Wall>();
                                wallComponent.DamageWall(wallComponent.hp);
                            }
                        }
                    }
                    break;
                default:

                    break;
            }
        }
        if (boardManager.gridPositions.ContainsKey(new Vector2(transform.position.x + xDir, transform.position.y + yDir)) || !Player.instance.onWorldBoard)
        {
            AttemptMove<Player>(xDir, yDir);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        /*switch (type)
        {
            case EnemyType.elite:
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        int playerX = x + (int)hitPlayer.gameObject.transform.position.x - 1;
                        int playerY = y + (int)hitPlayer.gameObject.transform.position.y - 1;
                        Vector3 VectorInstance = new Vector3(playerX, playerX, 0);
                        
                    }
                }
                break;
            default:
                break;
        }*/

        hitPlayer.LoseHealth(playerDamage);

        animator.SetTrigger("enemyAttack");

    }

    public SpriteRenderer getSpriteRenderer()
    {
        return spriteRenderer;
    }

    public void DamageEnemy(int loss)
    {
        hp -= loss;

        if (hp <= 0)
        {
            if (Random.Range(0, 2) == 0)
            {
                GameObject toinstance = Instantiate(Food[Random.Range(0, Food.Length)], gameObject.transform.position, Quaternion.identity);
                toinstance.transform.SetParent(transform.parent);

            }
            switch (type)
            {
                case EnemyType.scape:
                    GameObject toinstance = Instantiate(Weapon, gameObject.transform.position, Quaternion.identity);
                    toinstance.transform.SetParent(transform.parent);
                    break;
                default:

                    break;
            }
            target.GetComponent<Player>().SubirDeNivel(ExperienciaDeDerrota);
            GameManager.instance.RemoveEnemyFromList(this);
            Destroy(gameObject);
        }
    }
}
