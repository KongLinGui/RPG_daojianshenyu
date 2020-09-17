using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 敌人控制脚本
/// </summary>
public enum EnemyState//状态
{
    // 闲置 寻找目标 攻击 死亡
    Idle, FindTarget, Attack, Die
}

public enum EnemyType//类型
{
    //普通 boss
    Normal,Boss
}

public class Enemy : MonoBehaviour
{
    public int Id; //怪物id
    //调用脚本
    public UIPanelJoystick uiPanelJoystick;

    public bl_HUDText HUDRoot;

    public EnemyState state = EnemyState.Idle;//敌人开始状态为 闲置
    public EnemyType type = EnemyType.Normal;//敌人类型为普通类型

    public UnityEngine.AI.NavMeshAgent agent;//导航组件
    public float speed = 5f; //速度
    public float attackRange = 3f;//攻击范围

    public Animator ani;//动画组件

    public Transform targetObj;//玩家攻击目标

    public int HP = 100;//血量

    public GameObject hitFxPrefab;// 被击特效
    public GameObject deadFxPrefab;// 死亡特效
    private bool isDead = false;

    private bool IsAttack = false;

    public GameObject AinPrefab;//掉血预设物
    // Use this for initialization
    void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();//获取导航组件
        agent.speed = speed; //将导航速度和自己定义速度保持一致
        ani = GetComponent<Animator>();//获取动画组件

        HUDRoot = FindObjectOfType<bl_HUDText>();
        //获取特效
        hitFxPrefab = Resources.Load<GameObject>("HitFx/HitFx1");
        deadFxPrefab = Resources.Load<GameObject>("HitFx/DeadFx1");
        //初始化
        uiPanelJoystick = FindObjectOfType<UIPanelJoystick>();

        AinPrefab = Resources.Load<GameObject>("HitFx/TextAni");
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case EnemyState.Idle: //闲置
                ani.SetBool("CanMove", false);
                break;
            case EnemyState.FindTarget: //找目标
                if (IsNearTarget()) //找到目标
                {
                    state = EnemyState.Attack;//攻击
                }
                else
                {
                    ani.SetBool("CanMove", true);
                    agent.SetDestination(targetObj.position);
                }
                break;
            case EnemyState.Attack://攻击

                Attack();//执行攻击

                break;
            case EnemyState.Die:
                ToDie();

                break;
            default:
                break;
        }
    }
   
    //死亡
    public void ToDie()
    {
        if (isDead) { return; }
        isDead = true;
        //1.停掉自身开启的所有协程
        StopAllCoroutines();
        //2.关闭身上的导航组件
        agent.enabled = false;
        //3.播放死亡动画
        ani.SetTrigger("Dead");
        //4.开启协程
        StartCoroutine(ToDieMov());
        //5.更新任务进度
        GameController.Instance.UpdatePlayerkillQuest(Id, 1);
    }
    // 掉落物品生成
    void ItemInit()
    {
        //1 取以自我为圆心的圆上的一些点坐标
        Vector3 dir = transform.forward.normalized;
        List<Vector3> posList = new List<Vector3>();

        int count = Random.Range(3, 6);
        if (type == EnemyType.Boss)
        {
            count = Random.Range(10, 20);
        }
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = transform.position + dir * 2f*Random.Range(0.7f,1.2f);
            posList.Add(pos);
            //旋转dir 
            dir = Quaternion.AngleAxis(Random.Range(10,80), Vector3.up) * dir;
        }
        GameObject goldPrefab = Resources.Load<GameObject>("Level/Gold");

        for (int i = 0; i < posList.Count; i++)
        {
            GameObject clone = Instantiate(goldPrefab);
            clone.transform.position = transform.position;
            clone.GetComponent<LevelItemController>().Init(posList[i]);

        }
    }
    //死亡协成
    IEnumerator ToDieMov()
    {
        //1.等待死亡动画播放结束
        yield return new WaitForSeconds(1f);
        //2.生成一个死亡特效
        GameObject fxClone = Instantiate(deadFxPrefab);
        fxClone.transform.position = transform.position;
        Destroy(fxClone, 1f);
        //3.生成一些宝箱，金币，掉落在怪物周围
        ItemInit();
        //5.判断是否为Boss
        if (type == EnemyType.Boss)
        {
           
            //1.显示UI界面
            yield return new WaitForSeconds(1f);
            GameController.Instance.uiController.CreatePanel("PanelLevelOver");
            GameController.Instance.playerData.CurrentExp += 30;
            //获取工程中所有挂载Enemy脚本的物体
            Enemy[] enemys = GameObject.FindObjectsOfType<Enemy>();
            for (int i = 0; i < enemys.Length; i++)//遍历
            {
                Destroy(enemys[i].gameObject);  //删除所有
            }
        }
        else
        {
            GameController.Instance.playerData.CurrentExp += 10;
        }
       
        //4.删除
        Destroy(gameObject);
    }
   

    //攻击
    public void Attack()
    {
        if (IsAttack)
        {
            return;
        }
        //0.切换状态
        ani.SetBool("CanMove", false);
        agent.enabled = false;
        agent.enabled = true;
        //朝向敌人
        transform.LookAt(targetObj.position);
        //1.播放攻击动画 
        ani.SetTrigger("Attack0");
        //2.开启一个协程，配合攻击动画，完成behit
        StartCoroutine(AttackMov());
        IsAttack = true;
    }
    
    //攻击协成
    IEnumerator AttackMov()
    {
        yield return new WaitForSeconds(0.6f);
        Player player = targetObj.GetComponent<Player>();
        player.BeHit(Random.Range(50, 100));//攻击力大小
        yield return new WaitForSeconds(1f);
        state = EnemyState.FindTarget;//一次攻击完毕，将状态切换查找
        IsAttack = false;
    }

    /// <summary>
    /// 设置目标
    /// </summary>
    public void SetTarget(Transform tar)
    {
        if (targetObj != null) //如果有目标，不会再次设置目标
        {
            return;
        }
        targetObj = tar;
        Debug.Log("Enemy targetObj = " + targetObj.name);
        state = EnemyState.FindTarget;//将状态设置为追踪目标
    }


    //判断玩家是否接近目标
    public bool IsNearTarget()
    {
        if (targetObj == null)
        {
            Debug.LogError("IsNearTarget targetObj==null");
            return false;
        }
        float distance = Vector3.Distance(transform.position, targetObj.position);
        if (distance < attackRange) //玩家与目标接近
        {
            return true;
        }
        return false;
    }

    //被打
    public void BeHit(int value)
    {
        if (HP <= 0) //防止重复被打
        {
            return;
        }
        HP -= value;
        //掉血 
        Debug.Log("HP = " + HP);
        //生成掉血UI
        string hitInfo = "- " + value;
        //是否暴击 
        if (Random.Range(0, 10) == 0)
        {
            hitInfo = "暴击 " + (value * 3);
            HUDRoot.NewText(hitInfo, base.transform, Color.red, 8, 20f, -1f, 2.2f, bl_Guidance.Up);

        }
        else
        {
            if (Random.Range(0, 2) == 0)
            {
                HUDRoot.NewText(hitInfo, base.transform, Color.yellow, 8, 20f, -1f, 2.2f, bl_Guidance.LeftDown);
            }
            else
            {
                HUDRoot.NewText(hitInfo, base.transform, Color.yellow, 8, 20f, -1f, 2.2f, bl_Guidance.RightDown);
            }
        }

        //Transform clone = Instantiate(AinPrefab).transform;
        //clone.SetParent(GameObject.Find("Canvas").transform);
        //clone.GetComponent<UIFollow>().SetTarget(transform.Find("UI"));
        //clone.Find("Text").GetComponent<Text>().text = "- " + value;
        //Destroy(clone.gameObject, 2f);
        //生成掉血特效
        GameObject fxClone = Instantiate(hitFxPrefab);
        fxClone.transform.SetParent(transform);
        //Debug.Log(GetComponent<Collider>().bounds.center);是世界坐标系下的坐标
        fxClone.transform.position = GetComponent<Collider>().bounds.center;
        Destroy(fxClone, 1f);

        if (HP<=0)
        {
            state = EnemyState.Die;
        }

    }
}
