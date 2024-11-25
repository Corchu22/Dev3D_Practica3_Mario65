using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRestartGameElements
{
    void RestartGame();
}
public class GameManager : MonoBehaviour
{
    static GameManager m_GameManager;   


    public Animation m_deathUi;

    public List<IRestartGameElements> m_RestartGameElements = new List<IRestartGameElements>();
    // Start is called before the first frame update
    void Awake()
    {
        if (m_GameManager == null)
        {
            m_GameManager = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }
    static public GameManager GetGameManager()
    {
        return m_GameManager;
    }

    public void AddRestartGameElement(IRestartGameElements RestartGameElement)
    {
        m_RestartGameElements.Add(RestartGameElement);
    }
    public void RestartGame()
    {
        foreach(IRestartGameElements l_RestartGameElement in m_RestartGameElements)
        {
            l_RestartGameElement.RestartGame();
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
}
