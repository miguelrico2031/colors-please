
using System;
using ColorComparer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class DummyColorTester : MonoBehaviour
{
    [SerializeField] private Image _i1, _i2;
    [SerializeField] private TextMeshProUGUI _output;
    [SerializeField] private Comparer _comparer;
    

    public void OnGUI()
    {
        if(GUI.Button(new Rect(80, 80, 500, 100), "Test"))
            _output.text = $"porcentaje: {GetPercentage()}";

        if (GUI.Button(new Rect(700, 80, 500, 100), "Random"))
            _i1.color = RGB255.Random().ToColor();

    }


    private float GetPercentage()
    {
        var c1 = new RGB255(_i1.color);
        var c2 = new RGB255(_i2.color);
        return ComparerTool.GetSimilarity(c1, c2, _comparer);
    }
    
}