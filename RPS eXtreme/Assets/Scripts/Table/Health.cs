using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    public TextMeshProUGUI healthTextPlayer;
    public TextMeshProUGUI healthTextOpponent;
    

    public void setHealth(int health, bool player)
    {
        if(player) healthTextPlayer.text = health.ToString();
        else healthTextOpponent.text = health.ToString();
    }
}
