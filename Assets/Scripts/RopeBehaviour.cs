using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBehaviour : MonoBehaviour {

    public GameObject m_ropeJoint;
    public GameObject m_ropeEnd;
    public GameObject m_player;
    public GameObject m_hand;
    public float m_distanceToDestroy;
    public bool release = false;

    private float t = 0f;
    private float Xscale;
    private float diff;
    private float sin;
    private float distance;
    private float ropeJointX;
    private Vector2 ropeDistance;

    void Start()
    {
        if (m_player.GetComponent<PlayerControls>().m_frontAnchor)
            m_ropeEnd.GetComponent<SpriteRenderer>().sortingOrder = 4;

        m_ropeEnd.GetComponent<DistanceJoint2D>().distance = m_player.GetComponent<DistanceJoint2D>().distance;
        m_ropeEnd.GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    void Update()
    {
        t += 3.5f * Time.deltaTime;
        ropeDistance = m_ropeEnd.transform.position - m_ropeJoint.transform.position;
        distance = ropeDistance.magnitude;
        Xscale = distance / m_ropeEnd.GetComponent<SpriteRenderer>().size.x;
        m_ropeEnd.transform.localScale = new Vector3(Mathf.Lerp(0f, Xscale, t), m_ropeEnd.transform.localScale.y, m_ropeEnd.transform.localScale.z);

        if (!release)
        {
            m_ropeEnd.transform.position = m_hand.transform.position;
            diff = m_hand.transform.position.x - ropeJointX;
        }
        else
            diff = m_ropeEnd.transform.position.x - ropeJointX;


        if (m_player.transform.position.x - transform.position.x > m_distanceToDestroy)
            Destroy(gameObject);
        ropeJointX = m_ropeJoint.transform.position.x;
        sin = diff / distance;
    }

    void FixedUpdate()
    {
        if (sin <= 1 && sin >= -1)
                m_ropeEnd.transform.eulerAngles = new Vector3(0f, 0f,
                    90 + (Mathf.Asin(sin) * 180 / Mathf.PI));
    }
}
