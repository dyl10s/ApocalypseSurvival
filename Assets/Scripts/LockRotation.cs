using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    Transform tran;

    // Start is called before the first frame update
    void Start()
    {
        tran = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        tran.eulerAngles = new Vector3(tran.eulerAngles.x, tran.eulerAngles.y, 0);
    }
}
