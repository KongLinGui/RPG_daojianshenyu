using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

public class UIPanelLevelOver : MonoBehaviour {

   
    public Slider slider;
    public Text sliderText;
    public Text gradeText;
    public Image grade;//等级
    private void Awake()
    {
        //初始化      
        slider = transform.Find("Exp/Slider").GetComponent<Slider>();
        sliderText = transform.Find("Exp/TextSlider").GetComponent<Text>();
        gradeText= transform.Find("Level/Text").GetComponent<Text>();
        grade= transform.Find("Level/Level").GetComponent<Image>();
    }
    public void Init()
    {
        StartCoroutine(LoadMov());//开启加载协程
        // 存档          
        NetClient.Instance.SendRequest(new NetRequest("PlayerData",
            JsonConvert.SerializeObject(GameController.Instance.playerData)));
    }

    //进度条协成
    IEnumerator LoadMov()
    {

        yield return new WaitForSeconds(2f);
        for (int i = 0; i <= 100; i++)
        {

            yield return null;
            GameController.Instance.playerData.AddNowExp(1);
            slider.value = (float)GameController.Instance.playerData.CurrentExp / (float)GameController.Instance.playerData.NeedExp;

            sliderText.text = string.Format("{0:f0}/" + GameController.Instance.playerData.NeedExp, GameController.Instance.playerData.CurrentExp);
            gradeText.text = "LV " + GameController.Instance.playerData.Level;
        }
        if (GameController.Instance.playerData.AddGrade)
        {
            grade.gameObject.SetActive(true);
            GameController.Instance.playerData.AddGrade = false;
            //Destroy(Upgrade.gameObject, 2f);
            StartCoroutine(YinCang());
        }
    }

    //隐藏升级提示
    IEnumerator YinCang()
    {
        yield return new WaitForSeconds(2f);
        grade.gameObject.SetActive(false);
    }
    // Use this for initialization
    void Start ()
    {
        Init();
        //1.绑定返回按钮事件
        transform.Find("ButtonBack").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            GameController.Instance.uiController.DestroyAllPanel();
            UIPanelLoad.Instance.Init("MainCity", delegate ()
            {
                GameController.Instance.uiController.CreatePanel("PanelMainCity");
                
            });
        });
        //2.生成奖励物品

        //2.1 得到奖励物品的数据
        List<Item> rewardList = new List<Item>();
        List<Item> itemDataList = GameController.Instance.gameData.itemDataList;
        int rewardCount = Random.Range(2, 6);//奖励物品个数，随机
        for (int i = 0; i < rewardCount; i++)
        {
            //奖励物品，随机
            Item item = itemDataList[Random.Range(0, itemDataList.Count)].Clone();
            item.Amount = Random.Range(1, item.AmountMax);//奖励物品的数量，随机
            rewardList.Add(item);
        }
        //2.2 显示
        GameObject itemPrefab = transform.Find("ItemPrefab").gameObject;
        for (int i = 0; i < rewardList.Count; i++)
        {
            //生成
            Transform clone = Instantiate(itemPrefab).transform;
            clone.SetParent(transform.Find("Item/Grid"));
            clone.localScale = Vector3.one;
            //显示图片
            clone.Find("Icon").GetComponent<Image>().sprite =
                Resources.Load<Sprite>("Item/" + rewardList[i].Icon);
            //显示数量
            clone.Find("Text").GetComponent<Text>().text = ""+rewardList[i].Amount;

            //绑定事件 
            //改名子 
            clone.name = rewardList[i].Id.ToString();
            UIEventTrigger listener = UIEventTrigger.Get(clone.gameObject);
            listener.onPointerDown += delegate (PointerEventData data)
            {
                //将点击物体的name 转为 物品id，
                if (data.pointerEnter==null)
                {
                    return;
                }
                int id = int.Parse(data.pointerEnter.name);
                //通过id拿到数据
                for (int k = 0; k < rewardList.Count; k++)
                {
                    if (rewardList[k].Id == id)
                    {
                        //数据拿到了 
                        //1 显示 tip 
                        transform.Find("Tip").gameObject.SetActive(true);
                        //设置Tip显示位置
                        transform.Find("Tip").position = data.pointerEnter.transform.position; 
                        //2 显示UI
                        //显示图片
                        transform.Find("Tip/Icon").GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Item/" + rewardList[k].Icon);
                        //显示名称
                        transform.Find("Tip/Name").GetComponent<Text>().text = "" + rewardList[k].Name;
                        //显示描述
                        transform.Find("Tip/Desc").GetComponent<Text>().text = "描述：" + rewardList[k].Desc;

                    }
                }
            };
            listener.onPointerUp += delegate (PointerEventData data)
            {
                transform.Find("Tip").gameObject.SetActive(false);
            };
        }

        //2.3 添加到玩家的背包
        GameController.Instance.playerData.AddItem(rewardList);

        //3. 更新 玩家 关卡解锁进度
        if (GameController.Instance.currentLevel == GameController.Instance.playerData.LevelPlan)
        {
            GameController.Instance.playerData.LevelPlan++;//通关了，解锁进度+1
        }

    }

}
