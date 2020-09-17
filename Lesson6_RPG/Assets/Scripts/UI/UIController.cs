using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.EventSystems;

public class AccountInfo //账户信息
{
    public string Username;
    public string Password;
    
    public AccountInfo(string user,string pass)
    {
        Username = user;
        Password = pass;
    }
}
public class UIController : MonoBehaviour
{
    //list 用户对象
    public List<AccountInfo> accountInfoList = new List<AccountInfo>();

    //public Text logText;   

    private void Awake() 
    {
        //AccountInfo a = new AccountInfo("aaa","aaa");
        //AccountInfo b = new AccountInfo("bbb", "bbb");

        //accountInfoList.Add(a);
        //accountInfoList.Add(b);

        ////将对象转为json
        //string str_accountInfoList = JsonConvert.SerializeObject(accountInfoList);
        ////将json转为对象
        //List<AccountInfo> list = JsonConvert.DeserializeObject<List<AccountInfo>>(str_accountInfoList);

        //logText.text = str_accountInfoList;
        /*
        //将对象 a 转换为 json 字符串
        string str_a = JsonUtility.ToJson(a);
        //将字符串存入
        PlayerPrefs.SetString("a", str_a);
        Debug.Log(str_a);
        //将字符串取出
        string str_a1 = PlayerPrefs.GetString("a", "");
        //将json 字符串 转换为 对象 a1
        AccountInfo a1 = JsonUtility.FromJson<AccountInfo>(str_a1);
        */    
    }


    public void Init()
    {        
        //0.连接服务器
        NetClient.Instance.Start("172.16.7.71", 8886);
        StartCoroutine(NetClient.Instance.NetResponseMov());
        //1.生成登录界面
        CreatePanel("PanelLogin");
        StartCoroutine(ConnectMov());

        //GameController.Instance.CreateNewPlayer("小明");

        //UIPanelLoad.Instance.Init("Level", delegate ()
        // {
        //     GameController.Instance.InitLevel(1);
        // });

        //CreatePanel("PanelQuest");

    }

    IEnumerator ConnectMov()//连接
    {
        UIPanelWait.Instance.Wait();
        while (!NetClient.Instance.connected)
        {
            yield return null;
        }
        UIPanelWait.Instance.Stop();
        //连接成功 
        UIPanelTip.Instance.Init("连接服务器成功！");
        
    }

   

    //创建UI界面
    public Transform CreatePanel(string panelName)
    {
        //1.读取资源
        GameObject prefab = Resources.Load<GameObject>("UI/" + panelName);
        if (prefab == null)
        {
            Debug.LogError("ui panel not found");
        }
        //2.生成
        Transform uiClone = Instantiate(prefab).transform;
        //3.设置父物体，设置坐标属性
        uiClone.SetParent(transform);
        uiClone.localPosition = Vector3.zero;
        uiClone.localScale = Vector3.one;
        //4.返回引用
        return uiClone;
    }

    //删除所有界面
    public void DestroyAllPanel()
    {
        for (int i = transform.childCount-1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }


    //打开注册界面
    public void OpenResgisterPanel()
    {
        SoundController.Instance.PlayAudio("audio");
        //创建注册界面
        CreatePanel("PanelRegister");

    }

    //创建人物界面
    public void OpenPanelPlayer()
    {
        CreatePanel("PanelCreateRole");
    }
    
}
