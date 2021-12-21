using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField]
    private GameObject groundPrefab;

    float leftBound;

    private List<GroundElement> elements;
    private Vector3 genPos;
    private const float generateTerm = 200.0f;
    private float termDelta;
    private int noGroundStack;

    // Start is called before the first frame update
    void Start()
    {
        elements = new List<GroundElement>();

        //temp code
        elements.AddRange(FindObjectsOfType<GroundElement>());

        Debug.Log(elements.Count);

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
        Debug.Log(elements.Count);
        GenerateGround();
    }

    public void Shift(float speed)
    {
        for (int i = 0; i < elements.Count; i++)
        {
            GroundElement e = elements[i];
            Vector2 pos = e.transform.position;
            pos.x -= speed;
            e.transform.position = pos;

            if (e.transform.position.x < leftBound)
            {
                elements.Remove(e);
                Destroy(e.gameObject);
            }
        }
    }
    private void GenerateGround()
    {
        termDelta -= 1.0f;
        if(termDelta < 0f)
        {
            termDelta = generateTerm;

            int r = Random.Range(0, 5); // 0 ~ 4
            Debug.Log(r);

            if (r < 3)
            {
                noGroundStack = 0;
                GameObject element = Instantiate(groundPrefab, genPos, Quaternion.identity);
                elements.Add(element.GetComponent<GroundElement>());
            }
            else
            {
                ++noGroundStack;
                if(noGroundStack > 2)
                {
                    noGroundStack = 0;
                    GameObject element = Instantiate(groundPrefab, genPos, Quaternion.identity);
                    elements.Add(element.GetComponent<GroundElement>());
                }
            }
        }
    }
}
