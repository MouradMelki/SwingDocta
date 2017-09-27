using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GateBehaviour : MonoBehaviour {

    public GameObject m_player;
    public GameObject m_camera;
    public GameObject m_canvas;
    public float m_restartDelay;
    public float m_offset;

    private float m_distancePlayerGate;
    private float t = 0f;
    private float m_restartTimer;
    private bool m_crushPlayer;
    private bool m_startGame;
    private Vector2 moveAxis = Vector2.right;

    void Start () {
        m_distancePlayerGate = m_player.transform.position.x - transform.position.x;
    }

    void Update()
    {
        m_crushPlayer = m_player.GetComponent<PlayerLoose>().m_crushPlayer;
        m_startGame = m_canvas.GetComponent<CanvasControl>().m_startGame;

        if (m_camera.transform.position.x - transform.position.x < -20f)
            moveAxis = Vector2.zero;

        if (m_crushPlayer)
        {
            // .. increment a timer to count up to restarting.
            m_restartTimer += Time.deltaTime;

            // .. if it reaches the restart delay...
            if (m_restartTimer >= m_restartDelay)
            {
                // .. then reload the currently loaded level.
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    void FixedUpdate ()
    {
        if (m_startGame)
        {
            if (!m_crushPlayer)
            {
                if (t <= 1f)
                    t += 0.6f * Time.deltaTime;

                transform.position = new Vector3(m_player.transform.position.x -
                    Mathf.Lerp(m_distancePlayerGate, m_distancePlayerGate + m_offset, t),
                    transform.position.y, transform.position.z);
            }
            else if (m_crushPlayer)
                transform.Translate(moveAxis * (m_player.GetComponent<PlayerControls>().m_speed + 9) * Time.deltaTime, Space.World);
        }
    }
}
