using UnityEngine;
using UnityEngine.UI;

public class UIBoostControl : MonoBehaviour
{

    float cooldownTime = 10.0f;
    public Image CooldownBar;
    protected Color cooldownColor = Color.white;
    public float timeLeft { get; set; }
    // Use this for initialization
    void Start()
    {
        timeLeft = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        cooldownUpdate();
    }

    //public void Activate()
    //{
    //    cooldownUpdate();

    //}

    private void cooldownUpdate()
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
