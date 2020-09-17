using UnityEngine;
using System.Collections;

public class PlayerSkill_3 : PlayerSkill {

    public GameObject skillFx;//技能特效
    public float range = 2f;//范围

    void Start()
    {
        //初始化
        skillFx = Resources.Load<GameObject>("Skill/SkillFx4");
    }

    public override void StartSkill()//重写、覆盖
    {
        base.StartSkill();


        StartCoroutine(SkillMov());
    }

    IEnumerator SkillMov()
    {
        //1.生成特效            
        for (int i = 0; i < 100; i++)
        {

            yield return new WaitForSeconds(0.1f);
            Transform clone = Instantiate(skillFx).transform;
            clone.transform.position = player.transform.position + new Vector3(Random.Range(-2f, 2f), 6f, Random.Range(-2f, 2f));
            clone.gameObject.AddComponent<PlayerSkill_3_1>();
        }
        //3.删除特效
        //Destroy(clone.gameObject, 3f);
    }
}
