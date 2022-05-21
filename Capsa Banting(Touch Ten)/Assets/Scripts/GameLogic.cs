using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance { get; private set; }

    private FSM<States> StateMachine = new FSM<States>();

    public List<UI_AvatarSelectionButton> characterSelection;
    public TMP_Text selectedInfo;

    public Button continueButton;
    public Button ezConfigButton;

    public GameObject MainMenuPage;
    public GameObject CharacterSelectionPage;

    public bool EZMode;

    PlayerID selectedchar;

    public PlayerID selectedCharacter
    {
        get
        {
            return selectedchar;
        }
        set
        {
            selectedchar = value;
            selectedInfo.text = value.ToString() + " Selected to play!";
            foreach(UI_AvatarSelectionButton c in characterSelection)
            {
                if(c.avatarID != value)
                    c.ResetButton();
            }
        }
    }

    public enum States
    {
        IDLE,
        MAINMENU,
        CHARACTERSELECTION,
        GAMEPLAY,
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Dictionary<States, State> stateMap = new Dictionary<States, State>()
        {
            { States.IDLE,                      new State(null,null,null,null) },
            { States.MAINMENU,                  new State(MainMenuEnter,null,null,null) },
            { States.CHARACTERSELECTION,        new State(CharacterSelectionEnter,CharacterSelectionUpdate,null,null) },
            { States.GAMEPLAY,                  new State(GameplayEnter,null,null,null) },
        };
        StateMachine.Initialize(stateMap);
        StateMachine.ChangeState(States.IDLE);

        //EZ mode is on by default(ez mode make AI always Pass for every turn)
        EZMode = false;
        EzModeButton();
    }

    void MainMenuEnter()
    {
        selectedCharacter = PlayerID.INVALIDPLAYER;
        MainMenuPage.SetActive(true);
        CharacterSelectionPage.SetActive(false);
        ezConfigButton.gameObject.SetActive(true);
    }

    void CharacterSelectionEnter()
    {
        selectedInfo.text = "";
        MainMenuPage.SetActive(false);
        CharacterSelectionPage.SetActive(true);
    }

    void CharacterSelectionUpdate()
    {
        if (selectedchar == PlayerID.INVALIDPLAYER)
            continueButton.interactable = false;
        else
            continueButton.interactable = true;

    }

    public void EzModeButton()
    {
        if(EZMode)
        {
            EZMode = false;
            ezConfigButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            EZMode = true;
            ezConfigButton.GetComponent<Image>().color = Color.green;
        }
    }

    void GameplayEnter()
    {
        MainMenuPage.SetActive(false);
        CharacterSelectionPage.SetActive(false);
        ezConfigButton.gameObject.SetActive(false);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive); // Loading Game Scene  additively   
    }

    public void MainmenuPlayButton()
    {
        StateMachine.ChangeState(States.CHARACTERSELECTION);
    }

    public void BackToMainMenu()
    {
        StateMachine.ChangeState(States.MAINMENU);
    }

    public void CharacterSelectionContinueButton()
    {
        StateMachine.ChangeState(States.GAMEPLAY);
    }

    private void Start()
    {
        StateMachine.ChangeState(States.MAINMENU);
    }

    void Update()
    {
        StateMachine.Update();
    }
}
