using UnityEngine;
using System.Collections;

public class PlayerSkill_5 : PlayerSkill
{

    public GameObject jianPrefab;
    public Transform pos;
    public Transform enemy;    

    void Start()
    {
        //初始化
        jianPrefab = Resources.Load<GameObject>("Skill/SkillFx6");
    }

    public override void StartSkill()//重写、覆盖
    {
        base.StartSkill();

        StartCoroutine(Mov());
    }

    IEnumerator Mov()
    {
        //1.生成 一些剑
        Transform[] clones = new Transform[10];
        for (int i = 0; i < 10; i++)
        {
            Transform clone = Instantiate(jianPrefab).transform;
            clone.position = pos.position;
            clone.Rotate(transform.right, -90f); //绕自身x轴，转 -90度
            clone.Rotate(transform.up, -36f * i);
            clone.position += clone.forward * 2f;
            clones[i] = clone;
            yield return null;
        }

        yield return null;
        //2.旋转，指向前方 / Enemy
        for (int i = 0; i < clones.Length; i++)
        {
            StartCoroutine(LookMov(clones[i]));
        }

    }
    IEnumerator LookMov(Transform clone)
    {
        //Vector3 dian = clone.position + clone.forward * 1f;
        //clone.RotateAround(dian,clone.right, 90f);

        Vector3 forword = enemy.position - clone.position;
        Quaternion startRotation = clone.rotation;
        Quaternion endRotation = Quaternion.LookRotation(forword);

        for (int j = 1; j <= 10; j++)
        {
            //基于 世界坐标系下的  
            clone.rotation = Quaternion.Lerp(startRotation, endRotation, j * 0.1f);
            //clone.RotateAround(dian,, 9f);
            yield return new WaitForSeconds(0.1f);
        }


        while (true)
        {

            clone.LookAt(enemy);
            yield return null;

        }
    }
}

