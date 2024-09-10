namespace LPSharp.LPDriver.Contract
{
    /// <summary>
    /// Represents the solver type.
    /// </summary>
    public enum SolverType
    {
        /// <summary>
        /// Computational infrastructure for Operations Research (COIN-OR) linear programming solver.
        /// </summary>
        CLP,

        /// <summary>
        /// Google linear programming solver.
        /// </summary>
        GLOP,
    }
}
