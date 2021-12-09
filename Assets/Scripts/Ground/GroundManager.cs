using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField]
    private GameObject groundPrefab;
    private float altitude;

    private List<GroundElement> elements;

    // Start is called before the first frame update
    void Start()
    {
        altitude = 2.5f;
        elements = new List<GroundElement>();

        //temp code
        elements.AddRange(FindObjectsOfType<GroundElement>());

        Debug.Log(elements.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shift(float speed)
    {
        foreach (GroundElement e in elements)
        {
            Vector2 pos = e.transform.position;
            pos.x -= speed;
            e.transform.position = pos;
        }
    }
}
