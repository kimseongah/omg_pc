using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BadmintonScoreManager : MonoBehaviour
{
    private int playerScore;
    private int opponentScore;
    public TextMeshProUGUI scoreText, player1, player2;

    public static BadmintonScoreManager Instance { get; private set; }

    void Awake()
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
    }

    // 게임 시작 시점에 점수 초기화
    void Start()
    {
        ResetScores();

        player1.text = SocketManager.instance.player[0].name;
        player2.text = SocketManager.instance.player[1].name;
    }

    // 플레이어 점수 증가
    public void AddPointToPlayer()
    {
        playerScore++;
        UpdateScoreDisplay();
    }

    // 상대방 점수 증가
    public void AddPointToOpponent()
    {
        opponentScore++;
        UpdateScoreDisplay();
    }

    // 점수 디스플레이 업데이트
    private void UpdateScoreDisplay()
    {
        scoreText.text = $"{playerScore} : {opponentScore}";
        // UI 업데이트 로직 추가
    }

    // 점수 초기화
    public void ResetScores()
    {
        playerScore = 0;
        opponentScore = 0;
        UpdateScoreDisplay();
    }
}
