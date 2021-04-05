using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum eDirection
{
    Default,
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4,
    Moving
}

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] public bool se_IsMobile = false;
    [SerializeField] public bool se_IsTap = true;

    private float m_VerticalInput;
    private float m_HorizontalInput;
    private float m_CurrentHorizontal;
    private float m_CurrentVertical;
    private Vector3 m_DirectionVector;
    private Vector3 m_PlayerMoveDirectionVector;

    private eDirection m_CurrentDirection;
    private eDirection m_MovingDirection;

    private SwipeInput m_SwipeInput;

    private PlayerController m_PlayerController;

    public eDirection CurrentDirection
    {
        set { m_CurrentDirection = value; }
    }

    public eDirection MovingDirection
    {
        set { m_MovingDirection = value; }
    }

    private int testLevel = 1;

    private void Awake()
    {
        m_DirectionVector = transform.position;
        m_PlayerController = GetComponent<PlayerController>();
        m_SwipeInput = GetComponent<SwipeInput>();
    }

    private void Update()
    {
        if (se_IsMobile)
        {
            if (se_IsTap)
            {
                handleTapInput();
            }
            else
            {
                handleSwipeInput();
            }
            
        }
        else
        {
            handleKeyboardInput();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            FindObjectOfType<TipManager>().FadeTextOverTime();
        }

        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    FindObjectOfType<TipManager>().ActivateTipText(testLevel++);
        //}


        getInput();
        m_PlayerController.Move(m_DirectionVector, Time.deltaTime);
    }

    private void handleSwipeInput()
    {
        m_VerticalInput = m_SwipeInput.SwipeDelta.y;
        m_HorizontalInput = m_SwipeInput.SwipeDelta.x;

        if (m_SwipeInput.SwipeLeft && m_CurrentDirection != eDirection.Right)
        {
            m_CurrentHorizontal = -1;
            m_CurrentVertical = 0;
        }
        else if (m_SwipeInput.SwipeRight && m_CurrentDirection != eDirection.Left)
        {
            m_CurrentHorizontal = 1;
            m_CurrentVertical = 0;
        }
        else if (m_SwipeInput.SwipeUp && m_CurrentDirection != eDirection.Down)
        {
            m_CurrentVertical = 1;
            m_CurrentHorizontal = 0;
        }
        else if (m_SwipeInput.SwipeDown && m_CurrentDirection != eDirection.Up)
        {
            m_CurrentVertical = -1;
            m_CurrentHorizontal = 0;
        }
    }

    private void handleTapInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector3 touchPos = touch.position;

            Vector3 distVector = transform.position - touchPos;

            print(distVector);

            bool vertical = false;
            bool horizontal = false;

            if (Mathf.Abs(distVector.x) > Mathf.Abs(distVector.y))
            {
                horizontal = true;
            }
            else
            {
                vertical = true;
            }


            if (horizontal && distVector.x > 0 && m_CurrentDirection != eDirection.Right)
            {
                m_CurrentHorizontal = -1;
                m_CurrentVertical = 0;
            }
            else if (horizontal && distVector.x < 0 && m_CurrentDirection != eDirection.Left)
            {
                m_CurrentHorizontal = 1;
                m_CurrentVertical = 0;
            }
            else if (vertical && distVector.y < 0 && m_CurrentDirection != eDirection.Down)
            {
                m_CurrentVertical = 1;
                m_CurrentHorizontal = 0;
            }
            else if (vertical && distVector.y > 0 && m_CurrentDirection != eDirection.Up)
            {
                m_CurrentVertical = -1;
                m_CurrentHorizontal = 0;
            }
        }
    }

    private void handleKeyboardInput()
    {
        m_VerticalInput = Input.GetAxis("Vertical");
        m_HorizontalInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(m_HorizontalInput) > 0 || Mathf.Abs(m_VerticalInput) > 0 || m_CurrentDirection == eDirection.Default)
        {
            if ((m_HorizontalInput < 0 && m_CurrentDirection != eDirection.Right) ||
                (m_HorizontalInput > 0 && m_CurrentDirection != eDirection.Left) ||
                (m_VerticalInput > 0 && m_CurrentDirection != eDirection.Down) ||
                (m_VerticalInput < 0 && m_CurrentDirection != eDirection.Up))
            {
                m_CurrentDirection = eDirection.Moving;
                m_CurrentVertical = Input.GetAxis("Vertical");
                m_CurrentHorizontal = Input.GetAxis("Horizontal");
            }
        }
    }

    private void getInput()
    {
        if (m_DirectionVector == transform.position)
        {
            if (m_CurrentHorizontal > 0 && m_CurrentDirection != eDirection.Left && (m_DirectionVector + Vector3.right).x < Utils.s_GridWidth)
            {
                m_DirectionVector += Vector3.right;
                m_CurrentDirection = eDirection.Right;
                m_MovingDirection = eDirection.Right;
                m_PlayerMoveDirectionVector = Vector3.right;
            }

            else if (m_CurrentHorizontal < 0 && m_CurrentDirection != eDirection.Right && (m_DirectionVector + Vector3.left).x >= 0)
            {
                m_DirectionVector += Vector3.left;
                m_CurrentDirection = eDirection.Left;
                m_MovingDirection = eDirection.Left;
                m_PlayerMoveDirectionVector = Vector3.left;
            }

            else if (m_CurrentVertical > 0 && m_CurrentDirection != eDirection.Down && (m_DirectionVector + Vector3.up).y < Utils.s_GridHeight)
            {
                m_DirectionVector += Vector3.up;
                m_CurrentDirection = eDirection.Up;
                m_MovingDirection = eDirection.Up;
                m_PlayerMoveDirectionVector = Vector3.up;
            }

            else if (m_CurrentVertical < 0 && m_CurrentDirection != eDirection.Up && (m_DirectionVector + Vector3.down).y >= 0)
            {
                m_DirectionVector += Vector3.down;
                m_CurrentDirection = eDirection.Down;
                m_MovingDirection = eDirection.Down;
                m_PlayerMoveDirectionVector = Vector3.down;
            }
        }

        if (Utils.OnBorder(m_DirectionVector.x, m_DirectionVector.y))
        {
            m_CurrentDirection = eDirection.Default;
        }

        m_PlayerController.MoveDirection = m_PlayerMoveDirectionVector;
    }
}
