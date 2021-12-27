using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField]
    private GameObject groundPrefab;

    float leftBound;

    private Vector3 genPos;
    [SerializeField]
    private float generateTerm;
    private float termDelta;
    private int noGroundStack;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 screenGenPos = new Vector3(Screen.width, 0, 0);
        Vector3 tempGenPos = Camera.main.ScreenToWorldPoint(screenGenPos);

        leftBound = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        genPos = new Vector3(tempGenPos.x + 3f, tempGenPos.y, 0);

        termDelta = generateTerm;
        noGroundStack = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateGround(float delta)
    {
        termDelta -= delta;
        if(termDelta < 0f)
        {
            termDelta = generateTerm;

            int r = Random.Range(0, 5); // 0 ~ 4
            //Debug.Log(r);

            if (r < 3)
            {
                noGroundStack = 0;
                GameObject element = Instantiate(groundPrefab, genPos, Quaternion.identity);
                element.GetComponent<Ground>().Init();
            }
            else
            {
                ++noGroundStack;
                if(noGroundStack > 2)
                {
                    noGroundStack = 0;
                    GameObject element = Instantiate(groundPrefab, genPos, Quaternion.identity);
                    element.GetComponent<Ground>().Init();
                }
            }
        }
    }
}
