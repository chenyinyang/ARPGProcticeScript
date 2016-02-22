using UnityEngine;
using System.Collections;

public class CastingBar : MonoBehaviour {
    UISlider prograssBar;
    UILabel timeLeftText;
    UILabel skillName;
    UITexture skillIcon;
    public BaseCharacterBehavior.CastOver callBack;
    float castTimer = 0f;
    float castingTime;
    bool reverseProgress = false;
    public bool IsCasting { get { return gameObject.activeInHierarchy; } }

    const float UPDATE_INTERVAL = .05f;
    float updateTimer = 0;

    void Awake() {
        prograssBar = GetComponentInChildren<UISlider>();
        timeLeftText = prograssBar.GetComponentInChildren<UILabel>();
        skillName = transform.Find("SkillName").GetComponent<UILabel>();        
        skillIcon = GetComponentInChildren<UITexture>();
        
    }
	
	
	
	// Update is called once per frame
	void Update () {
        if (castTimer == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        if (Time.time - updateTimer > UPDATE_INTERVAL)
        {
            updateTimer = Time.time;
            float timeLeft = castingTime - (Time.time - castTimer);

            if (timeLeft <= 0)
            {
                gameObject.SetActive(false);
                return;
            }
            prograssBar.value = reverseProgress ? 1 - (timeLeft / castingTime) : (timeLeft / castingTime);
            timeLeftText.text = timeLeft.ToString("0.0");
        }
    }

    public void Casting(SkillName name, float totalTime,bool reverseProgress = false) {

        this.reverseProgress = reverseProgress;        
        prograssBar.value = reverseProgress?0:1;
        timeLeftText.text = totalTime.ToString("0.0");
        skillName.text = name.ToString();
        castTimer = Time.time;
        castingTime = totalTime;
        skillIcon.mainTexture = ResourceLoader.Skill.GetIcon(name);
    }
}
