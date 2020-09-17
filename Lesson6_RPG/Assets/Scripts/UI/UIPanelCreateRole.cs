using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
public class UIPanelCreateRole : MonoBehaviour
{
    public InputField nameInputField;
    public Transform role;
    // Use this for initialization
    void Awake()
    {
        nameInputField = transform.Find("Name/InputField").GetComponent<InputField>();
        transform.Find("ButtonOk").GetComponent<Button>().onClick.AddListener(ClickOk);
        transform.Find("ButtonRandom").GetComponent<Button>().onClick.AddListener(ClickRandomName);

        //拿到人物引用
        role = transform.Find("Camera/Role");
        UIEventTrigger.Get(transform.Find("RawImage").gameObject).onPointerDown += RotateRole;

    }

    public void RotateRole(PointerEventData data)//人物旋转方法
    {
        StartCoroutine(RotateRoleMov());//开启携程
    }

    IEnumerator RotateRoleMov()//控制人物角色旋转
    {
        Vector3 lastMouse = Vector3.zero;//上一帧的鼠标坐标
        while (Input.GetMouseButton(0))
        {
            if (lastMouse!=Vector3.zero)
            {
                //当前 - 上 = 这一帧的偏移向量
                Vector3 offset = Input.mousePosition - lastMouse;
                //人物旋转
                Vector3 rotateAngle = new Vector3(0, offset.x*0.5f, 0);
                role.Rotate(-rotateAngle);
            }
            lastMouse = Input.mousePosition;//赋值
            yield return null;
        }
    }

    public void ClickRandomName()
    {
        SoundController.Instance.PlayAudio("audio");
        string[] strs = { "悟空", "悟饭", "悟天", "克林", "比克","天津饭", "饺子"};
        string name = strs[Random.Range(0, strs.Length)];
        nameInputField.text = name;
    }


    public void ClickOk()
    {
        SoundController.Instance.PlayAudio("audio");
        if (nameInputField.text == "")
        {
            UIPanelNotice.Instance.Init("不能为空!!!");
            return;
        }
        else
        {
            //UIPanelNotice.Instance.Init("玩家创建成功!!!",delegate() 
            //{
            //    //创建新人物
            //    GameController.Instance.CreateNewPlayer(nameInputField.text);

            //    UIPanelLoad.Instance.Init("MainCity",delegate() 
            //    {
            //        GameController.Instance.uiController.CreatePanel("PanelMainCity");
            //    });

            //    Destroy(gameObject);
            //});


            //2.0 绑定回调事件
            //NetClient.Instance.netResponseHandler += DoCreateRoleNetResponse;
            ////2.1 发送数据     
            //NetClient.Instance.SendRequest(new NetRequest("CreateRole", nameInputField.text));
            //GameController.Instance.CreateNewPlayer(nameInputField.text);
            //NetClient.Instance.SendRequest(new NetRequest("PlayerData", JsonConvert.SerializeObject(GameController.Instance.playerData)));
            //UIPanelLoad.Instance.Init("MainCity", delegate ()
            //{
            //    GameController.Instance.uiController.CreatePanel("PanelMainCity");
            //    Destroy(gameObject);
            //});
            Debug.Log("1");
            NetClient.Instance.netResponseHandler += DoIntitleNetResponce;
            NetClient.Instance.SendRequest(new NetRequest("CreateRole", nameInputField.text));

        }
    }
    /// <summary>
    /// 处理创建角色的回调信息
    /// </summary>
    /// <param name="res"></param>
    public void DoCreateRoleNetResponse(NetResponse res)
    {
        if (res.Type == "CreateRole")
        {
            switch (res.Code)
            {
                case 1:
                    UIPanelNotice.Instance.Init("玩家创建成功!!!", delegate ()
                     {
                        //创建新人物
                        GameController.Instance.CreateNewPlayer(nameInputField.text);
                         string player = JsonConvert.SerializeObject(GameController.Instance.playerData);
                         NetClient.Instance.SendRequest(new NetRequest("PlayerData", player));
                         UIPanelLoad.Instance.Init("MainCity", delegate ()
                         {
                             GameController.Instance.uiController.CreatePanel("PanelMainCity");
                             Destroy(gameObject);
                         });                        
                     });
                    break;
                case 0:
                    UIPanelNotice.Instance.Init("角色已存在");
                    break;
            }
            // 解除回调
            NetClient.Instance.netResponseHandler -= DoCreateRoleNetResponse;
        }
    }


    public void DoIntitleNetResponce(NetResponse res)
    {
        Debug.Log("2");
        if (res.Code == 1)
        {
            Debug.Log("3");
            UIPanelNotice.Instance.Init("玩家创建成功!!!", delegate ()
            {
                //创建新人物
                GameController.Instance.CreateNewPlayer(nameInputField.text);
                //UIPanelLoad.Instance.Init("MainCity", delegate ()
                //{
                //    GameController.Instance.uiController.CreatePanel("PanelMainCity");
                //});

                NetClient.Instance.netResponseHandler += DoCreatePlayerData;
                NetClient.Instance.SendRequest(new NetRequest("PlayerData", 
                    JsonConvert.SerializeObject(GameController.Instance.playerData)));

                Destroy(gameObject);
            });
        }
        else if (res.Code == 0)
        {
            
            UIPanelNotice.Instance.Init("用户名已存在");
        }
        NetClient.Instance.netResponseHandler -= DoIntitleNetResponce;
    }

    public void DoCreatePlayerData(NetResponse res)
    {
        Debug.Log("4");   
        UIPanelLoad.Instance.Init("MainCity", delegate ()
        {
            GameController.Instance.uiController.CreatePanel("PanelMainCity");           
        });
        NetClient.Instance.netResponseHandler -= DoCreatePlayerData;
    }

}
