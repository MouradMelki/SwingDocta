using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

    public bool m_isSealing;
    public bool m_isFloor;
    public bool m_isSwitching;
    public bool m_isJumping;
    public bool m_hitUObject;
    public bool m_hitLObject;
    public bool m_isDeadZone = false;
    public int m_hitCounter = 0;
    public GameObject m_canvas;

    private Animator animator;
    private bool m_frontAnchor;
    private bool m_backAnchor;
    private bool m_crushPlayer;
    private bool m_startGame;
    private float m_timePressed;
    private float m_distance;
    private float m_lastSwingValue;
    private float m_newSwingValue;
    private float m_swingValue;
    private float t;
    private float timeOnHit;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Sealing")
        {
            if(!m_isSealing)
                m_hitCounter++;
            m_isSealing = true;
        }else if (col.gameObject.tag == "Floor")
        {
            if (!m_isFloor)
                m_hitCounter++;
            m_isFloor = true;
        }else if (col.gameObject.tag == "UObject")
        {
            if (!m_hitUObject)
                m_hitCounter++;
            m_hitUObject = true;
        }else if (col.gameObject.tag == "LObject")
        {
            if (!m_hitLObject)
                m_hitCounter++;
            m_hitLObject = true;
        }else if (col.gameObject.tag == "DeadZone")
            m_isDeadZone = true;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Sealing")
            m_isSealing = true;
        else if (col.gameObject.tag == "Floor")
            m_isFloor = true;
    }

    void Start () {
        animator = GetComponent<Animator>();
        m_isSealing = false;
        animator.SetBool("isSwinging", false);
    }

    void Update()
    {
        m_startGame = m_canvas.GetComponent<CanvasControl>().m_startGame;
        m_timePressed = GetComponent<PlayerControls>().m_timePressed;
        m_frontAnchor = GetComponent<PlayerControls>().m_frontAnchor;
        m_backAnchor = GetComponent<PlayerControls>().m_backAnchor;
        m_distance = GetComponent<PlayerControls>().m_distance;
        m_lastSwingValue = GetComponent<PlayerControls>().m_lastSwingValue;
        timeOnHit = GetComponent<PlayerControls>().timeOnHit;
        m_crushPlayer = GetComponent<PlayerLoose>().m_crushPlayer;

        if (m_startGame)
        {
            if (!m_crushPlayer)
            {
                if (GetComponent<DistanceJoint2D>().enabled)//if he is connected to the rope
                {
                    //not touching the ground
                    m_isFloor = false;
                    //set the animations
                    m_isJumping = false;

#if UNITY_STANDALONE || UNITY_WEBGL
                    if (Input.GetKeyUp(KeyCode.Space) == true && m_timePressed > 0.2f)//if he wants to shoot another rope directly
                    {
                        //set isSwitching to true to trigger the animation
                        m_isSwitching = true;

                        if (m_isSealing)
                            m_isSealing = false;
                    }
                    else if (Input.GetKeyUp(KeyCode.Space) == true && m_timePressed > 0 && m_timePressed < 0.2f) // if he wants to jump of the rope
                    {
                        if (m_isSealing)
                            m_isSealing = false;
                    }
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                    if (Input.touchCount > 0)
                    {
                        Touch myTouch = Input.touches[0];
                        if(myTouch.phase == TouchPhase.Ended && m_timePressed > 0.2f)
                        {
                            //set isSwitching to true to trigger the animation
                            m_isSwitching = true;

                            if (m_isSealing)
                                m_isSealing = false;
                        }else if (myTouch.phase == TouchPhase.Ended && m_timePressed >= 0 && m_timePressed <= 0.2f)
                        {
                            if (m_isSealing)
                                m_isSealing = false;
                        }
                    }
#endif //End of mobile platform dependendent compilation section started above with #elif
                    //set the value for the blend tree
                    m_newSwingValue = (transform.position.x - GetComponent<DistanceJoint2D>().connectedBody.transform.position.x) / m_distance;
                    //switching beteen ropes
                    if (m_isSwitching)
                    {
                        t += 2.5f * Time.deltaTime;
                        m_swingValue = Mathf.Lerp(m_lastSwingValue, m_newSwingValue, t);
                        onBlend(animator, m_swingValue, 0);
                        if (m_swingValue == m_newSwingValue)
                        {
                            m_isSwitching = false;
                            t = 0;
                        }
                    }
                    else if (!m_isSwitching)
                        onBlend(animator, m_newSwingValue, 0);
                }
                else if (!GetComponent<DistanceJoint2D>().enabled) // if he isn't connected to the rope
                {
                    m_isSealing = false;
                    m_isSwitching = false;
                    //set the animations
                    m_isJumping = true;

                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("ScientistHitUObject"))
                        m_hitUObject = false;

#if UNITY_STANDALONE || UNITY_WEBGL
                    if((m_hitLObject && Input.GetKey(KeyCode.Space) == true) || m_isFloor)
                        m_hitLObject = false;
                    
                    //after hitting the ground for a timeOnHit or while jumping
                    if (((m_isFloor && Time.time >= timeOnHit) || !m_isFloor) && Input.GetKeyUp(KeyCode.Space))
                        m_isFloor = false;
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                    if (Input.touchCount > 0)
                    {
                        Touch myTouch = Input.touches[0];
                        if ((myTouch.phase == TouchPhase.Stationary && m_hitLObject) || m_isFloor)
                            m_hitLObject = false;

                        if (((m_isFloor && Time.time >= timeOnHit) || !m_isFloor) && myTouch.phase == TouchPhase.Ended)
                            m_isFloor = false;
                    }
#endif //End of mobile platform dependendent compilation section started above with #elif
                }
            }
        }
    }

    void FixedUpdate () {

        if (m_startGame)
        {
            animator.SetBool("isSwinging", true);
            if (!m_crushPlayer)
            {
                //set the animations
                animator.SetBool("isJumping", m_isJumping);
                animator.SetBool("isFloor", m_isFloor);
                animator.SetBool("isSealing", m_isSealing);
                animator.SetBool("hitUObject", m_hitUObject);
                animator.SetBool("hitLObject", m_hitLObject);
                animator.SetBool("deadZone", m_isDeadZone);
            }
            else if (m_crushPlayer)
                animator.SetBool("crushPlayer", true);
        }
    }
    
    private void onBlend(Animator animator, float x, float y)
    {
        animator.SetFloat("PosX", x);
        animator.SetFloat("PosY", y);
    }
}
