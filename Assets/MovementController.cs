using UnityEngine;
using System.Collections.Generic;

public class MovementController : MonoBehaviour {

    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider)) {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if(validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        } else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

	void Update () {
        m_animator.SetBool("Grounded", m_isGrounded);

        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        bool walk = Input.GetKey(KeyCode.LeftShift);
        bool up = Input.GetKey(KeyCode.W);
        bool down = Input.GetKey(KeyCode.S);
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);

        if (up && right) {
            transform.rotation = Quaternion.LookRotation (new Vector3 (1, 0, 0));
        }

        else if (up && left) {
            transform.rotation = Quaternion.LookRotation (new Vector3 (0, 0, 1));
        }

        else if (down && right) {
            transform.rotation = Quaternion.LookRotation (new Vector3 (0, 0, -1));
        }

        else if (down && left) {
            transform.rotation = Quaternion.LookRotation (new Vector3 (-1, 0, 0));
        }

        else if (up) {
            transform.rotation = Quaternion.LookRotation (new Vector3 (1, 0, 1));
        }

        else if (down) {
            transform.rotation = Quaternion.LookRotation (new Vector3 (-1, 0, -1));
        }

        else if (left) {
            transform.rotation = Quaternion.LookRotation (new Vector3 (-1, 0, 1));
        }

         else if (right) {
            transform.rotation = Quaternion.LookRotation (new Vector3 (1, 0, -1));
        }

        if(walk)
        {
            v *= m_walkScale;
        }

        m_currentV = Mathf.Abs(Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation));
        m_currentH = Mathf.Abs(Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation));

        int moving = up || down || left || right ? 1 : 0;

        transform.position += transform.forward * moving * m_moveSpeed * Time.deltaTime;

        m_animator.SetFloat("MoveSpeed", moving);

        JumpingAndLanding();

        m_wasGrounded = m_isGrounded;
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && Input.GetKey(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }
}
