using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static RoundsSO;

public class RoundController : MonoBehaviour
{
    public GameObject enemyObj;
    public static RoundController inst;

    public EnemiesSO Enemies;
    public RoundsSO Rounds;

    public GameObject startCoord;
    public GameObject paren;

    public SpawnEnemy[] Cenemy;

    public int maxRounds;
    public int startingRound;
    public int currRound;
    public bool roundStarted;
    private int roundEndGold;
    private float timeTillRoundEnd;

    private float[] origTimesTillEnemySpawn;
    private float[] timesTillEnemySpawn;
    private int[] timesEnemyCanSpawn;

    private bool[] tickEnemySpawnCd;
    private bool tickingEnemySpawns;

    private int[] enemiesCanSpawn;
    private int[] enemiesSpawned;

    public static int currentRound;

    public bool spedUp;

    private float speedUpValue;

    void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        speedUpValue = 1f;
        startCoord = GameObject.FindGameObjectWithTag("StartCoord");
    }

    // Update is called once per frame
    void Update()
    {
        if (spedUp)
            speedUpValue = 2.5f;
        else
            speedUpValue = 1f;

        currentRound = currRound;
        if (roundStarted && timeTillRoundEnd > 0)
            if (!Manage.paused)
            {
                timeTillRoundEnd -= speedUpValue * Time.deltaTime * 1000;
                if (timeTillRoundEnd <= 0)
                {
                    Debug.Log("Round Ended");
                    ResetEnemyData();
                    DataManager.inst.IncrementGold(roundEndGold);
                    currRound++;
                    StartRound(currRound);
                }
            }

        if (tickingEnemySpawns && !Manage.paused)
        {
            for (int i = 0; i < tickEnemySpawnCd.Length; i++)
            {
                SpawnEnemies(Cenemy[i]);

                if (tickEnemySpawnCd[i] && enemiesSpawned[i] < timesEnemyCanSpawn[i])
                {
                    if (timesTillEnemySpawn[i] > 0)
                        timesTillEnemySpawn[i] -= speedUpValue * Time.deltaTime * 1000;
                    else
                    {
                        timesTillEnemySpawn[i] = origTimesTillEnemySpawn[i];
                        enemiesCanSpawn[i]++;
                        enemiesSpawned[i]++;
                    }
                }
            }

        }
    }

    public void StartGame()
    {
        if (!roundStarted)
        {
            currRound = startingRound;

            StartRound(currRound);
        }
        else if (!spedUp)
        {
            spedUp = true;
        }
        else
        {
            spedUp = false;
        }
    }

    public void StartRound(int roundNumber)
    {
        if (roundNumber > Rounds.rounds.Count)
        {
            Debug.Log("Ended on round: " + roundNumber);
            Debug.Log("MaxRounds: " + Rounds.rounds.Count);
            return;
        }

        foreach (var round in Rounds.rounds)
        {
            if (round.RoundNumber == roundNumber)
            {
                Debug.Log("Starting Round: " + roundNumber);
                timeTillRoundEnd = round.TotalRoundTime;
                roundEndGold = round.RoundGold;

                int b = 0;

                foreach (var enem in round.Enemies)
                {
                    b++;
                }

                Cenemy = new SpawnEnemy[b];
                timesEnemyCanSpawn = new int[b];
                timesTillEnemySpawn = new float[b];
                origTimesTillEnemySpawn = new float[b];
                tickEnemySpawnCd = new bool[b];
                enemiesCanSpawn = new int[b];
                enemiesSpawned = new int[b];

                int i = 0;
                foreach (var enemy in round.Enemies)
                {
                    Cenemy[i] = enemy;
                    timesEnemyCanSpawn[i] = enemy.enemyAmount;
                    timesTillEnemySpawn[i] = enemy.timeBetweenSpawns;
                    origTimesTillEnemySpawn[i] = enemy.timeBetweenSpawns;
                    tickEnemySpawnCd[i] = true;
                    //Debug.Log("Spawning Enemies");
                    tickingEnemySpawns = true;
                    i++;
                }
                roundStarted = true;
            }
        }
    }

    public void SpawnEnemies(SpawnEnemy enemyBase)
    {
        tickingEnemySpawns = true;

        for (int i = 0; i < enemiesCanSpawn.Length; i++)
        {
            if (enemiesCanSpawn[i] > 0)
            {
                if (enemyObj != null)
                {
                    var en = Instantiate(enemyObj);
                    Enemy enemy = en.GetComponent<Enemy>();
                    SpriteRenderer spr = en.GetComponent<SpriteRenderer>();
                    
                    foreach (var ed in Enemies.enemies)
                    {
                        if (ed.enemyType == enemyBase.enemyType)
                        {
                            spr.sprite = ed.sprite;
                            enemy.enemyData = ed;
                        }
                    }

                    en.name = enemyBase.enemyType + " Enemy";
                   //enemyObj. need to make the sprite change based on the enemy type
                    en.transform.position = startCoord.transform.position;
                    en.transform.SetParent(paren.transform);
                    en.layer = paren.layer;
                    en.SetActive(true);

                    enemy.enemyBase = enemyBase;
                    enemy.path.TrySelectNextTarget();
                }
                enemiesCanSpawn[i]--;
            }
        }
        //Debug.Log("Spawned " + enemy.enemyAmount + " " + enemy.enemyType + "s with a " + enemy.timeBetweenSpawns + " time between spawns.");
    }

    public void ResetEnemyData()
    {
        for (int i = 0; i < origTimesTillEnemySpawn.Length; i++)
        {
            origTimesTillEnemySpawn[i] = 0;
            timesTillEnemySpawn[i] = 0;
            timesEnemyCanSpawn[i] = 0;
            tickEnemySpawnCd[i] = false;
            tickingEnemySpawns = false;
            enemiesCanSpawn[i] = 0;
            enemiesSpawned[i] = 0;
        }
    }
}
