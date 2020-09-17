using UnityEngine;
using System.Collections;
/// <summary>
/// 怪物生成脚本
/// </summary>
public class EnemySpawn : MonoBehaviour
{

    public Transform bornFX; //生成特效
    public GameObject enemyPrefab; //预设物

    void OnTriggerEnter(Collider other)
    {
        //当碰到人物时
        if (other.tag == "Player")
        {
            //1.生成小怪
            Spawn(other.transform);
            //2.关闭碰撞盒，避免重复执行代码
            GetComponent<Collider>().enabled = false;
            other.GetComponent<Player>().SelfAttack();
        }
        
    }

    void Spawn(Transform player)
    {
        StartCoroutine(SpawnMov(player));
    }

    IEnumerator SpawnMov(Transform player)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            //怪物出生点
            Transform enemyPoint = transform.GetChild(i);

            Transform enemy = Instantiate(enemyPrefab).transform;
            enemy.position = enemyPoint.position;//设置坐标
            enemy.rotation = enemyPoint.rotation;//设置朝向
            //创建出生特效 
            var fx = Instantiate(bornFX);
            fx.position = enemyPoint.position;
            Destroy(fx.gameObject, 2f);

            yield return null;
            yield return null;
            //开启 导航组件
            enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
            //设置怪物追踪目标
            enemy.GetComponent<Enemy>().SetTarget(player);

        }
    }
}
