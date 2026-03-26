using UnityEngine;

public class ParameterUiTest : MonoBehaviour
{

    [SerializeField] ParameterUI parameterUI;
    [SerializeField] float value;

    [ContextMenu("セットHP")]
    public void SetHP()
    {
        parameterUI.SetHP(value);
    }

    [ContextMenu("セットO2")]
    public void SetOTwo()
    {
        parameterUI.SetOTwo(value);
    }

    [ContextMenu("セット燃料")]
    public void SetFuel()
    {
        parameterUI.SetFuel(value);
    }
}
