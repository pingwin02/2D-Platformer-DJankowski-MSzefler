using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKey : MonoBehaviour
{
    Vector3[] positions;

    int currentPosition = 0;

    private void Awake()
    {
        positions = new Vector3[2];
        positions[0] = this.transform.position;
        positions[1] = positions[0] + new Vector3(0, 1);
    }

    // Update is called once per frame
    private void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position,
            positions[currentPosition], Time.deltaTime);
        if (this.transform.position == positions[currentPosition])
        {
            currentPosition++;
            currentPosition %= 2;
        }

    }




}
