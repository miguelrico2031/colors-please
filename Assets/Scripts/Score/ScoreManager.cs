
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ScoreManager")]
public class ScoreManager : ScriptableObject, IScoreService
{
    [SerializeField] private int _minMoney, _maxMoney;
    [SerializeField] private float _noMoneyPercentageThreshold;
    [SerializeField] private float _tipPercentageThreshold;
    [SerializeField] private int _minTip;  
    [SerializeField] private int _maxTip;  
    
    
    
    
    public Score GetScore(RGB255 targetColor, RGB255 guessedColor)
    {
        Score score = new Score();
        float similarity = RGB255.GetSimilarity(targetColor, guessedColor);
        var percentage = 100f * similarity;
        score.Percentage = percentage;

        //hay 2 umbrales, el noMoney, que si el porcentaje es mas bajo que el umbral no se recibe nada de dinero
        // y el tip, que si el porcentaje es mas alto, se recibe ademas una propina
        //el dinero normal se calcula interpolando entre minMoney y MaxMoney, pero no se usa el porcentaje como
        //valor T, sino que hace interpolacion reversa para saber donde esta el porcentaje entre ambos umbrales
        //es decir si el umbral noMoney es 30% y el tip es 90%, si sacamos 50% de porcentaje, no es que nuestro valor
        //T para interpolar el dinero sea 0.5, sino 0.33
        //lo mismo hacemos para la propina, con interpolacion reversa entre el umbral tip y 100%.
        
        if (percentage > _noMoneyPercentageThreshold)
        { 
            float moneyT = Mathf.InverseLerp(_noMoneyPercentageThreshold, _tipPercentageThreshold, percentage);
            score.Money = (uint) Mathf.RoundToInt(Mathf.Lerp(_minMoney, _maxMoney, moneyT));
        }

        if (percentage >= _tipPercentageThreshold)
        {
            float tipT = Mathf.InverseLerp(_tipPercentageThreshold, 100, percentage);
            score.Tip = (uint) Mathf.RoundToInt(Mathf.Lerp(_minTip, _maxTip, tipT));
        }

        return score;
    }


}
