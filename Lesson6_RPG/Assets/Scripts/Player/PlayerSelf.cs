using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 自动战斗 挂在Playr上
/// </summary>
public class PlayerSelf : MonoBehaviour
{
    //获取组件
    public Animator anim;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    //获取技能
    public List<Transform> SelfVecList = new List<Transform>();
    public Coroutine seekCor;
    public Player player;
    public List<UISkill> skillList = new List<UISkill>();
    // Use this for initialization
    void Start()
    {
        //初始化
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GetComponent<Player>();

    }
    public void Init()
    {
        if (skillList.Count < 1)
        {
            //技能初始化
            GameObject skillObj = GameObject.Find("SkillGroup");
            for (int i = 0; i < skillObj.transform.childCount; i++)
            {
                skillList.Add(skillObj.transform.GetChild(i).GetComponent<UISkill>());
            }
        }
        //获取技能图标的父物体
        GameObject obj = GameObject.Find("MonsterPostion");
        int number = obj.transform.childCount;
        //便利获取
        for (int i = 0; i < number; i++)
        {
            if (obj.transform.GetChild(i).GetComponent<Collider>().enabled == true)
            {
                //连组获取
                SelfVecList.Add(obj.transform.GetChild(i).transform);
            }
        }
        //获取成功
        if (SelfVecList.Count > 0)
        {
            seekCor = StartCoroutine(SeekEnemy());

        }
    }
    /// <summary>
    /// 停止
    /// </summary>
    public void StopSelf()
    {
        navMeshAgent.enabled = false;
        navMeshAgent.enabled = true;
        if (seekCor != null)
        {
            StopCoroutine(seekCor);
        }
        if (doSkill != null)
        {
            StopCoroutine(doSkill);
        }
        navMeshAgent.updateRotation = false;
    }
    /// <summary>
    /// 开启自动战斗
    /// </summary>
    /// <returns></returns>
    IEnumerator SeekEnemy()
    {
        //开启导航组件 旋转角度
        navMeshAgent.updateRotation = true;

        //便利获取 生成怪点
        for (int w = 0; w < SelfVecList.Count; w++)
        {
            if ((SelfVecList[w].GetComponent<Collider>().enabled == true))
            {
                //设立目标
                navMeshAgent.SetDestination(SelfVecList[w].position);
                //开启动作
                anim.SetBool("run", true);
                bool seekSwamp = true;
                //判断怪点是否出现怪物
                while (seekSwamp)
                {
                    seekSwamp = SelfVecList[w].GetComponent<Collider>().enabled;

                    yield return new WaitForSeconds(0.2f);
                }
            }
            //取消自动导航
            navMeshAgent.enabled = false;
            navMeshAgent.enabled = true;
            bool noenemy = true;
            //便利怪物
            while (noenemy)
            {
                //找到怪物
                Collider[] enemys = Physics.OverlapSphere(transform.position, 10f);
                List<Transform> enemyList = new List<Transform>();
                for (int i = 0; i < enemys.Length; i++)
                {
                    if (enemys[i].tag == "Enemy" && enemys[i].gameObject.GetComponent<Enemy>().state != EnemyState.Die)
                    {
                        enemyList.Add(enemys[i].transform);
                    }
                }

                if (enemyList.Count == 0)
                {  //没有怪物
                    noenemy = false;
                }
                else
                {
                    //找到最近的一个
                    for (int i = 0; i < enemyList.Count; i++)
                    {
                        for (int j = i; j < enemyList.Count - 1 - i; j++)
                        {
                            float oneDis = (this.transform.position - enemyList[i].position).magnitude;
                            float twoDis = (this.transform.position - enemyList[j].position).magnitude;
                            if (oneDis > twoDis)
                            {
                                Transform tra = enemyList[i];
                                enemyList[i] = enemyList[j];
                                enemyList[j] = tra;
                            }
                        }
                    }
                    //等待时间
                    while (navMeshAgent.enabled == false)
                    {
                        yield return null;
                    }
                    //设立目标 自动导航
                    navMeshAgent.SetDestination(enemyList[0].position);

                    //周围怪物距离
                    bool noEnemy = true;
                    while (noEnemy)
                    {
                        yield return new WaitForSeconds(0.2f);
                        float dis = (this.transform.position - enemyList[0].position).magnitude;
                        if (dis < 1.5f)
                        {
                            noEnemy = false;
                            navMeshAgent.enabled = false;
                            navMeshAgent.enabled = true;
                            anim.SetBool("run", false);
                        }
                    }
                    //开启技能
                    doSkill = StartCoroutine(DoSkill());
                    //判断怪物死亡
                    while (enemyList[0].gameObject.GetComponent<Enemy>().state != EnemyState.Die)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    //停止技能
                    StopCoroutine(doSkill);

                }
            }
            yield return new WaitForSeconds(2f);

        }
    }
    Coroutine doSkill;
    //开启技能
    IEnumerator DoSkill()
    {
        int skillId = 0;
        while (true)
        {
            skillList[skillId % skillList.Count].ClickStartSkill();
            skillId++;
            yield return new WaitForSeconds(2f);
        }
    }

    //Coroutine doSkill;
    ////开启技能
    //IEnumerator DoSkill()
    //{
    //    int skillId = 0;
    //    while (true)
    //    {
    //        skillList[skillId % skillList.Count].StartSkill();
    //        yield return new WaitForSeconds(2f);
    //    }
    //}
}