using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// 背包
/// </summary>
public class UIPanelBag : MonoBehaviour {

    public Transform panel1;
    public Transform panel2;
    public Transform panel3;
   
    public bool isClearUp=false;

    public List<Item> playerItemList;//玩家背包数据

    public Transform role;//角色

    public Text playerName;//玩家姓名
    public int grade;//等级
    public int strength;//战力
   

    // Use this for initialization
    void Awake ()
    {
        //1.给三个标签页绑定事件
        for (int i = 1; i <= 3; i++)
        {
            Toggle toggle = transform.Find("Group/Toggle" + i).GetComponent<Toggle>();
            //标签文字显示效果事件绑定
            toggle.onValueChanged.AddListener(delegate (bool isOn)
            {
                toggle.transform.Find("Background/0/Text").gameObject.SetActive(isOn);
            });
            //面板切换事件绑定
            toggle.onValueChanged.AddListener(delegate (bool isOn)
            {
                toggle.transform.Find("Panel").gameObject.SetActive(isOn);
            });
        }

        //2.给按钮绑定事件 关闭，切换
        transform.Find("ButtonClose").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            SoundController.Instance.PlayAudio("audio");
            Destroy(gameObject);
        });
        transform.Find("ButtonClearUp").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            SoundController.Instance.PlayAudio("audio");
            ClearUp();
        });

        //3.初始化 数据
        panel1 = transform.Find("Group/Toggle1/Panel");
        panel2 = transform.Find("Group/Toggle2/Panel");
        panel3 = transform.Find("Group/Toggle3/Panel");
    

        //拿到人物引用
        role = transform.Find("Camera/tongren");
        //绑定角色旋转事件
        UIEventTrigger.Get(transform.Find("RawImage").gameObject).onPointerDown += RotateRole;
        //绑定角色切换事件
        UIEventTrigger.Get(transform.Find("ButtonBack").gameObject).onPointerDown += CutRole;
        UIEventTrigger.Get(transform.Find("ButtonBack1").gameObject).onPointerDown += CutRole1;
        //绑定显示装备事件
        UIEventTrigger.Get(transform.Find("RawImage/Button").gameObject).onPointerDown += ShowEquip;


        //初始化
        playerName = transform.Find("Role/Name/Text").GetComponent<Text>();
        grade = GameController.Instance.playerData.Level;//获取等级
        strength = GameController.Instance.playerData.Strength;//获取战力
       
        
    }
   

    private void Start()
    {
        //Test();//测试代码 作用：添加数据

        playerItemList = GameController.Instance.playerData.ItemList;
        Init();

        playerName.text = GameController.Instance.playerData.Name;//获取名字
        transform.Find("Role/HeadPortrait/Grade").GetComponent<Text>().text = "" + grade;
        strength = grade * 20 + GameController.Instance.playerData.LevelPlan * 30;
        transform.Find("Role/Text/Image/Strength").GetComponent<Text>().text = "" + strength;
    }

    public void Init()
    {
        //1.给三个面板生成20个格子
        InitPanelGrid(panel1, ItemType.Equip);
        InitPanelGrid(panel2, ItemType.Expendable);
        InitPanelGrid(panel3, ItemType.Material);
    }


    //控制角色旋转
    public void RotateRole(PointerEventData data)
    {
        StartCoroutine(RotateRoleMov());//开启角色旋转协成
    }
    IEnumerator RotateRoleMov()//角色旋转协成
    {
        Vector3 lastMouse = Vector3.zero;//上一帧的鼠标坐标
        while (Input.GetMouseButton(0))//按住鼠标左键
        {
            if (lastMouse != Vector3.zero)
            {
                //当前 - 上 = 这一帧的偏移向量
                Vector3 offset = Input.mousePosition - lastMouse;
                //人物旋转
                Vector3 rotateAngle = new Vector3(0, offset.x * 0.5f, 0);
                role.Rotate(-rotateAngle);
                //人物变大
                //role.position +=new Vector3(0, 0, offset.y) * Time.deltaTime;
            }
            lastMouse = Input.mousePosition;//赋值
            yield return null;
        }
    }

    public void CutRole(PointerEventData data)//人物切换
    {
        SoundController.Instance.PlayAudio("audio");
        transform.Find("ButtonBack1").gameObject.SetActive(true);
        transform.Find("ButtonBack").gameObject.SetActive(false);
        transform.Find("Role").gameObject.SetActive(true);
        transform.Find("RawImage").gameObject.SetActive(false);
    }
    public void CutRole1(PointerEventData data)//人物切换
    {
        SoundController.Instance.PlayAudio("audio");
        transform.Find("ButtonBack").gameObject.SetActive(true);
        transform.Find("ButtonBack1").gameObject.SetActive(false);
        transform.Find("Role").gameObject.SetActive(false);
        transform.Find("RawImage").gameObject.SetActive(true);
    }

    public void ShowEquip(PointerEventData data)//显示装备
    {
        SoundController.Instance.PlayAudio("audio");
        transform.Find("RawImage/ImageToukui/Image").gameObject.SetActive(true);//头盔
        transform.Find("RawImage/ImageToukui/Text").gameObject.SetActive(false);

        transform.Find("RawImage/ImageWuQi/Image").gameObject.SetActive(true);//武器
        transform.Find("RawImage/ImageWuQi/Text").gameObject.SetActive(false);

        transform.Find("RawImage/ImageXueZi/Image").gameObject.SetActive(true);//靴子
        transform.Find("RawImage/ImageXueZi/Text").gameObject.SetActive(false);

        transform.Find("RawImage/ImageZuoQi/Image").gameObject.SetActive(true);//坐骑
        transform.Find("RawImage/ImageZuoQi/Text").gameObject.SetActive(false);
    }

    public void Test()
    {
        GameController.Instance.CreateNewPlayer("小明");
        //2.1 得到奖励物品的数据
        List<Item> rewardList = new List<Item>();
        List<Item> itemDataList = GameController.Instance.gameData.itemDataList;
        int rewardCount = 10;// Random.Range(10,20);//奖励物品个数，随机
        for (int i = 0; i < rewardCount; i++)
        {
            //奖励物品，随机
            Item item = itemDataList[Random.Range(0, itemDataList.Count)].Clone();
            item.Amount = Random.Range(1, item.AmountMax);//奖励物品的数量，随机
            rewardList.Add(item);
        }
        GameController.Instance.playerData.ItemList.AddRange(rewardList);
    }

   
    public float PressTime;
    public Coroutine TimeCor;
    Dictionary<Transform, Item> ItemList = new Dictionary<Transform, Item>();

    /// <summary>
    /// 初始化面板 
    /// </summary>
    /// <param name="panel">面板</param>
    /// <param name="type">道具类型</param>
    public void InitPanelGrid(Transform panel,ItemType type)
    {
        //1.生成20个格子
        GameObject itemPrefab = transform.Find("ItemPrefab").gameObject;
        for (int i = 0; i < 20; i++)
        {
            //生成格子
            Transform clone = Instantiate(itemPrefab).transform;
            clone.SetParent(panel.Find("Grid"));
            clone.Find("Icon").gameObject.SetActive(false);
        }
        //2.将玩家背包数据，分别显示到不同面板中

        //2.1 将符合type的物品拿出来，放到一个list中
        List<Item> list = new List<Item>();
       

        for (int i = 0; i < playerItemList.Count; i++)
        {
            if (playerItemList[i].Type == type)
            {
                list.Add(playerItemList[i]);
            }
        }
        //2.2 显示UI
        for (int i = 0; i < list.Count; i++)
        {
            //生成
            Transform clone = panel.Find("Grid").GetChild(i);
            
            //显示图片
            clone.Find("Icon").GetComponent<Image>().sprite =
                Resources.Load<Sprite>("Item/" + list[i].Icon);
            clone.Find("Icon").gameObject.SetActive(true);
            //显示数量
            clone.Find("Count").GetComponent<Text>().text = "" + list[i].Amount;


            //改名称
            clone.name = list[i].Id.ToString();//把格子名称更换为链表中相应数据的ID字段
            ItemList.Add(clone, list[i]);
            UIEventTrigger listener = UIEventTrigger.Get(clone.gameObject);//点击事件
            
            listener.onPointerDown += delegate (PointerEventData data)//弹提示的按压时间
            {
                //将点击物体的name 转为 物品id，
                if (data.pointerEnter == null)
                {
                    return;
                }
                PointerDown();
                int id = int.Parse(data.pointerEnter.name);
                //通过id拿到数据
                for (int k = 0; k < list.Count; k++)
                {
                    if (list[k].Id == id)
                    {
                        //数据拿到了 
                        //1 显示 tip 
                        transform.Find("Tip").gameObject.SetActive(true);
                        //设置Tip显示位置
                        Vector3 pos = data.pointerEnter.transform.position;
                        if (pos.y>transform.Find("TipClamp").position.y)
                        {
                            pos.y = transform.Find("TipClamp").position.y;
                        }
                        transform.Find("Tip").position = pos;
                        //2 显示UI
                        //显示图片
                        transform.Find("Tip/Icon").GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Item/" + list[k].Icon);
                        //显示名称
                        transform.Find("Tip/Name").GetComponent<Text>().text = "" + list[k].Name;
                        //显示描述
                        transform.Find("Tip/Desc").GetComponent<Text>().text = "描述：" + list[k].Desc;

                    }
                }
            };
            listener.onPointerUp += delegate (PointerEventData data)//判断松开后提示框消失
            {
                transform.Find("Tip").gameObject.SetActive(false);                
            };
            listener.onPointerClick += delegate (PointerEventData data)
              {
                  if (data.pointerEnter == null)
                  {
                      return;
                  }
                  Transform date = data.pointerEnter.transform;                 
                  StopCoroutine(TimeCor);//关停 按压计时协成
                  if (PressTime < 0.1f)//鼠标抬起拿到鼠标按压的时间并判断
                  {
                      UIPanelTip.Instance.Init("使用物品");
                      int count = int.Parse(date.Find("Count").GetComponent<Text>().text);//把 转换为int类型
                      if (count > 0)
                      {
                          ItemList[date].Amount--;
                          count--;
                          if (count > 0)
                          {
                              date.Find("Count").GetComponent<Text>().text = count.ToString();
                          }
                          else
                          {
                              date.Find("Icon").gameObject.SetActive(false);
                              date.Find("Count").gameObject.SetActive(false);
                              Destroy(date.GetComponent<UIEventTrigger>());
                              //字典删除相应的物品数据
                              playerItemList.Remove(ItemList[date]);
                          }
                      }
                      GameController.Instance.playerData.ItemList = playerItemList;
                      OutInit();
                      StartCoroutine(UpdataItem());

                      //出现是否使用点击物品的提示框
                      //UIPanelNotice.Instance.Init("是否使用该物体", delegate ()
                      //{
                      //    Debug.Log(date.name);
                      //    int count = int.Parse(date.Find("Count").GetComponent<Text>().text);//把 转换为int类型
                      //    if (count > 0)
                      //    {
                      //        ItemList[date].Amount--;
                      //        count--;
                      //        if (count > 0)
                      //        {
                      //            date.Find("Count").GetComponent<Text>().text = count.ToString();
                      //        }
                      //        else
                      //        {
                      //            date.Find("Icon").gameObject.SetActive(false);
                      //            date.Find("Count").gameObject.SetActive(false);
                      //            Destroy(date.GetComponent<UIEventTrigger>());
                      //            //字典删除相应的物品数据
                      //            playerItemList.Remove(ItemList[date]);
                      //        }
                      //    }
                      //    GameController.Instance.playerData.ItemList = playerItemList;
                      //    OutInit();
                      //    StartCoroutine(UpdataItem());
                      //});  
                  }
                  if (PressTime >= 0.1f)
                  {
                      //物品属性展示面板隐藏
                      Debug.LogError("物品属性展示面板");
                  }
              };
            
        }
        //2.3 显示数量
        panel.Find("Image/Text").GetComponent<Text>().text = list.Count + "/20";
    }
    
    public void OutInit()
    {
        Destroygrid(panel1);
        Destroygrid(panel2);
        Destroygrid(panel3);
    }
    IEnumerator UpdataItem()
    {
        yield return null;
        Init();
    }
    public void Destroygrid(Transform panel)
    {
        int a = panel.Find("Grid").childCount;
        for (int i = a-1; i >=0; i--)
        {
            Destroy(panel.Find("Grid").GetChild(i).gameObject);
        }
    }
    //整理物品栏
    public void ClearUp()
    {
        SoundController.Instance.PlayAudio("audio");
        OutInit();
        GameController.Instance.playerData.BagClearUp();
        StartCoroutine(UpdataItem());
    }
   

    public void PointerDown()
    {
        print(PressTime);
        TimeCor = StartCoroutine(pressTime());
    }
   
    IEnumerator pressTime()
    {
        PressTime = 0f;

        while (true)
        {
            PressTime += Time.deltaTime;
            if (PressTime >= 0.3f)
            {
                //出现物品属性展示面板
                transform.Find("Tip").gameObject.SetActive(true);
            }
            yield return null;
        }
    }
}
