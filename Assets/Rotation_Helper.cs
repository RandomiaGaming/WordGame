using UnityEngine;
public static class Rotation_Helper
{
    public static float Deg_Clamp(float Input)
    {
        while (Input < 0 || Input >= 360)
        {
            Input -= 360 * Mathf.Sign(Input);
        }
        return Input;
    }
    public static float Deg_Distance(float Input_A, float Input_B)
    {
        Input_A = Deg_Clamp(Input_A);
        Input_B = Deg_Clamp(Input_B);
        float Output = Mathf.Max(Input_A, Input_B) - Mathf.Min(Input_A, Input_B);
        if (Output > 180)
        {
            Output = 180 - (Output - 180);
        }
        return Output;
    }
    public static Vector2 Deg_To_Vector(float Input)
    {
        Input = Deg_Clamp(Input);
        Vector2 Output = new Vector2();
        if (Input < 90)
        {
            Output.x = Input / 90;
            Output.y = (90 - Input) / 90;
        }
        else if (Input >= 90 && Input < 180)
        {
            Output.y = (Input - 90) / 90 * -1;
            Output.x = (90 - (Input - 90)) / 90;
        }
        else if (Input >= 180 && Input < 270)
        {
            Output.x = (Input - 180) / 90 * -1;
            Output.y = (90 - (Input - 180)) / 90 * -1;
        }
        else if (Input >= 270)
        {
            Output.y = (Input - 270) / 90;
            Output.x = (90 - (Input - 270)) / 90 * -1;
        }
        return Output;
    }
    public static Vector2 Vector_Clamp(Vector2 Input)
    {
        if (Input.x == Input.y)
        {
            return new Vector2(0.5f * Mathf.Sign(Input.x), 0.5f * Mathf.Sign(Input.y));
        }
        Vector2 Output = Vector2.zero;
        Output.x = Input.x / (Mathf.Abs(Input.x) + Mathf.Abs(Input.y));
        Output.y = Input.y / (Mathf.Abs(Input.x) + Mathf.Abs(Input.y));
        return Output;
    }
    public static float Vector_To_Deg(Vector2 Input)
    {
        Input = Vector_Clamp(Input);
        float Output = Input.x * 90;
        if (Mathf.Sign(Input.x) == 1 && Mathf.Sign(Input.y) == 1)
        {
            Output = Input.x * 90;
        }
        else if (Mathf.Sign(Input.x) == 1 && Mathf.Sign(Input.y) == -1)
        {
            Output = (Mathf.Abs(Input.y) * 90) + 90;
        }
        else if (Mathf.Sign(Input.x) == -1 && Mathf.Sign(Input.y) == -1)
        {
            Output = (Mathf.Abs(Input.x) * 90) + 180;
        }
        else if (Mathf.Sign(Input.x) == -1 && Mathf.Sign(Input.y) == 1)
        {
            Output = (Input.y * 90) + 270;
        }
        return Output;
    }
}
