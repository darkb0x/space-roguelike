﻿using UnityEngine;

namespace Game.UI.HUD
{
    public class HUDService : IService, IEntryComponent<UIWindowService>
    {
        private const string ROOT_HUD_CANVAS_PATH = "Prefabs/UI/HUD";

        public Canvas RootCanvas => _rootCanvas;
        public HUDContainer HUDContainer => _hudContainer;

        private HUDConfig _config;
        private Canvas _rootCanvas;
        private HUDContainer _hudContainer;

        public HUDService(HUDConfig config)
        {
            _config = config;
        }

        public void Initialize(UIWindowService windowService)
        {
            InitRootCanvas();
            InitHudContainer();

            windowService.SetHud(_rootCanvas);
        }

        private void InitRootCanvas()
        {
            _rootCanvas = Object.Instantiate(Resources.Load<GameObject>(ROOT_HUD_CANVAS_PATH)).GetComponent<Canvas>();
        }
        private void InitHudContainer()
        {
            _hudContainer = _rootCanvas.GetComponent<HUDContainer>();
            _hudContainer.Initialize(_config);
        }

        public THud GetHudElement<THud>(HUDElementID id) where THud : HUDElement
        {
            foreach (var hudElemenet in _hudContainer.GetHudElements())
            {
                if (hudElemenet.ID == id)
                    return hudElemenet as THud;
            }

            return null;
        }
    }
}
