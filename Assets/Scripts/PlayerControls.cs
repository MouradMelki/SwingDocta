using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    public float m_minSpeed = 16f;
    public float m_maxSpeed;
    public float m_speed;
    public float m_gravityScale;
    public float m_timePressed = 0f;
    public float max_distance;
    public float m_distance;
    public float timeOnHit = 0f;
    public float m_lastSwingValue;
    public GameObject m_rope;
    public GameObject m_frontSwingPoint;
    public GameObject m_backSwingPoint;
    public GameObject m_backHand;
    public GameObject m_frontHand;
    public GameObject m_frontHandAnchor;
    public GameObject m_backHandAnchor;
    public GameObject m_JointsControls;
    public GameObject m_canvas;
    public bool m_backAnchor;
    public bool m_frontAnchor;
    public Vector2 moveAxis;
    public Vector2 frozenMoveAxis;

    private bool m_isSealing;
    private bool m_isFloor;
    private bool m_isGravity;
    private bool m_crushPlayer;
    private bool m_startGame;
    private bool m_deadZone;
    private bool m_hitUObject;
    private bool m_hitLObject;
    private float m_currentPos = 0f;
    private float t = 0f;
    private float time = 0f;
    private float timeOnFloorSealing = 1f;
    private float diff;
    private float diff2;
    private GameObject Rope;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Sealing")
            timeOnHit = Time.time + timeOnFloorSealing;

        if (col.gameObject.tag == "Floor" && !m_isFloor)
                timeOnHit = Time.time + timeOnFloorSealing;
    }

    void Start()
    {
        m_crushPlayer = GetComponent<PlayerLoose>().m_crushPlayer;
        //set speed
        m_speed = m_minSpeed;
        //this value is to calculate time pressed bu space key
        m_timePressed = 0f;
        //set frontAnchor true cause player start with front hand
        m_frontAnchor = true;
        //set the maxDistance of rope
        max_distance = GetComponent<DistanceJoint2D>().distance;
        //set distance of rope on start
        m_distance = max_distance;

        GetComponent<DistanceJoint2D>().enabled = false;
        if (GetComponent<DistanceJoint2D>().enabled)
            m_isGravity = false;
        else if (!GetComponent<DistanceJoint2D>().enabled)
            m_isGravity = true;
    }

    void Update()
    {
        Debug.Log(moveAxis);
        m_isFloor = GetComponent<PlayerAnimation>().m_isFloor;
        m_isSealing = GetComponent<PlayerAnimation>().m_isSealing;
        m_crushPlayer = GetComponent<PlayerLoose>().m_crushPlayer;
        m_startGame = m_canvas.GetComponent<CanvasControl>().m_startGame;
        m_hitUObject = GetComponent<PlayerAnimation>().m_hitUObject;
        m_hitLObject = GetComponent<PlayerAnimation>().m_hitLObject;
        if (m_startGame)
        {
            if (!m_crushPlayer)
            {
                //calculate pressed time
                KeyPressedTimer();
                if (GetComponent<DistanceJoint2D>().enabled)//if he is connected to the rope
                {
                    if (!Rope)
                    {
                        Rope = Instantiate(m_rope, GetComponent<DistanceJoint2D>().connectedBody.transform.position, Quaternion.identity);
                        Rope.GetComponent<RopeBehaviour>().m_player = gameObject;
                        Rope.GetComponent<RopeBehaviour>().m_hand = m_frontHandAnchor;
                    }

                    //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBGL

                    if (Input.GetKeyUp(KeyCode.Space) == true && m_timePressed > 0.2f)//if he wants to shoot another rope directly
                    {
                        if (m_frontAnchor)//if rope is on front hand shoot rope from back hand
                        {
                            //release the rope
                            if (Rope)
                            {
                                Rope.GetComponent<RopeBehaviour>().release = true;
                                Rope.GetComponent<RopeBehaviour>().m_ropeEnd.GetComponent<Rigidbody2D>().gravityScale = 2;
                            }
                            //set frontHand to false and backHand to true
                            m_frontAnchor = false;
                            m_backAnchor = true;
                            //set the last blend tree value to make the switch correctly
                            m_lastSwingValue = (transform.position.x - GetComponent<DistanceJoint2D>().connectedBody.transform.position.x) / m_distance;
                            //set the connected body and the distance of the DistanceJoint2D
                            GetComponent<DistanceJoint2D>().connectedBody = m_backSwingPoint.GetComponent<Rigidbody2D>();
                            CalculateRopeDistance(m_backSwingPoint.transform.position, m_backHandAnchor.transform.position);
                            if (m_distance > max_distance)
                                m_distance = max_distance;
                            GetComponent<DistanceJoint2D>().distance = m_distance;
                        }
                        else if (m_backAnchor)//if rope is on back hand shoot rope from front hand
                        {
                            //release the rope
                            if (Rope)
                            {
                                Rope.GetComponent<RopeBehaviour>().release = true;
                                Rope.GetComponent<RopeBehaviour>().m_ropeEnd.GetComponent<Rigidbody2D>().gravityScale = 2;
                            }
                            //set backHand to false and frontHand to true
                            m_backAnchor = false;
                            m_frontAnchor = true;
                            //set the last blend tree value to make the switch correctly
                            m_lastSwingValue = (transform.position.x - GetComponent<DistanceJoint2D>().connectedBody.transform.position.x) / m_distance;
                            //set the connected body and the distance of the DistanceJoint2D
                            GetComponent<DistanceJoint2D>().connectedBody = m_frontSwingPoint.GetComponent<Rigidbody2D>();
                            CalculateRopeDistance(m_frontSwingPoint.transform.position, m_frontHandAnchor.transform.position);
                            if (m_distance > max_distance)
                                m_distance = max_distance;
                            GetComponent<DistanceJoint2D>().distance = m_distance;
                        }
                        Rope = Instantiate(m_rope, GetComponent<DistanceJoint2D>().connectedBody.transform.position, Quaternion.identity);
                        Rope.GetComponent<RopeBehaviour>().m_player = gameObject;
                        //Create the rope
                        if (m_frontAnchor)
                            Rope.GetComponent<RopeBehaviour>().m_hand = m_frontHandAnchor;
                        else if (m_backAnchor)
                            Rope.GetComponent<RopeBehaviour>().m_hand = m_backHandAnchor;
                        //Remove gravity to give the feel that the player is sucked by the black whole
                        m_isGravity = false;
                    }
                    else if ((Input.GetKeyUp(KeyCode.Space) == true && m_timePressed > 0 && m_timePressed < 0.2f)) // if he wants to jump of the rope
                    {
                        //release the rope and set its gravity
                        if (Rope)
                        {
                            Rope.GetComponent<RopeBehaviour>().release = true;
                            Rope.GetComponent<RopeBehaviour>().m_ropeEnd.GetComponent<Rigidbody2D>().gravityScale = 2;
                        }
                        //deactivate the rope
                        GetComponent<DistanceJoint2D>().enabled = false;
                        //set the anchores to false
                        m_backAnchor = false;
                        m_frontAnchor = false;
                        //make the player jump
                        if (!m_hitUObject && !m_hitLObject)
                            GetComponent<Rigidbody2D>().AddForce(moveAxis * Time.deltaTime);
                        //change moveAxis to forward
                        moveAxis = new Vector2(1, 0f);
                        //set the gravity to true
                        m_isGravity = true;
                    }

                    //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

                    if (m_hitUObject || m_hitLObject)
                    {
                        //release the rope and set its gravity
                        if (Rope)
                        {
                            Rope.GetComponent<RopeBehaviour>().release = true;
                            Rope.GetComponent<RopeBehaviour>().m_ropeEnd.GetComponent<Rigidbody2D>().gravityScale = 2;
                        }
                        //deactivate the rope
                        GetComponent<DistanceJoint2D>().enabled = false;
                        //set the anchores to false
                        m_backAnchor = false;
                        m_frontAnchor = false;
                        //change moveAxis to forward
                        moveAxis = new Vector2(1, 0f);
                        //set the gravity to true
                        m_isGravity = true;
                    }

                    //Check if Input has registered more than zero touches            
                    if (Input.touchCount > 0)
                    {
                        Touch myTouch = Input.touches[0];
                        //Check if the phase of that touch equals Began
                        if (myTouch.phase == TouchPhase.Ended && m_timePressed > 0.2f)
                        {
                            if (m_frontAnchor)//if rope is on front hand shoot rope from back hand
                            {
                                //release the rope
                                if (Rope)
                                {
                                    Rope.GetComponent<RopeBehaviour>().release = true;
                                    Rope.GetComponent<RopeBehaviour>().m_ropeEnd.GetComponent<Rigidbody2D>().gravityScale = 2;
                                }
                                //set frontHand to false and backHand to true
                                m_frontAnchor = false;
                                m_backAnchor = true;
                                //set the last blend tree value to make the switch correctly
                                m_lastSwingValue = (transform.position.x - GetComponent<DistanceJoint2D>().connectedBody.transform.position.x) / m_distance;
                                //set the connected body and the distance of the DistanceJoint2D
                                GetComponent<DistanceJoint2D>().connectedBody = m_backSwingPoint.GetComponent<Rigidbody2D>();
                                CalculateRopeDistance(m_backSwingPoint.transform.position, m_backHandAnchor.transform.position);
                                if (m_distance > max_distance)
                                    m_distance = max_distance;
                                GetComponent<DistanceJoint2D>().distance = m_distance;
                            }
                            else if (m_backAnchor)//if rope is on back hand shoot rope from front hand
                            {
                                //release the rope
                                if (Rope)
                                {
                                    Rope.GetComponent<RopeBehaviour>().release = true;
                                    Rope.GetComponent<RopeBehaviour>().m_ropeEnd.GetComponent<Rigidbody2D>().gravityScale = 2;
                                }
                                //set backHand to false and frontHand to true
                                m_backAnchor = false;
                                m_frontAnchor = true;
                                //set the last blend tree value to make the switch correctly
                                m_lastSwingValue = (transform.position.x - GetComponent<DistanceJoint2D>().connectedBody.transform.position.x) / m_distance;
                                //set the connected body and the distance of the DistanceJoint2D
                                GetComponent<DistanceJoint2D>().connectedBody = m_frontSwingPoint.GetComponent<Rigidbody2D>();
                                CalculateRopeDistance(m_frontSwingPoint.transform.position, m_frontHandAnchor.transform.position);
                                if (m_distance > max_distance)
                                    m_distance = max_distance;
                                GetComponent<DistanceJoint2D>().distance = m_distance;
                            }
                            Rope = Instantiate(m_rope, GetComponent<DistanceJoint2D>().connectedBody.transform.position, Quaternion.identity);
                            Rope.GetComponent<RopeBehaviour>().m_player = gameObject;
                            //Create the rope
                            if (m_frontAnchor)
                                Rope.GetComponent<RopeBehaviour>().m_hand = m_frontHandAnchor;
                            else if (m_backAnchor)
                                Rope.GetComponent<RopeBehaviour>().m_hand = m_backHandAnchor;
                            //Remove gravity to give the feel that the player is sucked by the black whole
                            m_isGravity = false;
                        }
                        else if ((myTouch.phase == TouchPhase.Ended && m_timePressed > 0 && m_timePressed < 0.2f))
                        {
                            //release the rope and set its gravity
                            if (Rope)
                            {
                                Rope.GetComponent<RopeBehaviour>().release = true;
                                Rope.GetComponent<RopeBehaviour>().m_ropeEnd.GetComponent<Rigidbody2D>().gravityScale = 2;
                            }
                            //deactivate the rope
                            GetComponent<DistanceJoint2D>().enabled = false;
                            //set the anchores to false
                            m_backAnchor = false;
                            m_frontAnchor = false;
                            //make the player jump
                            GetComponent<Rigidbody2D>().AddForce(moveAxis * Time.deltaTime);
                            //change moveAxis to forward
                            moveAxis = new Vector2(1, 0f);
                            //set the gravity to true
                            m_isGravity = true;
                        }

                    }
#endif //End of mobile platform dependendent compilation section started above with #elif
                }
                else if (!GetComponent<DistanceJoint2D>().enabled) // if he isn't connected to the rope
                {
                    if ((m_isFloor && Time.time >= timeOnHit) || !m_isFloor)//after hitting the ground for a timeOnHit or while jumping
                    {
#if UNITY_STANDALONE || UNITY_WEBGL

                        if (Input.GetKeyUp(KeyCode.Space) == true)//Shoot the rope
                        {
                            //set the bools
                            m_backAnchor = false;
                            m_frontAnchor = true;
                            GetComponent<DistanceJoint2D>().enabled = true;
                            GetComponent<DistanceJoint2D>().connectedBody = m_frontSwingPoint.GetComponent<Rigidbody2D>();
                            Rope = Instantiate(m_rope, GetComponent<DistanceJoint2D>().connectedBody.transform.position, Quaternion.identity);
                            Rope.GetComponent<RopeBehaviour>().m_player = gameObject;
                            Rope.GetComponent<RopeBehaviour>().m_hand = m_frontHandAnchor;
                            CalculateRopeDistance(m_frontSwingPoint.transform.position, m_frontHandAnchor.transform.position);
                            if (m_distance > max_distance)
                                m_distance = max_distance;
                            GetComponent<DistanceJoint2D>().distance = m_distance;
                            m_isGravity = false;
                        }
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

                        if (Input.touchCount > 0)
                        {
                            Touch myTouch = Input.touches[0];
                            if (myTouch.phase == TouchPhase.Ended)//Shoot the rope
                            {
                                //set the bools
                                m_backAnchor = false;
                                m_frontAnchor = true;
                                GetComponent<DistanceJoint2D>().enabled = true;
                                GetComponent<DistanceJoint2D>().connectedBody = m_frontSwingPoint.GetComponent<Rigidbody2D>();
                                Rope = Instantiate(m_rope, GetComponent<DistanceJoint2D>().connectedBody.transform.position, Quaternion.identity);
                                Rope.GetComponent<RopeBehaviour>().m_player = gameObject;
                                Rope.GetComponent<RopeBehaviour>().m_hand = m_frontHandAnchor;
                                CalculateRopeDistance(m_frontSwingPoint.transform.position, m_frontHandAnchor.transform.position);
                                if (m_distance > max_distance)
                                    m_distance = max_distance;
                                GetComponent<DistanceJoint2D>().distance = m_distance;
                                m_isGravity = false;
                            }
                        }

#endif //End of mobile platform dependendent compilation section started above with #elif
                    }
                }
                //x speed constaint
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);

                //calculate move axis so it's perpendicular to the rope
                if (GetComponent<DistanceJoint2D>().enabled)//if he is connected to the rope
                    CalculateMoveAxis(GetComponent<DistanceJoint2D>().connectedBody.transform.position,
                        transform.position + new Vector3(GetComponent<DistanceJoint2D>().anchor.x,
                        GetComponent<DistanceJoint2D>().anchor.y, 0f));

                //rotation of the hands
                if (m_frontAnchor)
                {
                    GetComponent<DistanceJoint2D>().anchor = new Vector2(m_frontHandAnchor.transform.position.x - transform.position.x,
                                   m_frontHandAnchor.transform.position.y - transform.position.y);
                    diff = (m_frontHandAnchor.transform.position.x - m_frontSwingPoint.transform.position.x) / m_distance;

#if UNITY_STANDALONE || UNITY_WEBGL
                    if (Input.GetKey(KeyCode.Space))
                        diff2 = (m_backHandAnchor.transform.position.x - m_backSwingPoint.transform.position.x) / m_distance;
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                    if(Input.touchCount > 0)
                    {
                        Touch myTouch = Input.touches[0];
                        if(myTouch.phase == TouchPhase.Stationary)
                            diff2 = (m_backHandAnchor.transform.position.x - m_backSwingPoint.transform.position.x) / m_distance;
                    }
#endif //End of mobile platform dependendent compilation section started above with #elif
                }
                else if (m_backAnchor)
                {
                    GetComponent<DistanceJoint2D>().anchor = new Vector2(m_backHandAnchor.transform.position.x - transform.position.x,
                        m_backHandAnchor.transform.position.y - transform.position.y);
                    diff = (m_backHandAnchor.transform.position.x - m_backSwingPoint.transform.position.x) / m_distance;

#if UNITY_STANDALONE || UNITY_WEBGL
                    if (Input.GetKey(KeyCode.Space))
                        diff2 = (m_frontHandAnchor.transform.position.x - m_frontSwingPoint.transform.position.x) / m_distance;
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                    if (Input.touchCount > 0)
                    {
                        Touch myTouch = Input.touches[0];
                        if (myTouch.phase == TouchPhase.Stationary)
                            diff2 = (m_frontHandAnchor.transform.position.x - m_frontSwingPoint.transform.position.x) / m_distance;
                    }
#endif //End of mobile platform dependendent compilation section started above with #elif
                }

#if UNITY_STANDALONE || UNITY_WEBGL
                if (Input.GetKey(KeyCode.Space) && !m_frontAnchor && !m_backAnchor)
                    diff2 = (m_frontHandAnchor.transform.position.x - m_frontSwingPoint.transform.position.x) / m_distance;
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                if (Input.touchCount > 0)
                {
                    Touch myTouch = Input.touches[0];
                    if (myTouch.phase == TouchPhase.Stationary && !m_frontAnchor && !m_backAnchor)
                        diff2 = (m_frontHandAnchor.transform.position.x - m_frontSwingPoint.transform.position.x) / m_distance;
                }
#endif //End of mobile platform dependendent compilation section started above with #elif

                if (Mathf.RoundToInt(transform.position.x) % 250 == 0 && m_currentPos < Mathf.RoundToInt(transform.position.x) && m_speed <= m_maxSpeed)
                {
                    m_currentPos = Mathf.RoundToInt(transform.position.x);
                    m_JointsControls.GetComponent<RopeJointsControls>().m_speedOfJoint++;
                    m_speed++;
                }
            }
            else if (m_crushPlayer)
            {
                m_isGravity = false;
                //release the rope and set its gravity
                if (Rope)
                {
                    Rope.GetComponent<RopeBehaviour>().release = true;
                    Rope.GetComponent<RopeBehaviour>().m_ropeEnd.GetComponent<Rigidbody2D>().gravityScale = 2;
                }
                //deactivate the rope
                GetComponent<DistanceJoint2D>().enabled = false;
                //set the anchores to false
                m_backAnchor = false;
                m_frontAnchor = false;
            }
            //gravity control
            if (m_isGravity)
                GetComponent<Rigidbody2D>().gravityScale = m_gravityScale;
            else
                GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }

    void FixedUpdate()
    {
        if (m_startGame)
        {
            if (!m_crushPlayer)
            {
                //rotation of the hands
                if (m_frontAnchor)
                {
                    if (diff >= 0 && diff <= 1)
                        m_frontHand.transform.eulerAngles = new Vector3(0f, 0f, (Mathf.Asin(diff) * 180 / Mathf.PI) + 180);
                    else if (diff < 0 && diff >= -1)
                        m_frontHand.transform.eulerAngles = new Vector3(0f, 0f, (Mathf.Asin(diff) * 180 / Mathf.PI) + 180);

#if UNITY_STANDALONE || UNITY_WEBGL
                    if (Input.GetKey(KeyCode.Space) && diff2 >= -1 && diff2 <= 1)
                        m_backHand.transform.eulerAngles = new Vector3(0f, 0f, (Mathf.Asin(diff2) * 180 / Mathf.PI) + 180);
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                    if (Input.touchCount > 0)
                    {
                        Touch myTouch = Input.touches[0];
                        if (myTouch.phase == TouchPhase.Stationary && diff2 >= -1 && diff2 <= 1)
                            m_backHand.transform.eulerAngles = new Vector3(0f, 0f, (Mathf.Asin(diff2) * 180 / Mathf.PI) + 180);
                    }
#endif //End of mobile platform dependendent compilation section started above with #elif
                }
                else if (m_backAnchor)
                {
                    if (diff >= 0 && diff <= 1)
                        m_backHand.transform.eulerAngles = new Vector3(0f, 0f, (Mathf.Asin(diff) * 180 / Mathf.PI) + 180);
                    else if (diff < 0 && diff >= -1)
                        m_backHand.transform.eulerAngles = new Vector3(0f, 0f, (Mathf.Asin(diff) * 180 / Mathf.PI) + 180);

#if UNITY_STANDALONE || UNITY_WEBGL
                    if (Input.GetKey(KeyCode.Space) && diff2 >= -1 && diff2 <= 1)
                        m_frontHand.transform.eulerAngles = new Vector3(0f, 0f, (Mathf.Asin(diff2) * 180 / Mathf.PI) + 180);
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                    if (Input.touchCount > 0)
                    {
                        Touch myTouch = Input.touches[0];
                        if (myTouch.phase == TouchPhase.Stationary && diff2 >= -1 && diff2 <= 1)
                            m_frontHand.transform.eulerAngles = new Vector3(0f, 0f, (Mathf.Asin(diff2) * 180 / Mathf.PI) + 180);
                    }
#endif //End of mobile platform dependendent compilation section started above with #elif
                }

#if UNITY_STANDALONE || UNITY_WEBGL
                if (Input.GetKey(KeyCode.Space) && !m_frontAnchor && !m_backAnchor && diff2 >= -1 && diff2 <= 1)
                    m_frontHand.transform.eulerAngles = new Vector3(0f, 0f, (Mathf.Asin(diff2) * 180 / Mathf.PI) + 180);
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                if (Input.touchCount > 0)
                {
                    Touch myTouch = Input.touches[0];
                    if (myTouch.phase == TouchPhase.Stationary && !m_frontAnchor && !m_backAnchor && diff2 >= -1 && diff2 <= 1)
                        m_frontHand.transform.eulerAngles = new Vector3(0f, 0f, (Mathf.Asin(diff2) * 180 / Mathf.PI) + 180);
                }
#endif //End of mobile platform dependendent compilation section started above with #elif

                if (m_isFloor)
                    transform.Translate(moveAxis * (m_speed - 10) * Time.deltaTime, Space.World);
                else
                    transform.Translate(moveAxis * m_speed * Time.deltaTime, Space.World);
            }
        }
    }

    void CalculateRopeDistance(Vector2 jointPosition, Vector2 playerPosition) // length of the rope
    {
        Vector2 playerToJoint = jointPosition - playerPosition;
        m_distance = playerToJoint.magnitude;
    }

    void CalculateMoveAxis(Vector2 jointPosition, Vector2 playerPosition) //Calculate the moveAxis so it's perpendicular to the rope When swinging
    {
        Vector2 playerToJoint = jointPosition - playerPosition;
        Vector2 orthPlayerToJoint = new Vector2(playerToJoint.y, -playerToJoint.x);
        float lengthOfVect = orthPlayerToJoint.magnitude;
        moveAxis = orthPlayerToJoint / lengthOfVect;
    }

    void KeyPressedTimer() //claculate the time pressing space
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.Space))
            time = Time.time;

        if (Input.GetKey(KeyCode.Space))
            m_timePressed = Time.time - time;

        
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];
            if (myTouch.phase == TouchPhase.Began)
                time = Time.time;

            if (myTouch.phase == TouchPhase.Stationary)
                m_timePressed = Time.time - time;
        }
#endif //End of mobile platform dependendent compilation section started above with #elif
    }
}
