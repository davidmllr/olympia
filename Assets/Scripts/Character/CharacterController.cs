using UnityEngine;

namespace Character
{
    /// <summary>
    /// </summary>
    public class CharacterController : MonoBehaviour
    {
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");

        private Animator _animator;
        private int _currentJumpCount;
        private bool _grounded;
        private GroundSensor _groundSensor;

        private Rigidbody2D _rb;
        [SerializeField] private int jumpCount = 2;
        [SerializeField] private float jumpForce = 2.0f;

        /// <summary>
        /// </summary>
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _groundSensor = transform.Find("GroundSensor")
                .GetComponent<GroundSensor>();
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// </summary>
        private void Update()
        {
            /* check if character just landed on the ground */
            if (!_grounded && _groundSensor.State())
            {
                _grounded = true;
                _currentJumpCount = 0;
            }

            /* check if character just started falling */
            if (_grounded &&
                !_groundSensor.State())
            {
                _animator.SetBool(IsJumping, true);
                _grounded = false;
            }

            /*
         
            -- Handle input and movement --
            var inputX = Input.GetAxis("Horizontal");

            // Swap direction of sprite depending on walk direction

            m_spriteRenderer.flipX = inputX < 0.0f;

            // Move
            _animator.SetFloat("Speed", Mathf.Abs(inputX * m_speed));
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        
            */
            
            //Jump
            if (Input.GetButtonDown("Jump") &&
                (_grounded || _currentJumpCount < jumpCount))
                Jump();
        }

        /// <summary>
        /// </summary>
        private void Jump()
        {
            _animator.SetBool(IsJumping, true);
            _grounded = false;
            _currentJumpCount++;
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            _groundSensor.Disable(0.2f);
        }
    }
}