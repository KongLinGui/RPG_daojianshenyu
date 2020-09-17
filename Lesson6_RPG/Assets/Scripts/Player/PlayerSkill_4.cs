using UnityEngine;
using System.Collections;

public class PlayerSkill_4 : PlayerSkill
{

    public Player playFly;//玩家起飞
    public GameObject FlyPrefab;//特效

    // Use this for initialization
    void Start()
    {
        //初始化
        playFly = FindObjectOfType<Player>();
        //获取特效
        FlyPrefab = Resources.Load<GameObject>("Skill/SkillFx5");
        
    }
    public override void StartSkill()//重写、覆盖
    {
        base.StartSkill();

        //开启协成
        StartCoroutine(FlySky());

    }
    //玩家起飞协成
    IEnumerator FlySky()
    {
        playFly.IsFly = true;
        //关闭导航
        playFly.agent.enabled = false;

        playFly.transform.position = new Vector3(playFly.transform.position.x, 5f, playFly.transform.position.z);

        yield return new WaitForSeconds(0.5f);
        Transform Clone = Instantiate(FlyPrefab).transform;
        Clone.position = new Vector3(playFly.transform.position.x, 0.04f, playFly.transform.position.z);
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.2f);
            Collider[] colls = Physics.OverlapSphere(Clone.position, 2f);
            for (int j = 0; j < colls.Length; j++)
            {
                if (colls[j].tag == "Enemy")
                {
                    Enemy e = colls[j].GetComponent<Enemy>();
                    if (e.state != EnemyState.Die)
                    {
                        e.BeHit(30);
                        //e.state = EnemyState.Die;
                    }
                }
            }
        }
        yield return new WaitForSeconds(1f);
        playFly.transform.position = new Vector3(playFly.transform.position.x, 0.04f, playFly.transform.position.z);
        playFly.agent.enabled = true;
        playFly.IsFly = false;
        Destroy(Clone.gameObject,0.3f);

    }
}
