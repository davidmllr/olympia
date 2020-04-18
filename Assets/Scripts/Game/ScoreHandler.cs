namespace Game
{
    /// <summary>
    /// This class is used to keep track of the score obtained by the user.
    /// </summary>
    public class ScoreHandler
    {
        private long _score;

        /// <summary>
        /// Adds a point to the score.
        /// </summary>
        public void Add()
        {
            _score++;
        }
        
        /// <summary>
        /// Remove a point from the score (if it isn't zero).
        /// </summary>
        public void Remove()
        {
            if(_score > 0) _score--;
        }

        /// <summary>
        /// Gets the current score.
        /// </summary>
        /// <returns>The current score</returns>
        public long Get()
        {
            return _score;
        }

        /// <summary>
        /// Resets the current score to zero.
        /// </summary>
        public void Reset()
        {
            _score = 0;
        }
    }
}