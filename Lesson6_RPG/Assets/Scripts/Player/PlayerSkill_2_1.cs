using UnityEngine;
using System.Collections;

public class PlayerSkill_2_1 : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();
            if (e.state != EnemyState.Die)
            {
                //e.BeHit((int)skillData.HitValue);
                e.BeHit(20);
            }
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * 5 * Time.deltaTime, Space.Self);
    }
  
}
