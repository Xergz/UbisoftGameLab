using UnityEngine;
using UnityEngine.UI;

public class UIBoostControl : MonoBehaviour
{

    public float cooldownTime = 8.0f;
    public Image CooldownBar;

	public bool IsReady { get; set; }

	public float ElapsedTime { get; private set; }

    // Use this for initialization
    void Awake()
    {
        ElapsedTime = 0.0f;
		IsReady = true;
    }

    // Update is called once per frame
    void Update()
    {
		if(!IsReady) {
			cooldownUpdate();
		}
    }

    //public void Activate()
    //{
    //    cooldownUpdate();

    //}

    private void cooldownUpdate()
    {
		if(ElapsedTime < cooldownTime - 0.01) {
			ElapsedTime += Time.deltaTime;
			CooldownBar.fillAmount = ElapsedTime / cooldownTime;
		} else {
			ElapsedTime = 0F;
			IsReady = true;
			CooldownBar.fillAmount = 1;
		}
    }

}
