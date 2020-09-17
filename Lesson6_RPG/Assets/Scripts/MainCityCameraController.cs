using UnityEngine;
using System.Collections;

/// <summary>
/// 主城相机控制脚本:实现鼠标滑动，相机移动
/// </summary>
public class MainCityCameraController : MonoBehaviour {

    public Vector3 lastMouse;//上一帧鼠标
    public Transform target;
    public Vector3 targetOffset;
    public float Speed;
    public float Smooth = 5f;
    // Use this for initialization
    void Start ()
    {
        //拿到 相机跟随的物体
        target = GameObject.FindGameObjectWithTag("Misc").transform.Find("CameraTarget");
        //向量
        targetOffset = target.position - transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButton(0))
        {
            if (lastMouse != Vector3.zero)
            {
                //偏移值
                Vector3 offset = Input.mousePosition - lastMouse;
                Vector3 move = new Vector3(offset.x, 0, offset.y).normalized;

                target.Translate(-move * Speed * Time.deltaTime);
                //限制坐标
                Vector3 pos = target.localPosition;
                pos.x = Mathf.Clamp(pos.x, -23f, 23f);
                pos.z = Mathf.Clamp(pos.z, -15f, 40f);
                target.localPosition = pos;
            }
            lastMouse = Input.mousePosition;
        }
        else
        {
            lastMouse = Vector3.zero;
        }
	}

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position - targetOffset, Smooth * Time.deltaTime);
    }
}
