using UnityEngine;
using System.Collections;

/// <summary>
/// 等待脚本
/// </summary>
public class UIPanelWait : MonoBehaviour {

    private static UIPanelWait instance;
    public static UIPanelWait Instance
    {
        get
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
            }
            Transform panel = GameController.Instance.uiController.CreatePanel("PanelWait");
            instance = panel.GetComponent<UIPanelWait>();

            return instance;
        }
    }

    public Transform big, small;

    void Awake()
    {
        instance = this;
        big = transform.Find("Image1");
        small = transform.Find("Image2");
    }

    void Update()
    {
        big.Rotate(Vector3.forward);
        small.Rotate(-Vector3.forward);
    }
    /// <summary>
    /// 开启
    /// </summary>
    public void Wait() { }
    /// <summary>
    /// 关闭
    /// </summary>
    public void Stop()
    {
        Destroy(gameObject);
    }

}
