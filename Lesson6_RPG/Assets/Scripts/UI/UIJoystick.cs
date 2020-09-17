using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
/// <summary>
/// 控制杆
/// </summary>
public class UIJoystick : MonoBehaviour
{

    private Transform handler;//红点

    public float Range = 300;

    public bool IsUse = false;


    // Use this for initialization
    void Start()
    {
        handler = transform.Find("Handler");

        //绑定事件
        UIEventTrigger listener = UIEventTrigger.Get(gameObject);
        if (Application.platform == RuntimePlatform.WindowsEditor)//如果是PC平台
        {
            listener.onPointerDown += OnDownJoystickOnPC;
        }
        else if (Application.platform == RuntimePlatform.Android)//如果是安卓平台
        {
            listener.onPointerDown += OnDownJoystickOnMobile;
        }

    }

    //鼠标按压事件 基于移动端
    public void OnDownJoystickOnMobile(PointerEventData eventData)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            var t = Input.GetTouch(i);
            if (t.phase == TouchPhase.Began)
            {
                if (!IsUse)
                {
                    StartCoroutine(DoJoystickOnMobile(t));
                }
                break;
            }
        }
    }
    // 正确的使用touch 在移动端可用 

    IEnumerator DoJoystickOnMobile(Touch touch)
    {
        IsUse = true;
        while (touch.phase != TouchPhase.Ended)
        {
            //触点坐标
            Vector3 mousePos = new Vector3(touch.position.x, touch.position.y, 0);
            //圆盘的圆心到鼠标 的向量
            Vector3 offset = mousePos - transform.position;
            if (offset.magnitude > Range)
            {
                //offset 限制
                offset = Range * offset.normalized;
            }
            handler.position = offset + transform.position;

            GameController.Instance.InputJoystick = offset.normalized;

            int finger = touch.fingerId;
            yield return null;
            for (int i = 0; i < Input.touchCount; i++)
            {
                var newt = Input.GetTouch(i);
                if (newt.fingerId == finger)
                {
                    touch = newt;
                }
            }

        }

        handler.localPosition = Vector3.zero;
        GameController.Instance.InputJoystick = Vector2.zero;

        IsUse = false;
    }

    //鼠标按压事件 基于PC端
    public void OnDownJoystickOnPC(PointerEventData data)
    {
        StartCoroutine(DoJoystickOnPC());//开启协程
    }

    IEnumerator DoJoystickOnPC()
    {
        while (Input.GetMouseButton(0))
        {
            //鼠标坐标
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            //圆心到鼠标 的向量
            Vector3 offset = mousePos - transform.position;
            if (offset.magnitude > Range)
            {
                //offset 限制
                offset = Range * offset.normalized;
            }
            handler.position = offset + transform.position;

            GameController.Instance.InputJoystick = offset.normalized;

            yield return null;
        }
        handler.localPosition = Vector3.zero;
        GameController.Instance.InputJoystick = Vector2.zero;
    }

}
