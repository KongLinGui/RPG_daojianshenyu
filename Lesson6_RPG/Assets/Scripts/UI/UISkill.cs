using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
/// <summary>
/// 控制技能图标的一些逻辑
/// </summary>
public class UISkill : MonoBehaviour {

    public Skill skillData;

    private Transform mask;//遮罩图片
    private Text cdText;//遮罩图片
    private float cdTime=3f;//CD时间
    private bool IsCD = false;//是否在冷却中

    void Start()
    {
        mask = transform.Find("Mask");
        cdText = mask.Find("Text").GetComponent<Text>();
       
    }

    //初始化
    public void Init(Skill data)
    {
        //1.绑定按钮事件 
        transform.GetComponent<Button>().onClick.AddListener(ClickStartSkill);

        //2.获取技能数据（包括cd，名字，描述了）
        skillData = data;

        //3.显示UI
        transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Skill/" + skillData.Icon);
        transform.Find("Text").GetComponent<Text>().text = skillData.Name;

    }


    public void ClickStartSkill()
    {
        if (IsCD)
        {
            return;
        }
        //1.通知玩家开始释放技能
        GameController.Instance.PlayerStartSkill(skillData.Id);   
       
    }

    //开始冷却  
    public void CdStart()
    { 
        //Debug.Log(skillData.Name + " 开始冷却");
        //开始CD冷却
        StartCoroutine(DoSkillCD());
        IsCD = true;
    }
    IEnumerator DoSkillCD()
    {
        //显示mask
        mask.gameObject.SetActive(true);
        float time = cdTime;
        //获取mask上的image组件
        Image maskImage = mask.GetComponent<Image>();
        while (time>0)
        {
            //显示文本
            cdText.text = string.Format("{0:f2}", time);
            //改变fill amount
            maskImage.fillAmount = time / cdTime;
            yield return null;
            time -= Time.deltaTime;
        }
        //冷却结束
        mask.gameObject.SetActive(false);
        IsCD = false;
    }

    
}
