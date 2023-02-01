using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace Game.Turret
{
    using Player.Inventory;

    [CreateAssetMenu(fileName = "turret data", menuName = "Turret/new data")]
    public class TurretData : ScriptableObject
    {
        [Serializable]
        public struct DroppedItem
        {
            public InventoryItem Item;
            public int Amount;
        }
        [Serializable]
        public struct Variable
        {
            public enum VariableTypeEnum
            {
                Int, Float, String, Prefab
            }

            public string Name;
            public VariableTypeEnum VariableType;

            [ShowIf("VariableType", VariableTypeEnum.Int), AllowNesting, SerializeField] private int IntVariable;
            [ShowIf("VariableType", VariableTypeEnum.Float), AllowNesting, SerializeField] private float FloatVariable;
            [ShowIf("VariableType", VariableTypeEnum.String), AllowNesting, SerializeField] private string StringVariable;
            [ShowIf("VariableType", VariableTypeEnum.Prefab), AllowNesting, SerializeField] private GameObject PrefabVariable;

            public object GetValue()
            {
                switch (VariableType)
                {
                    case VariableTypeEnum.Int:
                        return IntVariable;
                    case VariableTypeEnum.Float:
                        return FloatVariable;
                    case VariableTypeEnum.String:
                        return StringVariable;
                    case VariableTypeEnum.Prefab:
                        return PrefabVariable;
                }
                return null;
            }
        }

        [SerializeField] private GameObject BulletPrefab;
        [SerializeField] private float Damage = 1;
        [SerializeField] private float TimeBtwAttack = 0.3f;
        [SerializeField] private float Recoil = 0f;
        [Space]
        [SerializeField, Tooltip("The size of zone which detect enemyes")] private float ColliderSize = 7f;

        [Header("BODY/LEGS")]
        [SerializeField] private Sprite BodySprite;
        [Header("CANON")]
        [SerializeField] private Sprite CanonSprite;
        [Space]

        [SerializeField] private List<DroppedItem> DroppedItems = new List<DroppedItem>(1);
        [SerializeField] private List<Variable> AdditionalVariables = new List<Variable>();

        public GameObject _bulletPrefab => BulletPrefab;
        public float _damage => Damage;
        public float _timeBtwAttack => TimeBtwAttack;
        public float _recoil => Recoil;
        public float _colliderSize => ColliderSize;
        public Sprite _bodySprite => BodySprite;
        public Sprite _canonSprite => CanonSprite;
        public List<DroppedItem> _droppedItems => DroppedItems;
        public List<Variable> _additionalVariables => AdditionalVariables;

        public object GetVariable(string key)
        {
            foreach (var variable in AdditionalVariables)
            {
                if(variable.Name == key)
                {
                    return variable.GetValue();
                }
            }
            Debug.LogWarning($"Variable {key} in {name} is not valid");
            return null;
        }
    }
}
