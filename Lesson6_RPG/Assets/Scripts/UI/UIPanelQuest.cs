using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
/// <summary>
/// 任务界面
/// </summary>
public class UIPanelQuest : MonoBehaviour {

    public GameObject questPrefab;//人物预设物
    public Transform grid;//网格

    void Awake()
    {
        //1.绑定事件 
        transform.Find("ButtonClose").GetComponent<Button>().onClick.AddListener(delegate () 
        {
            SoundController.Instance.PlayAudio("audio");
            Destroy(gameObject);
        });

        //2.获取引用
        //2.1 拿到 任务UI预设物
        questPrefab = transform.Find("QuestPrefab").gameObject;
        grid = transform.Find("Scroll View/Viewport/Content");

        //3.初始化
        Init();
    }
    /// <summary>
    /// 初始化
    /// </summary>
    void Init()
    {
        //0.更新玩家任务进度
        GameController.Instance.playerData.UpdateQuestPlane();

        //1.拿到玩家任务数据，显示    
        for (int i = 0; i < GameController.Instance.playerData.QuestList.Count; i++)
        {
            //生成
            Quest quest = GameController.Instance.playerData.QuestList[i];
            CreateQuestUI(quest);

        }
    }
    
    //生成任务UI
    public void CreateQuestUI(Quest quest)
    {
        Transform clone = Instantiate(questPrefab).transform;
        clone.SetParent(grid);
        clone.localScale = Vector3.one;
        clone.name = quest.Id + "";
        //显示名称
        clone.Find("Name").GetComponent<Text>().text = quest.Name;
        //显示描述
        clone.Find("Desc").GetComponent<Text>().text = quest.Desc;
        //显示进度/领取按钮

        //quest.CurrentProgress = quest.TotalProgress; //测试代码

        if (quest.CurrentProgress < quest.TotalProgress)
        {
            clone.Find("Plane").GetComponent<Text>().text = quest.CurrentProgress + "/" + quest.TotalProgress;
        }
        else
        {
            clone.Find("ButtonGet").gameObject.SetActive(true);

        }
    }

    // 点击领取奖励按钮
    public void ClickButtonGet(GameObject clone)
    {
        SoundController.Instance.PlayAudio("audio");
        //1.删除
        Destroy(clone.gameObject);
        //1.1 拿到 任务数据
        int questId = int.Parse(clone.name);
        for (int i = GameController.Instance.playerData.QuestList.Count-1; i >=0 ; i--)
        {
            if (questId == GameController.Instance.playerData.QuestList[i].Id)
            {
                //2.显示提示框
                Quest quest = GameController.Instance.playerData.QuestList[i];
                UIPanelTip.Instance.Init(quest.RewardMessage);

                //3.删除旧数据 添加新数据
                GameController.Instance.playerData.QuestList.RemoveAt(i);

                //Debug.Log("quest id = " + quest.Id);

                //3.1 获取新数据 preid 
                for (int j = 0; j < GameController.Instance.gameData.questDataList.Count; j++)
                {
                    //Debug.Log("questDataList id " + GameController.Instance.gameData.questDataList[j].PreId);

                    //找到preid是之前完成任务的id，说明就是新数据
                    if (GameController.Instance.gameData.questDataList[j].PreId == quest.Id)
                    {
                        //Debug.Log("newQuest ");
                        Quest newQuest = GameController.Instance.gameData.questDataList[j].Clone();
                        //添加新数据
                        GameController.Instance.playerData.QuestList.Insert(i, newQuest);
                        //4.重新创建新任务UI
                        CreateQuestUI(newQuest);

                        break;
                    }
                }

            }
        }     
    }
}
