using UnityEngine;
using System.Collections;
/// <summary>
/// 在玩家脚下生成一个特效，多段伤害
/// </summary>
public class PlayerSkill_2 : PlayerSkill
{
    public GameObject skillFx;

    void Start()
    {
        //初始化
        skillFx = Resources.Load<GameObject>("Skill/SkillFx2");
    }

    public override void StartSkill()//重写、覆盖
    {
        base.StartSkill();

        StartCoroutine(SkillMov());
    }

    IEnumerator SkillMov()
    {
        //1.生成特效
        for (int i = 0; i < 20; i++)
        {
            Transform clone = Instantiate(skillFx).transform;
            clone.SetParent(player.transform);
            clone.localPosition = new Vector3(0, 0.8f, 0);
            clone.localEulerAngles = Vector3.zero;

            //1.1 给特效挂载 脚本
            clone.gameObject.AddComponent<PlayerSkill_2_1>();
            yield return null;
            Destroy(clone.gameObject, 5f);
        }
       
    }

}
