using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using UnityEngine;

namespace Logic.SerializationSystem
{
    [Serializable]
    public class SerializationContainer
    {
        [JsonProperty]
        private Dictionary<string, SerializationData> SerializedData = new ();

        public void SerializeALl(Dictionary<string, SerializableBehaviour> container)
        {
            foreach (var kvp in container)
                kvp.Value.Serialize();
            SerializedData = container
                .Where(x => x.Value.IsSerializable)
                .ToDictionary(
                    x => x.Key,
                    x => new SerializationData(x.Value.PrefabKey, x.Value._serializedFields)
                );
        }
        public void DeserializeAll<T>(Dictionary<string, T> container, out Dictionary<string, SerializationData> missing) where T : SerializableBehaviour
        {
            missing = new();
            Dictionary<string, T> found = new (container);
            foreach (var kvp in SerializedData)
            {
                if (found.ContainsKey(kvp.Key))
                {
                    found[kvp.Key].Deserialize(kvp.Value._serializedFields);
                }
                else
                    missing.TryAdd(kvp.Key, kvp.Value);
            }
        }
    }

    [Serializable]
    public struct SerializationData
    {
        public string PrefabKey;
        public Dictionary<string, object> _serializedFields;

        public SerializationData(string prefabKey, Dictionary<string, object> serializedFields)
        {
            PrefabKey = prefabKey;
            _serializedFields = serializedFields;
        }
    }
}