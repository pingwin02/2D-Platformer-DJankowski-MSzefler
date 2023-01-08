using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Door : MonoBehaviour
{
    GameObject lockIcon;

    Vector3 startPosition;

    Vector3 endPosition;

    bool opening;


    private void Update()
    {
        if (opening)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position,
            endPosition, Time.deltaTime);
        }
        else
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position,
            startPosition, Time.deltaTime);
        }
    }
    private void Awake()
    {
        lockIcon = GameObject.FindGameObjectWithTag("Lock");
        startPosition = this.transform.position;
        endPosition = startPosition;
        endPosition.y += 3.25f;

    }


    [ContextMenu("Open")]
    public void Open()
    {
        opening = true;
        lockIcon.SetActive(false);
    }

    [ContextMenu("Close")]
    public void Close()
    {
        opening = false;
        lockIcon.SetActive(true);
    }
}
