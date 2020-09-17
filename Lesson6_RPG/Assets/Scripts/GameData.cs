using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//物品类别 
public enum ItemType
{
    //装备 消耗品 材料 
    Equip, Expendable, Material
}

// 物品类 
public class Item
{
    // ID
    public int Id;
    // 名字
    public string Name;
    //类别 
    public ItemType Type;
    //图标信息 
    public string Icon;
    //数量
    public int Amount;
    //最大数量
    public int AmountMax;
    //价值 
    public int Value;
    //描述 
    public string Desc;

    public Item() { }

    public Item(int id, string name, ItemType type, string icon, int amountmax, int value, string desc)
    {
        this.Id = id;
        this.Name = name;
        this.Type = type;
        this.Icon = icon;
        this.AmountMax = amountmax;
        this.Value = value;
        this.Desc = desc;
    }


    // 克隆！！！
    public Item Clone()
    {
        Item newItem = new Item
            (this.Id, this.Name, this.Type, this.Icon, this.AmountMax, this.Value, this.Desc);

        return newItem;
    }

}

//玩家类
public class PlayerData//玩家数据
{
    //名字
    public string Name;   
    //背包
    public List<Item> ItemList;
    //关卡解锁进度
    public int LevelPlan;
    //任务
    public List<Quest> QuestList;
    //技能
    public List<Skill> SkillList;
    //等级
    public int Level;
    //当前经验
    public int CurrentExp;
    //升级经验
    public int NeedExp;
    //战力
    public int Strength;

    public bool AddGrade = false;
    public void AddNowExp(int addExp)
    {

        CurrentExp += addExp;                      
        if (CurrentExp >= NeedExp)
        {
            AddGrade = true;
            NeedExp += 100;
            CurrentExp = 0;
            Level += 1;
        }

    }

    public PlayerData(string name)
    {
        NeedExp = CurrentExp+Level*10;
        CurrentExp = 0;
        Level = 1;
        Strength = Level * 20 + LevelPlan * 30;
        Name = name; //拿到名字       
        ItemList = new List<Item>();
        LevelPlan = 1;
        QuestList = new List<Quest>();
        SkillList = new List<Skill>();
       
    }

    /// <summary>
    /// 背包整理
    /// </summary>
    public void BagClearUp()
    {
        //整理代码 
        for (int i = 0; i < ItemList.Count; i++)
        {
            for (int j = i + 1; j < ItemList.Count; j++)
            {
                //找到同类了
                if (ItemList[j].Id == ItemList[i].Id)
                {
                    //没满
                    if (ItemList[i].Amount < ItemList[i].AmountMax)
                    {
                        //把J的加到I上去            
                        ItemList[i].Amount += ItemList[j].Amount;
                        //溢出了
                        if (ItemList[i].Amount > ItemList[i].AmountMax)
                        {
                            int yu = ItemList[i].Amount - ItemList[i].AmountMax;
                            ItemList[j].Amount = yu;
                            ItemList[i].Amount = ItemList[i].AmountMax;
                            break;
                        }
                        else
                        {
                            ItemList[j].Amount = 0;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        //删除其中数量为0的元素
        for (int i = ItemList.Count - 1; i > 0; i--)
        {
            if (ItemList[i].Amount == 0)
            {
                ItemList.RemoveAt(i);
            }
        }


    }

    /// <summary>
    /// 给玩家背包添加数据,限制背包上限
    /// </summary>
    public void AddItem(List<Item> list)
    {
        //1.将全部数据添加到尾部  
        ItemList.AddRange(list);
        //2.整理 
        BagClearUp();
        //3.定义三个变量，记录不同类型道具的数量，超出20的删除
        int equipCount = 0;//装备
        int materialCount = 0;//材料
        int eatCount = 0;//消耗品（吃的）
        bool isChao = false;
        for (int i = 0; i < ItemList.Count; i++)
        {
            switch (ItemList[i].Type)
            {
                case ItemType.Equip:
                    if (equipCount >= 20)
                    {
                        isChao = true;
                        ItemList[i].Amount = 0;
                    }
                    else { equipCount++; }
                    break;
                case ItemType.Expendable:
                    if (eatCount >= 20)
                    {
                        isChao = true;
                        ItemList[i].Amount = 0;
                    }
                    else { eatCount++; }
                    break;
                case ItemType.Material:
                    if (materialCount >= 20)
                    {
                        isChao = true;
                        ItemList[i].Amount = 0;
                    }
                    else { materialCount++; }
                    break;
            }
        }

        //3.删除数量为0的道具
        for (int i = ItemList.Count-1; i>=0; i--)
        {
            if (ItemList[i].Amount == 0)
            {
                ItemList.RemoveAt(i);
            }
        }
        //4.超出上限，提示
        if (isChao)
        {
            UIPanelTip.Instance.Init("背包已满！快整理");
        }
        //使用冒泡排序，将list 按id 从小到大排序
        for (int i = 0; i < ItemList.Count - 1; i++)
        {
            for (int j = 0; j < ItemList.Count - 1 - i; j++)
            {
                if (ItemList[j].Id > ItemList[j + 1].Id)
                {
                    Item temp = ItemList[j];
                    ItemList[j] = ItemList[j + 1];
                    ItemList[j + 1] = temp;
                }
            }
        }
    }

    /// <summary>
    /// 更新任务进度（收集，探索）
    /// </summary>
    public void UpdateQuestPlane()
    {
        for (int i = 0; i < QuestList.Count; i++)
        {
            if (QuestList[i].Type == QuestType.Collection)//收集
            {
                int count = 0;
                for (int j = 0; j < ItemList.Count; j++)
                {
                    if (ItemList[j].Id == QuestList[i].ContentId)
                    {
                        count++;
                    }
                }
                QuestList[i].CurrentProgress = count;
            }
            else if (QuestList[i].Type == QuestType.Explore)//探索
            {
                if ((LevelPlan-1)>= QuestList[i].ContentId)
                {
                    QuestList[i].CurrentProgress = 1;
                }
                
            }
        }
    }
}

//任务类型  
public enum QuestType
{
    //击杀 收集 寻人 探索 
    Kill, Collection, SearchNPC, Explore
}
//任务类
public class Quest
{
    //ID 
    public int Id;
    //名字
    public string Name;
    //类型 
    public QuestType Type;
    //描述 
    public string Desc;
    //当前进度
    public int CurrentProgress;
    //总进度
    public int TotalProgress;
    //内容Id
    public int ContentId;
    //奖励 
    public string RewardMessage;
    //解锁任务 id 
    public int PreId;

    public Quest() { }

    public Quest(int id, string name, QuestType type, string desc, int totalProgress, int contentId, string rewardMessage, int preId)
    {
        this.Id = id;
        this.Name = name;
        this.Type = type;
        this.Desc = desc;
        this.TotalProgress = totalProgress;
        this.ContentId = contentId;
        this.RewardMessage = rewardMessage;
        this.PreId = preId;
    }


    // 克隆！！！
    public Quest Clone()
    {
        Quest newQuest = new Quest
            (this.Id, this.Name, this.Type, this.Desc, this.TotalProgress, this.ContentId, this.RewardMessage, this.PreId);

        return newQuest;
    }
}


public class GameData : MonoBehaviour
{

    public List<Item> itemDataList = new List<Item>();//原始物品数据
    public List<Quest> questDataList = new List<Quest>();//原始任务数据

    public List<Skill> skillDataList = new List<Skill>();//原始技能数据

    //原始数据的生成
    public void DataInit()
    {
        //消耗品
        Item item1 = new Item
            (1001, "大还丹", ItemType.Expendable, "27000003", 99, 50, "困了饿了，就吃大还丹！！");
        Item item2 = new Item
            (1002, "净化药水", ItemType.Expendable, "27010005", 99, 20, "这是一个能帮助你脱离低级趣味的药水");
        //材料
        Item item3 = new Item
            (1003, "鸟蛋", ItemType.Material, "27020000", 1, 1000, "这不是一般的鸟蛋，这是。。你猜？");
        Item item4 = new Item
            (1004, "宝瓶", ItemType.Material, "27000000", 5, 1000, "这里面装的是什么呢？。。。");
        //装备 
        Item item5 = new Item
            (1005, "印度飞毯", ItemType.Equip, "27000001", 5, 9999, "能飞的毯子，你怕不怕？");
        Item item6 = new Item
           (1006, "屠龙宝刀", ItemType.Equip, "27000002", 5, 9999, "屠龙宝刀，点击就送！");


        itemDataList.Add(item1);
        itemDataList.Add(item2);
        itemDataList.Add(item3);
        itemDataList.Add(item4);
        itemDataList.Add(item5);
        itemDataList.Add(item6);

        //练习 使用冒泡排序 按照 价值 -> 最大数量 -> id
        // 
        /*
        for (int i = 0; i < itemDataList.Count-1; i++)
        {
            for (int j = 0; j < itemDataList.Count - 1 -i; j++)
            {
                if (itemDataList[j].Value < itemDataList[j+1].Value)
                {
                    //交换
                    Item temp = itemDataList[j];
                    itemDataList[j] = itemDataList[j + 1];
                    itemDataList[j + 1] = temp;
                }
                else if (itemDataList[j].Value == itemDataList[j + 1].Value)
                {
                    if (itemDataList[j].AmountMax < itemDataList[j + 1].AmountMax)
                    {
                        //交换
                        Item temp = itemDataList[j];
                        itemDataList[j] = itemDataList[j + 1];
                        itemDataList[j + 1] = temp;
                    }
                    else if (itemDataList[j].AmountMax == itemDataList[j + 1].AmountMax)
                    {
                        if (itemDataList[j].Id < itemDataList[j + 1].Id)
                        {
                            //交换
                            Item temp = itemDataList[j];
                            itemDataList[j] = itemDataList[j + 1];
                            itemDataList[j + 1] = temp;
                        }
                    }
                }
            }
        }

        //屠龙宝刀->印度飞毯->宝瓶->鸟蛋->大还丹->净化药水
        for (int i = 0; i < itemDataList.Count; i++)
        {
            Debug.Log(itemDataList[i].Name);
        }
        */



        //任务数据生成
        //击杀任务 
        Quest quest1 = new Quest(2001, "击杀", QuestType.Kill, "击杀僵尸，为民除害！", 1, 101, "少侠好身手！！！", 0);
        Quest quest2 = new Quest(2002, "击杀", QuestType.Kill, "击杀僵尸，为民除害！", 3, 101, "少侠好身手！！！", 2001);
        Quest quest3 = new Quest(2003, "击杀", QuestType.Kill, "击杀僵尸，为民除害！", 6, 101, "少侠好身手！！！", 2002);
        Quest quest4 = new Quest(2004, "击杀", QuestType.Kill, "击杀僵尸，为民除害！", 9, 101, "少侠好身手！！！", 2003);
        Quest quest5 = new Quest(2005, "击杀", QuestType.Kill, "击杀僵尸，为民除害！", 12, 101, "少侠好身手！！！", 2004);

        questDataList.Add(quest1);
        questDataList.Add(quest2);
        questDataList.Add(quest3);
        questDataList.Add(quest4);
        questDataList.Add(quest5);

        //收集任务
        Quest quest21 = new Quest(2101, "收集", QuestType.Collection, "收集鸟蛋，成为收藏家！", 1, 1003, "少侠好身家！！！", 0);
        Quest quest22 = new Quest(2102, "收集", QuestType.Collection, "收集鸟蛋，成为收藏家！", 2, 1003, "少侠好身家！！！", 2101);
        Quest quest23 = new Quest(2103, "收集", QuestType.Collection, "收集鸟蛋，成为收藏家！", 3, 1003, "少侠好身家！！！", 2102);
        Quest quest24 = new Quest(2104, "收集", QuestType.Collection, "收集鸟蛋，成为收藏家！", 4, 1003, "少侠好身家！！！", 2103);
        Quest quest25 = new Quest(2105, "收集", QuestType.Collection, "收集鸟蛋，成为收藏家！", 5, 1003, "少侠好身家！！！", 2104);

        questDataList.Add(quest21);
        questDataList.Add(quest22);
        questDataList.Add(quest23);
        questDataList.Add(quest24);
        questDataList.Add(quest25);

        //探索任务
        Quest quest31 = new Quest(2201, "探索", QuestType.Explore, "通关关卡1，成为探索家！", 1, 1, "你在探索的路上又进了一步！！！", 0);
        Quest quest32 = new Quest(2202, "探索", QuestType.Explore, "通关关卡2，成为探索家！", 1, 2, "你在探索的路上又进了一步！！！", 2201);
        Quest quest33 = new Quest(2203, "探索", QuestType.Explore, "通关关卡3，成为探索家！", 1, 3, "你在探索的路上又进了一步！！！", 2202);
        Quest quest34 = new Quest(2204, "探索", QuestType.Explore, "通关关卡4，成为探索家！", 1, 4, "你在探索的路上又进了一步！！！", 2203);
        Quest quest35 = new Quest(2205, "探索", QuestType.Explore, "通关关卡5，成为探索家！", 1, 5, "你在探索的路上又进了一步！！！", 2204);

        questDataList.Add(quest31);
        questDataList.Add(quest32);
        questDataList.Add(quest33);
        questDataList.Add(quest34);
        questDataList.Add(quest35);



        //技能数据的生成         id  名称         类型              动画参数 冷却时间 伤害 图标          描述
        Skill skill1 = new Skill(1, "喷火", SkillType.NeedTarget, "Attack1", 1f, 50, "Skill1Icon", "释放这个技能的时候，一定要使劲儿！");
        //添加数据
        skillDataList.Add(skill1);
        Skill skill2 = new Skill(2, "地刺阵", SkillType.DontTarget, "Attack2", 1f, 50, "Skill2Icon", "释放这个技能的时候，一定要使劲儿！");
        //添加数据
        skillDataList.Add(skill2);
        Skill skill3 = new Skill(3, "落剑阵", SkillType.DontTarget, "Attack3", 1f, 50, "Skill3Icon", "释放这个技能的时候，一定要使劲儿！");
        //添加数据
        skillDataList.Add(skill3);
    }
}
