﻿namespace redd096
{
    using UnityEngine;

    [AddComponentMenu("redd096/Singletons/Game Manager")]
    [DefaultExecutionOrder(-100)]
    public class GameManager : Singleton<GameManager>
    {
        public LevelManager levelManager { get; private set; }
        public UIManager uiManager { get; private set; }

        protected override void SetDefaults()
        {
            //get references
            levelManager = FindObjectOfType<LevelManager>();
            uiManager = FindObjectOfType<UIManager>();
        }
    }
}