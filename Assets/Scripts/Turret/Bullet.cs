using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    FoxController fox;
    // Start is called before the first frame update
    void Start()
    {
        fox = GameObject.FindGameObjectWithTag("Player").GetComponent<FoxController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && fox.GetActive())
        {
            fox.Die();
        }
    }
}
