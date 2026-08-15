using ArTiX.Inventory;
using ArTiX.Utils;
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
                    if (lastConveyor != null) lastConveyor.transform.localScale = Vector3.one;
                    lastConveyor = value;
                    if (lastConveyor != null)
                        lastConveyor.transform.localScale = Vector3.one * 1.2f;
                }
            }
            private Conveyor firstConveyor;
            public Conveyor FirstConveyor
            {
                get => firstConveyor;
                private set
                {
                    if (firstConveyor != null) firstConveyor.transform.localScale = Vector3.one;
                    firstConveyor = value;
                    if (firstConveyor != null)
                        firstConveyor.transform.localScale = Vector3.one * .8f;
                }
            }

            private const int TICKS_PER_UPDATE = 5;
            private int tickCounter;

            public int NbConveyor { get; private set; } = 0;

            public ConveyorBelt(Conveyor conveyor)
            {
                Debug.Log("create new belt");
                TickSystem.Instance.OnTick += IncrementTickCounter;
                firstConveyor = lastConveyor = conveyor;
                NbConveyor++;
            }

            public void AddConveyor(Conveyor newConveyor)
            {
                if (lastConveyor.IsAdjacent(newConveyor.Origin))
                {
                    Debug.Log("add at end of belt");

                    newConveyor.previousConveyor = LastConveyor;
                    LastConveyor.nextConveyor = newConveyor;

                    LastConveyor.UpdateRotation();
                    newConveyor.UpdateRotation();

                    LastConveyor = newConveyor;
                    newConveyor.belt = this;
                }
                if (firstConveyor.IsAdjacent(newConveyor.Origin) && NbConveyor > 1)
                {
                    Debug.Log("add at start of belt");

                    newConveyor.nextConveyor = FirstConveyor;
                    FirstConveyor.previousConveyor = newConveyor;

                    FirstConveyor.UpdateRotation();
                    newConveyor.UpdateRotation();
                    newConveyor.belt = this;

                    if (!newConveyor.IsLastConveyor) FirstConveyor = newConveyor;
                }
                
                if (newConveyor.belt == this) NbConveyor++;
            }

            public void MergeBelt(in ConveyorBelt mergedBelt)
            {
                Debug.Log(message: "merge belt");

                void MergeFromLast(in ConveyorBelt mergedBelt)
                {
                    mergedBelt.FirstConveyor.previousConveyor = LastConveyor;
                    LastConveyor = mergedBelt.LastConveyor;
                    Conveyor conv = LastConveyor;
                    while (conv != null)
                    {
                        conv.belt = this;
                        conv = conv.previousConveyor;
                    }
                }
                void MergeFromFirst(in ConveyorBelt mergedBelt)
                {
                    mergedBelt.LastConveyor.nextConveyor = FirstConveyor;
                    FirstConveyor = mergedBelt.FirstConveyor;
                    Conveyor conv = FirstConveyor;
                    while (conv != null)
                    {
                        conv.belt = this;
                        conv = conv.nextConveyor;
                    }
                }

                if (LastConveyor.IsAdjacent(mergedBelt.FirstConveyor.Origin)) MergeFromLast(mergedBelt);
                else if (LastConveyor.IsAdjacent(mergedBelt.LastConveyor.Origin))
                {
                    mergedBelt.Reverse(); 
                    MergeFromLast(mergedBelt);
                }
                else if (FirstConveyor.IsAdjacent(mergedBelt.LastConveyor.Origin)) MergeFromFirst(mergedBelt);
                else if (FirstConveyor.IsAdjacent(mergedBelt.FirstConveyor.Origin))
                {
                    mergedBelt.Reverse();
                    MergeFromFirst(mergedBelt);
                }

                mergedBelt.Clear();
            }

            public void RemoveConveyor(in Conveyor conveyor)
            {
                if (conveyor == null) return;
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

            private void Clear()
            {
                LastConveyor = FirstConveyor = null;
                TickSystem.Instance.OnTick -= IncrementTickCounter;
            }
        }

        private ConveyorBelt belt;
        private Conveyor previousConveyor;
        private Conveyor nextConveyor;
        private ItemSO item;

        private bool IsLastConveyor => belt.LastConveyor == this;
        private bool IsFirstConveyor => belt.FirstConveyor == this;

        // The base direction of the conveyor is right, so right is considered to be front.
        private Vector2Int FrontCell => Origin + Vector2Int.right.Rotate(transform.eulerAngles.y);
        private Vector2Int BackCell => Origin + Vector2Int.left.Rotate(transform.eulerAngles.y);

        public override void Initialize(Vector2Int origin)
        {
            base.Initialize(origin);

            GridBuildingSystem grid = GridBuildingSystem.Instance;
            List<ConveyorBelt> adjacentBeltList = new List<ConveyorBelt>();
            Conveyor adjacentConveyor;

            foreach (Vector2Int adjacentCell in AdjacentCellArray)
            {
                if (grid.GetObjectOnCell(adjacentCell) is Conveyor)
                {
                    adjacentConveyor = grid.GetObjectOnCell(adjacentCell) as Conveyor;
                    if (!adjacentBeltList.Contains(adjacentConveyor.belt))
                        adjacentBeltList.Add(adjacentConveyor.belt);
                }
            }

            switch (adjacentBeltList.Count)
            {
                case 0:
                    belt = new ConveyorBelt(this);
                    break;
                case 1:
                    adjacentBeltList[0].AddConveyor(this);
                    belt ??= new ConveyorBelt(this);
                    break;
                case 2:
                    // 1. Decide which belt the other will be merged into based on rotation
                    adjacentConveyor = null;

                    if (grid.GetObjectOnCell(BackCell) is Conveyor)
                        adjacentConveyor = grid.GetObjectOnCell(BackCell) as Conveyor;
                    else if (grid.GetObjectOnCell(FrontCell) is Conveyor)
                        adjacentConveyor = grid.GetObjectOnCell(FrontCell) as Conveyor;

                    if (adjacentConveyor == null)
                    {
                        foreach (ConveyorBelt conveyorBelt in adjacentBeltList)
                        {
                            if (conveyorBelt.LastConveyor.IsAdjacent(Origin))
                            {
                                adjacentConveyor = conveyorBelt.LastConveyor;
                                break;
                            }
                            else if (conveyorBelt.FirstConveyor.IsAdjacent(Origin))
                            {
                                adjacentConveyor = conveyorBelt.FirstConveyor;
                                break;
                            }
                        }

                        if (adjacentConveyor == null)
                        {
                            belt = new ConveyorBelt(this);
                            return;
                        }
                    }

                    adjacentConveyor.belt.AddConveyor(this);
                    adjacentBeltList.Remove(adjacentConveyor.belt);

                    // Call the merge func
                    belt.MergeBelt(adjacentBeltList[0]);
                    UpdateRotation();
                    break;
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