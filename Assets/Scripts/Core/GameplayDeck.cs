using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GameplayDeck : MonoBehaviour
{
    [System.Serializable]
    public struct DeckCard
    {
        public Card cardData;
    }

    [SerializeField] private int remainingCards;
    [SerializeField] private Queue<DeckCard> queue;
    [SerializeField] List<DeckCard> deckCards;


    public Queue<DeckCard> GetQueue() => queue;

    private void Awake()
    {
        queue = new Queue<DeckCard>(deckCards);
        remainingCards = queue.Count;
    }

    public Card Draw()
    {
        if(queue.Count <= 0)
        {
            print("ERROR: Deck runs out, returning null");
            return null;
        }

        var drawnDeckCard = queue.Dequeue();
        remainingCards = queue.Count;
        return drawnDeckCard.cardData;
    }
}





// To enable showing queue in inspector
[CustomEditor(typeof(GameplayDeck))]
public class InspectQueueScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameplayDeck myScript = (GameplayDeck)target;

        if (!Application.isPlaying) return;

        int maxPeek = 5;
        EditorGUILayout.LabelField($"Queue Contents: (max peek: {maxPeek})");

        int peekCounter = 0;
        foreach (var item in myScript.GetQueue())
        {
            peekCounter++;
            EditorGUILayout.LabelField($"{peekCounter}: {item.cardData}");
            if(peekCounter >= maxPeek)
            {
                break;
            }
        }
    }
}