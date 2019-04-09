using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handCollision : MonoBehaviour
{

    public Transform mHandMesh;
    // Start is called before the first frame update
    void Start()
    {
        mHandMesh.position = Vector3.Lerp(mHandMesh.position, transform.position, Time.deltaTime * 15.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
