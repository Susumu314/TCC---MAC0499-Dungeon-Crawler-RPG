using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform playerTransform;
    // Start is called before the first frame update
    void Awake()
    {
        transform.parent = null;
        this.transform.eulerAngles = new Vector3(
            90,
            0,
            0
        );
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(playerTransform.position.x, this.transform.position.y, playerTransform.position.z);
    }
}
