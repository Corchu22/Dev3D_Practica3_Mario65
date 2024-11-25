using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IRestartGameElement
{
    void RestartGame();
}
public class GameManager : MonoBehaviour
{
    static GameManager m_GameManager;
    List<IRestartGameElement> m_RestartGameElement = new List<IRestartGameElement>();

    private void Awake()
    {
        if (m_GameManager == null)
        {
            m_GameManager = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
            GameManager.Destroy(gameObject);
    }

    static public GameManager GetGameManager()
    {
        return m_GameManager;
    }

    public void AddRestartGameElement(IRestartGameElement RestartgameElement)
    {
        m_RestartGameElement.Add(RestartgameElement);
    }

    public void RestartGame()
    {
        foreach (IRestartGameElement l_Restart in m_RestartGameElement)
            l_Restart.RestartGame();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

}
