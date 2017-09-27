using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundControl : MonoBehaviour {

    public List<Sprite> m_backgrounds;
    public float m_speed;
    public GameObject m_player;
    
    private float m_distance = 0f;
    private int counter = 0;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && counter < 1 && !m_player.GetComponent<PlayerLoose>().m_crushPlayer)
        {
            counter++;
            GameObject m_newBackground = Instantiate(gameObject,
                transform.position + new Vector3(GetComponent<SpriteRenderer>().size.x * transform.localScale.x, 0f, 0f),
                Quaternion.identity);
            m_newBackground.GetComponent<SpriteRenderer>().sprite = m_backgrounds[Random.Range(0, m_backgrounds.Count)];
        }
    }

    void FixedUpdate()
    {
        if (!m_player.GetComponent<PlayerLoose>().m_crushPlayer)
        {
            m_distance = m_player.transform.position.x - transform.position.x;
            if (m_player.transform.position.x - transform.position.x > GetComponent<SpriteRenderer>().size.x * transform.localScale.x)
                Destroy(gameObject);
        }
    }
}
