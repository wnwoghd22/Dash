using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject groundPrefab;
    private float altitude;

    private List<GroundElement> elements;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        altitude = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
