public static class Calc
{
    public static int CorrectAngle(int angle)
    {
        if (angle < 0)
            return angle + 360;
        else if (angle > 359)
            return angle - 360;

        return angle;
    }

    // this is lazy because there's no way this angle would not be in 90 degree increments
    public static int GetXFromAngle(int angle)
    {
        return angle switch
        {
            90 => 1,
            270 => -1,
            _ => 0,
        };
    }

    public static int GetYFromAngle(int angle)
    {
        return angle switch
        {
            0 => -1,
            180 => 1,
            _ => 0,
        };
    }
}