using UnityEngine;

namespace Character
{
    /// <summary>
    /// </summary>
    public class GroundSensor : MonoBehaviour
    {
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");

        private Animator _animator;

        private int _colCount;
        private float _disableTimer;

        /// <summary>
        /// </summary>
        private void OnEnable()
        {
            _colCount = 0;
        }

        /// <summary>
        /// </summary>
        private void Awake()
        {
            _animator = GetComponentInParent<Animator>();
        }

        /// <summary>
        /// </summary>
        private void Update()
        {
            _disableTimer -= Time.deltaTime;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool State()
        {
            if (_disableTimer > 0)
                return false;
            return _colCount > 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Ground")) return;
            _colCount++;
            _animator.SetBool(IsJumping, false);
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Ground")) return;
            _colCount--;
        }

        /// <summary>
        /// </summary>
        /// <param name="duration"></param>
        public void Disable(float duration)
        {
            _disableTimer = duration;
        }
    }
}