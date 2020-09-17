using UnityEngine;
using System.Collections;

public class AllTextMove : MonoBehaviour
{

    public int Speed = 2;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * Speed);
    }

}