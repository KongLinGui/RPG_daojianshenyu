using UnityEngine;
using System.Collections;
/// <summary>
/// 在玩家脚下生成一个特效，多段伤害
/// </summary>
public class PlayerSkill_1 : PlayerSkill {

    public GameObject skillFx;//技能特效
    public float range = 2f;//范围

    void Start()
    {
        //初始化
        skillFx = Resources.Load<GameObject>("Skill/SkillFx1");
    }

    public override void StartSkill()//重写、覆盖
    {
        base.StartSkill();
       

        StartCoroutine(SkillMov());
    }

    IEnumerator SkillMov()
    {
        //1.生成特效
        Transform clone = Instantiate(skillFx).transform;
        clone.position = player.transform.position;
        //2.每隔一段时间，检测周围怪物，造成伤害
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.2f);
            Collider[] colls = Physics.OverlapSphere(clone.position, range);
            for (int j = 0; j < colls.Length; j++)
            {
                if (colls[j].tag == "Enemy")
                {
                    Enemy e = colls[j].GetComponent<Enemy>();
                    if (e.state != EnemyState.Die)
                    {
                        //e.BeHit((int)skillData.HitValue);
                        e.BeHit(20);
                    }
                }
            }
        }
        //3.删除特效
        Destroy(clone.gameObject,3f);
    }
}
