using UnityEngine;

public static class UIMove
{
    public static Vector2 Huddoin(Vector2 start , Vector2 goal , float t)
    {
        if (t > 1) t = 1;
        if (t < 0) t = 0;

        Vector2 pos = Vector2.zero;

        pos = start + ((goal - start) * (t * t)/*Overshoot(t)*/);

        return pos;
    }

    public static Vector2 Huddoout(Vector2 start, Vector2 goal, float t)
    {
        if (t > 1) t = 1;
        if (t < 0) t = 0;

        Vector2 pos = Vector2.zero;

        pos = start + ((goal - start) * t);

        return pos;
    }

    //static float Overshoot(float t , float s = 1.5f/*オーバーシュート量*/)
    //{

    //}
}
