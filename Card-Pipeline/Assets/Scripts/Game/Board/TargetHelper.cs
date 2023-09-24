// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: TargetHelper.cs
// Modified: 2023/05/21 @ 19:22
// Brief: Class for handling target selection and condition checking

#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

public class TargetHelper : MonoBehaviour
{
    public static TargetHelper Instance;
    private Action<List<ITarget>> callback;
    private ABILITY_TARGET_FLAGS flags;
    [SerializeField] private Material InvalidTarget;

    private bool meetAll;
    private MeshRenderer mr;
    private ITarget source;
    [SerializeField] private GameObject TargetIndicator;

    private List<ITarget> targets;


    private int TargetsToSelect = 1;
    [SerializeField] private Material ValidTarget;

    public bool CurrentlyRequestingTarget { get; private set; }


    public void RequestTarget(ITarget source, ABILITY_TARGET_FLAGS condition, int count, Action<List<ITarget>> action,
        bool mustMeetAllTargets = false, ITarget[] existingTargets = null)
    {
        //Handle existing targets 
        if (existingTargets != null && existingTargets.Length != 0)
        {
            targets = new List<ITarget>(existingTargets);
            targets.ForEach(x => x.CanBeTargeted = false);
            count -= targets.Count;
        }
        else
        {
            targets = new List<ITarget>();
        }

        //Assign values
        CurrentlyRequestingTarget = true;
        TargetsToSelect = count;
        flags = condition;
        this.source = source;
        callback = action;
        meetAll = mustMeetAllTargets;
        TargetIndicator.SetActive(true);
        Debug.LogFormat("Beginning target request of {0} targets", count);
    }

    public void StopRequestingTarget()
    {
        CurrentlyRequestingTarget = false;
        TargetIndicator.SetActive(false);
    }

    private void Start()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        TargetIndicator.SetActive(false);
        mr = TargetIndicator.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (!CurrentlyRequestingTarget) return;
        if (Physics.Raycast(MouseUtils.GetMousePosition(), Vector3.down, out RaycastHit hit, 1))
        {
            TargetIndicator.transform.position = hit.point;

            if (hit.collider.GetComponent<ITarget>() is not { } target || target is Card { currentZone: not InPlay })
            {
                mr.material = InvalidTarget;
                return;
            }

            if (!target.CanBeTargeted || !target.MeetsCondition(source, flags, meetAll)) return;
            if (mr.material != ValidTarget) mr.material = ValidTarget;
            if (!Input.GetMouseButtonDown(0)) return;

            target.CanBeTargeted = false;
            targets.Add(target);
            TargetsToSelect--;
            Debug.LogFormat("Target Selected: {0} - {1} targets left to select", target, TargetsToSelect);
            if (TargetsToSelect != 0) return; //continue target selection until 0 targets

            //complete selection 
            foreach (ITarget t in targets) t.CanBeTargeted = true;
            callback.Invoke(targets); //Invoke method that needed targets 
            Debug.Log("Targeting Complete");
            StopRequestingTarget();
        }
    }


    public static List<ITarget> GetLegalTargets(ITarget perspective, ABILITY_TARGET_FLAGS condition, bool meetAll)
    {
        return Board.instance.GetAllTargets().Where(x => x.MeetsCondition(perspective, condition, meetAll)).ToList();
    }

    public static int GetLegalTargetCount(ITarget perspective, ABILITY_TARGET_FLAGS condition, bool meetAll,
        out List<ITarget> list)
    {
        return (list = GetLegalTargets(perspective, condition, meetAll)).Count;
    }
}