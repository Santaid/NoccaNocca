using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    public void IsQuit(bool quit){
        if(quit){
            Application.Quit();
        }
    }
}
