using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanelNotice : MonoBehaviour
{
    private static UIPanelNotice instance;
    public static UIPanelNotice Instance
    {
        get
        {
            if (instance == null)
            {
                Transform panel = GameController.Instance.uiController.CreatePanel("PanelNotice");
                instance = panel.GetComponent<UIPanelNotice>();
            }
            return instance;
        }
    }
    void Awake()
    {
        //1. 拿到引用，绑定事件
        contentText = transform.Find("Content").GetComponent<Text>();
        transform.Find("ButtonOk").GetComponent<Button>().onClick.AddListener(ClickOk);
        transform.Find("ButtonCancel").GetComponent<Button>().onClick.AddListener(ClickCancel);

        instance = this;
    }


    //委托方法的定义
    public delegate void UIPanelNoticeHandler();
    //提示文本
    public Text contentText;
    //声明委托
    public UIPanelNoticeHandler noticeHandler;

    //初始化
    public void Init(string msg, UIPanelNoticeHandler callback = null)
    {
        //拿到委托
        noticeHandler = callback;

        //显示信息的赋值
        contentText.text = msg;
    }
    // 点击事件 确定
    public void ClickOk()
    {
        SoundController.Instance.PlayAudio("audio");
        if (noticeHandler !=null)
        {
            noticeHandler();//执行委托
        }
        Destroy(gameObject);
    }
    // 点击事件 取消
    public void ClickCancel()
    {
        SoundController.Instance.PlayAudio("audio");
        Destroy(gameObject);
    }
}
