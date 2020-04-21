using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static GameSetup GS = new GameSetup();
    public Transform[] spawnPoints;
    // Start is called before the first frame update

    private void OnEnable()
    {
        if (GS != null)
        {

        }

        GS = this;



    }



}
