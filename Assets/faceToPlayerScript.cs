using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceToPlayerScript : MonoBehaviour
{
    [SerializeField] Transform objectToFace;

    // Update is called once per frame
    void Update()
    {
        //NEW
        Vector3 targetPosition = new Vector3(objectToFace.position.x,
            transform.position.y,
            objectToFace.position.z);

        transform.LookAt(targetPosition);
    }
}
