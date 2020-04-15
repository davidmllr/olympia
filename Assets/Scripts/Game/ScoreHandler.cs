namespace Game
{
    /// <summary>
    /// </summary>
    public class ScoreHandler
    {
        private long _score;

        /// <summary>
        /// </summary>
        public void Add()
        {
            _score++;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public long Get()
        {
            return _score;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _score = 0;
        }
    }
}