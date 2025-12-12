using UnityEngine;
using System.Collections;
using TMPro;
public class ZombieSystem : MonoBehaviour
{
    public int round;
    private int baseNumZombz = 6;
    private int zombiesInRound;
    private int zombiesSpawnedInRound = 0;
    private int zombiesKilled = 0;
    float zombieSpawnTimer = 0;
    public Transform[] zombieSpawnPoints;
    public GameObject zombieEnemy;

    static int playerScore = 0;
    static int playerPoints = 0;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI pointText;
    public TextMeshProUGUI roundText;

    private bool delayingRound = false;

    public static ZombieSystem Instance;

    private void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    private void Start()
    {
        StartRound();
    }

    void Update()
    {
        HandleZombieSpawn();

        if(!delayingRound && zombiesInRound > 0 && zombiesKilled >= zombiesInRound)
        {
            delayingRound = true;
            StartCoroutine(RoundTransition());
        }
    }

    private IEnumerator RoundTransition()
    {
        yield return new WaitForSeconds(8f);
        NextRound();
        delayingRound = false;
    }

    public static void AddPoints(int pointValue)
    {
        playerScore += pointValue;
        playerPoints += pointValue;

        if(Instance != null) Instance.UpdateUI();
    }

    private void UpdateUI()
    {
        if(scoreText != null) scoreText.text = playerScore.ToString();
        if(pointText != null) pointText.text = playerPoints.ToString();
        if(roundText != null) roundText.text = round.ToString();
    }

    private void StartRound()
    {
        round = 1;
        zombiesInRound = baseNumZombz;
        zombiesSpawnedInRound = 0;
        zombiesKilled = 0;
        zombieSpawnTimer = 0f;
    }

    private void NextRound()
    {
        round++;
        if(Instance != null) Instance.UpdateUI();

        zombiesInRound = Mathf.RoundToInt(zombiesInRound * 1.2f);
        zombiesSpawnedInRound = 0;
        zombiesKilled = 0;
        zombieSpawnTimer = 0f;
    }

    private void HandleZombieSpawn()
    {
        if(zombiesSpawnedInRound < zombiesInRound)
        {
            if(zombieSpawnTimer > 3)
            {
                SpawnZombie();
                zombieSpawnTimer = 0;
            }
            else
            {
                zombieSpawnTimer += Time.deltaTime;
            }
        }
    }

    private void SpawnZombie()
    {
        Vector3 randomSpawnPoint = zombieSpawnPoints[Random.Range (0, zombieSpawnPoints.Length)].position;
        Instantiate(zombieEnemy, randomSpawnPoint, Quaternion.identity);
        zombiesSpawnedInRound++;
    }

    public void OnZombieKilled()
    {
        zombiesKilled++;
    }

}
