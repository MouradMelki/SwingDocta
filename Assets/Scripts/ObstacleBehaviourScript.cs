using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehaviourScript : MonoBehaviour {

    public List<GameObject> m_obstacles;

    private GameObject m_obstacle;
    private bool m_tunnelMachinePassed;

    void Start()
    {
        m_obstacle = Instantiate(m_obstacles[Random.Range(0, m_obstacles.Count)]);
        m_obstacle.GetComponent<MachineControl>().m_player = gameObject;
    }

    void Update()
    {
        if (!m_obstacle)
        {
            GameObject newObstacle = Instantiate(m_obstacles[Random.Range(0, m_obstacles.Count)]);
            m_obstacle = newObstacle;
            m_obstacle.GetComponent<MachineControl>().m_player = gameObject;
        }
    }
}
