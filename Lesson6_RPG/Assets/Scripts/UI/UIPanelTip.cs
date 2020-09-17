using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanelTip : MonoBehaviour {

    private static UIPanelTip instance;
    public static UIPanelTip Instance
    {
        get
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
            }
            Transform panel = GameController.Instance.uiController.CreatePanel("PanelTip");
            instance = panel.GetComponent<UIPanelTip>();

            return instance;
        }
    }
    public Text contentText;

    void Awake()
    {
        //1. 拿到引用，绑定事件
        contentText = transform.Find("Image/Text").GetComponent<Text>();
        instance = this;

        Destroy(gameObject, 5f);
    }

    public void Init(string msg)
    {
        contentText.text = msg;
    }
}
