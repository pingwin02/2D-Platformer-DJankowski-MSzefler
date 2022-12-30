using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Door : MonoBehaviour
{
    GameObject lockIcon;

    Vector2 endPosition;

    bool keyCollected;

    private void Update()
    {
        if (keyCollected)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position,
            endPosition, Time.deltaTime);
        }
    }
    private void Awake()
    {
        lockIcon = GameObject.FindGameObjectWithTag("Lock");
        endPosition = this.transform.position;
    }


    [ContextMenu("Open")]
    public void Open()
    {
        endPosition += new Vector2(0, 3);
        lockIcon.SetActive(false);
        keyCollected = true;
        
    }
}
