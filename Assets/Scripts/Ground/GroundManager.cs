using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] groundPrefabs;

    float leftBound;

    private Vector3 genPos;
    [SerializeField]
    private float generateTerm = 14.5f;
    private float termDelta;

    private PlayerContoroller player;

    private float shiftSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 screenGenPos = new Vector3(Screen.width, 0, 0);
        Vector3 tempGenPos = Camera.main.ScreenToWorldPoint(screenGenPos);

        leftBound = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        genPos = new Vector3(tempGenPos.x + 15f, tempGenPos.y, 0);

        termDelta = 0;

        player = GameObject.Find("Player").GetComponent<PlayerContoroller>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        GenerateGround(player.MoveSpeed * shiftSpeed);
    }

    public void GenerateGround(float delta)
    {
        termDelta -= delta;
        if(termDelta < 0f)
        {
            termDelta = generateTerm;

            int r = Random.Range(0, groundPrefabs.Length);
            Debug.Log(r);

            GameObject element = Instantiate(groundPrefabs[r], genPos, Quaternion.identity);
               
        }
    }
}
