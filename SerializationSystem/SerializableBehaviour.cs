using System;
using System.Collections.Generic;
using System.Linq;

using Logic.GameCustom.Abstracts;
using Logic.SerializationSystem.Entities;

using Newtonsoft.Json;
using Sirenix.OdinInspector;

using UnityEngine;

namespace Logic.SerializationSystem
{
    public class SerializableBehaviour : MonoBehaviour
    {
        public const string PrefabPath = "Prefabs/";

        private protected bool _cantBeSerialized = false;
        [BoxGroup("Serialization")] [DisableIf("_cantBeSerialized")] public bool IsSerializable = false;
        [BoxGroup("Serialization")] [ReadOnly] public string PrefabKey;
        public bool HasPrefabKey => !string.IsNullOrEmpty(PrefabKey);
        
        private bool _isLoaded = false;
        private protected bool IsLoaded => _isLoaded;
        public void MarkAsLoaded() => _isLoaded = true;

        #if UNITY_EDITOR
        [Button][BoxGroup("Serialization")] 
        public void UpdatePrefabKey()
        {
            if (Application.isPlaying) return;
            if (string.IsNullOrEmpty(PrefabKey))
                return;
            string path = "Assets/Resources/" + PrefabPath + PrefabKey + ".prefab";
            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
                return;
            PrefabKey = gameObject.name;
            prefab.name = PrefabKey;
            prefab.GetComponent<SerializableBehaviour>().PrefabKey = PrefabKey;
            UnityEditor.EditorUtility.SetDirty(prefab);
            UnityEditor.AssetDatabase.SaveAssets();
        }
        [Button][BoxGroup("Serialization")] 
        public void CreatePrefab()
        {
            if (Application.isPlaying) return;
            if (!string.IsNullOrEmpty(PrefabKey))
            {
                UpdatePrefabKey();
                return;
            }
            PrefabKey = gameObject.name;
            string path = "Assets/Resources/" + PrefabPath + gameObject.name + ".prefab";
            System.IO.Directory.CreateDirectory("Assets/Resources/Prefabs/");
            
            UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, path, UnityEditor.InteractionMode.UserAction);
        }
        [Button][BoxGroup("Serialization")] 
        public void DeletePrefab()
        {
            if (Application.isPlaying) return;
            if (string.IsNullOrEmpty(PrefabKey))
                return;
            string path = "Assets/Resources/Prefabs/" + PrefabKey + ".prefab";
            UnityEditor.AssetDatabase.DeleteAsset(path);
            PrefabKey = string.Empty;
        }
        #endif
        
        public Dictionary<string, object> _serializedFields = new ();
        
        [ShowInInspector, ReadOnly][BoxGroup("Serialization")] 
        private Dictionary<string, ISerialize> _serializeRules = new ();
        
        private protected void RegisterSerializable<T>(string key, Func<T> _get, Action<T> _set)
            => _serializeRules.TryAdd(key, new SerializeEntity<T>(_get, _set));

        public void Serialize()
        {
            _serializedFields = _serializeRules
                .ToDictionary(x => x.Key, x => x.Value.Get());
        }
        public void Deserialize(Dictionary<string, object> _cache)
        {
            foreach (var rule in _serializeRules)
                if (_cache.ContainsKey(rule.Key))
                    rule.Value.Set(_cache[rule.Key]);
            MarkAsLoaded();
        }
    }
}