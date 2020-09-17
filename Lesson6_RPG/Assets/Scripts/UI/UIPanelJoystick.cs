using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIPanelJoystick : MonoBehaviour {

    //Key Value  字典的定义
    public Dictionary<int, UISkill> uiSkillDic = new Dictionary<int, UISkill>();

    private Slider slider;//经验条    
    
    private float MaxExp ;//最大经验
    private float NowExp ;//当前经验
    private float Grade;//等级

    // Use this for initialization
    void Awake ()
    {
        //初始化      
        slider = transform.Find("ImageExp/Slider").GetComponent<Slider>();
                           
        Transform playerUI = GameObject.FindGameObjectWithTag("Player").transform.Find("UI");

        //给攻击按钮绑定事件
        transform.Find("ButtonAttack").GetComponent<Button>().onClick.AddListener(ClickAttack);
        //给自动战斗绑定事件
        transform.Find("Button").GetComponent<Button>().onClick.AddListener(Autofight);

        //给名片设置目标
        transform.Find("Name").GetComponent<UIFollow>().SetTarget(playerUI);
        transform.Find("Name").GetComponent<Text>().text = GameController.Instance.playerData.Name;

    }

    void Update()
    {
        PlayerExp();
    }
    //玩家经验
    public void PlayerExp()
    {
        MaxExp = GameController.Instance.playerData.NeedExp;//拿到升级经验
        NowExp = GameController.Instance.playerData.CurrentExp;//拿到当前经验
        Grade = GameController.Instance.playerData.Level;//拿到等级
        slider.value = (float)NowExp / (float)MaxExp;
        if (NowExp>=MaxExp)
        {
            GameController.Instance.playerData.Level += 1;
            GameController.Instance.playerData.CurrentExp-=(int) MaxExp;
            GameController.Instance.playerData.NeedExp = GameController.Instance.playerData.Level * 10;
            slider.value = (float)NowExp / (float)MaxExp;
        }
        transform.Find("ImageExp/GradeText/Grade").GetComponent<Text>().text = "" + Grade;
    }

    public void ClickAttack()
    {
        GameController.Instance.PlayerFindTarget();
    }

    //初始化
    public void Init()
    {
        //1.拿到技能数据，初始化技能按钮
        for (int i = 0; i < GameController.Instance.playerData.SkillList.Count; i++)
        {
            UISkill uiskill = transform.Find("Skill/ButtonSkill" + (i + 1)).GetComponent<UISkill>();
            Skill skill = GameController.Instance.playerData.SkillList[i];
            uiskill.Init(skill);
            //将uiskill，引用存入字典结构
            uiSkillDic.Add(skill.Id, uiskill);          
        }
    }  

    public void UISkillStartCd(int skillid)
    {
        //1.通知对应的技能图标 UISkill
        uiSkillDic[skillid].CdStart();
    }

    public void Autofight()//自动战斗
    {
        GameController.Instance.player.Find();
    }
}
