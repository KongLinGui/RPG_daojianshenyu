using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

public class UIPanelMainCity : MonoBehaviour
{

    public Text playerName;//玩家姓名
    public int grade;//等级
    public int strength;//战力   

    //聊天功能
    public InputField inputField;//聊天室输入框
    public Text contentText;//聊天室文本  
    public Coroutine netClientCor;//协同程序
    public ScrollRect scrollRect;//控制滑动条
    public string Name;//用户姓名

    private void Awake()
    {
        //初始化
        playerName = transform.Find("Picture frame/Name").GetComponent<Text>();
        grade = GameController.Instance.playerData.Level;//获取等级
        strength = GameController.Instance.playerData.Strength;//获取战力

        print(FindObjectOfType<MainCityCameraController>().name);
        Transform misc = GameObject.Find("Misc").transform;
        
        //1.绑定事件
        transform.Find("ButtonLevel").GetComponent<Button>().onClick.AddListener(ClickOpenLevelPanel);
        transform.Find("ButtonBag").GetComponent<Button>().onClick.AddListener(ClickOpenBagPanel);
        transform.Find("ButtonQuest").GetComponent<Button>().onClick.AddListener(ClickOpenQuestPanel);       
        transform.Find("ButtonStore").GetComponent<Button>().onClick.AddListener(ClickStorePlayer);//点击存储
        transform.Find("ButtonQuit").GetComponent<Button>().onClick.AddListener(GameExit);//退出

        //2.设置目标
        transform.Find("ButtonLevel").GetComponent<UIFollow>().SetTarget(misc.Find("Level"));//关卡
        transform.Find("ButtonQuest").GetComponent<UIFollow>().SetTarget(misc.Find("Quest"));//任务
        transform.Find("ButtonBag").GetComponent<UIFollow>().SetTarget(misc.Find("Bag"));//背包
        transform.Find("ButtonRanking List").GetComponent<UIFollow>().SetTarget(misc.Find("Ranking List"));//排行榜

        //聊天功能
        contentText = transform.Find("Panel/Scroll View/Viewport/Content").GetComponent<Text>();
        inputField = transform.Find("Panel/InputField").GetComponent<InputField>();
        scrollRect = transform.Find("Panel/Scroll View").GetComponent<ScrollRect>();

        //点击进入排行榜界面
        transform.Find("ButtonRanking List").GetComponent<Button>().onClick.AddListener(ClickOpenRankingListPanel);
        //喇叭
        transform.Find("Panel/Button").GetComponent<Button>().onClick.AddListener(ClickUseHorn);
    }

    void Start()
    {
        playerName.text = GameController.Instance.playerData.Name;//获取名字
        transform.Find("Picture frame/Name/Grade").GetComponent<Text>().text = "等级：" + grade;
        strength = grade * 20 + GameController.Instance.playerData.LevelPlan * 30;
        transform.Find("Picture frame/Strength").GetComponent<Text>().text = "" + strength;
        Name = playerName.text;
        StartCoroutine(ConnectMov());
    }

    //连接服务器
    IEnumerator ConnectMov()
    {
       
        yield return null;
               
        //绑定事件
        NetClient.Instance.netResponseHandler += DoContentTextNetResponse;// 处理登录逻辑
        //开启协程
        netClientCor = StartCoroutine(NetClient.Instance.NetResponseMov());
        NetClient.Instance.SendRequest(new NetRequest("Chat", string.Format("---欢迎<color=#C42424FF>{0}</color>进入聊天室---",
           GameController.Instance.playerData.Name)));
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) &&
            EventSystem.current.currentSelectedGameObject == inputField.gameObject)
        {
            if (inputField.text != "")
            {
                NetClient.Instance.SendRequest(new NetRequest("Chat", Name + "：" + inputField.text));
                inputField.text = "";
            }
            inputField.ActivateInputField();
        }
    }

    /// <summary>
    /// 更新文本
    /// </summary>
    /// <param name="message">文本内容</param>
    public void UpdateContentText(string message)
    {
        contentText.text += message;
        contentText.text += "\n";//换行
        scrollRect.verticalNormalizedPosition = 0;
    }
   
    /// <summary>
    /// 处理在文本框逻辑中的回应的委托方法
    /// </summary>
    /// <param name="response"></param>
    public void DoContentTextNetResponse(NetResponse response)
    {
        Debug.Log("1");
        isOk = true;
        string message = response.Data;
        
        //if (response.Type == "In")
        //{
        //    Debug.Log(response.Data);
        //    message = string.Format("欢迎<color=#C42424FF>{0}</color>进入聊天室", response.Data);
        //}
        //else if (response.Type == "Out")
        //{
        //    Debug.Log(response.Data);
        //    message = string.Format("{0}退出聊天室", response.Data);
        //}
        if (response.Type== "UseHorn")
        {
            Debug.Log("2");
            GameObject clone = Resources.Load<GameObject>("UI/HornMask");
            Transform clone1 = Instantiate(clone).transform;
            clone1.transform.SetParent(GameObject.Find("Canvas").transform);
            clone1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
            clone1.transform.Find("Text").GetComponent<Text>().text = message;
            float a = clone1.GetComponent<RectTransform>().sizeDelta.x;
            //开下面的协成
            StartCoroutine(GetTextWidthMov(clone1.Find("Text").GetComponent<RectTransform>()));
        }
        else if (response .Type == "Chat")
            UpdateContentText(message);

    }
    bool isOk = false;
    IEnumerator GetTextWidthMov(RectTransform textRect)
    {
        yield return null;
        //判断文字的长度是多少
        Debug.Log("textRect.anchoredPosition.x=" + textRect.anchoredPosition.x);
        textRect.GetComponent<RectTransform>().anchoredPosition = new Vector2(200+textRect.sizeDelta.x/2,0);
        while (textRect.anchoredPosition.x>(-300f-textRect.sizeDelta.x/2))
        {
            yield return null;
        }
        isOk = false;
        Destroy(textRect.parent.gameObject,2f);
    }


    //关卡界面
    public void ClickOpenLevelPanel()
    {
        SoundController.Instance.PlayAudio("audio");
        //Debug.Log("ButtonLevelButtonLevelButtonLevel");
        GameController.Instance.uiController.CreatePanel("PanelLevel");
    }
    //背包界面
    public void ClickOpenBagPanel()
    {
        SoundController.Instance.PlayAudio("audio");
        //Debug.Log("ButtonLevelButtonLevelButtonLevel");
        GameController.Instance.uiController.CreatePanel("PanelBag");
    }
    //任务界面
    public void ClickOpenQuestPanel()
    {
        SoundController.Instance.PlayAudio("audio");
        //Debug.Log("ButtonLevelButtonLevelButtonLevel");
        GameController.Instance.uiController.CreatePanel("PanelQuest");
    }
    //排行榜界面
    public void ClickOpenRankingListPanel()
    {
        SoundController.Instance.PlayAudio("audio");
        GameController.Instance.uiController.CreatePanel("PanelPower");
    }
    //喇叭
    public void ClickUseHorn()
    {
        NetClient.Instance.SendRequest(new NetRequest("UseHorn", GameController.Instance.playerData.Name+":"+ inputField.text));
    }
    //存储数据
    public void ClickStorePlayer()
    {
        SoundController.Instance.PlayAudio("audio");
        UIPanelNotice.Instance.Init("是否存储数据", delegate () {
            // 存档          
            NetClient.Instance.SendRequest(new NetRequest("PlayerData", 
                JsonConvert.SerializeObject(GameController.Instance.playerData)));
        });
    }
    //退出
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//将当前电脑平台运行状态关闭
#else
         Application.Quit();//该方法是直接退出移动端状态           
#endif
    }
    void OnDestroy()
    {
        NetClient.Instance.SendRequest(new NetRequest("Chat", string.Format("---欢迎<color=#C42424FF>{0}</color>退出聊天室---",
           GameController.Instance.playerData.Name)));
        NetClient.Instance.netResponseHandler -= DoContentTextNetResponse;// 处理登录逻辑
    }
}
