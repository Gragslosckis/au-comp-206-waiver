using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CharactersSO;

public class Projectile : MonoBehaviour
{
    public CharData charData;
    public ProjData projData;

    public Vector3 enemyPos;
    public float range;
    private int pierceAmt;

    private Vector3 moveDir;

    public float projLifetime;

    private float baseSpeed;

    // Start is called before the first frame update
    void Start()
    {
        range = projData.projHitboxSize;
        pierceAmt = projData.basePierce;
        baseSpeed = 1f;

        switch (projData.charType)
        {
			//unique stuff added here later
            case CharType.Archer:

                transform.up = enemyPos - transform.position;
                moveDir = enemyPos - transform.position;
                moveDir = moveDir.normalized;

                break;
            case CharType.Wizard:

                transform.up = enemyPos - transform.position;
                moveDir = enemyPos - transform.position;
                moveDir = moveDir.normalized;

                break;
            case CharType.Rogue:

                transform.up = enemyPos - transform.position;
                moveDir = enemyPos - transform.position;
                moveDir = moveDir.normalized;

                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RoundController.inst.spedUp)
            baseSpeed = 2.5f;

        switch (projData.charType)
        {
			//unique stuff added here later
            case CharType.Archer:

                projLifetime += projData.baseSpeed * baseSpeed * (Time.deltaTime * 1000);
                if (projLifetime >= projData.baseProjLifetime) Destroy(gameObject);

                transform.position += moveDir * projData.baseSpeed * baseSpeed * (Time.deltaTime * 1000);

                if (GetEnemiesInRange(range).Count > 0)
                    foreach (var enemy in GetEnemiesInRange(range))
                    {
                        if (pierceAmt > 0)
                        {
                            if (enemy.lastHitProjectile != this)
                            {
                                int overDamage = 0;
                                enemy.HP -= projData.baseDamage;

                                if (enemy.HP < 0)
                                    overDamage = Mathf.Abs(enemy.HP);

                                DataManager.inst.IncrementGold(projData.baseDamage - overDamage);
                                pierceAmt--;
                            }
                            enemy.lastHitProjectile = this;
                        }
                        else
                            Destroy(gameObject);
                    }
                if (pierceAmt <= 0)
                    Destroy(gameObject);

                break;
            case CharType.Wizard:

                projLifetime += projData.baseSpeed * baseSpeed * (Time.deltaTime * 1000);
                if (projLifetime >= projData.baseProjLifetime) Destroy(gameObject);

                transform.position += moveDir * projData.baseSpeed * baseSpeed * (Time.deltaTime * 1000);

                if (GetEnemiesInRange(range).Count > 0)
                    foreach (var enemy in GetEnemiesInRange(range))
                    {
                        if (pierceAmt > 0)
                        {
                            if (enemy.lastHitProjectile != this)
                            {
                                int overDamage = 0;
                                enemy.HP -= projData.baseDamage;

                                if (enemy.HP < 0)
                                    overDamage = Mathf.Abs(enemy.HP);

                                DataManager.inst.IncrementGold(projData.baseDamage - overDamage);
                                pierceAmt--;
                            }
                            enemy.lastHitProjectile = this;
                        }
                        else
                            Destroy(gameObject);
                    }
                if (pierceAmt <= 0)
                    Destroy(gameObject);

                break;
            case CharType.Rogue:

                projLifetime += projData.baseSpeed * baseSpeed * (Time.deltaTime * 1000);
                if (projLifetime >= projData.baseProjLifetime) Destroy(gameObject);

                transform.position += moveDir * projData.baseSpeed * baseSpeed * (Time.deltaTime * 1000);

                if (GetEnemiesInRange(range).Count > 0)
                    foreach (var enemy in GetEnemiesInRange(range))
                    {
                        if (pierceAmt > 0)
                        {
                            if (enemy.lastHitProjectile != this)
                            {
                                int overDamage = 0;
                                enemy.HP -= projData.baseDamage;

                                if (enemy.HP < 0)
                                {
                                    overDamage = Mathf.Abs(enemy.HP);
                                }
                                DataManager.inst.IncrementGold(projData.baseDamage - overDamage);
                                pierceAmt--;
                            }
                            enemy.lastHitProjectile = this;
                        }
                        else
                            Destroy(gameObject);
                    }
                if (pierceAmt <= 0)
                    Destroy(gameObject);

                break;
        }
    }

    public List<Enemy> GetEnemiesInRange(float inRange)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("enemy");
        List<Enemy> enemies = new List<Enemy>(objects.Select(_ => _.GetComponent<Enemy>()));
        List<Enemy> enemiesInRange = new List<Enemy>();

        Vector3 ourPos = transform.position;

        foreach (var enemy in enemies)
        { 
            Vector3 theirPos = enemy.transform.position;
            float distance = Vector3.Distance(ourPos, theirPos);

            if (distance > inRange)
                continue;

            enemiesInRange.Add(enemy);
        }

        return enemiesInRange;
    }
}
