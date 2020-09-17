using UnityEngine;
using System.Collections;
using DG.Tweening;
/// <summary>
/// 关卡道具控制脚本
/// </summary>
public class LevelItemController : MonoBehaviour {

    public GameObject getFxPrefab;//得到道具后的特效

    //初始化 参数： 掉落终点
    public void Init(Vector3 endPos)
    {
        //0 获取预设物
        getFxPrefab = Resources.Load<GameObject>("Level/GetItemFx");

        //1.朝终点做抛物线运动

        Tween tw = transform.DOJump(endPos, 1f, 1, 0.5f);
        tw.SetEase(Ease.Linear);

        tw.OnComplete(delegate ()
        {
            //2.到达终点，等1s，飞向人物
            //Debug.Log("到达目标点");
            StartCoroutine(FlyToPlayer());
        });
       
    }

    IEnumerator FlyToPlayer()//飞向人物
    {
        yield return new WaitForSeconds(1f);
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        float dis = Vector3.Distance(player.position, transform.position);
        while (dis>0.1f)
        {
            transform.LookAt(player);
            transform.Translate(transform.forward *20*Time.deltaTime,Space.World);
            yield return null;
            dis = Vector3.Distance(player.position, transform.position);
        }
        //到达人物身上 生成个特效
        Transform clone = Instantiate(getFxPrefab).transform;
        clone.position = player.position;
        Destroy(clone.gameObject, 0.5f);

        Destroy(gameObject);
    }

}
