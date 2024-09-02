using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public GameObject model;
    public Vector3 rotation;
    // Update is called once per frame
    void Update()
    {
        model.transform.Rotate(rotation * Time.deltaTime);
    }
}
