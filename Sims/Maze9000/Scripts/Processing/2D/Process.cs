using MazeGen.CellStructures;


namespace MazeGen.Processing
{
    public abstract class Process
    {
        // Returns true if process succeeded
        public abstract bool RunProcess();


        // Call RunProcess() until it returns false
        public void RunWhileHasSteps()
        {
            while (RunProcess ())
            {
            }
        }

        // Returns true if all steps returned true, or false if any step returns false
        public bool RunForSteps(int runForSteps)
        {
            for (int i = 0; i < runForSteps; i++)
            {
                if (!RunProcess ())
                    return false;
            }
            return true;
        }

    }
}
