using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedPlatforms : MonoBehaviour
{

    [SerializeField] GameObject platformPrefab;

    const int PLATFORMS_NUM = 10;

    GameObject[] platforms;

    Vector3[] positions;

    public float radius = 2;

    public float speed = 1;

    private int currentPosition = 0;

    private int next = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameState.GS_GAME) return;

        for (int i = 0; i < PLATFORMS_NUM; i++) {
            currentPosition = (i + next) % PLATFORMS_NUM;
            if (platforms[i].transform.position == positions[currentPosition]) next++;
            platforms[i].transform.position = Vector3.MoveTowards(platforms[i].transform.position, positions[currentPosition], speed * Time.deltaTime);
        }
    }

    void Awake()
    {
        platforms = new GameObject[PLATFORMS_NUM];
        positions = new Vector3[PLATFORMS_NUM];

        for (int i = 0; i < PLATFORMS_NUM; i++)
        {
            positions[i] = new Vector3(radius * Mathf.Cos(2 * Mathf.PI*i/PLATFORMS_NUM) + this.transform.position.x, 
                    radius * Mathf.Sin(2 * Mathf.PI * i / PLATFORMS_NUM) + this.transform.position.y, 
                    this.transform.position.z);

            platforms[i] = Instantiate(platformPrefab, positions[i], Quaternion.identity);
        }
    }
}
