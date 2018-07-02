using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스페아스바를 누르면 위로 이동한다

public class BulletControl : MonoBehaviour
{

    public float Speed = 5.0f;
    private Transform mytransform = null;

    // Use this for initialization
    void Start()
    {
        mytransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveAmount = Speed * Vector3.up * Time.deltaTime;
        mytransform.Translate(moveAmount);

        if (mytransform.position.y > 5.0f)
        {
            Destroy(gameObject);
        }

    }
}
