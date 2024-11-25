using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MarioController : MonoBehaviour, IRestartGameElement
{
    public enum TpunchType
    {
        RIGHT_HAND = 0,
        LEFT_HAND,
        KICK
    }

    CharacterController m_CharacterController;
    Animator m_Animator;
    public Camera m_Camera;

    Checkpoint m_CurrentCheckpoint;

    public float m_WalkSpeed = 2.0f;
    public float m_RunSpeed = 8.0f;
    public float m_LerpRotationPct = 0.8f;
    float m_VerticalSpeed = 0.0f;

    [Header("Punch")]
    public float m_PunchComboAvailableTime = 1.3f;
    int m_CurrentPunchId = 0;
    float m_lastPunchTime;

    [Header("Punch Colliders")]
    public GameObject m_LeftHandPunchHitCollider;
    public GameObject m_RightHandPunchHitCollider;
    public GameObject m_RightFootKickHitCollider;


    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public int m_PunchHitButton = 0;
    public KeyCode m_JumpKeyCode = KeyCode.Space;


    Vector3 m_StartPosition;
    Quaternion m_StartRotation;


    [Header("Jump")]

    public float m_JumpVerticalSpeed = 5.0f;
    public float m_killJumpVerticalSpeed = 8.0f;
    public float m_WaitStartJumpTime = 0.12f;
    public float m_MaxAngleNeededToKillGoomba = 15.0f;
    public float m_MinVerticalSpeedTokillGoomba = -1.0f;

    private bool m_IsDead = false; 
    public Transform m_RespawnPoint; 

    
    private float m_IdleTime = 0.0f; 
    private bool m_HasMovement = false; 

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    void Start()
    {
        m_LeftHandPunchHitCollider.gameObject.SetActive(false);
        m_RightHandPunchHitCollider.gameObject.SetActive(false);
        m_RightFootKickHitCollider.gameObject.SetActive(false);
        GameManager.GetGameManager().AddRestartGameElement(this);
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
    }

    void Update()
    {
        
        Vector3 l_Forward = m_Camera.transform.forward;
        Vector3 l_Right = m_Camera.transform.right;
        l_Forward.y = 0.0f;
        l_Right.y = 0.0f;
        l_Forward.Normalize();
        l_Right.Normalize();

        m_HasMovement = false;
        Vector3 l_Movement = Vector3.zero;

        if (Input.GetKey(m_RightKeyCode))
        {
            l_Movement = l_Right;
            m_HasMovement = true;
        }
        else if (Input.GetKey(m_LeftKeyCode))
        {
            l_Movement = -l_Right;
            m_HasMovement = true;
        }

        if (Input.GetKey(m_UpKeyCode))
        {
            l_Movement += l_Forward;
            m_HasMovement = true;
        }
        else if (Input.GetKey(m_DownKeyCode))
        {
            l_Movement -= l_Forward;
            m_HasMovement = true;
        }

        if (!m_HasMovement)
        {
            m_IdleTime += Time.deltaTime; 
        }
        else
        {
            m_IdleTime = 0.0f; 
        }

       
        if (m_IdleTime >= 10.0f) 
        {
            m_Animator.SetBool("SpecialIdle", true); 
        }
        else
        {
            m_Animator.SetBool("SpecialIdle", false); 
        }

        
        float l_Speed = 0.0f;

        if (m_HasMovement)
        {
            if (Input.GetKey(m_RunKeyCode))
            {
                l_Speed = m_RunSpeed;
                m_Animator.SetFloat("Speed", 1.0f);
            }
            else
            {
                l_Speed = m_WalkSpeed;
                m_Animator.SetFloat("Speed", 0.2f);
            }

           
            Quaternion l_desiredRotation = Quaternion.LookRotation(l_Movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, l_desiredRotation,
                m_LerpRotationPct * Time.deltaTime);
        }
        else
        {
            m_Animator.SetFloat("Speed", 0.0f);
        }

        if (CanJump() && Input.GetKeyDown(m_JumpKeyCode))
            Jump();
       
        l_Movement.Normalize();
        l_Movement = l_Movement * l_Speed * Time.deltaTime;

        
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.Below) != 0 && m_VerticalSpeed < 0.0f)
            m_Animator.SetBool("Falling", false);
        else
            m_Animator.SetBool("Falling", true);

        if (((l_CollisionFlags & CollisionFlags.Below) != 0 && m_VerticalSpeed < 0.0f) ||
                (l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
                m_VerticalSpeed = 0.0f;

            UpdatePunch();

        m_CharacterController.Move(l_Movement);

    }

    bool CanJump()
    {
        return true;
    }

    void Jump()
    {
        m_Animator.SetTrigger("Jump");
        StartCoroutine(ExecuteJump());
    }

    IEnumerator ExecuteJump()
    {
        yield return new WaitForSeconds(m_WaitStartJumpTime); 
        m_VerticalSpeed = m_JumpVerticalSpeed;
        m_Animator.SetBool("Falling", false);
    }


    void UpdatePunch()
    {
        if(Input.GetMouseButtonDown(m_PunchHitButton)&& CanPunch())
        {
            PunchCombo();
        }
    }

    bool CanPunch()
    {
        return true;
    }

    void PunchCombo()
    {
        m_Animator.SetTrigger("Punch");
        float l_DiffTime = Time.time - m_lastPunchTime;
        if (l_DiffTime <= m_PunchComboAvailableTime)
        {
            m_CurrentPunchId = (m_CurrentPunchId + 1);
            if (m_CurrentPunchId >= 3)
                m_CurrentPunchId = 0;
                //m_CurrentPunchId=(m_CurrentPunchId+1)%3;
        }
        else
            m_CurrentPunchId = 0;
        m_lastPunchTime = Time.time;
        m_Animator.SetInteger("PunchCombo", m_CurrentPunchId);
    }

    public void EnableHitCollider(TpunchType PunchType, bool Active)
    {
        switch (PunchType)
        {
            case TpunchType.LEFT_HAND:
                m_LeftHandPunchHitCollider.SetActive(Active);
                break;
            case TpunchType.RIGHT_HAND:
                m_RightHandPunchHitCollider.SetActive(Active);
                break;
            case TpunchType.KICK:
                m_RightFootKickHitCollider.SetActive(Active);
                break;
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.CompareTag("Goomba")) 
        {
            if (IsUpperHit(hit.transform))
            {
                hit.gameObject.GetComponent<GoombaController>().Kill();
                m_VerticalSpeed = m_killJumpVerticalSpeed;
            }
            else
            {
                Debug.Log("player must be hit");
            }
        }
            
    }


    bool IsUpperHit(Transform GoombaTransform)
    {
        Vector3 l_GoombaDirection = transform.position - GoombaTransform.position;
        l_GoombaDirection.Normalize();
        float l_DotAngle = Vector3.Dot(l_GoombaDirection, Vector3.up);
        Debug.Log("m_VerticalSpeed" + m_VerticalSpeed);
        if (l_DotAngle >= Mathf.Cos(m_MaxAngleNeededToKillGoomba * Mathf.Deg2Rad) && m_VerticalSpeed <= m_MinVerticalSpeedTokillGoomba)
            return true;
        return false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            m_CurrentCheckpoint = other.GetComponent<Checkpoint>();
        }
    }
    public void RestartGame()
    {
        m_CharacterController.enabled = false;

        if (m_CurrentCheckpoint == null)
        {
            transform.position = m_StartPosition;
            transform.rotation = m_StartRotation;
        }
        else
        {
            transform.position = m_CurrentCheckpoint.m_RespawnPosition.position;
            transform.rotation = m_CurrentCheckpoint.m_RespawnPosition.rotation;
        }

       
        m_CharacterController.enabled = true;
    }


    public void Die()
    {
        if (m_IsDead) return; 

        m_IsDead = true; 
        m_Animator.SetBool("IsDead", true); 
        m_CharacterController.enabled = false; 

        Invoke(nameof(Respawn), 3.0f);
    }


    private void Respawn()
    {
        Debug.Log("Respawning at: " + (m_RespawnPoint != null ? m_RespawnPoint.position.ToString() : "No Respawn Point!"));

        
        if (m_RespawnPoint != null)
        {
            transform.position = m_RespawnPoint.position;
        }
        else
        {
            Debug.LogWarning("No Respawn Point assigned!");
        }

        m_Animator.SetBool("IsDead", false); 
        m_Animator.Play("Blend Tree", 0, 0f);

        m_IsDead = false;
        m_CharacterController.enabled = true; 
    }



    public void Step(AnimationEvent _Animation)
    {

    }
}
