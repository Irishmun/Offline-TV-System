namespace OTS.Util
{
    //TODO: decide on how START_AND_END and PER_SEASON handle knowing which episodes have already been played
    //TODO: add a "new_episodes" enum to handle when a new episode will air (weekly,monthly,syndicated)

    /// <summary>How important is it that the episodes are in sequential order</summary>
    public enum SequentialImportance
    {
        /// <summary>Episode order does not matter, any episode can play after any episode</summary>
        ANY_ORDER = 0,
        /// <summary>Every episode has to play in order.</summary>
        ALL_IN_ORDER = 1,
        /// <summary>Only the first and last episode of the season matter, 
        /// every episode in between can be played in any order</summary>
        START_AND_END = 2,
        /// <summary>Episode order does not matter, 
        /// but the next season can not be played until 
        /// the current season has played all of its episodes</summary>
        PER_SEASON = 3
    }
}
