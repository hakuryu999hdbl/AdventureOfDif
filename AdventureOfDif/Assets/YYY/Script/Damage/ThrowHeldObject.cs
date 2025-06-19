using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowHeldObject : MonoBehaviour
{
    public GameObject impactEffect; // 落地特效，可为 null



    float zHeight = 0f;
    float zVelocity = 0f;
    float gravity = -20f; // 同主角设定
    float groundY;        // 扔出那一刻的位置
    bool hasLanded = false;



    private bool hasBeenThrown = false;

    // 由玩家投掷时调用
    public void Launch()
    {
        hasBeenThrown = true;
        zVelocity = 10f;
        groundY = transform.position.y;
    }
    private void Update()
    {
        if (!hasBeenThrown || hasLanded) return;

        zVelocity += gravity * Time.deltaTime;
        zHeight += zVelocity * Time.deltaTime;

        if (zHeight <= 0f)
        {
            zHeight = 0f;
            hasLanded = true;

            Explode();
        }

        // 伪3D的 Y 位置更新
        Vector3 pos = transform.position;
        pos.y = groundY + zHeight;
        transform.position = pos;
    }

    void Explode()
    {
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasBeenThrown) return;


        //玩家和队友伤害
        if (collision.gameObject.tag == "Enemy")
        {

            if (collision.gameObject.GetComponent<Enemy>() != null )
            {

                collision.gameObject.GetComponent<Enemy>().ChangeHealth(100, 1);//击飞伤害

                GameObject effectPrefabs = Instantiate(impactEffect, transform.position, Quaternion.identity);
                Destroy(effectPrefabs, 1f);

                Destroy(gameObject);
            }


        }



     
    }

}
