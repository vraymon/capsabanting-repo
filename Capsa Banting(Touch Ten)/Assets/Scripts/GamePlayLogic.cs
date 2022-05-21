using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePlayLogic : MonoBehaviour
{
    public static GamePlayLogic Instance { get; private set; }

    private FSM<GameStates> StateMachine = new FSM<GameStates>();

    public enum GameStates
    {
        IDLE,
        STARTING,
        GAMEPLAY,
        CHANGETURN,
        RESULT,
    }

    public CardDealer Dealer;

    public CardGroup CardOnTable;

    public List<Player> Players;

    public Transform TableCardPos;
    public Transform CardGroupPos;

    public bool EnableInput;

    public TMP_Text infoText;
    public TMP_Text playerinfoText;
    public GameObject InteractionButton;

    public float TurnDuration = 0;

    public Image TimerBar;
    public Button PlayButton;
    public Button PassButton;

    public Button GroupCardButton;

    public GameObject BackToMenu;
    public GameObject Timer;

    //rank button
    public TMP_Text GroupedCard;
    public TMP_Text playedCard;

    Player WinnerPlayer;


    RaycastHit hit;
    Touch touches;
    Vector2 touchPos;

    Player playerOnTurn;

    bool enablePlay;
    bool enableGroup;
    bool FirstTurn;

    int Turn = -1;
    int totalGroupedCard = 0;

    float currentTurnTime;

    List<PlayerID> usedID = new List<PlayerID>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Dictionary<GameStates, State> stateMap = new Dictionary<GameStates, State>()
        {
            { GameStates.IDLE,          new State(null,null,null,null) },
            { GameStates.STARTING,      new State(StartingEnter,StartingUpdate,null,null) },
            { GameStates.GAMEPLAY,      new State(GameplayEnter,GamePlayUpdate,null,null) },
            { GameStates.CHANGETURN,    new State(ChangeTurnEnter,null,null,null) },
            { GameStates.RESULT,        new State(ResultEnter,null,null,null) },
        };
        StateMachine.Initialize(stateMap);
        StateMachine.ChangeState(GameStates.IDLE);
    }

    void StartingEnter()
    {
        Dealer.GetDeck();
        Dealer.Shuffle(Dealer.Deck);

        playedCard.text = "";
        GroupedCard.text = "";
        infoText.text = "Shuffeling Deck";
        playerinfoText.text = "";
        InteractionButton.SetActive(false);
        BackToMenu.SetActive(false);

        int distributeCard = 0;
        foreach(Player p in Players)
        {
            if (p.Type == PlayerType.Player)
            {
                p.ID = GameLogic.Instance.selectedCharacter;
                usedID.Add(p.ID);
            }
            else
            {
                for(int i = 0; i < (int)PlayerID.INVALIDPLAYER; i++ )
                {
                    if(!usedID.Contains((PlayerID)i))
                    {
                        p.ID = (PlayerID)i;
                        usedID.Add((PlayerID)i);
                        break;
                    }
                }
            }

            int nextLastIdx = distributeCard+13;
            for(int i = distributeCard; i < nextLastIdx; i++)
            {
                distributeCard++;
                p.HandCard.Add(Dealer.Deck[i]);
            }

            var hc = p.HandCard.OrderByDescending(x => x.OveralValue).ToList();
            hc.Reverse();
            p.HandCard = hc;
            p.emotion = Emotion.Happy;
            p.InitPlayer();
        }
    }

    void StartingUpdate()
    {
        //Handle UI Here
        float t = StateMachine.StateElapseTime;
        if (WaitForSecond(t,1f) == true)
        {
            infoText.text = "Game Start";
        }
        
        if (WaitForSecond(t,2f) == true)
        {
            infoText.text = "";
            foreach (Player p in Players)
            {
                for(int i = 0; i < p.playerCard.Count; i++)
                {
                    if(p.Type == PlayerType.Player)
                        p.playerCard[i].HideCard = false;
                    else
                        p.playerCard[i].HideCard = true;
                    
                    p.playerCard[i].card = p.HandCard[i];
                    p.playerCard[i].SetSprite();
                }
                
            }
            StateMachine.ChangeState(GameStates.CHANGETURN);
        }
    }

    void GameplayEnter()
    {
        playerOnTurn = Players[Turn];
        playerOnTurn.playerState = PlayerState.DOINGTURN;
        currentTurnTime = TurnDuration;
        EnableInput = true;

        if (playerOnTurn.Type == PlayerType.Player)
        {
            playerinfoText.text = "Your Turn!";
        }
        else
        {
            playerinfoText.text = "Opponent Turn!";
            InteractionButton.SetActive(false);
            //let ai play higest card in hand IF card can be played
            AIPlayGame();
        }
    }

    void GamePlayUpdate()
    {
        if(playerOnTurn.Type == PlayerType.Player)
        {
            if (WaitForSecond(StateMachine.StateElapseTime, 1))
            {
                playerinfoText.text = "";
                InteractionButton.SetActive(true);
            }
        }
        else
        {
            if (WaitForSecond(StateMachine.StateElapseTime, 1))
            {
                playerinfoText.text = "";
            }
        }

        if(playerOnTurn.PassTurn == true)
        {
            playerinfoText.text = "Skip due to pass";
            if (WaitForSecond(StateMachine.StateElapseTime, 2))
            {
                playerinfoText.text = "";
                StateMachine.ChangeState(GameStates.CHANGETURN);
            }
        }

        GroupCardButton.interactable = enableGroup;

        currentTurnTime -= Time.deltaTime;
        TimerBar.fillAmount =  currentTurnTime/ TurnDuration;

        PlayButton.interactable = enablePlay;

        if(currentTurnTime <= 0)
        {
            if(playerOnTurn.Type == PlayerType.Player)
            {
                enablePlay = false;
                InteractionButton.SetActive(false);
            }
            
            StateMachine.ChangeState(GameStates.CHANGETURN);
        }
    }

    void ChangeTurnEnter()
    {

        FirstTurn = Turn == -1 ? true : false;

        Turn++;
        if (Turn == 4)
        {
            Turn = 0;
        }
            

        if(!FirstTurn)
        {
            if (playerOnTurn.Type == PlayerType.Player)
            {
                foreach (Card c in playerOnTurn.SelectedCard)
                {
                    PlayerCard pc = playerOnTurn.playerCard.Find(x => x.card == c);
                    pc.cardstate = CardState.IDLE;
                    pc.gameObject.transform.localPosition = new Vector3(pc.gameObject.transform.localPosition.x, 0, pc.gameObject.transform.localPosition.z);
                }
            }
            playerOnTurn.SelectedCard.Clear();
            playerOnTurn.SelectedCardGroup = null;
        }
        
        if(isThereAWinner())
        {
            StateMachine.ChangeState(GameStates.RESULT);
        }
        else
        {
            StateMachine.ChangeState(GameStates.GAMEPLAY);
        }
        
    }

    void ResultEnter()
    {
        foreach(Player p in Players)
        {
            if(p != WinnerPlayer)
            {
                p.emotion = Emotion.Angry;
                p.avatar.SetAvatar(p.emotion);
            }
        }

        ClearTable();
        GroupCardButton.gameObject.SetActive(false);
        InteractionButton.SetActive(false);
        Timer.SetActive(false);
        BackToMenu.SetActive(true);
        infoText.text = WinnerPlayer.ID.ToString() + " is the Winner!";
    }
    bool isThereAWinner()
    {
        foreach(Player p in Players)
        {
            if(p.HandCard.Count == 0)
            {
                WinnerPlayer = p;
                return true;
            }
        }
        return false;
    }

    bool WaitForSecond(float current, float target)
    {
        float t = Mathf.Min(current,target);
        if(t == target)
            return true;
        else
            return false;
    }

    void CardSelected(GameObject cardobj)
    {
        PlayerCard pc = cardobj.GetComponent<PlayerCard>();
        Card c = pc.card;

        if(pc.cardstate == CardState.IDLE)
        {
            playerOnTurn.SelectedCard.Add(c);
            pc.cardstate = CardState.SELECTED;

            if (playerOnTurn.Type == PlayerType.Player)
            {
                //animate card here
                cardobj.transform.localPosition = new Vector3(cardobj.transform.localPosition.x, cardobj.transform.localPosition.y + 0.75f, cardobj.transform.localPosition.z);
            }
        }
        else
        {
            if(pc.cardstate == CardState.SELECTED)
            {
                playerOnTurn.SelectedCard.Remove(pc.card);
                pc.cardstate = CardState.IDLE;
                cardobj.transform.localPosition = new Vector3(cardobj.transform.localPosition.x, 0, cardobj.transform.localPosition.z);
            }
        }
    }

    void CardGroupSelected(GameObject groupobj)
    {
        PlayerCard pc = groupobj.GetComponent<PlayerCard>();
        Card c = pc.card;

        if(pc.cardstate == CardState.IDLE)
        {
            foreach (CardGroup g in playerOnTurn.GroupedCard)
            {
                if (g.GroupContent.Contains(c))
                {
                    playerOnTurn.SelectedCardGroup = g;
                    playerOnTurn.SelectedCard.AddRange(g.GroupContent);
                    pc.cardstate = CardState.SELECTED;
                    if (playerOnTurn.Type == PlayerType.Player)
                    {
                        //animate card here
                        groupobj.transform.localPosition = new Vector3(groupobj.transform.localPosition.x, groupobj.transform.localPosition.y + 0.75f, groupobj.transform.localPosition.z);
                    }
                    break;
                }

            }
        }
        else if(pc.cardstate == CardState.SELECTED)
        {
            playerOnTurn.SelectedCardGroup = null;
            playerOnTurn.SelectedCard.Clear();
            pc.cardstate = CardState.IDLE;
            if (playerOnTurn.Type == PlayerType.Player)
            {
                //animate card here
                groupobj.transform.localPosition = new Vector3(groupobj.transform.localPosition.x, 0, groupobj.transform.localPosition.z);
            }
        }
    }

    public void PlayCard()
    {
        int x = -1;
        CardGroup playedgroup = playerOnTurn.SelectedCardGroup;

        if (playerOnTurn.GroupedCard.Contains(playedgroup))
            playerOnTurn.GroupedCard.Remove(playedgroup);

        foreach (Card c in playedgroup.GroupContent)
        {
            x++;
            PlayerCard pc = playerOnTurn.playerCard.Find(x => x.card == c);
            pc.HideCard = false;
            pc.SetSprite();
            GameObject obj = pc.gameObject;

            obj.transform.parent = TableCardPos;
            obj.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); ;
            obj.transform.localPosition = new Vector3(obj.transform.localRotation.x + (x * 0.4f), 0, 0);
            playerOnTurn.HandCard.Remove(c);
        }

        CardOnTable = playedgroup;

        if (CardOnTable.HandRank == HandRank.Single)
        {
            if(CardOnTable.cardvalue == CardValue.JOKER)
                playedCard.text = CardOnTable.cardvalue.ToString();
            else
                playedCard.text = CardOnTable.cardvalue.ToString() + " " + CardOnTable.suite.ToString();
        }
        else
        {
            playedCard.text = CardOnTable.HandRank.ToString();
        }

        if(playerOnTurn.Type == PlayerType.Player)
        {
            if(playerOnTurn.GroupedCard.Count > 0)
            {
                UpdateGroupedCardInfo();
            }
        }

        StateMachine.ChangeState(GameStates.CHANGETURN);

        enableGroup = false;
        enablePlay = false;
    }

    public void Pass()
    {
        if(playerOnTurn.Type == PlayerType.Player)
        {
            enablePlay = false;
        }

        playerOnTurn.PassTurn = true;

        int passPlayer = 0;
        foreach(Player p in Players)
        {
            if (p.PassTurn)
                passPlayer++;
        }

        if(passPlayer == Players.Count-1)
        {
            CardOnTable = null;
            playedCard.text = "";
            ClearTable();
            ResetPlayerPassStatus();
        }

        StateMachine.ChangeState(GameStates.CHANGETURN);
    }

    public void GroupingCard()
    {
        CardGroup cg = playerOnTurn.SelectedCardGroup;

        totalGroupedCard++;
        int z = -1;
        foreach(Card c in cg.GroupContent)
        {
            z++;
            PlayerCard pc = playerOnTurn.playerCard.Find(x => x.card == c);
            pc.cardstate = CardState.IDLE;
            GameObject obj = pc.gameObject;
            obj.transform.tag = "CardGroup";

            obj.transform.parent = CardGroupPos;
            obj.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f); ;
            obj.transform.localPosition = new Vector3((-2f + totalGroupedCard), 0, (z * 0.5f));
        }

        GroupedCard.text += cg.HandRank.ToString() + "\n";

        playerOnTurn.GroupedCard.Add(cg);
        playerOnTurn.SelectedCardGroup = null;
        playerOnTurn.SelectedCard.Clear();
        enablePlay = false;
        enableGroup = false;
    }

    void PreviewSelectedCards(List<Card> handcard)
    {
        CardGroup g;
        if(handcard.Count == 0)
        {
            enablePlay = false;
            enableGroup = false;
        }
        else if(handcard.Count < 2)
        {
            List<Card> c = new List<Card>(handcard);
            g = new CardGroup(c, HandRank.Single, c[0].suite, c[0].cardvalue, c[0].cardvalue);
            playerOnTurn.SelectedCardGroup = g;
            enablePlay = true;
            enableGroup = false;
        }
        else if(handcard.Count > 1)
        {
            if(handcard.Count == 2)
            {
                if(Dealer.GetPair(handcard))
                {
                    List<Card> c = new List<Card>(handcard);
                    g = new CardGroup(c, HandRank.Pair, default(Suite), c.Max(x => x.cardvalue), c.Max(x => x.cardvalue));
                    playerOnTurn.SelectedCardGroup = g;
                    enablePlay = true;
                    enableGroup = true;
                }
                else
                {
                    enablePlay = false;
                    enableGroup = false;
                }
            }
            if(handcard.Count == 3)
            {
                if (Dealer.GetTriple(handcard))
                {
                    List<Card> c = new List<Card>(handcard);
                    g = new CardGroup(c, HandRank.Triple, default(Suite), c.Max(x => x.cardvalue), c.Max(x => x.cardvalue));
                    playerOnTurn.SelectedCardGroup = g;
                    enablePlay = true;
                    enableGroup = true;
                }
                else
                {
                    enablePlay = false;
                    enableGroup = false;
                }
            }
            if(handcard.Count == 5)
            {
                if (Dealer.GetFourOfKind(handcard))
                {
                    List<Card> x = new List<Card>(handcard);
                    List<Card> c = Dealer.GetFourCard(handcard);
                    g = new CardGroup(x, HandRank.FourofKind, default(Suite), c.Max(x => x.cardvalue), c.Max(x => x.cardvalue));
                    playerOnTurn.SelectedCardGroup = g;
                    enablePlay = true;
                    enableGroup = true;
                }
                else if (Dealer.GetFlush(handcard))
                {
                    List<Card> c = new List<Card>(handcard);
                    g = new CardGroup(c, HandRank.Flush, default(Suite), c.Max(x => x.cardvalue), c.Max(x => x.cardvalue));
                    playerOnTurn.SelectedCardGroup = g;
                    enablePlay = true;
                    enableGroup = true;
                }
                else if (Dealer.GetStraighFlush(handcard))
                {
                    List<Card> c = new List<Card>(handcard);
                    g = new CardGroup(c, HandRank.StraightFlush, default(Suite), c.Max(x => x.cardvalue), c.Max(x => x.cardvalue));
                    playerOnTurn.SelectedCardGroup = g;
                    enablePlay = true;
                    enableGroup = true;
                }
                else if (Dealer.GetStraight(handcard))
                {
                    List<Card> c = new List<Card>(handcard);
                    g = new CardGroup(c, HandRank.Straight, default(Suite), c.Max(x => x.cardvalue), c.Max(x => x.cardvalue));
                    playerOnTurn.SelectedCardGroup = g;
                    enablePlay = true;
                    enableGroup = true;
                }
                else if (Dealer.GetFullHouse(handcard))
                {
                    List<Card> x = new List<Card>(handcard);
                    List<Card> c = Dealer.GetTripleCard(handcard);
                    g = new CardGroup(x, HandRank.FullHouse, default(Suite), c.Max(x => x.cardvalue), c.Max(x => x.cardvalue));
                    playerOnTurn.SelectedCardGroup = g;
                    enablePlay = true;
                    enableGroup = true;
                }
                else
                {
                    enablePlay = false;
                    enableGroup = false;
                }
            }
            if (handcard.Count > 5)
            {
                if (Dealer.GetDragon(handcard))
                {
                    List<Card> c = new List<Card>(handcard);
                    g = new CardGroup(c, HandRank.Dragon, default(Suite), c.Max(x => x.cardvalue), c.Max(x => x.cardvalue));
                    playerOnTurn.SelectedCardGroup = g;
                    enablePlay = true;
                }
                else
                {
                    enablePlay = false;
                }
            }
        }

        if(CardOnTable != null)
        {
            if(playerOnTurn.SelectedCardGroup.HandRank != CardOnTable.HandRank)
            {
                if (playerOnTurn.SelectedCardGroup.cardvalue == CardValue.JOKER)
                    enablePlay = true;
                else
                    enablePlay = false;
            }
            else
            {
                if (playerOnTurn.SelectedCardGroup.totalCardValue > CardOnTable.totalCardValue)
                    enablePlay = true;
                else
                    enablePlay = false;
            }
        }
    }

    void UpdateGroupedCardInfo()
    {
        GroupedCard.text = "";
        foreach (CardGroup cg in playerOnTurn.GroupedCard)
        {
            GroupedCard.text += cg.HandRank.ToString() + "\n";
        }
    }

    void ResetPlayerPassStatus()
    {
        foreach(Player p in Players)
        {
            p.PassTurn = false;
        }
    }

    void ClearTable()
    {
        foreach(Transform t in TableCardPos.transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    void AIPlayGame()
    {
        if (GameLogic.Instance.EZMode)
            Pass();
        else
        {
            playerOnTurn.SelectedCard.Clear();
            playerOnTurn.SelectedCardGroup = null;
            playerOnTurn.GroupedCard.Clear();

            HandRank tablerank;
            if (CardOnTable != null)
                tablerank = CardOnTable.HandRank;
            else
                tablerank = HandRank.Undefined;
            switch (tablerank)
            {
                case HandRank.Undefined:
                case HandRank.Single:
                    for (int i = 0; i < playerOnTurn.playerCard.Count; i++)
                    {
                        if (playerOnTurn.playerCard[i] == playerOnTurn.playerCard.Last())
                        {
                            Pass();
                            playerOnTurn.SelectedCard.Clear();
                            break;
                        }
                        else
                        {
                            CardSelected(playerOnTurn.playerCard[i].gameObject);
                            PreviewSelectedCards(playerOnTurn.SelectedCard);
                        }

                        if (enablePlay)
                        {

                            PlayCard();
                            playerOnTurn.SelectedCard.Clear();
                            break;
                        }
                        else
                        {
                            playerOnTurn.SelectedCard.Clear();
                        }
                    }
                    break;
                case HandRank.Pair:
                    for (int i = 0; i < playerOnTurn.playerCard.Count; i++)
                    {
                        if (playerOnTurn.playerCard[i] == playerOnTurn.playerCard.Last())
                        {
                            Pass();
                            break;
                        }
                        else
                        {
                            CardSelected(playerOnTurn.playerCard[i].gameObject);
                            PreviewSelectedCards(playerOnTurn.SelectedCard);
                        }

                        if (playerOnTurn.SelectedCard.Count == 2)
                        {
                            if (enablePlay)
                            {
                                PlayCard();
                                playerOnTurn.SelectedCard.Clear();
                                break;
                            }
                            else
                            {
                                foreach (PlayerCard pc in playerOnTurn.playerCard)
                                {
                                    pc.cardstate = CardState.IDLE;
                                    pc.transform.localPosition = new Vector3(pc.transform.localPosition.x, 0, pc.transform.localPosition.z);
                                }

                                playerOnTurn.SelectedCard.Clear();
                                CardSelected(playerOnTurn.playerCard[i].gameObject);
                            }
                        }
                    }
                    break;
                case HandRank.Triple:
                    for (int i = 0; i < playerOnTurn.playerCard.Count; i++)
                    {
                        if (playerOnTurn.playerCard[i] == playerOnTurn.playerCard.Last())
                        {
                            Pass();
                            playerOnTurn.SelectedCard.Clear();
                            break;
                        }
                        else
                        {
                            CardSelected(playerOnTurn.playerCard[i].gameObject);
                            PreviewSelectedCards(playerOnTurn.SelectedCard);
                        }

                        if (playerOnTurn.SelectedCard.Count == 3)
                        {
                            if (enablePlay)
                            {
                                PlayCard();
                                playerOnTurn.SelectedCard.Clear();
                                break;
                            }
                            else
                            {
                                foreach (PlayerCard pc in playerOnTurn.playerCard)
                                {
                                    pc.cardstate = CardState.IDLE;
                                    pc.transform.localPosition = new Vector3(pc.transform.localPosition.x, 0, pc.transform.localPosition.z);
                                }
                                playerOnTurn.SelectedCard.Clear();
                                CardSelected(playerOnTurn.playerCard[i].gameObject);
                                CardSelected(playerOnTurn.playerCard[i - 1].gameObject);
                            }
                        }
                    }
                    break;
                case HandRank.FourofKind:
                case HandRank.Flush:
                case HandRank.FullHouse:
                case HandRank.Straight:
                case HandRank.StraightFlush:
                    for (int i = 0; i < playerOnTurn.playerCard.Count; i++)
                    {
                        if (playerOnTurn.playerCard[i] == playerOnTurn.playerCard.Last())
                        {
                            Pass();
                            playerOnTurn.SelectedCard.Clear();
                            break;
                        }
                        else
                        {
                            CardSelected(playerOnTurn.playerCard[i].gameObject);
                            PreviewSelectedCards(playerOnTurn.SelectedCard);
                        }

                        if (playerOnTurn.SelectedCard.Count == 5)
                        {
                            if (enablePlay)
                            {

                                PlayCard();
                                playerOnTurn.SelectedCard.Clear();
                                break;
                            }
                            else
                            {
                                foreach (PlayerCard pc in playerOnTurn.playerCard)
                                {
                                    pc.cardstate = CardState.IDLE;
                                    pc.transform.localPosition = new Vector3(pc.transform.localPosition.x, 0, pc.transform.localPosition.z);
                                }

                                playerOnTurn.SelectedCard.Clear();
                                CardSelected(playerOnTurn.playerCard[i].gameObject);
                                CardSelected(playerOnTurn.playerCard[i - 1].gameObject);
                                CardSelected(playerOnTurn.playerCard[i - 2].gameObject);
                                CardSelected(playerOnTurn.playerCard[i - 3].gameObject);
                            }
                        }
                    }
                    break;
            }
        }
    }

    

    public void EnteringGameplay()
    {
        StateMachine.ChangeState(GameStates.STARTING);
    }

    private void Start()
    {
        EnteringGameplay();
    }

    public void BackToMenuButton()
    {
        SceneManager.UnloadSceneAsync("GameScene"); // Unloading game secen
        GameLogic.Instance.BackToMainMenu();
    }

    void Update()
    { 
        if(EnableInput)
        {
            if (Input.touchCount > 0)
            {
                int layerMask = 1 << 6;
                touches = Input.GetTouch(0);
                touchPos = Camera.main.ScreenToWorldPoint(touches.position);
                if (touches.phase == TouchPhase.Began)
                {
                    if(Physics.Raycast(touchPos, transform.TransformDirection(Vector3.forward), out hit, 100f, layerMask ))
                    {
                        if (hit.collider.tag == "Card")
                        {
                            CardSelected(hit.collider.gameObject);
                            PreviewSelectedCards(playerOnTurn.SelectedCard);
                        }
                        else if (hit.collider.tag == "CardGroup")
                        {
                            CardGroupSelected(hit.collider.gameObject);
                            PreviewSelectedCards(playerOnTurn.SelectedCard);
                        }
                    }    
                }
            }
        }

        StateMachine.Update();
    }
}
