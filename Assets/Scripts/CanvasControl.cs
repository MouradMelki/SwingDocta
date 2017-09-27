using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasControl : MonoBehaviour {

    public GameObject m_player;
    public Text m_score;
    public Image m_gameOver;
    public Image Live1;
    public Image Live2;
    public Image Live3;
    public Button m_pressToPlay;
    public bool m_startGame = false;

    void Start()
    {

    }

    void Update()
    {
        if(m_pressToPlay)
            m_pressToPlay.GetComponent<Button>().onClick.AddListener(TaskOnClick);

        if (!m_player.GetComponent<PlayerLoose>().m_crushPlayer)
            m_score.GetComponent<Text>().text = Mathf.RoundToInt(m_player.transform.position.x).ToString();

        if (m_player.GetComponent<PlayerLoose>().m_crushPlayer)
            m_gameOver.GetComponent<Animator>().SetBool("GameOver", true);

        switch (m_player.GetComponent<PlayerAnimation>().m_hitCounter)
        {
            case 1:
                if(Live1)
                    Destroy(Live1.GetComponent<Image>());
                break;
            case 2:
                if (Live2)
                    Destroy(Live2.GetComponent<Image>());
                break;
            case 3:
                if (Live3)
                    Destroy(Live3.GetComponent<Image>());
                break;
        }
    }

    void TaskOnClick()
    {
        m_startGame = true;
        m_player.GetComponent<DistanceJoint2D>().enabled = true;
        Destroy(m_pressToPlay.GetComponent<Button>());
        Destroy(m_pressToPlay.GetComponent<Image>());
    }
}
