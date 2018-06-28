using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public float bulletSpeed = 5.0f;
    private Transform mytransform = null;
    // Use this for initialization
    void Start()
    {
        mytransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveAmount = bulletSpeed * Vector3.up * Time.deltaTime;
        mytransform.Translate(moveAmount);

        if (mytransform.position.y > 5.0f)
        {
            Destroy(gameObject);
            MainControl.Score -= 13;
        }

    }
}
