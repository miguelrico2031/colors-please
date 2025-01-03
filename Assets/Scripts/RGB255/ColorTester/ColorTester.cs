
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ColorTester : MonoBehaviour
{
    [SerializeField] private Image _image1, _image2;
    [SerializeField] private RectTransform _buttonsParent;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _finishButton;
    [SerializeField] private Button _buttonPrefab;
    [SerializeField] private TextMeshProUGUI _tutorial;
    [SerializeField] private int _maxSelectionsPerRound = 3;
    [SerializeField] private string _filePath;

    private readonly Dictionary<Comparer, int> _comparersPoints = new();
    private readonly Dictionary<Comparer, Button> _buttons = new();
    private float _selections;

    private void Start()
    {
        _nextButton.onClick.AddListener(Next);
        _finishButton.onClick.AddListener(Finish);

        foreach (Comparer comparer in Enum.GetValues(typeof(Comparer)))
        {
            var button = Instantiate(_buttonPrefab, _buttonsParent);
            _buttons.Add(comparer, button);
            button.onClick.AddListener(() => SelectComparer(comparer));
            

            button.gameObject.SetActive(false);
        }
    }


    private void Next()
    {
        if (_tutorial.gameObject.activeSelf)
        {
            _tutorial.gameObject.SetActive(false);
            foreach(var button in _buttons.Values)
                button.gameObject.SetActive(true);
        }
        MixButtons();
        
        _selections = 0;
        //generar randoms
        var c1 = RGB255.Random();
        var c2 = RGB255.Random();
        
        _image1.color = c1.ToColor();
        _image2.color = c2.ToColor();
        
        foreach(var (key, btn) in _buttons)
        {
            var similarity = ComparerTool.GetSimilarity(c1, c2, key);
            btn.interactable = true;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = $"{similarity:P}";
        }
    }

    private void Finish()
    {
        SaveComparerScores();
    }

    private void SelectComparer(Comparer comparer)
    {
        if (_selections >= _maxSelectionsPerRound)
            return;
        _buttons[comparer].interactable = false;
        if(!_comparersPoints.TryAdd(comparer, 1))
            _comparersPoints[comparer]++;
        
        _selections++;
    }

    private void MixButtons()
    {
        // Crear un array con los hijos
        Transform[] children = new Transform[_buttonsParent.childCount];
        for (int i = 0; i < _buttonsParent.childCount; i++)
        {
            children[i] = _buttonsParent.GetChild(i);
        }

        // Randomizar el orden
        for (int i = 0; i < children.Length; i++)
        {
            Transform temp = children[i];
            int randomIndex = UnityEngine.Random.Range(i, children.Length);
            children[i] = children[randomIndex];
            children[randomIndex] = temp;
        }

        // Asignar los hijos en el nuevo orden
        foreach (Transform child in children)
        {
            child.SetSiblingIndex(0);
        }
    
    }
    
    public void SaveComparerScores()
    {
        // Convertir a una lista de pares clave-valor
        string text = "{\n";
        int count = 0;
        foreach (var pair in _comparersPoints)
        {
            text += $"\t\"{pair.Key}\": {pair.Value}";
            text += (++count == _comparersPoints.Count) ? "\n" : ",\n";
        }

        if (_comparersPoints.Count > 0) text = text.TrimEnd(',');
        File.WriteAllText(_filePath, text + "}");
    }


}
