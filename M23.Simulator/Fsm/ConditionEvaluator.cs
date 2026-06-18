namespace M23.Simulator.Fsm;


/*
 * Checks if the system can keep running in the new configuration.
 */
public static class ConditionEvaluator
{
    public static bool CanRun(
        string inputBelt,
        FlapPosition flap,
        OutputBeltState m3,
        OutputBeltState m4,
        BeltState otherInputBelt)
    {
        bool m3Running = m3 == OutputBeltState.Running;
        bool m4Running = m4 == OutputBeltState.Running;
        bool otherStopped = otherInputBelt == BeltState.Stopped;

        return (inputBelt, flap) switch
        {
            ("M1", FlapPosition.S6) => m4Running && otherStopped,
            ("M2", FlapPosition.S6) => m4Running && otherStopped,
            ("M1", FlapPosition.S7) => m3Running,
            ("M2", FlapPosition.S7) => m4Running,
            ("M1", FlapPosition.S8) => m3Running && otherStopped,
            ("M2", FlapPosition.S8) => m3Running && otherStopped,
            _ => false
        };
    }
}