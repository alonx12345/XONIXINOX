using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeInput : MonoBehaviour
{
    [Header("Tweaks")]
    [SerializeField] private float m_deadzone = 100f;

    [SerializeField] private float m_doubleTapDelta = .5f;

    private VariableJoystick m_VariableJoystick;

    [Header("Logic")]
    private bool m_tap, m_doubleTap, m_swipeLeft, m_swipeRight, m_swipeUp, m_swipeDown;

    private Vector2 m_swipeDelta, m_startTouch;

    private float m_lastTap;
    private float m_sqrtDeadzone;
    private bool m_IsDragging = false;

    #region Public properties
    public bool Tap
    {
        get { return m_tap; }
    }

    public bool DoubleTap
    {
        get { return m_doubleTap; }
    }

    public Vector2 SwipeDelta
    {
        get { return m_swipeDelta; }
    }

    public bool SwipeLeft
    {
        get { return m_swipeLeft; }
    }

    public bool SwipeRight
    {
        get { return m_swipeRight; }
    }

    public bool SwipeUp
    {
        get { return m_swipeUp; }
    }

    public bool SwipeDown
    {
        get { return m_swipeDown; }
    }

    private Vector2 m_PrevMousePos = Vector2.zero;

    private Vector2 m_PrevTouchPos = Vector2.zero;

    private bool swiped = false;
    private bool firstSwipe = true;
    Vector3 firstDirection = Vector2.zero;

    #endregion

    private void Start()
    {
        m_sqrtDeadzone = m_deadzone * m_deadzone;
        m_VariableJoystick = FindObjectOfType<VariableJoystick>();
    }

    private void Update()
    {
        m_tap = m_doubleTap = m_swipeLeft = m_swipeRight = m_swipeUp = m_swipeDown = false;

        //UpdateMobile();
        //UpdateStandalone();
        //DistanceCalc();

        //UpdateStanaloneNew();
        //UpdateMobileNew1();
        UpdateMobile2();
    }

    private void UpdateMobile2()
    {
        //if (direction)
        Vector3 direction = Vector3.up * m_VariableJoystick.Vertical + Vector3.right * m_VariableJoystick.Horizontal;
        if (direction != Vector3.zero && !swiped)
        {
            swiped = true;
            firstDirection = direction;
        }
        else if (swiped && direction != firstDirection)
        {
            if (firstSwipe)
            {
                direction -= firstDirection;
                firstSwipe = false;
            }

            MoveStandAlone(direction);
        }
    }

    private void UpdateStanaloneNew()
    {
        Vector2 currentMousePos = Input.mousePosition;
        Vector2 currentMousePosDelta = Vector2.zero;

        if (Input.GetMouseButton(0))
        {
            if (m_PrevMousePos != Vector2.zero)
            {
                currentMousePosDelta = currentMousePos - m_PrevMousePos;
            }

            m_PrevMousePos = currentMousePos;

            MoveStandAlone(currentMousePosDelta);
        }
    }

    private void UpdateMobileNew1()
    {
        if (Input.touches.Length > 0)
        {
            Vector2 currentTouchPos = Input.touches[0].position;
            Vector2 currentTouchPosDelta = Vector2.zero;

            if (Input.touches[0].phase == TouchPhase.Began)
            {
                currentTouchPosDelta = currentTouchPos - m_PrevTouchPos;
            }

            m_PrevTouchPos = currentTouchPos;

            MoveStandAlone(currentTouchPosDelta);
        }

       
    }

    private void MoveStandAlone(Vector2 i_MoveDelta)
    {
        if (i_MoveDelta.SqrMagnitude() > m_deadzone)
        {
            float x = i_MoveDelta.x;
            float y = i_MoveDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x < 0)
                {
                    m_swipeLeft = true;
                }
                else if (x > 0)
                {
                    m_swipeRight = true;
                }
            }
            else
            {
                if (y < 0)
                {
                    m_swipeDown = true;

                }
                else if (y > 0)
                {
                    m_swipeUp = true;
                }
            }

            //m_startTouch = m_swipeDelta = Vector2.zero;
        }
    }

    private void UpdateStandalone()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_tap = true;
            m_startTouch = Input.mousePosition;
            m_IsDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_IsDragging = false;
            Reset();
        }
    }

    private void UpdateMobileNew()
    {
        if (Input.touches.Length > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                //m_doubleTap = Time.time - m_lastTap < m_doubleTapDelta;
                //m_lastTap = Time.time;
                m_tap = true;
                m_startTouch = Input.touches[0].position;
                m_IsDragging = true;

            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                Reset();
                m_IsDragging = false;
            }
        }
    }

    private void UpdateMobile()
    {
        if (Input.touches.Length != 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                m_tap = true;
                m_startTouch = Input.touches[0].position;
                m_doubleTap = Time.time - m_lastTap < m_doubleTapDelta;
                m_lastTap = Time.time;
                m_IsDragging = true;

            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                m_startTouch = m_swipeDelta = Vector2.zero;
                m_IsDragging = false;
            }

            //if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            //{
            //    m_startTouch = m_swipeDelta = Vector2.zero;
            //    m_IsDragging = false;
            //}
        }

       

        m_swipeDelta = Vector2.zero;

        if (m_startTouch != Vector2.zero && Input.touches.Length != 0)
        {
            m_swipeDelta = Input.touches[0].position - m_startTouch;
        }
        //else if (Input.GetMouseButton(0))
        //{
        //    m_swipeDelta = (Vector2)Input.mousePosition - m_startTouch;
        //}

        

        if (m_swipeDelta.SqrMagnitude() > m_deadzone)
        {
            float x = m_swipeDelta.x;
            float y = m_swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x < 0)
                {
                    m_swipeLeft = true;
                }
                else if (x > 0)
                {
                    m_swipeRight = true;
                }
            }
            else
            {
                if (y < 0)
                {
                    m_swipeDown = true;

                }
                else if (y > 0)
                {
                    m_swipeUp = true;
                }
            }

            //m_startTouch = m_swipeDelta = Vector2.zero;
        }
    }

    private void DistanceCalc()
    {
        m_swipeDelta = Vector2.zero;

        if (m_IsDragging)
        {
            if (Input.touches.Length > 0)
            {
                m_swipeDelta = Input.touches[0].position - m_startTouch;
            }
            else if (Input.GetMouseButton(0))
            {
                m_swipeDelta = (Vector2) Input.mousePosition - m_startTouch;
            }
        }

        print(m_swipeDelta);

        if (SwipeDelta.magnitude > 2)
        {
            float x = m_swipeDelta.x;
            float y = m_swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x < 0)
                {
                    m_swipeLeft = true;
                }
                else if (x > 0)
                {
                    m_swipeRight = true;
                }
            }
            else
            {
                if (y < 0)
                {
                    m_swipeDown = true;

                }
                else if (y > 0)
                {
                    m_swipeUp = true;
                }
            }

            Reset();
        }
    }



    private void Reset()
    {
        m_startTouch = m_swipeDelta = Vector2.zero;
    }
}
