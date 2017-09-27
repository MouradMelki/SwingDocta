using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeJointsControls : MonoBehaviour {


    public GameObject m_frontSwingPoint;
    public GameObject m_backSwingPoint;
    public GameObject m_backHand;
    public GameObject m_frontHand;
    public GameObject m_player;
    public GameObject m_canvas;
    public float m_speedOfJoint;
    public float m_maxDistanceOfJoint;

    private float maxLengthOfRope;
    private float maxXPos = 0f;
    private Vector2 moveAxis;
    private bool m_backAnchor;
    private bool m_frontAnchor;
    private bool m_startGame;

    void Start()
    {
        maxLengthOfRope = m_player.GetComponent<DistanceJoint2D>().distance;
        m_maxDistanceOfJoint = CalculateMaxDistanceOfJoint(maxLengthOfRope, transform.position.y - m_backHand.transform.position.y);
        moveAxis = new Vector2(1, 0f);
    }

    void Update()
    {
        m_startGame = m_canvas.GetComponent<CanvasControl>().m_startGame;
    }

    void FixedUpdate()
    {
        if (m_startGame)
        {
            if (!m_player.GetComponent<PlayerLoose>().m_crushPlayer)
            {
                m_backAnchor = m_player.GetComponent<PlayerControls>().m_backAnchor;
                m_frontAnchor = m_player.GetComponent<PlayerControls>().m_frontAnchor;
                if (m_frontAnchor && !m_backAnchor)
                {
                    m_maxDistanceOfJoint = CalculateMaxDistanceOfJoint(maxLengthOfRope, transform.position.y - m_backHand.transform.position.y);
                    maxXPos = m_backHand.transform.position.x + m_maxDistanceOfJoint;

                    //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBGL
                    if (Input.GetKey(KeyCode.Space) || Input.GetKeyUp(KeyCode.Space))
                    {
                        if (m_backSwingPoint.transform.position.x >= maxXPos)
                            m_backSwingPoint.transform.position = new Vector2(maxXPos, transform.position.y);
                        else
                            m_backSwingPoint.transform.Translate(moveAxis * m_speedOfJoint * Time.deltaTime, Space.World);
                    }
                    else
                        m_backSwingPoint.transform.position = new Vector2(m_backHand.transform.position.x, transform.position.y);
                    //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE        
                    if (Input.touchCount > 0)
                    {
                        Touch myTouch = Input.touches[0];
                        if (myTouch.phase == TouchPhase.Stationary || myTouch.phase == TouchPhase.Ended)
                        {
                            if (m_backSwingPoint.transform.position.x >= maxXPos)
                                m_backSwingPoint.transform.position = new Vector2(maxXPos, transform.position.y);
                            else
                                m_backSwingPoint.transform.Translate(moveAxis * m_speedOfJoint * Time.deltaTime, Space.World);
                        }
                    }
                    else
                        m_backSwingPoint.transform.position = new Vector2(m_backHand.transform.position.x, transform.position.y);
#endif //End of mobile platform dependendent compilation section started above with #elif
                }

                if (!m_frontAnchor)
                {
                    m_maxDistanceOfJoint = CalculateMaxDistanceOfJoint(maxLengthOfRope, transform.position.y - m_frontHand.transform.position.y);
                    maxXPos = m_frontHand.transform.position.x + m_maxDistanceOfJoint;

                    //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBGL
                    if (Input.GetKey(KeyCode.Space) || Input.GetKeyUp(KeyCode.Space))
                    {
                        if (m_frontSwingPoint.transform.position.x >= maxXPos)
                            m_frontSwingPoint.transform.position = new Vector2(maxXPos, transform.position.y);
                        else
                            m_frontSwingPoint.transform.Translate(moveAxis * m_speedOfJoint * Time.deltaTime, Space.World);
                    }
                    else
                        m_frontSwingPoint.transform.position = new Vector2(m_frontHand.transform.position.x, transform.position.y);
                    //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                    //Store the first touch detected.
                    if (Input.touchCount > 0)
                    {
                        Touch myTouch = Input.touches[0];
                        if (myTouch.phase == TouchPhase.Stationary || myTouch.phase == TouchPhase.Ended)
                        {
                            if (m_frontSwingPoint.transform.position.x >= maxXPos)
                                m_frontSwingPoint.transform.position = new Vector2(maxXPos, transform.position.y);
                            else
                                m_frontSwingPoint.transform.Translate(moveAxis * m_speedOfJoint * Time.deltaTime, Space.World);
                        }
                    }
                    else
                        m_frontSwingPoint.transform.position = new Vector2(m_frontHand.transform.position.x, transform.position.y);
#endif //End of mobile platform dependendent compilation section started above with #elif
                }
            }
        }
    }

    float CalculateMaxDistanceOfJoint(float maxLengthOfRope, float distanceBetweenPlayerAndSealing)
    {
        float maxDistanceOfJoint;
        maxDistanceOfJoint = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(maxLengthOfRope, 2) - Mathf.Pow(distanceBetweenPlayerAndSealing, 2)));
        return maxDistanceOfJoint;
    }
}
