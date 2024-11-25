using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaController : MonoBehaviour,IRestartGameElement
{
    CharacterController characterController;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

    }

    void Start()
    {
        
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        GameManager.GetGameManager().AddRestartGameElement(this);
    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
        characterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        characterController.enabled = true;
    }

    public void Kill()
    {
        gameObject.SetActive(false);
    }
}
