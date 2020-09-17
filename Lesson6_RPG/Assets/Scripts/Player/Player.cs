using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 游戏人物脚本
/// </summary>

//人物状态
public enum PlayerState
{
    //闲置 移动 寻找目标 攻击 死亡 自动战斗
    Idle,Move,FindTarget,Attack,Die, Autofight
}

public class Player : MonoBehaviour
{
    public bl_HUDText HUDRoot;
    //人物默认状态为闲置状态
    public PlayerState state = PlayerState.Idle;
    
    //动画组件
    public Animator ani;
    //导航组件
    public UnityEngine.AI.NavMeshAgent agent;

    public float speed = 5f; //速度
    public float attackRange = 3f;//攻击范围
    
    public Transform targetObj;//玩家攻击目标

    public bool IsAttack = false;
    private Skill currentSkill;//记录玩家是否需要释放技能

    public GameObject hitFxPrefab;// 被击特效
    public GameObject textAinPrefab;//掉血预设物

    public List<Skill> skillDataList;
    //字典
    public Dictionary<int, PlayerSkill> playerSkillDic = new Dictionary<int, PlayerSkill>();

    public bool IsFly = true;//是否起飞


    public bool IsSelf = false;
    public Vector3 Endpoint;//终点
    // Use this for initialization
    void Awake ()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //agent.updateRotation = false;
        agent.speed = speed; //将导航速度和自己定义速度保持一致
        ani = GetComponent<Animator>();
        HUDRoot = FindObjectOfType<bl_HUDText>();
        hitFxPrefab = Resources.Load<GameObject>("HitFx/HitFx1");
        IsFly = false;//不起飞

        textAinPrefab = Resources.Load<GameObject>("HitFx/TextAni");

    }

    public void Init()
    {
        
        //1. 拿到技能数据
        skillDataList = GameController.Instance.playerData.SkillList;

        //2. 完成playerskill的初始化
        for (int i = 0; i < skillDataList.Count; i++)
        {
            PlayerSkill playerskill = transform.Find("Skill"+(i+1)).GetComponent<PlayerSkill>();          
            playerskill.Init(skillDataList[i],this);
           
            //3.将 playerskill 添加到字典
            playerSkillDic.Add(skillDataList[i].Id, playerskill);
        }

    }



    // Update is called once per frame
    void Update ()
    {
        //检测输入
        CheckInput();

        switch (state)
        {
            case PlayerState.Idle: //闲置
                ani.SetBool("CanMove", false);
                break;
            case PlayerState.Move: //移动
                Move();
                ani.SetBool("CanMove", true);
                break;
            case PlayerState.Autofight://自动战斗
                ani.SetBool("CanMove", true);
                break;
            case PlayerState.FindTarget: //找目标
                if (IsNearTarget()) //找到目标
                {
                    state = PlayerState.Attack;
                }
                else
                {
                    ani.SetBool("CanMove", true);
                    agent.SetDestination(targetObj.position);
                }

                break;
            case PlayerState.Attack://攻击
                // 判断是否已经开始攻击了，避免重复攻击的情况
                if (IsAttack)
                {
                    return;
                }
                if (currentSkill == null)
                {
                    Attack();//执行攻击
                }
                else
                {
                    StartCoroutine(SkillMov(currentSkill));//开启技能
                }
             
                break;
            default:
                break;
        }

        //Endpoint = GameObject.FindGameObjectWithTag("GameObject").transform.position;
        Endpoint = GameObject.Find("Misc/EnemySpawnBoss/Cube").transform.position;
    }

    //攻击
    public void Attack()
    {
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
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.2f); 
            //behit
            Enemy e = targetObj.GetComponent<Enemy>();
            //对其状态进行检测 如果死亡，则终止攻击状态
            if (e.state == EnemyState.Die)
            {
                targetObj = null;
                break;
            }
            e.BeHit(Random.Range(5, 30));
        }
        state = PlayerState.Idle;//一次攻击完毕，将状态切换为静止
        SelfAttack();
        IsAttack = false;
    }
    //技能
    public void StartSkill(int skillid)
    {
        //0.判断是否是攻击状态
        if (IsAttack)
        {
            return;
        }
        //1.通过id拿到当前技能数据，判断技能释放条件是否成立
        //1.1 拿数据
        Skill skill = new Skill();
        for (int i = 0; i < skillDataList.Count; i++)
        {
            if (skillDataList[i].Id == skillid)
            {
                skill = skillDataList[i];
            }
        }
        //1.2 判断
        if (skill.Type == SkillType.NeedTarget) //需要目标
        {   
            if (FindTarget())//找到目标
            {
                currentSkill = skill;
            }
        }
        else if(skill.Type == SkillType.DontTarget) //不需要目标
        {
            //1. 切换状态
            state = PlayerState.Attack;
            //2. 直接执行技能
            StartCoroutine(SkillMov(skill));
        }

    }

    //技能协成
    IEnumerator SkillMov(Skill data)
    {

        IsAttack = true;
        //0.切换状态
        ani.SetBool("CanMove", false);
        agent.enabled = false;
        agent.enabled = true;
        // 通知 UISkill 开始冷却

        GameController.Instance.UISkillStartCd(data.Id);

        //1.播放对应的技能动画 
        ani.SetTrigger(data.AniParam);

        //2.开启一个协程，配合攻击动画
        yield return new WaitForSeconds(0.5f);
        //3.调用对应的PlayerSkill 开始技能执行
        playerSkillDic[data.Id].StartSkill();
        //3.1 添加 技能释放的文字提示
        HUDRoot.NewText(data.Name, base.transform, Color.blue, 20, 20f, -1f, 2.2f, bl_Guidance.Up);

        //4.当前动画播放完毕后，再重置状态
        yield return new WaitForSeconds(0.4f);

        //获取动画层 0 指Base Layer
        //AnimatorStateInfo stateinfo = ani.GetCurrentAnimatorStateInfo(0);
        //while (stateinfo.normalizedTime < 0.99f)
        //{
        //    Debug.Log(stateinfo.normalizedTime);
        //    yield return null;
        //}

        //5. 重置
        currentSkill = null;
        state = PlayerState.Idle;//释放技能完毕，将状态切换为静止       
        IsAttack = false;
        
    }

    //通过检测输入来改变人物状态
    void CheckInput()
    {
        if (GameController.Instance.InputJoystick != Vector3.zero&& IsFly ==false)//摇杆动了 不起飞
        {
            if (state!= PlayerState.Attack)
            {
                agent.enabled = false;
                agent.enabled = true;
                state = PlayerState.Move;
                currentSkill = null;
            }
            
        }
        else //摇杆没动  静止，查找目标，攻击
        {
            if (state == PlayerState.Move)
            {
                state = PlayerState.Idle;
            }
        }
    }

    //移动
    void Move()
    {
        //使用 agent 实现人物移动，旋转
        Vector3 joystick = GameController.Instance.InputJoystick;
        agent.velocity = new Vector3(joystick.x, 0, joystick.y) * speed;
        //旋转
        transform.rotation = Quaternion.LookRotation(new Vector3(joystick.x, 0, joystick.y));
    }

    /// <summary>
    /// 设置目标
    /// </summary>
    public void SetTarget(Transform tar)
    {
        state = PlayerState.FindTarget;//将状态设置为追踪目标

        if (targetObj!=null && Vector3.Distance(targetObj.position,transform.position)<5f) //如果有目标且依然在攻击范围内，不会再次设置目标
        {
            return;
        }
        targetObj = tar;
        Debug.Log("targetObj = " + targetObj.name);
        
    }

    //找
    public void Find()
    {
        IsSelf = true;
       
        //设置移动目标
        agent.SetDestination(Endpoint);
        state=PlayerState.Autofight;//自动战斗
    }
    // 碰撞触发或者怪死触发
    public void SelfAttack()
    {
        if (IsSelf)
        {
            if (!FindTarget())
            {
                Find();
            }
        }
    }


    // 找目标
    public bool FindTarget()
    {
        //Debug.Log("FindTarget");
        float range = 5f; //查找半径
        //将会返回以参数1为原点和参数2为半径的球体内“满足一定条件”的碰撞体集合，
        //此时我们把这个球体称为 3D相交球。
        Collider[] colliders = Physics.OverlapSphere(transform.position,range);

        // 把其中是enemy，且状态不能为die的物体拿出来，放到list中
        List<Transform> enemys = new List<Transform>();

        for (int i = 0; i < colliders.Length; i++)
        {
            Debug.Log(colliders[i].name);
            if (colliders[i].tag == "Enemy")
            {
                if (colliders[i].GetComponent<Enemy>().state != EnemyState.Die)
                {
                    enemys.Add(colliders[i].transform);
                }                
            }
        }
        if (enemys.Count == 0)
        {
            return false;
        }
        else
        {
            //找到其中最近的，设置为目标
            Transform e = enemys[0]; //取第一个
            //取第一个 和玩家的距离
            float dis = Vector3.Distance(e.position,transform.position);
            for (int i = 1; i < enemys.Count; i++)
            {
                //取 剩下的 和玩家的距离
                float disNew = Vector3.Distance(transform.position, enemys[i].position);
                if (disNew < dis)//如果离玩家更近
                {
                    e = enemys[i];
                    dis = disNew;
                }
            }

            SetTarget(e);//设置为目标
        }
        return true;
    }


    //判断玩家是否接近目标
    public bool IsNearTarget()
    {
        if (targetObj==null)
        {
            Debug.LogError("IsNearTarget targetObj==null");
            return false;
        }
        float distance = Vector3.Distance(transform.position, targetObj.position);
        if (distance<attackRange) //玩家与目标接近
        {
            return true;
        }
        return false;
    }

    //被打
    public void BeHit(int value)
    {
        Transform clone = Instantiate(textAinPrefab).transform;
        clone.SetParent(GameObject.Find("Canvas").transform);
        clone.GetComponent<UIFollow>().SetTarget(transform.Find("UI"));
        clone.Find("Text").GetComponent<Text>().text = "- " + value;
        Destroy(clone.gameObject, 2f);

        //生成掉血特效
        GameObject fxClone = Instantiate(hitFxPrefab);
        fxClone.transform.SetParent(transform);
        //Debug.Log(GetComponent<Collider>().bounds.center);是世界坐标系下的坐标
        fxClone.transform.position = GetComponent<Collider>().bounds.center;
        Destroy(fxClone, 1f);
    }
}
