using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject[] waypoints;

    int currentWaypoint = 0;

    [SerializeField] private float speed = 1.0f;

    private float moveRange = 0.1f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameState.GS_GAME) return;

        float currentDistance = Vector2.Distance(transform.position, waypoints[currentWaypoint].transform.position);
        if (currentDistance < moveRange)
        {
            currentWaypoint++;
            currentWaypoint = currentWaypoint % waypoints.Length;
        }
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypoint].transform.position, speed * Time.deltaTime);
    }
}
