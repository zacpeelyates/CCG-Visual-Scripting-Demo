// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: VisualNode.cs
// Modified: 2023/05/13 @ 22:29
// Brief: Extenstion of GraphView Node 

#region

using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

public class VisualNode : Node
{
    public CardEffectNode data;
    public Port inputPort;
    public Action<VisualNode> OnNodeSelected;
    public Port outputPort;

    public VisualNode(CardEffectNode data, CardEffectGraphView graphView) : base(
        "Assets/Scripts/UI/CardEditor/CardEffectNode.uxml")
    {
        if (data == null) return;
        this.data = data;
        title = data.GetType().ToString().RemoveString("CardEffect", "Node");
        viewDataKey = data.GUID;
        style.left = data.CachedGraphPosition.x;
        style.top = data.CachedGraphPosition.y;
        CreatePorts();
        OnNodeSelected = graphView.OnNodeSelected;
        SetupNodeClass();
    }

    public sealed override string title
    {
        get => base.title;
        set => base.title = value;
    }

    public void SetupNodeClass()
    {
        AddToClassList(data switch
        {
            CardEffectRootNode => "root",
            CardEffectActionNode => "action",
            CardEffectDecoratorNode => "decorator",
            CardEffectCompositeNode => "composite",
            _ => "unknown"
        });
    }

    public void UpdateNodeStateClasses()
    {
        ClearNodeStateClass();
        AddToNodeStateClass();
    }

    private void ClearNodeStateClass() //remove from all classes 
    {
        RemoveFromClassList("running");
        RemoveFromClassList("success");
        RemoveFromClassList("failure");
        RemoveFromClassList("idle");
        RemoveFromClassList("unknown");
    }

    private void AddToNodeStateClass() //add to appropriate node state class 
    {
        AddToClassList(data?.State switch
        {
            CardEffectNode.NodeState.Running => "running",
            CardEffectNode.NodeState.Success => "success",
            CardEffectNode.NodeState.Failure => "failure",
            CardEffectNode.NodeState.Idle => "idle",
            _ => "unknown"
        });
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(data, "Move");
        data.CachedGraphPosition.x = newPos.xMin;
        data.CachedGraphPosition.y = newPos.yMin;
        EditorUtility.SetDirty(data);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    private void CreatePorts()
    {
        inputPort = data.GetInputPortType() switch //instantiate input port 
        {
            CardEffectNode.PortType.Single => InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(VisualNode)),
            CardEffectNode.PortType.Multi => InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(VisualNode)),
            _ => inputPort
        };

        outputPort = data.GetOutputPortType() switch //instantiate output port 
        {
            CardEffectNode.PortType.Single => InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(VisualNode)),
            CardEffectNode.PortType.Multi => InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(VisualNode)),
            _ => outputPort
        };

        if (inputPort != null) 
        {
            //setup input port 
            inputPort.portName = "";
            inputPort.style.flexDirection = FlexDirection.Column;
            inputPort.style.alignSelf = Align.Center;
            inputPort.style.translate = new Translate(0, Length.Percent(25), 0);
            inputContainer.Add(inputPort);
        }

        if (outputPort == null) return;
        //setup output port
        outputPort.portName = "";
        outputPort.style.flexDirection = FlexDirection.ColumnReverse;
        outputPort.style.alignSelf = Align.Center;
        outputPort.style.translate = new Translate(0, Length.Percent(-25), 0);
        outputContainer.Add(outputPort);
    }

    public void UpdateDescription()
    {
        Label l = this.Q<Label>("description");
        if (l == null) return;
        l.text = data?.GetDescriptionString(false);
    }
}