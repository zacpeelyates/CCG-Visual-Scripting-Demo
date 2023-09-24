// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: VisualManager.cs
// Modified: 2023/05/15 @ 03:06

#region

using UnityEngine;

#endregion

public class VisualManager : MonoBehaviour
{
    public static VisualManager Instance;
    public Vector3 cardHoverOffset;
    public GameObject cardPrefab;
    [SerializeField] private Card cardPreview;

    public void ShowCardPreview(Card card)
    {
        cardPreview.CopyCardDisplay(card);
        ShowCardPreview();
    }

    public void ShowCardPreview()
    {
        cardPreview.gameObject.SetActive(true);
    }

    public void HideCardPreview()
    {
        cardPreview.gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        GameStateManager.GetInstance().BeginGame();
    }
}