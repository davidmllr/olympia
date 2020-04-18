using UnityEngine;

namespace Character
{
    /// <summary>
    /// This class handles the basic controlling of the character used in the game.
    /// Moving in general was disabled, for now, only jumping is possible.
    /// It is mostly adapted from a CharacterController off the Asset Store by Sven Thole, which you can find here:
    /// https://assetstore.unity.com/packages/2d/characters/bandits-pixel-art-104130
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
        /// Assigns some variables when the script is loaded.
        /// </summary>
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _groundSensor = transform.Find("GroundSensor")
                .GetComponent<GroundSensor>();
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Checks current state of the character in every frame.
        /// Also checks for input by the user to perform a jump.
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
            
            /* Jump */
            if (Input.GetButtonDown("Jump") &&
                (_grounded || _currentJumpCount < jumpCount))
                Jump();
        }

        /// <summary>
        /// Lets the character perform a jump.
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