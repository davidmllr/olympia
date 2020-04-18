using UnityEngine;

namespace Character
{
    /// <summary>
    /// This class handles ground checking related to the jumping behaviour by the character.
    /// It is mostly adapted from a CharacterController off the Asset Store by Sven Thole, which you can find here:
    /// https://assetstore.unity.com/packages/2d/characters/bandits-pixel-art-104130
    /// </summary>
    public class GroundSensor : MonoBehaviour
    {
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");

        private Animator _animator;

        private int _colCount;
        private float _disableTimer;

        /// <summary>
        /// When script is enabled, set column count to zero.
        /// </summary>
        private void OnEnable()
        {
            _colCount = 0;
        }

        /// <summary>
        /// When script awakes, assign animator.
        /// </summary>
        private void Awake()
        {
            _animator = GetComponentInParent<Animator>();
        }

        /// <summary>
        /// Every frame, reduce the disable timer by the time that passed since the last frame.
        /// </summary>
        private void Update()
        {
            _disableTimer -= Time.deltaTime;
        }

        /// <summary>
        /// Checks if character is currently on the ground.
        /// </summary>
        /// <returns>If character is on the ground</returns>
        public bool State()
        {
            if (_disableTimer > 0)
                return false;
            return _colCount > 0;
        }

        /// <summary>
        /// When character enters a trigger, which is ground, stop all jumping animation and increase column count.
        /// </summary>
        /// <param name="other">Collider2D that was entered</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Ground")) return;
            _colCount++;
            _animator.SetBool(IsJumping, false);
        }

        /// <summary>
        /// When character leaves a trigger, which is ground, decrease column count.
        /// </summary>
        /// <param name="other">Collider2D that was left</param>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Ground")) return;
            _colCount--;
        }

        /// <summary>
        /// Performs a disabling mechanism to make sure jumps are not glitching.
        /// </summary>
        /// <param name="duration">Duration of the disabling mechanism</param>
        public void Disable(float duration)
        {
            _disableTimer = duration;
        }
    }
}