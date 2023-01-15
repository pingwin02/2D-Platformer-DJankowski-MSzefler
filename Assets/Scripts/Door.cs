using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Door : MonoBehaviour
{
    GameObject lockIcon;

    Vector3 startPosition;

    Vector3 endPosition;

    public bool isOpened;


    private void Update()
    {
        if (isOpened)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position,
            endPosition, Time.deltaTime);
            lockIcon.SetActive(false);
        }
        else
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position,
            startPosition, Time.deltaTime);
            lockIcon.SetActive(true);
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
        isOpened = true;
    }

    [ContextMenu("Close")]
    public void Close()
    {
        isOpened = false;
    }
}
