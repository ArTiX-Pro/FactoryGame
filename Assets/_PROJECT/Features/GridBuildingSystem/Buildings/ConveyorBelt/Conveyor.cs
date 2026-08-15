using ArTiX.Effects.Tween;
using ArTiX.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public class Conveyor : PlacedObject
    {
        private class ConveyorBelt
        {
            private Conveyor lastConveyor;
            public Conveyor LastConveyor
            {
                get => lastConveyor;
                private set
                {
                    lastConveyor = value;
                    lastConveyor.transform.TweenScale(Vector3.one * 1.2f,
                        new MasterTween.AnimParams(
                            duration: 1, 
                            transition: MasterTween.ETransition.Arch2)
                        );
                }
            }
            private Conveyor firstConveyor;
            public Conveyor FirstConveyor
            {
                get => firstConveyor;
                private set
                {
                    firstConveyor = value;
                    firstConveyor.transform.TweenScale(Vector3.one * .8f,
                        new MasterTween.AnimParams(
                            duration: 1,
                            transition: MasterTween.ETransition.Arch2)
                        );
                }
            }

            private const int TICKS_PER_UPDATE = 5;
            private int tickCounter;

            public int NbConveyor { get; private set; } = 0;

            public ConveyorBelt(Conveyor conveyor)
            {
                Debug.Log("create new belt");
                TickSystem.Instance.OnTick += IncrementTickCounter;
                FirstConveyor = LastConveyor = conveyor;
                NbConveyor++;
            }

            public void AddConveyor(Conveyor newConveyor, Conveyor adjacentConveyor)
            {
                if (adjacentConveyor.nextConveyor != null && adjacentConveyor.previousConveyor != null)
                    return;

                if (adjacentConveyor == LastConveyor) // Add after
                {
                    Debug.Log("add at end of belt");
                    LastConveyor = newConveyor;

                    adjacentConveyor.nextConveyor = newConveyor;
                    newConveyor.previousConveyor = adjacentConveyor;
                    adjacentConveyor.UpdateRotation();
                    newConveyor.UpdateRotation();
                }
                else // Add before
                {
                    Debug.Log("add at start of belt");
                    FirstConveyor = newConveyor;
                    newConveyor.nextConveyor = adjacentConveyor;
                    adjacentConveyor.previousConveyor = newConveyor;
                    adjacentConveyor.UpdateRotation();
                    newConveyor.UpdateRotation();
                }
                NbConveyor++;
            }

            public void MergeBelt(in Conveyor mergingConveyor, in Conveyor adjacentConveyor)
            {
                Debug.Log("merge belt");
                if (mergingConveyor == LastConveyor) // Merging the last part
                {
                    if (adjacentConveyor.IsLastConveyor)
                        adjacentConveyor.belt.Reverse();
                    
                    adjacentConveyor.previousConveyor = mergingConveyor;
                    mergingConveyor.nextConveyor = adjacentConveyor;
                    adjacentConveyor.UpdateRotation();

                    LastConveyor = adjacentConveyor.belt.LastConveyor;

                    Conveyor conv = LastConveyor;
                    while (conv.belt != this)
                    {
                        conv.belt = this;
                        conv = conv.previousConveyor;
                    }
                }
                else
                {
                    if (adjacentConveyor.IsFirstConveyor)
                        adjacentConveyor.belt.Reverse();

                    adjacentConveyor.nextConveyor = mergingConveyor;
                    mergingConveyor.previousConveyor = adjacentConveyor;
                    adjacentConveyor.UpdateRotation();

                    FirstConveyor = adjacentConveyor.belt.FirstConveyor;

                    Conveyor conv = FirstConveyor;
                    while (conv.belt != this)
                    {
                        conv.belt = this;
                        conv = conv.nextConveyor;
                    }
                }
            }

            public void RemoveConveyor(in Conveyor conveyor)
            {

            }

            private void Reverse()
            {
                FirstConveyor = LastConveyor;
                Conveyor conv = FirstConveyor;
                while (conv != null)
                {
                    conv.Reverse();
                    if (conv.nextConveyor == null) break;

                    conv = conv.nextConveyor;
                }
                LastConveyor = conv;
            }

            private void IncrementTickCounter(object sender, EventArgs e)
            {
                tickCounter++;

                if (tickCounter == TICKS_PER_UPDATE)
                {
                    tickCounter = 0;
                    MoveBelt();
                }
            }

            private void MoveBelt()
            {
                Conveyor currentConveyor = LastConveyor.previousConveyor;
                while (currentConveyor != null)
                {
                    currentConveyor.MoveItem();

                    if (currentConveyor.IsFirstConveyor) // We finished the movement. Forcing the stop to avoid infinite loop
                        return;

                    currentConveyor = currentConveyor.previousConveyor;
                }
            }
        }

        private ConveyorBelt belt;
        private Conveyor previousConveyor;
        private Conveyor nextConveyor;
        private ItemSO item;

        private bool IsLastConveyor => belt.LastConveyor == this;
        private bool IsFirstConveyor => belt.FirstConveyor == this;

        public override void Initialize(Vector2Int origin)
        {
            base.Initialize(origin);

            // Check for adjacent conveyors
            Vector2Int[] adjacentCellList =
            {
                Origin + Vector2Int.right,
                Origin + Vector2Int.up,
                Origin + Vector2Int.left,
                Origin + Vector2Int.down
            };

            GridBuildingSystem grid = GridBuildingSystem.Instance;
            List<Conveyor> adjacentConveyorList = new List<Conveyor>();
            Conveyor adjacentConveyor = null;
            ConveyorBelt biggestBelt = null;

            foreach (Vector2Int adjacentCell in adjacentCellList)
            {
                if (grid.GetObjectOnCell(adjacentCell) is Conveyor)
                    adjacentConveyor = grid.GetObjectOnCell(adjacentCell) as Conveyor;

                if (adjacentConveyor == null || adjacentConveyorList.Contains(adjacentConveyor)) continue;
                
                if (biggestBelt == null || (adjacentConveyor.belt.NbConveyor > biggestBelt.NbConveyor
                && (adjacentConveyor.IsLastConveyor || adjacentConveyor.IsFirstConveyor)))
                {
                    biggestBelt = adjacentConveyor.belt;
                }

                adjacentConveyorList.Add(adjacentConveyor);
            }

            if (adjacentConveyorList.Count == 4) // Conveyor not active if surrounded by 4 conveyors
            {
                Debug.Log(message: "Inactive conveyor");
                return;
            }

            if (adjacentConveyor == null) 
            {
                belt = new ConveyorBelt(this); // Alone
                return;
            }
            
            belt = adjacentConveyor.belt;
            belt.AddConveyor(this, adjacentConveyor);
            adjacentConveyorList.Remove(adjacentConveyor);

            if ((nextConveyor != null || previousConveyor != null) && adjacentConveyorList.Count == 1)
            {
                adjacentConveyor = adjacentConveyorList[0];
                if (adjacentConveyor.belt == belt) return;

                belt.MergeBelt(this, adjacentConveyor);
                
                if (IsFirstConveyor)
                    previousConveyor = adjacentConveyor;
                else
                    nextConveyor = adjacentConveyor;

                UpdateRotation();
            }
        }

        public void MoveItem()
        {
            if (item == null || nextConveyor.item != null) return;

            nextConveyor.item = item;
            item = null;
        }

        private void UpdateRotation()
        {
            if (nextConveyor != null)
            {
                transform.rotation =
                    Quaternion.Euler(
                        x: 0,
                        y: -Vector2.SignedAngle(Vector2.right, nextConveyor.Origin - Origin),
                        z: 0
                    );
            }
            else if (previousConveyor != null)
                transform.rotation = previousConveyor.transform.rotation;
        }

        private void Reverse()
        {
            Conveyor previousConv = previousConveyor;
            previousConveyor = nextConveyor;
            nextConveyor = previousConv;
            UpdateRotation();
        }
    }
}