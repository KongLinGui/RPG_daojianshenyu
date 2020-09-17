using UnityEngine;
using System.Collections;

public class PlayerSkill_3_1 : MonoBehaviour {

    public GameObject Fx;
    public float Speed = 15f;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();
            if (e.state != EnemyState.Die)
            {
                //e.BeHit((int)skillData.HitValue);
                e.BeHit(20);
            }
        }
        if (other.tag == "Fx")
        {
            GameObject fx = Instantiate(Fx);
            fx.transform.position = transform.position+new Vector3(0,-5f,0);
            Destroy(gameObject);
            Destroy(fx.gameObject, 0.5f);
        }
    }
    // Use this for initialization
    void Start()
    {
        Fx= Resources.Load<GameObject>("Skill/Fx");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Speed * Time.deltaTime, Space.Self);
    }
}
