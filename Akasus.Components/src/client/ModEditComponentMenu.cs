// File: ModEditComponentMenu.cs
// Project: Akasus.Components
// Author: Akasus (akasus.nevelas@gmail.com)
// Description:

using LogicLog;
using LogicWorld.UI;
using UnityEngine;

namespace Akasus.Components.Client
{
    public abstract class ModEditComponentMenu : EditComponentMenu
    {
        private ILogicLogger logger = LogicLogger.For("ModEditComponentMenu");
        private bool uiCreated = false;
        protected virtual void CreateUIComponents(Transform contentRect)
        {

        }
        public override void Initialize()
        {
            base.Initialize();
            if (uiCreated) return;

            logger.Info("Initialize of ModEditComponentMenu");
            if (transform == null)
            {
                logger.Error("Transform is NULL!");
                return;
            }

            foreach (Transform t in transform)
            {
                logger.Info(t.name);
            }

            var trans = transform.Find("Menu").Find("Menu Content");
            if (trans == null)
            {
                logger.Error("Cant find Menu Content!");
                return;
            }
            logger.Info("Menu Content Transform: " + trans.name);
            CreateUIComponents(trans);
            uiCreated = true;

        }
    }
}