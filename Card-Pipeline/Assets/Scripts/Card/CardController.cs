// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardController.cs
// Modified: 2023/05/15 @ 03:06

#region

using UnityEngine;
using UnityEngine.UIElements;

#endregion

internal class CardController : MonoBehaviour
{
    private bool attacking;
    private bool cached;
    private CardSlot cachedCardSlot;

    private Vector3 cachedPosition;

    public bool canAttack = true;
    [HideInInspector] public Card card;
    [HideInInspector] public IZone currentZone;

    private bool drag;

    private Coroutine moveRoutine;

    private void Start()
    {
        card = GetComponent<Card>();
    }

    public void OnMouseEnter() 
    {
        if (currentZone is Hand) //hand hover 
        {
            if (!cached)
            {
                cachedPosition = transform.position;
                cached = true;
            }

            moveRoutine = StartCoroutine(MoveUtils.MoveToTargetPosition(transform,
                cachedPosition + VisualManager.Instance.cardHoverOffset, 0.4f, MathUtils.SmoothStep, 3));
        }
    }

    public void OnMouseOver() //right click card preview 
    {
        if (Input.GetMouseButtonDown((int)MouseButton.RightMouse)) VisualManager.Instance.ShowCardPreview(card);
    }

    public void OnMouseExit() //stop hover
    {
        VisualManager.Instance.HideCardPreview();
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        if (currentZone is Hand) transform.position = cachedPosition;
    }

    public void OnMouseDown() //select 
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        if (currentZone is InPlay) HandleClickInPlay();
    }

    public void HandleClickInPlay()
    {
        GameStateManager gsm = GameStateManager.GetInstance();
        if (gsm.IsFriendlyCombat && canAttack && !attacking) //attack
        {
            attacking = true;
            cachedPosition = transform.position;
            transform.position += VisualManager.Instance.cardHoverOffset;
        }
    }


    public void OnMouseDrag() 
    {
        if (!drag) drag = true;

        if (currentZone is not Hand) return; 
        transform.position = MouseUtils.GetMousePosition();
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1)) return;
        if (hit.collider.GetComponent<CardSlot>() is { } cardSlot && cardSlot != cachedCardSlot) cachedCardSlot = cardSlot; //store slot to play card on release
    }

    public void OnMouseUp()
    {
        if (currentZone is Hand) //play card 
        {
            if (moveRoutine != null) StopCoroutine(moveRoutine);
            if (cachedCardSlot == null || !Board.instance.PlayCard(card, cachedCardSlot)) transform.position = cachedPosition; //if couldnt play card, move it back to hand
            drag = false;
        }
        else if (currentZone is InPlay && GameStateManager.GetInstance().IsFriendlyCombat && canAttack) //attack
        {
            if (Physics.Raycast(MouseUtils.GetMousePosition(), Vector3.down, out RaycastHit hit, 1))
            {
                ITarget target = null;
                if (hit.collider.GetComponent<ITarget>() is { } t) target = t;
                else if (hit.collider.GetComponent<CardSlot>() is { } slot) target = slot.card;
                if (target != null && target.Owner != card.Owner)
                {
                    card.HandleAttack(target);
                    canAttack = false;
                    Debug.Log("attacked " + target);
                }
            }

            attacking = false;
            transform.position = cachedPosition;
        }
    }
}