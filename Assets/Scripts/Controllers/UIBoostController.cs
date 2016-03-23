using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIboostControl : MonoBehaviour
{

    float cooldownTime = 5.0f;
    public Image CooldownBar;
    protected Color cooldownColor = Color.white;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void cooldownUpdate(float timeLeft)
    {
        timeLeft = cooldownTime - timeLeft;
        if (timeLeft > 0.01)
        {
            CooldownBar.color = cooldownColor;
            CooldownBar.fillAmount = (timeLeft / (cooldownTime));
        }
        else {
            CooldownBar.fillAmount = 0;
            CooldownBar.color = Color.white;
        }
    }

}
