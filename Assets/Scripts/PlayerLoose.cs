using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoose : MonoBehaviour {

    public GameObject m_gate;
    public GameObject m_camera;
    public float newGateOffset = 0f;
    public float m_restartDelay;
    public bool m_crushPlayer = false;

    private float tGateAppear = 0f;
    private float tGateDisappear = 0f;
    private float timeOnCollision = 0f;
    private float time = 0f;
    private float m_originalOffset;
    private bool m_isSealing;
    private bool m_isFloor;
    private bool m_deadZone;
    private int m_hitCounter;

    void Start () {
        m_originalOffset = m_gate.GetComponent<GateBehaviour>().m_offset;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Gate")
            gameObject.SetActive(false);
    }

    void Update ()
    {
        m_isFloor = GetComponent<PlayerAnimation>().m_isFloor;
        m_isSealing = GetComponent<PlayerAnimation>().m_isSealing;
        m_deadZone = GetComponent<PlayerAnimation>().m_isDeadZone;
        m_hitCounter = GetComponent<PlayerAnimation>().m_hitCounter;
        CollisionTimer();

        if (m_gate)
        {
            if (!m_isFloor && m_gate.GetComponent<GateBehaviour>().m_offset == m_originalOffset)
            {
                tGateAppear = 0f;
                tGateDisappear = 0f;
            }
        }

        if (((m_isFloor || m_isSealing) && timeOnCollision >= 3f) || m_deadZone || m_hitCounter >= 3)
            m_crushPlayer = true;
    }

    void FixedUpdate()
    {
        if (m_gate)
        {
            if (!m_isFloor && !m_isSealing)
            {
                tGateDisappear += 0.6f * Time.deltaTime;
                if (tGateDisappear > 3f)
                {
                    m_gate.GetComponent<GateBehaviour>().m_offset = Mathf.Lerp(m_gate.GetComponent<GateBehaviour>().m_offset,
                        m_originalOffset, tGateDisappear - 3f);
                    timeOnCollision = 0f;
                }
            }

            if (m_isFloor && timeOnCollision < 5f)
            {
                tGateAppear += 0.1f * Time.deltaTime;
                m_gate.GetComponent<GateBehaviour>().m_offset = Mathf.Lerp(m_gate.GetComponent<GateBehaviour>().m_offset,
                    newGateOffset, tGateAppear);
            }
        }
    }

    void CollisionTimer() //claculate the time pressing space
    {
        if (m_isFloor || m_isSealing)
            timeOnCollision += Time.deltaTime;
        else if (!m_isSealing && !m_isFloor && timeOnCollision > 3f)
            timeOnCollision = 0f;
    }
}
