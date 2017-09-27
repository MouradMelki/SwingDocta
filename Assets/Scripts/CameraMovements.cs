using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovements : MonoBehaviour {

    public GameObject player;

    private float m_offset;
    private float m_camX;
    private bool m_crushPlayer;

    private void Awake()
    {
        m_offset = transform.position.x - player.transform.position.x;
    }

    void Update()
    {
        m_crushPlayer = player.GetComponent<PlayerLoose>().m_crushPlayer;
    }

    void LateUpdate()
    {
        if (!m_crushPlayer)
        {
            if (player.transform.position.x >= m_camX)
                transform.position = new Vector3(player.transform.position.x + m_offset, transform.position.y, transform.position.z);
            m_camX = player.transform.position.x;
        }
    }
}
