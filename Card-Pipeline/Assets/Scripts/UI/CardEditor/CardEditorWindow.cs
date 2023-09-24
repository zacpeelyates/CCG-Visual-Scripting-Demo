// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEditorWindow.cs
// Modified: 2023/05/13 @ 22:29
// Brief: Editor Window for Card Editor

#region

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

public class CardEditorWindow : EditorWindow
{
    private static CardEffectGraphView cardEffectGraphView;
    private static NodeInspector nodeInspector;
    private static Label TreeDescription;

    public static CardEditorWindow instance;

    [SerializeField] private VisualTreeAsset m_VisualTreeAsset;

    public void CreateGUI()
    {
        //Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        //Instantiate UXML
        m_VisualTreeAsset =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/UI/CardEditor/CardEditorWindow.uxml");
        m_VisualTreeAsset.CloneTree(root);

        //Import USS
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/UI/CardEditor/CardEditorWindow.uss");
        root.styleSheets.Add(styleSheet);

        //Query root for values
        cardEffectGraphView = root.Q<CardEffectGraphView>();
        nodeInspector = root.Q<NodeInspector>();
        TreeDescription = root.Q<Label>("Tree-Desc");

        //Setup callbacks
        cardEffectGraphView.OnNodeSelected = OnNodeSelectionChanged;
        cardEffectGraphView.OnGraphViewChange += UpdateDescriptionText;
         
        //Initialize 
        PopulateGraphView();
        UpdateDescriptionText();
    }

    public static void Open()
    {
        instance?.Close();
        instance = null;
        instance = GetWindow<CardEditorWindow>("Card Editor");
    }

    public void UpdateDescriptionText()
    {
        if (cardEffectGraphView == null || cardEffectGraphView.CurrentTree == null) return;
        TreeDescription.text = cardEffectGraphView.CurrentTree.Parse(true);
        cardEffectGraphView.UpdateDescription();
    }

    private void OnEnable()
    {
        Selection.selectionChanged += PopulateGraphView;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= PopulateGraphView;
    }

    private void PopulateGraphView()
    {
        Object obj = Selection.activeObject;
        if (obj == null) return;
        //Populate with active object 
        if (obj is CardData card) cardEffectGraphView.Populate(card.GetEffectTree());
        else if (obj is CardEffectTree tree) cardEffectGraphView.Populate(tree);
    }

    private void OnNodeSelectionChanged(VisualNode node)
    {
        nodeInspector.UpdateSelection(node);
    }

    private void OnInspectorUpdate()
    {
        cardEffectGraphView?.UpdateNodeClasses();
    }

    private void OnDestroy()
    {
        EditorUtility.SetDirty(cardEffectGraphView.CurrentTree);
        AssetDatabase.SaveAssets();
    }
}