using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    public Transform m_FollowObject; 
    float m_Yaw = 0.0f;
    float m_Pitch = 0.0f;
    public float m_MaxPitch = 60.0f;
    public float m_MinPitch = -60.0f;
    public float m_MinCameradistance = 5.0f;
    public float m_ManxCameradistance = 15.0f;
    public float m_YawSpeed = 0.0f;
    public float m_Pitchspeed = 0.0f;
    public LayerMask m_layerMask;
    public float m_OffsetHit = 0.1f;

    private float m_NoMovementTimer = 0.0f; 
    private bool m_IsIdle = false; 
    private bool m_IsCameraBehind = false; 
    private Vector3 m_LastPosition; 

    private Vector3 m_TargetPosition; 
    private Quaternion m_TargetRotation;
    private Vector3 m_SmoothPositionVelocity; 
    private float m_SmoothRotationVelocity; 
    public float m_SmoothTime = 0.5f; 

    private void Start()
    {
        m_LastPosition = m_FollowObject.position;
    }

    private void LateUpdate()
    {
        bool isMoving = Vector3.Distance(m_FollowObject.position, m_LastPosition) > 0.1f ||
                        Mathf.Abs(Input.GetAxis("Mouse X")) > 0.1f ||
                        Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.1f;

        if (isMoving)
        {
            m_NoMovementTimer = 0.0f; 
            m_IsIdle = false; 

            if (m_IsCameraBehind)
            {
                m_IsCameraBehind = false;
            }
        }
        else
        {
            m_NoMovementTimer += Time.deltaTime; 
        }

        m_LastPosition = m_FollowObject.position; 

        if (m_NoMovementTimer >= 5.0f && !m_IsCameraBehind)
        {
            m_IsCameraBehind = true; 
            PositionCameraBehindMario(); 
        }

        if (!m_IsCameraBehind)
        {
            HandleCameraRotation();
        }
        else
        {
            HandleCameraRotation();

            transform.position = Vector3.SmoothDamp(transform.position, m_TargetPosition, ref m_SmoothPositionVelocity, m_SmoothTime);

            transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRotation, m_SmoothTime * Time.deltaTime);
        }
    }

    private void HandleCameraRotation()
    {
        float l_HorizontalAxis = Input.GetAxis("Mouse X");
        float l_VerticalAxis = Input.GetAxis("Mouse Y");

        Vector3 l_lookDirection = m_FollowObject.position - transform.position;
        float l_DistanceToplayer = l_lookDirection.magnitude;
        l_lookDirection.y = 0.0f;
        l_lookDirection.Normalize();

        m_Yaw = Mathf.Atan2(l_lookDirection.x, l_lookDirection.z) * Mathf.Rad2Deg;

        m_Yaw += l_HorizontalAxis * m_YawSpeed * Time.deltaTime;
        m_Pitch += l_VerticalAxis * m_Pitchspeed * Time.deltaTime;

        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
        float l_YawInRadiant = m_Yaw * Mathf.Deg2Rad;
        float l_PitchInRadiants = m_Pitch * Mathf.Deg2Rad;

        Vector3 l_CameraForward = new Vector3(Mathf.Sin(l_YawInRadiant) * Mathf.Cos(l_PitchInRadiants),
            Mathf.Sin(l_PitchInRadiants),
            Mathf.Cos(l_YawInRadiant) * Mathf.Cos(l_PitchInRadiants));
        l_DistanceToplayer = Mathf.Clamp(l_DistanceToplayer, m_MinCameradistance, m_ManxCameradistance);
        Vector3 l_DesiredPosition = m_FollowObject.position - l_CameraForward * l_DistanceToplayer;

        Ray l_Ray = new Ray(m_FollowObject.position, -l_CameraForward);
        if (Physics.Raycast(l_Ray, out RaycastHit l_RayCastHit, l_DistanceToplayer, m_layerMask.value))
        {
            l_DesiredPosition = l_RayCastHit.point + l_CameraForward * m_OffsetHit;
        }

        transform.position = l_DesiredPosition;
        transform.LookAt(m_FollowObject.position);
    }

    private void PositionCameraBehindMario()
    {
        m_TargetPosition = m_FollowObject.position - m_FollowObject.forward * m_ManxCameradistance;
        m_TargetRotation = Quaternion.LookRotation(m_FollowObject.position - m_TargetPosition);

        m_SmoothPositionVelocity = Vector3.zero;
        m_SmoothRotationVelocity = 0f;
    }
}
