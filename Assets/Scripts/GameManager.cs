using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public float timeBeforeNextRound;
    public float timeBeforeNextMatch;
    public float slimeResetX;
    public float ballResetY;
    public int pointsToWin;

    private int scoreLeft;
    private int scoreRight;
    private TextMeshProUGUI textLeft;
    private TextMeshProUGUI textRight;
    private GameObject winText;
    private GameObject ball;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        InitGame();
    }

    void InitGame()
    {
        scoreLeft = 0;
        scoreRight = 0;

        textLeft = GameObject.Find("ScoreLeft").GetComponent<TextMeshProUGUI>();
        textRight = GameObject.Find("ScoreRight").GetComponent<TextMeshProUGUI>();
        winText = GameObject.Find("WinText");
        winText.SetActive(false);

        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    public void AddPoint(bool left)
    {
        if (left)
        {
            scoreRight++;
        } 
        else
        {
            scoreLeft++;
        }
        UpdateUi();

        if(scoreRight == pointsToWin || scoreLeft == pointsToWin)
        {
            winText.SetActive(true);
            if (scoreRight < scoreLeft)
            {
                winText.GetComponent<TextMeshProUGUI>().text = "J1 WON";
            } 
            else
            {
                winText.GetComponent<TextMeshProUGUI>().text = "J2 WON";
            }
            StartCoroutine("NextGame");
        }
        else
        {
            StartCoroutine("NextRound");
        }
    }

    void UpdateUi()
    {
        textLeft.text = scoreLeft.ToString();
        textRight.text = scoreRight.ToString();
    }

    IEnumerator NextRound()
    {
        yield return new WaitForSeconds(timeBeforeNextRound);
        Reset();
        yield return null;
    }

    IEnumerator NextGame()
    {
        yield return new WaitForSeconds(timeBeforeNextMatch);
        Reset();

        scoreLeft = 0;
        scoreRight = 0;
        UpdateUi();

        winText.SetActive(false);
        yield return null;
    }

    private void Reset()
    {
        // Destroy ball pieces
        GameObject pieces = GameObject.FindGameObjectWithTag("BallPieces");
        Destroy(pieces);

        // Reset players positions
        GameObject p1 = GameObject.Find("Slime1");
        p1.GetComponent<Slime>().Init(new Vector3(-slimeResetX, 0, 0));
        GameObject p2 = GameObject.Find("Slime2");
        p2.GetComponent<Slime>().Init(new Vector3(slimeResetX, 0, 0));

        // Reset ball position
        ball.GetComponent<Rigidbody2D>().transform.position = new Vector3(0, ballResetY, 0);
        ball.GetComponent<Ball>().Init();
    }
}
