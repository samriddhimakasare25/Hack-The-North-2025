using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ChoiceUI : MonoBehaviour
{
    [System.Serializable]
    public class ChoiceData
    {
        public string id;
        public string text;      // visible line
        public int rewardCash;   // +$
        public int damageHp;     // -HP (internal only)
    }

    [Header("Setup")]
    public RectTransform listRoot;         // drag your ChoicesPanel here
    public GameObject choiceButtonPrefab;  // drag your Button(TMP) prefab here

    readonly List<GameObject> spawned = new();
    System.Action<ChoiceData> onChoiceSelected;

    public void Init(System.Action<ChoiceData> callback) => onChoiceSelected = callback;

    public void ShowChoices(List<ChoiceData> choices)
    {
        Clear();
        foreach (var c in choices)
        {
            var go = Object.Instantiate(choiceButtonPrefab, listRoot);
            var btn = go.GetComponent<Button>();
            var label = go.GetComponentInChildren<TMP_Text>();

            label.alignment = TextAlignmentOptions.Midline;
            label.color = Color.black;
            label.text = $"{c.text}   (+${c.rewardCash})";

            var captured = c;
            btn.onClick.AddListener(() => onChoiceSelected?.Invoke(captured));
            spawned.Add(go);
        }
    }

    public void Clear()
    {
        foreach (var g in spawned) if (g) Object.Destroy(g);
        spawned.Clear();
    }
}
