using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameController");
                _instance = go.AddComponent<GameController>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Header("Particle/Spell Settings")]
    public bool shouldAllowParticle = true;

    [Header("Game State")]
    public bool isGamePaused = false;

    [Header("Game Speed")]
    [Range(0.1f, 10.0f)]
    public float gameTimeScale = 1.0f;

    [Header("Wizard Settings")]
    public bool allowWizardMovement = true;

    public bool allowWizardAttack = true;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        Time.timeScale = isGamePaused ? 0f : gameTimeScale;
    }

    public void SetAllowParticle(bool allow)
    {
        shouldAllowParticle = allow;
    }

    public void SetGamePaused(bool paused)
    {
        isGamePaused = paused;
    }

    public void SetGameTimeScale(float scale)
    {
        gameTimeScale = Mathf.Clamp(scale, 0.1f, 3.0f);
    }

    public void SetAllowWizardMovement(bool allow)
    {
        allowWizardMovement = allow;
    }

    public void SetAllowWizardAttack(bool allow)
    {
        allowWizardAttack = allow;
    }
}

