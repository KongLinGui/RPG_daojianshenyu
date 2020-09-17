using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// UI注册界面
/// </summary>
public class UIPanelRegister : MonoBehaviour {

    public UIController uiController;
    //按钮 注册按钮 返回按钮
    public Button buttonOk, buttonBack;
    //输入框账号 密码 确认密码
    public InputField userName, passWord, againPassWord;

    private void Awake()
    {
        //获取参与代码逻辑的组件
        buttonBack = transform.Find("ButtonBack").GetComponent<Button>();
        buttonOk = transform.Find("ButtonOk").GetComponent<Button>();
        buttonOk.onClick.AddListener(ClickRegister);//绑定注册按钮
        buttonBack.onClick.AddListener(ClickBackLogin);//绑定返回按钮

        userName = transform.Find("UserName/InputField").GetComponent<InputField>();
        passWord = transform.Find("PassWord/InputField").GetComponent<InputField>();
        againPassWord = transform.Find("AgainPassWord/InputField").GetComponent<InputField>();
      
    }
    // Use this for initialization
    void Start()
    {
        Init();
    }

    public void Init()
    {
        uiController = GameController.Instance.uiController;//精简代码
    }

    //注册方法
    public void ClickRegister()
    {
        SoundController.Instance.PlayAudio("audio");
        //if (PlayerPrefs.GetString("accountInfoList", "") != "")
        //{
        //    uiController.accountInfoList = JsonConvert.DeserializeObject<List<AccountInfo>>(PlayerPrefs.
        //        GetString("accountInfoList", ""));
        //}
        ////1.判断是否为空
        //if (userName.text == "" || passWord.text == "" || againPassWord.text == "")
        //{
        //    //提示
        //    Debug.Log("不能为空");
        //    UIPanelNotice.Instance.Init("不能为空");
        //    return;
        //}
        ////2.判断账号重复
        //for (int i = 0; i < uiController.accountInfoList.Count; i++)
        //{
        //    if (userName.text == uiController.accountInfoList[i].Username)
        //    {
        //        //提示
        //        Debug.Log("账号重复");
        //        UIPanelNotice.Instance.Init("账号重复");
        //        return;
        //    }            
        //}
        ////3.两次密码是否一致
        //if (passWord.text != againPassWord.text)
        //{
        //    Debug.Log("密码不一致");
        //    UIPanelNotice.Instance.Init("密码不一致");
        //    return;
        //}
        //else
        //{
        //    UIPanelNotice.Instance.Init("注册成功!");
        //    //5.直接跳转
        //    ClickBackLogin();
        //}
        ////4.存入
        //if (passWord.text == againPassWord.text)
        //{
        //    UIPanelNotice.Instance.Init("注册成功!");

        //    //存在数组里
        //    uiController.accountInfoList.Add(new AccountInfo(userName.text, passWord.text));
        //    //把链表转换成字符串
        //    string str = JsonConvert.SerializeObject(uiController.accountInfoList);
        //    //将转换的字符串存在平台上
        //    NetClient.Instance.SendRequest(new NetRequest("Register", str));
        //    //PlayerPrefs.SetString("accountInfoList", str);

        //}
        ////5.直接跳转
        //ClickBackLogin();

        // 2.0 绑定回调事件
        NetClient.Instance.netResponseHandler += DoRegisterNetResponse;
        // 2.1 发送数据     
        AccountInfo account = new AccountInfo(userName.text, passWord.text);
        string str_account = JsonUtility.ToJson(account);
        NetClient.Instance.SendRequest(new NetRequest("Register", str_account));
       
    }
    /// <summary>
    /// 处理注册的回调信息
    /// </summary>
    public void DoRegisterNetResponse(NetResponse response)
    {
        //Debug.Log("12111111");
        if (response.Type == "Register")
        {
            switch (response.Code)
            {
                case 1:
                    //1.提示                 
                    //UIPanelNotice.Instance.Init("注册成功!");
                    //Debug.Log("123123123123");
                    UIPanelTip.Instance.Init("注册成功!");
                    break;
                case 0:
                    //UIPanelNotice.Instance.Init("用户名已存在！");
                    UIPanelTip.Instance.Init("用户名已存在！");
                    userName.text = "";
                    passWord.text = "";
                    againPassWord.text = "";
                    break;
                case -1:
                    //UIPanelNotice.Instance.Init("密码小于3位啦！");
                    UIPanelTip.Instance.Init("密码小于3位啦！");
                    break;
            }
        }
    }

    //返回登录界面
    public void ClickBackLogin()
    {
        SoundController.Instance.PlayAudio("audio");
        //1.清空信息
        Destroy(gameObject);
        //passWord.text = "";
        //againPassWord.text = "";
    }
}
