
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu( menuName = "ScriptableObjects/Day/RandomizedDay")]
public class RandomizedDay : ADay
{
    [field: SerializeField] public override int MinigamesCount { get; protected set; }

    [Header("Variable para evitar minijuegos repetidos")]
    [SerializeField] private int _nonRepeatingSequenceSize;
   
    [Header("Para que salga un minijuego con mas frecuencia\nse puede poner varias veces")]
    [SerializeField] private List<Minigame> _minigamesPool;
    
    public override IEnumerable<Minigame> GetMinigames()
    {
        return GenerateRandomMinigames();
    }

    //Esto es a lo mejor un poco fumada pero:
    //lo que hace es randomizar los minijuegos, o sea generar una lista de minijuegos de tamaño MinigamesCount.
    //Con el non repeating sequence size lo que hace es evitar que salgan muchos repetidos:
    //si esa variable es 0 no hace nunca nada, si es 1 no van a salir 2 minijuegos repetidos seguidos,
    //si es 2, para que salga un minijuego repetido tienen que salir al menos 2 diferentes,
    //o sea: minijuego A, minijuego B, minijuego C, minijuego A
    //si es 3 serian 3 minijuegos de por medio antes que se pueda repetir alguno, etc.
    private List<Minigame> GenerateRandomMinigames()
    {
        var minigames = new List<Minigame>();
        for (int i = 0; i < MinigamesCount; i++)
        {
            Minigame minigame;
            do minigame = _minigamesPool[Random.Range(0, _minigamesPool.Count)];
            while (GetDistanceToSameMinigame(minigames, minigame) <= _nonRepeatingSequenceSize);
        }
        
        return minigames;
    }

    private int GetDistanceToSameMinigame(List<Minigame> minigames, Minigame minigame)
    {
        int distance = 0;
        for (int i = minigames.Count - 1; i >= 0; i--)
        {
            distance++;
            if (minigames[i] != minigame) continue;

            return distance;
        }

        return int.MaxValue;
    }
    
    
}
