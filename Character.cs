using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static CharactersSO;

public class Character : MonoBehaviour
{
    public CharData charData;
    public ProjData projData;

    private CircleCollider2D colliderRange;
    [SerializeField]
    private GameObject rangeShow;

    int projLayer;

    private float origShotCd;
    private float shotCd;
    private float range;
    private float shotSpeed;

    private bool changedSpeedOnce;

    private TargetingTypes targettingType;

    // Start is called before the first frame update
    void Start()
    {
        colliderRange = gameObject.GetComponent<CircleCollider2D>();
        colliderRange.radius = charData.baseRange / 100;
        float newRange = colliderRange.radius / 5;
        rangeShow.transform.localScale = new Vector3(newRange, newRange, newRange);
        range = charData.baseRange;
        shotCd = 0;
        origShotCd = projData.baseShotCdMS;
        shotSpeed = projData.baseSpeed;
        projLayer = LayerMask.NameToLayer("Projectiles");
    }

    // Update is called once per frame
    void Update()
    {
        targettingType = charData.targetingTypes;

        shotCd -= Time.deltaTime * 1000;
        shotCd = shotCd < 0 ? 0 : shotCd;

        if (RoundController.inst.spedUp && shotCd > 0 && !changedSpeedOnce)
        {
            shotCd -= (shotCd / 2.5f);
            changedSpeedOnce = true;
        }
        else if (!RoundController.inst.spedUp)
        {
            changedSpeedOnce = false;
        }

        switch (charData.charType)
        {
			//unique stuff added here later
            case CharType.Archer:

                if (isEnemyNearby(range))
                {
                    if (shotCd <= 0)
                    {
                        Vector3 enemyPos = new Vector3();

                        if (projData.targetEnemies)
                        {
                            switch (targettingType)
                            {
                                case TargetingTypes.First:
                                    enemyPos = GetEnemyFurthestAlongPath(range).transform.position;
                                    break;
                                case TargetingTypes.Last:
                                    enemyPos = GetEnemyClosestToStartOfPath(range).transform.position;
                                    break;
                                case TargetingTypes.Strong:
                                    enemyPos = GetEnemyWithHighestHP(range).transform.position;
                                    break;
                            }
                        }
                        else
                            enemyPos = GetNearest(range).transform.position;

                        GameObject[] proj = new GameObject[projData.baseShotAmt];

                        for (int i = 0; i < proj.Length; i++)
                        {
                            proj[i] = Instantiate(projData.prefab, transform.position, transform.rotation, transform);
                            if (proj[i].layer != projLayer) proj[i].layer = projLayer;

                            Projectile projectile = proj[i].GetComponent<Projectile>();
                            ProjData newProjData = projData;

                            newProjData.baseSpeed = shotSpeed;
                            projectile.charData = charData;
                            projectile.projData = newProjData;
                            projectile.enemyPos = enemyPos;
                        }

                        shotCd = origShotCd;
                        if (RoundController.inst.spedUp)
                        {
                            shotCd /= 2.5f;
                        }
                    }
                }
                break;
            case CharType.Wizard:

                if (isEnemyNearby(range))
                {
                    if (shotCd <= 0)
                    {
                        var proj = Instantiate(projData.prefab, transform.position, transform.rotation, transform);
                        if (proj.layer != projLayer) proj.layer = projLayer;

                        Vector3 enemyPos = new Vector3();

                        switch (targettingType)
                        {
                            case TargetingTypes.First:
                                enemyPos = GetEnemyFurthestAlongPath(range).transform.position;
                                break;
                            case TargetingTypes.Last:
                                enemyPos = GetEnemyClosestToStartOfPath(range).transform.position;
                                break;
                            case TargetingTypes.Strong:
                                enemyPos = GetEnemyWithHighestHP(range).transform.position;
                                break;
                        }

                        Projectile projectile = proj.GetComponent<Projectile>();
                        ProjData newProjData = projData;

                        newProjData.baseSpeed = shotSpeed;
                        projectile.charData = charData;
                        projectile.projData = newProjData;
                        projectile.enemyPos = enemyPos;

                        shotCd = origShotCd;
                        if (RoundController.inst.spedUp)
                        {
                            shotCd /= 2.5f;
                        }
                    }
                }
                break;
            case CharType.Rogue:

                if (isEnemyNearby(range))
                {
                    if (shotCd <= 0)
                    {
                        var proj = Instantiate(projData.prefab, transform.position, transform.rotation, transform);
                        if (proj.layer != projLayer) proj.layer = projLayer;

                        Vector3 enemyPos = new Vector3();

                        switch (targettingType)
                        {
                            case TargetingTypes.First:
                                enemyPos = GetEnemyFurthestAlongPath(range).transform.position;
                                break;
                            case TargetingTypes.Last:
                                enemyPos = GetEnemyClosestToStartOfPath(range).transform.position;
                                break;
                            case TargetingTypes.Strong:
                                enemyPos = GetEnemyWithHighestHP(range).transform.position;
                                break;
                        }

                        Projectile projectile = proj.GetComponent<Projectile>();
                        ProjData newProjData = projData;

                        newProjData.baseSpeed = shotSpeed;
                        projectile.charData = charData;
                        projectile.projData = newProjData;
                        projectile.enemyPos = enemyPos;

                        shotCd = origShotCd;
                        if (RoundController.inst.spedUp)
                        {
                            shotCd /= 2.5f;
                        }
                    }
                }
                break;
        }
        
    }

    public bool isEnemyNearby(float inRange)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        if (enemies.Length <= 0)
            return false;

        Vector3 ourPos = transform.position;

        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 theirPos = enemies[i].transform.position;
            float distance = Vector3.Distance(ourPos, theirPos);

            if (distance > inRange)
                continue;
            else
                return true;
        }
        return false;
    }

    public Enemy GetNearest(float inRange)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

        Vector3 ourPos = transform.position;

        float bestDistance = float.PositiveInfinity;
        int bestIndex = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 theirPos = enemies[i].transform.position;
            float distance = Vector3.Distance(ourPos, theirPos);

            if (distance > inRange)
                continue;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestIndex = i;
            }
        }

        GameObject nearestGo = enemies[bestIndex];
        Enemy nearest = nearestGo.GetComponent<Enemy>();
        return nearest;
    }

    public Enemy GetEnemyFurthestAlongPath(float inRange)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        PathFollower[] followers = enemies.Select(_ => _.GetComponent<PathFollower>()).ToArray();

        Vector3 ourPos = transform.position;
        float bestDistance = -1f;
        int bestIndex = 0;

        for (int i = 0; i < followers.Length; i++)
        {
            float d = followers[i].distAlongTrack;
            Vector3 theirPos = enemies[i].transform.position;
            float distance = Vector3.Distance(ourPos, theirPos);

            if (distance > inRange)
                continue;

            if (d > bestDistance)
            {
                bestDistance = d;
                bestIndex = i;
            }
        }

        PathFollower furthest = followers[bestIndex];
        return furthest.gameObject.GetComponent<Enemy>();
    }

    public Enemy GetEnemyClosestToStartOfPath(float inRange)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        PathFollower[] followers = enemies.Select(_ => _.GetComponent<PathFollower>()).ToArray();

        Vector3 ourPos = transform.position;
        float bestDistance = float.PositiveInfinity;
        int bestIndex = 0;

        for (int i = 0; i < followers.Length; i++)
        {
            float d = followers[i].distAlongTrack;
            Vector3 theirPos = enemies[i].transform.position;
            float distance = Vector3.Distance(ourPos, theirPos);

            if (distance > inRange)
                continue;

            if (d < bestDistance)
            {
                bestDistance = d;
                bestIndex = i;
            }
        }

        PathFollower closestToStart = followers[bestIndex];
        return closestToStart.gameObject.GetComponent<Enemy>();
    }

    public Enemy GetEnemyWithHighestHP(float inRange)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

        Vector3 ourPos = transform.position;
        int bestHP = int.MinValue;
        int bestIndex = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            int d = enemies[i].gameObject.GetComponent<Enemy>().enemyData.enemyHP;

            Vector3 theirPos = enemies[i].transform.position;
            float distance = Vector3.Distance(ourPos, theirPos);

            if (distance > inRange)
                continue;

            if (d > bestHP)
            {
                bestHP = d;
                bestIndex = i;
            }
        }
        Enemy strongestEnemy = enemies[bestIndex].gameObject.GetComponent<Enemy>();
        return strongestEnemy;
    }
}
