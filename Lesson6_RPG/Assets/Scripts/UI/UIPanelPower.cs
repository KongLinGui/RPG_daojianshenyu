using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;

public class RolePower//角色战力
{
    public string Name;//姓名
    public int Power;//战力
    public RolePower(string name,int power)
    {
        this.Name = name;
        this.Power = power;
    }
}

/// <summary>
/// 战力排行榜
/// </summary>
public class UIPanelPower : MonoBehaviour {

    public Transform PowerPrefab;//战力预设物
    public Transform Create;//生成
   
    public int grade;//等级
    public int power1;//战力 

    void Awake()
    {
        PowerPrefab = transform.Find("PowerPrefab").transform;
        Create = transform.Find("Scroll View/Viewport/Content").transform;

        //1.绑定事件 
        transform.Find("ButtonClose").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            SoundController.Instance.PlayAudio("audio");
            Destroy(gameObject);
        });

        grade = GameController.Instance.playerData.Level;//获取等级
        //transform.Find("PowerPrefab/Grade").GetComponent<Text>().text = "等级：" + grade;

        power1 = GameController.Instance.playerData.Strength;//获取战力                
        power1 = grade * 20 + GameController.Instance.playerData.LevelPlan * 30;
        
        //战力信息
        Debug.Log(GameController.Instance.playerData.Name);
        Debug.Log(power1);

        RolePower power = new RolePower(GameController.Instance.playerData.Name,power1);

        //绑定回调事件
        NetClient.Instance.netResponseHandler += DoRankNetResponse;
        NetClient.Instance.SendRequest(new NetRequest("Rank", JsonConvert.SerializeObject(power)));
       
    }

    List<RolePower> rolepower = new List<RolePower>();
    /// <summary>
    /// 处理服务器回应数据
    /// </summary>
    /// <param name="response"></param>
    public void DoRankNetResponse(NetResponse response)
    {
        rolepower = JsonConvert.DeserializeObject<List<RolePower>>(response.Data);
        StartCoroutine(Rank());  //开启排行榜协成
    }
    IEnumerator Rank()//排行榜
    {
        for (int i =transform.Find("Scroll View/Viewport/Content").childCount-1; i>=0; i--)
        {
            Destroy(transform.Find("Scroll View/Viewport/Content").GetChild(i).gameObject);
        }
        yield return null;
        //为所得链表中的数据进行排序
        for (int i = 0; i<rolepower.Count; i++)
        {
            for (int j = i+1; j < rolepower.Count; j++)
            {
                if (rolepower[i].Power<=rolepower[j].Power)
                {
                    RolePower temp = rolepower[i];
                    rolepower[i] = rolepower[j];
                    rolepower[j] = temp;
                }
            }
        }
        //显示排行信息
        for (int i = 0; i < rolepower.Count; i++)
        {
            Transform clone = Instantiate(PowerPrefab);
            clone.SetParent(transform.Find("Scroll View/Viewport/Content"));

            clone.transform.Find("Name").GetComponent<Text>().text = rolepower[i].Name;//名字
            clone.transform.Find("Ranking").GetComponent<Text>().text = "第 " + (i + 1) + " 名";//名次
            clone.transform.Find("Power").GetComponent<Text>().text = "战力：" + rolepower[i].Power;//战力
           
        }
    }
    void OnDestroy()//删除
    {
        //解除回调的绑定
        NetClient.Instance.netResponseHandler -= DoRankNetResponse;
    }
}
