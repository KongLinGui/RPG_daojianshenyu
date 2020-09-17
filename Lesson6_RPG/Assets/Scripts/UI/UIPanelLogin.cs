using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
/// <summary>
/// 登录界面
/// </summary>
public class UIPanelLogin : MonoBehaviour
{
    private UIController uiController;

    //按钮
    public Button buttonLogin; //登录
    public Button buttonRegister;//注册
    //输入框
    public InputField userName;//名称
    public InputField passWord;//密码
    //记住我
    public Toggle toggle;

  
    void Awake()
    {
        Debug.Log("UIPanelLogin Awake");
        //获取参与代码逻辑的组件
        buttonLogin = transform.Find("ButtonLogin").GetComponent<Button>();
        buttonRegister = transform.Find("ButtonRegister").GetComponent<Button>();
        userName = transform.Find("Username/InputField").GetComponent<InputField>();
        passWord = transform.Find("Password/InputField").GetComponent<InputField>();
        toggle = transform.Find("Toggle").GetComponent<Toggle>();

        buttonRegister.onClick.AddListener(ClickRegister);//绑定注册按钮
        buttonLogin.onClick.AddListener(ClickLogin);//绑定登录按钮
      
    }

    private void Start()
    {
        Debug.Log("UIPanelLogin Start");
        Init();
    }

    public void Init()
    {
        uiController = GameController.Instance.uiController;//精简代码量

        //初始化状态  通过判断记住我的状态，来显示信息 
        //1. 拿数据
        string remember = PlayerPrefs.GetString("Remember","");
        string str_account = PlayerPrefs.GetString("Account", "");
        //2. 判断数据
        if (str_account == "")
        {
            return;
        }
        //3.转为对象
        AccountInfo account = JsonUtility.FromJson<AccountInfo>(str_account);
        //4.对remember进行判断
        if (remember == "" || remember == "False")
        {
            userName.text = account.Username;
        }
        else
        {
            userName.text = account.Username;
            passWord.text = account.Password;
        }
        if (remember=="True")
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
    }
    //点击登录
    public void ClickLogin()
    {
        //string UserName = userName.text;
        //string PassWord = passWord.text;

        //if (PlayerPrefs.GetString("accountInfoList", "") != "")
        //{
        //    uiController.accountInfoList = JsonConvert.DeserializeObject<List<AccountInfo>>(PlayerPrefs.
        //        GetString("accountInfoList", ""));

        //}

        SoundController.Instance.PlayAudio("audio");
        //1.判断为空的情况
        if (userName.text == "" || passWord.text == "")
        {
            //提示
            Debug.Log("不能为空");
            //UIPanelNotice.Instance.Init("不能为空");
            UIPanelTip.Instance.Init("不能为空");
            return;
        }
        //2.0 绑定回调事件
        NetClient.Instance.netResponseHandler += DoLoginNetResponse;
        //2.1 发送数据
        AccountInfo account = new AccountInfo(userName.text, passWord.text);
        NetClient.Instance.SendRequest(new NetRequest("Login", 
            JsonConvert.SerializeObject(account)));
        
        //2.判断账号 密码是否正确               
        //for (int i = 0; i < uiController.accountInfoList.Count; i++)
        //{
        //    //判断账号密码
        //    if (userName.text == uiController.accountInfoList[i].Username &&
        //        passWord.text == uiController.accountInfoList[i].Password)
        //    {
        //        UIPanelNotice.Instance.Init("登录成功", delegate ()
        //        {
        //            uiController.CreatePanel("PanelCreateRole");

        //        });
        //        Destroy(gameObject);

        //        //1.提示
        //        Debug.Log("登录成功");
        //        uiController.CreatePanel("PanelCreateRole");

        //        UIPanelNotice.Instance.Init("登录成功", delegate ()
        //        {
        //            GameController.Instance.UserName = UserName;
        //            GameController.Instance.PassWord = PassWord;
        //            bool isSave = GameController.Instance.GetData(UserName, PassWord);
        //            if (isSave)
        //            {
        //                uiController.DestroyAllPanel();
        //                // 进入主城
        //                GameController.Instance.GetData(UserName, PassWord);

        //                UIPanelLoad.Instance.Init("MainCity", delegate ()
        //                {
        //                    GameController.Instance.uiController.CreatePanel("PanelMainCity");
        //                });
        //            }
        //            else
        //            {
        //                // 进入创建人物
        //                uiController.OpenPanelPlayer();
        //            }
        //        });
        //        if (toggle.isOn)
        //        {
        //            AccountInfo account = new AccountInfo(userName.text, passWord.text);
        //            string str_account = JsonUtility.ToJson(account);
        //            PlayerPrefs.SetString("Account", str_account);
        //            PlayerPrefs.SetString("Remember", toggle.isOn.ToString());
        //        }
        //        else
        //        {
        //            AccountInfo account = new AccountInfo(userName.text, "");
        //            string str_account = JsonUtility.ToJson(account);
        //            PlayerPrefs.SetString("Account", str_account);
        //            PlayerPrefs.SetString("Remember", toggle.isOn.ToString());
        //        }
        //        //2.1存账号密码      
        //        //存在数组里
        //        //把链表转换成字符串
        //        //uiController.accountInfoList.Add(new AccountInfo(userName.text, passWord.text));
        //        //string str = JsonConvert.SerializeObject(uiController.accountInfoList);
        //        //PlayerPrefs.GetString("accountInfoList", str);                
        //        //2.2存记住我的状态

        //        Destroy(gameObject);//删除自己
        //        return;
        //    }
        //}
        //3. 提示 
        //Debug.Log("帐号或密码错误！");
        //UIPanelNotice.Instance.Init("帐号或密码错误！");
    }

    /// <summary>
    /// 处理登录回应
    /// </summary>
    public void DoLoginNetResponse(NetResponse response)
    {
        
        if (response.Type == "Login")//登录
        {
            switch (response.Code)
            {
                case 1:
                    //1.提示
                    Debug.Log("登录成功");                  
                    //2.1存账号密码
                    AccountInfo account = new AccountInfo(userName.text, passWord.text);
                    string str_account = JsonUtility.ToJson(account);
                    PlayerPrefs.SetString("Account", str_account);
                    //2.2存记住我的状态
                    PlayerPrefs.SetString("Remember", toggle.isOn.ToString());
                   
                    //3.拿到玩家数据
                    if (response.Data=="")//没数据
                    {
                        UIPanelNotice.Instance.Init("登录成功", delegate ()
                        {
                            uiController.CreatePanel("PanelCreateRole");//进入创建人物界面
                            Destroy(gameObject);
                        });                  
                    }
                    else
                    {
                        //将玩家数据初始化
                        GameController.Instance.playerData = JsonConvert.DeserializeObject
                            <PlayerData>(response.Data);
                        //直接进入主城
                        UIPanelLoad.Instance.Init("MainCity", delegate ()
                        {
                            GameController.Instance.uiController.DestroyAllPanel();//删除所有界面
                            GameController.Instance.uiController.CreatePanel("PanelMainCity");//进入主城
                        });
                                               
                    }
                    break;

                case 0:
                    //UIPanelNotice.Instance.Init("用户名或密码错误!!!");
                    UIPanelTip.Instance.Init("用户名或密码错误!!!");
                    break;
                //case -1:
                //    UIPanelNotice.Instance.Init("密码长度小于3");
                //    break;
            }
            //解除回调的绑定 
            NetClient.Instance.netResponseHandler -= DoLoginNetResponse;
        }
    }

    //点击注册
    public void ClickRegister()
    {
        SoundController.Instance.PlayAudio("audio");
        uiController.OpenResgisterPanel();//打开注册面板
    }
}
