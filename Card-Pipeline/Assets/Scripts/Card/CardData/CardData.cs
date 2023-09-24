// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardData.cs
// Modified: 2023/05/21 @ 19:22
// Brief: Card Data ScriptableObject

#region

using System;
using UnityEditor;
using UnityEngine;

#endregion

[CreateAssetMenu(menuName = "Card")]
public class CardData : ScriptableObject
{
    public enum CardType
    {
        Creature,
        //Spell //not implemented 
    }

    #region Editable Data

    public string Name;
    public int Cost;
    public int Attack;
    public int Health;

    #endregion

    [NonSerialized] public Card card = null;

    [SerializeField] private CardEffectTree Effect;


    [HideInInspector] public bool IsClone;


    private string path = string.Empty;
    private string DefaultPath => $"Assets/ScriptableObjects/CardEffectTrees/Generated/{name}DefaultEffectTree.asset";
    public CardType Type;


    public CardEffectTree GetEffectTree()
    {
        return Effect == null ? LoadOrCreateEffectAsset() : Effect;
    }

    private CardEffectTree LoadOrCreateEffectAsset()
    {
        if (IsClone) return null; //don't create an effect tree for a clone
        if (path == string.Empty) //use default path if path is empty
            path = DefaultPath;
        CardEffectTree existingTree = AssetDatabase.LoadAssetAtPath<CardEffectTree>(path);
        if (existingTree == null) //if there was no tree at path
        {
            Effect = CreateInstance<CardEffectTree>();
            Effect.name = name + "DefaultEffectTree";
            AssetDatabase.CreateAsset(Effect, path);
        }
        else //use existing tree
        {
            Effect = existingTree;
        }
        Effect.SetOwner(this);
        AssetDatabase.SaveAssets();
        return Effect;
    }

    public void Init()
    {
        if (Effect == null) LoadOrCreateEffectAsset();
        if (Effect == null) return;
        Effect.LoadFromScriptableObject();
        Effect.SetOwner(this);
        Effect.SetupFields();
    }

    public void OnDestroy()
    {
        if (Application.isPlaying) return;
        if (path == DefaultPath) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Effect)); 
        //delete default effect tree when asset is destroyed
    }

    public CardData Clone()
    {
        CardData clone = CreateInstance<CardData>();
        clone.Type = Type;
        clone.Name = Name;
        clone.Cost = Cost;
        clone.Attack = Attack;
        clone.Health = Health;
        clone.Effect = Effect.Clone(clone); 
        clone.IsClone = true;

        return clone;
    }

    //https://forum.unity.com/threads/ondestroy-and-ondisable-are-not-called-when-deleting-a-scriptableobject-file.1129220/#post-7259671
    private class OnDestroyProcessor : AssetModificationProcessor
    {
        // Cache the type for reuse.
        private static readonly Type _type = typeof(CardData);

        // Limit to certain file endings only.
        private static readonly string _fileEnding = ".asset";

        public static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions _)
        {
            if (!path.EndsWith(_fileEnding))
                return AssetDeleteResult.DidNotDelete;

            var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (assetType != null && (assetType == _type || assetType.IsSubclassOf(_type)))
            {
                var asset = AssetDatabase.LoadAssetAtPath<CardData>(path);
                asset.OnDestroy();
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}