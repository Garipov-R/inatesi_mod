using InatesiCharacter.Testing.InatesiArch.Character.Abilities;
using InatesiCharacter.Testing.InatesiArch.WeaponsTest;
using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.InventorySystems
{
    public class CharacterInventoryInteraction : AbilityBase
    {
        private InventoryContainer InventoryContainer { get => CharacterBase.InventoryContainer; }

        public override void Start()
        {
            //_inventoryContainer = new InventoryContainer();
            CharacterBase.InventoryContainer = new InventoryContainer();


            InventoryProccessTest();
        }

        public override void Update()
        {
            if (Input.Pressed("Use"))
            {
                RaycastHit hit;
                var isHit = Physics.Raycast(
                    CharacterMotion.LookSource.LookPosition(),
                    CharacterMotion.LookSource.Transform.forward,
                    out hit,
                    15f + Vector3.Distance(CharacterMotion.transform.position, CharacterMotion.LookSource.Transform.position),
                    CharacterMotion.RaycastLayer,
                    QueryTriggerInteraction.Collide
                );

                if (isHit == true)
                {
                    Debug.Log($"hit: {hit.transform.name}");

                    if (hit.transform.TryGetComponent(out InventorySystems.PickUpInventoryItem pickUpInventoryItem))
                    {
                        var status = InventoryContainer.Add(pickUpInventoryItem.InventoryItem, pickUpInventoryItem.ActivePickUp);

                        if (status == true)
                        {
                            pickUpInventoryItem.Take();
                            UnityEngine.GameObject.Destroy(pickUpInventoryItem.gameObject);
                        }
                    }
                }
            }


            if (Input.Pressed("slot1")) SetActiveInventoryitem(0);
            if (Input.Pressed("slot2")) SetActiveInventoryitem(1);
            if (Input.Pressed("slot3")) SetActiveInventoryitem(2);
            if (Input.Pressed("slot4")) SetActiveInventoryitem(3);

            if (Input.Pressed("drop")) DropInventoryItem();
        }


        #region Invontory behaviour

        private void InventoryProccessTest()
        {
            InventoryContainer.OnAdded += (item, active) =>
            {
                Debug.Log($"name item: { item.Name} {item.SlotIndex}");
                if (active == true) SetActiveInventoryitem(item.SlotIndex);
            };
        }

        private void AddInventoryItem(int id)
        {
            var item = InventoryContainer.GetSlot(id);

            if (item == null)
                return;


        }

        private void SetActiveInventoryitem(int id)
        {
            var item = InventoryContainer.GetSlot(id);
            var currentItem = InventoryContainer.ActiveItem;

            if ( item == currentItem)
                return;

            CharacterVars.CharacterWorldInteractionSystem.DestroyRightHandObject();
            SetTypeInventoryItem(null, null);

            InventoryContainer.ActiveSlotIndex = id;

            if (item == null)
            {
                return;
            }
                

            var g = GameObject.Instantiate(item.ItemScriptableObject.Prefab);

            SetTypeInventoryItem(g, item);

            if (g != null)
            {
                CharacterVars.CharacterWorldInteractionSystem.SetRightHandObject(g);

                if (g.TryGetComponent(out Rigidbody rb)) { rb.isKinematic = true; }
            }
        }

        private void DropInventoryItem()
        {
            if (InventoryContainer.TryGetActiveItem(out var item) == true)
            {
                CharacterVars.CharacterWorldInteractionSystem.DestroyRightHandObject();

                //UnityEngine.Object.Destroy(_currentInventoryItem);
                var g = UnityEngine.Object.Instantiate(item.ItemScriptableObject.Prefab); 
                g.transform.SetPositionAndRotation(
                    CharacterMotion.transform.position + CharacterMotion.transform.forward * 1f + CharacterMotion.transform.up * 2f,
                    Quaternion.identity
                );

                if (g.TryGetComponent(out Rigidbody rb)) 
                {
                    rb.AddForce(CharacterMotion.transform.forward * 5f + CharacterMotion.transform.up * 10f, ForceMode.VelocityChange);
                }

                switch (item.TypeItem)
                {
                    case TypeItem.Weapon:
                        var attackAbility = CharacterBase.GetAbility<Attack>() as Attack;
                        attackAbility.Drop();
                        break;
                }

                InventoryContainer.Remove(item);
            }

            if (InventoryContainer.GetRandomItem(out InventorySystems.InventoryItem inventoryItem) == true)
            {
                SetActiveInventoryitem(inventoryItem.SlotIndex);
            }
        }

        private void SetTypeInventoryItem(GameObject g, InventoryItem inventoryItem) 
        {
            if (g == null)
            {
                var attackAbility = CharacterBase.GetAbility<Attack>() as Attack;
                attackAbility.SetEmpty();



                return;
            }

            switch(inventoryItem.TypeItem)
            {
                case InventorySystems.TypeItem.Weapon:
                    var attackAbility = CharacterBase.GetAbility<Attack>() as Attack;
                    var weaponBase = g.GetComponent<WeaponsTest.WeaponBase>();
                    var weaponItem = ((object)inventoryItem.ItemScriptableObject as WeaponItemScriptableObject);
                    weaponBase.ViewModel = weaponItem.ViewModel;
                    attackAbility.SetWeapon(weaponBase);
                    
                    g.layer = CharacterBase.CharacterGameObject.layer;
                    g.GetComponent<Collider>().isTrigger = true;
                    if(g.TryGetComponent(out MeshCollider meshCollider)) meshCollider.isTrigger = true;
                    for (int i = 0; i < g.transform.childCount; i++)
                    {
                        g.transform.GetChild(i).gameObject.layer = CharacterBase.CharacterGameObject.layer;
                        if ( g.transform.GetChild(i).TryGetComponent(out Collider collider))
                            collider.isTrigger = true; 
                        if (g.transform.GetChild(i).TryGetComponent(out MeshCollider c))
                        {
                            c.isTrigger = true;
                        }
                    }


                    var player = Game.PlayerInstance;
                    if (player != null)
                    {
                        if (g.TryGetComponent(out Renderer renderer))
                        {
                            if (player.FirstPersonCamera == true)
                            {
                                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                            }
                            else
                            {
                                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                            }
                        }

                        var renderers = g.GetComponentsInChildren<Renderer>();
                        foreach (var r in renderers)
                        {

                            if (player.FirstPersonCamera == true)
                            {
                                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                            }
                            else
                            {
                                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                            }
                        }
                    }

                    break;
            }

        }

        #endregion
    }
}