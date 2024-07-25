﻿using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using Heart_Module.Data.Scripts.HeartModule.Weapons.Setup.Adding;
using Heart_Module.Data.Scripts.HeartModule.ExceptionHandler;
using VRage.ModAPI;

namespace Heart_Module.Data.Scripts.HeartModule.Weapons.AiTargeting
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    internal class WeaponManagerAi : MySessionComponentBase
    {
        public static WeaponManagerAi I;

        private Dictionary<IMyCubeGrid, GridAiTargeting> GridTargetingMap = new Dictionary<IMyCubeGrid, GridAiTargeting>();
        private Dictionary<IMyCubeGrid, List<SorterWeaponLogic>> GridWeapons => WeaponManager.I.GridWeapons;

        public GridAiTargeting GetTargeting(IMyCubeGrid grid)
        {
            if (GridTargetingMap.ContainsKey(grid))
                return GridTargetingMap[grid];
            return null;
        }

        public override void LoadData()
        {
            // Ensure this runs only on the server to avoid unnecessary calculations on clients
            if (!MyAPIGateway.Session.IsServer)
            {
                SetUpdateOrder(MyUpdateOrder.NoUpdate);
                return;
            }

            // Subscribe to grid addition and removal events
            HeartData.I.OnGridAdd += InitializeGridAI;
            HeartData.I.OnGridRemove += CloseGridAI;
            I = this;

            // Initialize AI for all existing grids
            HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
            MyAPIGateway.Entities.GetEntities(entities, e => e is IMyCubeGrid);

            foreach (var entity in entities)
            {
                InitializeGridAI(entity as IMyCubeGrid);
            }
        }

        protected override void UnloadData()
        {
            HeartData.I.OnGridAdd -= InitializeGridAI;
            HeartData.I.OnGridRemove -= CloseGridAI;
            I = null;
        }

        public override void UpdateAfterSimulation()
        {
            // AI update logic here, potentially throttled for performance
            UpdateAITargeting();
        }

        public GridAiTargeting GetOrCreateGridAiTargeting(IMyCubeGrid grid)
        {
            if (!GridTargetingMap.ContainsKey(grid))
            {
                var gridAiTargeting = new GridAiTargeting(grid);
                GridTargetingMap.Add(grid, gridAiTargeting);
                HeartLog.Log($"Grid AI initialized for grid '{grid.DisplayName}' [{(gridAiTargeting.Enabled ? "ENABLED" : "DISABLED")}]");
            }

            return GridTargetingMap[grid];
        }

        private void InitializeGridAI(IMyCubeGrid grid)
        {
            if (grid.Physics == null) return;

            HeartLog.Log($"Attempting to initialize Grid AI for grid '{grid.DisplayName}'");
            GetOrCreateGridAiTargeting(grid);
        }

        private void CloseGridAI(IMyCubeGrid grid)
        {
            if (grid.Physics == null) return;

            if (GridTargetingMap.ContainsKey(grid))
            {
                GridTargetingMap[grid].Close();
                GridTargetingMap.Remove(grid);
                HeartLog.Log($"Grid AI closed for grid '{grid.DisplayName}'");
            }
            else
            {
                HeartLog.Log($"Attempted to close Grid AI on a non-tracked grid: '{grid.DisplayName}'");
            }
        }

        private void UpdateAITargeting()
        {
            foreach (var targetingKvp in GridTargetingMap)
            {
                targetingKvp.Value.UpdateTargeting(); // Method to be implemented in GridAiTargeting class
            }
        }
    }
}
