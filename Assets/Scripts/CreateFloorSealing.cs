using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFloorSealing : MonoBehaviour {

    public GameObject m_sealing;
    public GameObject m_floor;
    public GameObject m_player;
    public float m_offset;
    
    private float m_distance = 0f;
    private int counter = 0;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && counter < 1 && !m_player.GetComponent<PlayerLoose>().m_crushPlayer)
        {
            counter++;
            GameObject m_newFloor = Instantiate(m_floor,
                m_floor.gameObject.transform.localPosition + new Vector3(m_floor.GetComponent<BoxCollider2D>().size.x - m_offset, 0f, 0f),
                Quaternion.identity);
        }
    }
	
	void Update () {
        if (!m_player.GetComponent<PlayerLoose>().m_crushPlayer)
        {
            m_distance = m_player.transform.position.x - transform.position.x;
            if (m_player.transform.position.x - transform.position.x > m_floor.GetComponent<BoxCollider2D>().size.x)
                Destroy(m_floor);
        }
    }
}
