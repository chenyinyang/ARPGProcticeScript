using UnityEngine;
using System.Collections.Generic;

public class MyGUI : MonoBehaviour {
    public static MyGUI Instence {
        get; private set;
    }
    public Camera UICamera;
    public Transform UIRoot;
    public Transform MainCamera;
    public List<IClosablePanel> openedPanel;
    public GameObject HealthBar;
    private UISlider health;

    public GameObject ManaBar;
    private UISlider mana;

    public GameObject EnergyBar;
    private UISlider energy;

    public CastingBar CastBar;

    private BuffPanelGUI puffPanelGUI;
    public UISprite[] SkillBtn;


    public BuildBloodOnGUI buildBloodOnGUI;

    public Follower[] playersFollowers;

    const float UPDATE_INTERVAL = .1f;
    float updateTimer = 0;
    void Awake() {

        Instence = this;
        openedPanel = new List<IClosablePanel>();
        health = HealthBar.GetComponent<UISlider>();
        mana = ManaBar.GetComponent<UISlider>();
        energy = EnergyBar.GetComponent<UISlider>();
        puffPanelGUI = GetComponentInChildren<BuffPanelGUI>();
        buildBloodOnGUI.gameObject.SetActive(false);
        UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
        UIRoot = GameObject.Find("UI Root").transform;
        MainCamera = Camera.main.transform;
        Messenger.AddListener<float, float>(MessengerTopic.PLAYER_HEALTH_UPDATE, PlayerHealthUpdate);
        Messenger.AddListener<float, float>(MessengerTopic.PLAYER_ENERGY_UPDATE, PlayerEnergyUpdate);
        Messenger.AddListener<float, float>(MessengerTopic.PLAYER_MANA_UPDATE, PlayerManaUpdate);
    }
    // Use this for initialization
    void Start() {
        puffPanelGUI.SetBuffIcon(new BuffSkill[0]);
        openedPanel.Add(CharacterPanel.Instance);
        openedPanel.Add(TowerInfoPanel.Instance);
        InvokeRepeating("UpdatePlayersFollower", 0, .1f);
    }
    void Update() {
        if (Input.GetButtonUp("OpenCharacterPanel"))
        {            
            if (CharacterPanel.Instance.IsOpen)
            {
                CharacterPanel.Instance.Trigger();
                MyGUI.Instence.ShowBuildBlood();
            }
            else
            {
                CloseAllOpenedPanel();
                CharacterPanel.Instance.Trigger();
                MyGUI.Instence.HideBuildBlood();
            }
        }
    }
    public void CloseAllOpenedPanel() {
        openedPanel.ForEach(p => p.Close());
        MyGUI.Instence.ShowBuildBlood();
    }
    void PlayerHealthUpdate(float curValue, float maxValue) {
        health.value = curValue / maxValue;
        HealthBar.GetComponentInChildren<UILabel>().text = curValue.ToString("0") + " / " + maxValue;
    }
    void PlayerManaUpdate(float curValue, float maxValue)
    {
        mana.value = curValue / maxValue;
        ManaBar.GetComponentInChildren<UILabel>().text = curValue.ToString("0") + " / " + maxValue;
    }
    void PlayerEnergyUpdate(float curValue, float maxValue)
    {
        energy.value = curValue / maxValue;
        EnergyBar.GetComponentInChildren<UILabel>().text = curValue.ToString("0") + " / " + maxValue;
    }
    // Update is called once per frame
    public bool healthLowTranFlag = true;
    int transCount = 0;
    void FixedUpdate() {
        if (Time.time - updateTimer > UPDATE_INTERVAL)
        {
            updateTimer = Time.time;
            if (health.value < .3)
            {
                if (healthLowTranFlag)
                {
                    health.foregroundWidget.color = Color.Lerp(health.foregroundWidget.color, Color.clear, Time.deltaTime * 3);
                }
                else
                {
                    health.foregroundWidget.color = Color.Lerp(health.foregroundWidget.color, Color.red, Time.deltaTime * 3);
                }
                transCount++;
                if (transCount >= 20)
                {
                    healthLowTranFlag = !healthLowTranFlag;
                    transCount = 0;
                }
            }
            else if (health.value != 1)
            {
                health.foregroundWidget.color = Color.Lerp(Color.clear, Color.red, 0.5f + health.value / 2);
            }
        }
    }

    public void SetSkillBtn(int index, Texture2D icon, float cooldown, float maxCooldown) {

        SkillBtn[index].transform.Find("Icon").GetComponent<UITexture>().mainTexture = icon;
        SetSkillBtn(index, cooldown, maxCooldown);
    }
    public void SetSkillBtn(int index, float cooldown, float maxCooldown)
    {
        if (cooldown <= 0)
        {
            SkillBtn[index].transform.Find("CooldownMask").gameObject.SetActive(false);
            SkillBtn[index].transform.Find("CooldownLabel").gameObject.SetActive(false);
        }
        else
        {
            SkillBtn[index].transform.Find("CooldownMask").gameObject.SetActive(true);
            SkillBtn[index].transform.Find("CooldownLabel").gameObject.SetActive(true);
            SkillBtn[index].transform.Find("CooldownMask").GetComponent<UISprite>().fillAmount = cooldown / maxCooldown;
            SkillBtn[index].transform.Find("CooldownLabel").GetComponent<UILabel>().text = cooldown.ToString("0.0");
        }
    }
    public void SetBuffIcon(BuffSkill[] buffs) {
        puffPanelGUI.SetBuffIcon(buffs);
    }
    public void ShowCastBar(SkillName name, float totalTime) {
        CancelCastBar();
        CastBar.gameObject.SetActive(true);
        CastBar.Casting(name, totalTime);
    }
    public void CancelCastBar()
    {
        CastBar.gameObject.SetActive(false);
    }

    bool buildBloodHideByUI = false;
    public void ShowBuildBlood()
    {        
        buildBloodHideByUI = false;
        if (buildBloodOnGUI.gameObject.activeSelf == false && buildBloodOnGUI.build!=null)
            buildBloodOnGUI.gameObject.SetActive(true);
    }
    public void ShowBuildBlood(Build building) {

        if(buildBloodOnGUI.gameObject.activeSelf ==false && !buildBloodHideByUI)
            buildBloodOnGUI.gameObject.SetActive(true);
        if (buildBloodOnGUI.build != building)
            buildBloodOnGUI.InitialWithTarget(building);
    }
    public void HideBuildBlood()
    {
        buildBloodHideByUI = true ;
        if (buildBloodOnGUI.gameObject.activeSelf)
        {
            buildBloodOnGUI.gameObject.SetActive(false);
            buildBloodOnGUI.build = null;
        }
    }
    public void HideBuildBlood(Build building)
    {
        if (buildBloodOnGUI.gameObject.activeSelf && buildBloodOnGUI.build == building && !buildBloodHideByUI)
        {
            buildBloodOnGUI.gameObject.SetActive(false);
            buildBloodOnGUI.build = null;
        }
    }
    public void UpdatePlayersFollower() {
        for (int i = 0; i < playersFollowers.Length; i++)
        {
            if (Player.Instance.Followers.Count > i)
            {                
                playersFollowers[i].BindNPC(Player.Instance.Followers[i], null);
                playersFollowers[i].gameObject.SetActive(true);
            }
            else {
                playersFollowers[i].gameObject.SetActive(false);
            }
        }
    }
}
