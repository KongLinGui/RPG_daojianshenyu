using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
/// <summary>
/// 选择关卡
/// </summary>
public class UIPanelLevel : MonoBehaviour
{
    public GameObject buttonPrefab;//按钮模版
    public Transform contentGrid;//按钮生成的地方
    public ScrollRect scrollRect;

    //public float scrollRectHorizontalNormalizedPosition;

    public bool IsStartCor = false;

    void Start()
    {
        Init();        
    }

    void Update()
    {
        if (IsStartCor == true)
        {
            return;
        }
        if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - 0f) < 0.02f)
        {
            transform.Find("Group/Toggle1").GetComponent<Toggle>().isOn = true;
        }
        if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - 0.5f) < 0.02f)
        {
            transform.Find("Group/Toggle2").GetComponent<Toggle>().isOn = true;
        }
        if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - 1f) < 0.02f)
        {
            transform.Find("Group/Toggle3").GetComponent<Toggle>().isOn = true;
        }


    }


    public void Init()
    {
        scrollRect = transform.Find("Scroll View").GetComponent<ScrollRect>();
        //scrollRect.horizontalNormalizedPosition   
        //float 范围 0 - 1

        //给toggle绑定事件

        for (int i = 1; i <= 3; i++)
        {
            UIEventTrigger listener = UIEventTrigger.Get(transform.Find("Group/Toggle" + i).gameObject);
            listener.onPointerClick += delegate (PointerEventData data)
            {           
                if (data.pointerEnter.transform.parent.parent.GetComponent<Toggle>().isOn)
                {
                    //return;
                }
                //Debug.Log(data.pointerEnter.transform.parent.parent.name);
                float endValue = float.Parse(data.pointerEnter.name);
                StopAllCoroutines();
                StartCoroutine(ScrollMov(endValue));
            };
        }


        // 之前代码
        //transform.Find("Group/Toggle3").GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn)
        //{
        //    if (isOn == true)
        //    {
        //        StopAllCoroutines();
        //        StartCoroutine(ScrollMov(1f));
        //    }

        //});

        buttonPrefab = transform.Find("Button").gameObject;
        contentGrid = transform.Find("Scroll View/Viewport/Content");

        //1.生成 24个按钮
        for (int i = 1; i <= 24; i++)
        {
            Transform buttonClone = Instantiate(buttonPrefab).transform;
            buttonClone.SetParent(contentGrid);
            buttonClone.localScale = Vector3.one;
            buttonClone.name = i.ToString();//改名字
            //2.根据解锁情况，修改按钮的显示内容
            if (i > GameController.Instance.playerData.LevelPlan) //未解锁
            {
                buttonClone.Find("Button").gameObject.SetActive(false);
                buttonClone.Find("Button_Lock").gameObject.SetActive(true);
                //3.给按钮绑定事件
                buttonClone.Find("Button_Lock").GetComponent<Button>().onClick.AddListener(ClickLockButton);
            }
            else//解锁
            {
                buttonClone.Find("Button").gameObject.SetActive(true);
                buttonClone.Find("Button/Text").GetComponent<Text>().text = "" + i;
                buttonClone.Find("Button_Lock").gameObject.SetActive(false);
                //3.给按钮绑定事件
                //手动绑定
            }
        }
    }

    public void ClickUnLockButton(GameObject button) //点击解锁button
    {
        SoundController.Instance.PlayAudio("audio");
        string str = "是否进入关卡" + button.name;
        int index = int.Parse(button.name);
        if (index%3==0)
        {
            UIPanelNotice.Instance.Init(str, delegate ()
            {
                //加载场景 关卡
                UIPanelLoad.Instance.Init("Level0", delegate ()
                {
                    GameController.Instance.InitLevel(index);
                });
            });
        }
        else if(index % 3 == 1)
        {
            UIPanelNotice.Instance.Init(str, delegate ()
            {
                //加载场景 关卡
                UIPanelLoad.Instance.Init("Level1", delegate ()
                {
                    GameController.Instance.InitLevel(index);
                });
            });
        }
        else if (index % 3 == 2)
        {
            UIPanelNotice.Instance.Init(str, delegate ()
            {
                //加载场景 关卡
                UIPanelLoad.Instance.Init("Level2", delegate ()
                {
                    GameController.Instance.InitLevel(index);
                });
            });
        }

    }
    public void ClickLockButton() //点击未解锁button
    {
        SoundController.Instance.PlayAudio("audio");
        UIPanelNotice.Instance.Init("该关卡未解锁，请通关之前关卡！");
    }


    //开启协程
    IEnumerator ScrollMov(float endValue)
    {
        IsStartCor = true;
        float between = Mathf.Abs( scrollRect.horizontalNormalizedPosition - endValue);
        while (between > 0.02f) 
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, endValue, 5f * Time.deltaTime);
            yield return null;
            between = Mathf.Abs(scrollRect.horizontalNormalizedPosition - endValue);
        }
        IsStartCor = false;
    }

    public void ClickClosePanel()
    {
        SoundController.Instance.PlayAudio("audio");
        Destroy(gameObject);
    }

}
