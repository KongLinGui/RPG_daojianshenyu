using UnityEngine;
using System.Collections;
/// <summary>
/// 跟随脚本
/// </summary>
public class UIFollow : MonoBehaviour {
    
    public Transform target; //跟随目标


    public void SetTarget(Transform tar)
    {
        target = tar; //设置跟随目标
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError("没有给UIFollow 设置目标！");
        }
        else
        {
            //取得 3D物体在屏幕坐标系中的坐标
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
            //将UI物体坐标赋值
            transform.position = new Vector3(screenPos.x, screenPos.y, 0);

        }
    }

}
