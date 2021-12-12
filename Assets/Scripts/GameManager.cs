using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerContoroller player;
    private GroundManager ground;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerContoroller>();
        ground = FindObjectOfType<GroundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ShiftGround();
    }

    public void ShiftGround() => ground.Shift(player.MoveSpeed);

    public void Death()
    {
        SceneManager.LoadScene("Game");
    }
}
