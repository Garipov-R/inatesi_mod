using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.LeoEcs3;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.LeoEcs4.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.Stuff
{
    public class PropPanelControl : MonoBehaviour
    {
        private const string c_PropsNameElement = "props";
        private const string c_EntityNameElement = "entity";
        private const string c_SettingsNameElement = "settings";
        private const string c_NPCNameElement = "npc";

        [SerializeField] private UIDocument _UIDocument;
        [SerializeField] private VisualTreeAsset _PropElementAsset;
        [SerializeField] private List<GameObject> _Props = new List<GameObject>();
        [SerializeField] private List<GameObject> _Characters = new List<GameObject>();
        [SerializeField] private List<CharacterSO> _CharacterSOList = new List<CharacterSO>();

        public UIDocument UIDocument { get => _UIDocument; set => _UIDocument = value; }
        public VisualTreeAsset PropElementAsset { get => _PropElementAsset; set => _PropElementAsset = value; }


        private Button _EntityButton;
        private Button _PropsButton;
        private Button _SettingsButton;
        private Button _NPCButton;
        private string _ActivePanel;
        private SetupLeoEcs _setupLeoEcs;


        [Zenject.Inject]
        private void Construct(SetupLeoEcs setupLeoEcs)
        {
            _setupLeoEcs = setupLeoEcs;
        }

        private void Start()
        {
            _UIDocument.rootVisualElement.Q().visible = false;
            var menu_buttons = _UIDocument.rootVisualElement.Q<VisualElement>("menu-buttons");

            foreach (var button in menu_buttons.Children())
            {
                if (button.Q<VisualElement>().viewDataKey == c_PropsNameElement)
                {
                    _PropsButton = button.Q<Button>();
                    _PropsButton.clicked += () =>
                    {
                        _ActivePanel = c_PropsNameElement;
                        UpdateContainer();
                        UpdateButtonsContainer(c_PropsNameElement);
                    };
                }
                else if (button.Q<VisualElement>().viewDataKey == c_EntityNameElement)
                {
                    _EntityButton = button.Q<Button>();
                    _EntityButton.clicked += () =>
                    {
                        _ActivePanel = c_EntityNameElement;
                        UpdateContainer();
                        UpdateButtonsContainer(c_EntityNameElement);
                    };
                }
                else if (button.Q<VisualElement>().viewDataKey == c_SettingsNameElement)
                {
                    _SettingsButton = button.Q<Button>();
                    _SettingsButton.clicked += () =>
                    {
                        _ActivePanel = c_SettingsNameElement;
                        UpdateContainer();
                        UpdateButtonsContainer(c_SettingsNameElement);
                    };
                }
                else if (button.Q<VisualElement>().viewDataKey == c_NPCNameElement)
                {
                    _NPCButton = button.Q<Button>();
                    _NPCButton.clicked += () =>
                    {
                        _ActivePanel = c_NPCNameElement;
                        UpdateContainer();
                        UpdateButtonsContainer(c_NPCNameElement);
                    };
                }

            }

            var botToggle = _UIDocument.rootVisualElement.Q<Toggle>("bots-toggle");
            botToggle.value = true;
            botToggle.RegisterValueChangedCallback(BotToggleHandleCallback);
        

            _ActivePanel = c_PropsNameElement;
            UpdateButtonsContainer(c_PropsNameElement);
            UpdateContainer();
        }

        private void BotToggleHandleCallback(ChangeEvent<bool> evt)
        {
            Debug.Log("HandleCallback invoked with value " + evt.newValue);

            foreach(var entity in _setupLeoEcs.World.Filter<BotComponent>().End())
            {
                ref var botComponent = ref _setupLeoEcs.World.GetPool<BotComponent>().Get(entity);
                botComponent.enable = evt.newValue;
            }
        }

        private void UpdateButtonsContainer(string activeButton)
        {
            var menu_buttons = _UIDocument.rootVisualElement.Q<VisualElement>("menu-buttons");

            foreach (var button in menu_buttons.Children())
            {
                button.RemoveFromClassList("active");

                if (button.viewDataKey == activeButton)
                {
                    button.AddToClassList("active");
                }
            }
        }

        private void UpdateContainer()
        {
            _UIDocument.rootVisualElement.Q<VisualElement>("prop_elements_container").Clear();

            var setingsPanel = _UIDocument.rootVisualElement.Q<VisualElement>("settings-content");
            var propElemPanel = _UIDocument.rootVisualElement.Q<VisualElement>("prop_elements_container");

            setingsPanel.style.display = DisplayStyle.None;
            propElemPanel.style.display = DisplayStyle.None;

            if (_ActivePanel == c_PropsNameElement)
            {
                foreach (var prop in _Props)
                {
                    var item = _PropElementAsset.Instantiate();

                    item.Q<Button>().userData = prop;
                    item.Q<Button>().text = prop.name;
                    item.Q<Button>().RegisterCallback<ClickEvent>(OnClickSpawnProp);

                    _UIDocument.rootVisualElement.Q<VisualElement>("prop_elements_container").Add(item);
                }

                propElemPanel.style.display = DisplayStyle.Flex;
            }
            else if (_ActivePanel == c_EntityNameElement)
            {
                foreach (var entity in _Characters)
                {
                    var item = _PropElementAsset.Instantiate();

                    item.Q<Button>().userData = entity;
                    item.Q<Button>().text = entity.name;
                    item.Q<Button>().RegisterCallback<ClickEvent>(OnClickSpawnEntity);

                    _UIDocument.rootVisualElement.Q<VisualElement>("prop_elements_container").Add(item);
                }

                propElemPanel.style.display = DisplayStyle.Flex;
            }
            else if (_ActivePanel == c_NPCNameElement)
            {
                foreach (var entity in _CharacterSOList)
                {
                    var item = _PropElementAsset.Instantiate();

                    item.Q<Button>().userData = entity;
                    item.Q<Button>().text = entity.name;
                    item.Q<Button>().RegisterCallback<ClickEvent>(OnClickSpawnNPC);

                    _UIDocument.rootVisualElement.Q<VisualElement>("prop_elements_container").Add(item);
                }

                propElemPanel.style.display = DisplayStyle.Flex;
            }
            else if (_ActivePanel == c_SettingsNameElement)
            {
                setingsPanel.style.display = DisplayStyle.Flex;


                int botsAmount = 0;
                foreach (var entity in _setupLeoEcs.World.Filter<BotComponent>().End())
                {
                    botsAmount += 1;
                }

                _UIDocument.rootVisualElement.Q<Label>("bots-amount").text = "BOTS: " + botsAmount.ToString();
            }
        }



        private void OnClickSpawnProp(ClickEvent evt)
        {
            var button = evt.currentTarget as Button;
            Vector3 spawnPosition = GetSpawnPoint();

            var prefab = (button.userData as GameObject);
            spawnPosition.y += prefab.transform.localScale.y;


            ref var spawnComponent = ref SpawnComponent();
            spawnComponent.gameObject = button.userData as GameObject;
            spawnComponent.position = spawnPosition;
            spawnComponent.rotation = Quaternion.LookRotation((GetPlayerPosition() - spawnComponent.position).normalized, Vector3.up);
        }

        private void OnClickSpawnEntity(ClickEvent evt)
        {
            var button = evt.currentTarget as Button;

            ref var spawnComponent = ref SpawnComponent();
            spawnComponent.gameObject = button.userData as GameObject;
            spawnComponent.position = GetSpawnPoint();
            spawnComponent.rotation = Quaternion.LookRotation((GetPlayerPosition() - spawnComponent.position).normalized, Vector3.up);
        }

        private void OnClickSpawnNPC(ClickEvent evt)
        {
            var button = evt.currentTarget as Button;
;
            ref var spawnComponent = ref SpawnComponent();
            spawnComponent.gameObject = (button.userData as CharacterSO).Prefab;
            spawnComponent.position = GetSpawnPoint();
            spawnComponent.rotation = Quaternion.LookRotation((GetPlayerPosition() - spawnComponent.position).normalized, Vector3.up);
            //spawnComponent.rotation = Quaternion.identity;
            spawnComponent.data = button.userData;
        }

        private ref SpawnComponentEvent SpawnComponent()
        {
            var spawnEntity = _setupLeoEcs.World.NewEntity();
            var spawnPool = _setupLeoEcs.World.GetPool<SpawnComponentEvent>();
            spawnPool.Add(spawnEntity);

            return ref spawnPool.Get(spawnEntity);
        }

        private Vector3 GetPlayerPosition()
        {
            var playerFilter = _setupLeoEcs.World.Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            var playerPool = _setupLeoEcs.World.GetPool<CharacterComponent>();
            var charactersPool = _setupLeoEcs.World.GetPool<CharacterComponent>();
            Vector3 result = Vector3.zero;


            foreach (int entity in playerFilter)
            {
                ref var characterComponent = ref charactersPool.Get(entity);
                result = characterComponent.GameObject.transform.position;
            }

            return result;
        }

        private Vector3 GetPlayerRotation()
        {
            var playerFilter = _setupLeoEcs.World.Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            var playerPool = _setupLeoEcs.World.GetPool<CharacterComponent>();
            var charactersPool = _setupLeoEcs.World.GetPool<CharacterComponent>();
            Vector3 result = Vector3.zero;


            foreach (int entity in playerFilter)
            {
                ref var characterComponent = ref charactersPool.Get(entity);
                result = characterComponent.GameObject.transform.position;
            }

            return result;
        }

        private Vector3 GetSpawnPoint()
        {
            var playerFilter = _setupLeoEcs.World.Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            var playerPool = _setupLeoEcs.World.GetPool<CharacterComponent>();
            var charactersPool = _setupLeoEcs.World.GetPool<CharacterComponent>();
            Vector3 spawnPosition = Vector3.zero;


            foreach (int entity in playerFilter)
            {
                if (playerPool.Has(entity))
                {
                    ref var characterComponent = ref charactersPool.Get(entity);

                    var direction = characterComponent.CharacterMotionBase.LookSource.LookDirection();

                    var cameraPosition = characterComponent.CharacterMotionBase.LookSource.LookPosition();

                    var isHit = Physics.Raycast(
                        cameraPosition,
                        direction,
                        out RaycastHit hitInfo,
                        1000f,
                        characterComponent.CharacterMotionBase.RaycastLayer,
                        QueryTriggerInteraction.Ignore
                    );

                    if (isHit)
                    {
                        spawnPosition = hitInfo.point;
                    }
                }
            }

            return spawnPosition;
        }
    }
}