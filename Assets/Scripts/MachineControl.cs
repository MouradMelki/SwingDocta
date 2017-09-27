using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineControl : MonoBehaviour {

    public GameObject m_lowerObject;
    public GameObject m_upperObject;
    public GameObject m_player;
    public float m_distanceToDestroy;
    public float m_maxDistance;
    public float m_minDistance = 12f;
    public float m_createDistance = 15f;
    public float m_yDistance = 4f;

    private float m_distance;

    void Start () {
        transform.position = new Vector3(m_createDistance + m_player.transform.position.x, -29f);

        if(m_lowerObject && m_upperObject && m_maxDistance > m_minDistance)
        {
            m_distance = Random.Range(m_minDistance, m_maxDistance);
            transform.position = new Vector2(transform.position.x, Random.Range(-m_yDistance + transform.position.y,
                m_yDistance + transform.position.y));
            m_lowerObject.transform.localPosition = new Vector2(0f, -m_distance / 2);
            m_upperObject.transform.localPosition = new Vector2(0f, m_distance / 2);
        }
    }

    private void Update()
    {
        if (m_player.transform.position.x - transform.position.x > m_distanceToDestroy)
            Destroy(gameObject);
    }
}
