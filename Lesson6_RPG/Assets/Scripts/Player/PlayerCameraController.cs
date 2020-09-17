using UnityEngine;
using System.Collections;

public class PlayerCameraController : MonoBehaviour
{

    public Transform target;
    public int Smooth = 5;
    public float Speed = 5;
    public Vector3 offset;

    void Start()
    {

    }

    public void Init(Transform tar)
    {
        target = tar;
        offset = target.position - this.transform.position;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        this.transform.position =
            Vector3.Lerp(this.transform.position, target.position - offset, Smooth * Time.deltaTime);

    }
}
