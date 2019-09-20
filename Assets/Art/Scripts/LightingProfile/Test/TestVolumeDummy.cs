#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVolumeDummy : MonoBehaviour
{
    public float speed = 1;
    public float limit = 30;
    private float currenXPos;
    private bool changeDirection;

    void Start()
    {
        currenXPos = transform.position.x;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        float newPos = Time.deltaTime * speed * (changeDirection ? 1 : -1);
        pos.x += newPos;
        transform.position = pos;
    }
}
