using UnityEngine;
using System.Collections;

public enum SkillType
{
    //需要目标，不需要目标
    NeedTarget,DontTarget
}

public class Skill
{
    //id
    public int Id;
    //名称
    public string Name;
    //类型
    public SkillType Type;
    //动画参数
    public string AniParam;
    //冷却时间
    public float CdTime;
    //伤害
    public float HitValue;
    //图标 
    public string Icon;
    //描述
    public string Desc;

    public Skill() { }
    //构造方法
    public Skill(int id,string name,SkillType type,string aniparam,float cdtime,float hitvalue,string icon,string desc)
    {
        this.Id = id;
        this.Name = name;
        this.Type = type;
        this.AniParam = aniparam;
        this.CdTime = cdtime;
        this.HitValue = hitvalue;
        this.Icon = icon;
        this.Desc = desc;
    }

    public Skill Clone()
    {
        Skill newSkill = new Skill(this.Id, this.Name, this.Type, this.AniParam, this.CdTime, this.HitValue, this.Icon, this.Desc);
        return newSkill;
     }
}

/// <summary>
/// 控制技能逻辑 Base类
/// </summary>
public class PlayerSkill : MonoBehaviour
{
    public Skill skillData; //技能数据
    public Player player;



    //初始化方法
    public void Init(Skill skill,Player p)
    {
        //1. 拿到当前技能数据
        skillData = skill;
        //2. 拿到任务引用
        this.player = p;
    }


    //开始执行技能 虚方法
    public virtual void StartSkill() //通过虚函数实现
    {
        //1.开始执行技能逻辑
        Debug.Log(skillData.Name + " 开始执行");
    }

}
