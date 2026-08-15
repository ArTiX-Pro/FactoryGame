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
                    if (lastConveyor != null) lastConveyor.transform.localScale = Vector3.one;
                    lastConveyor = value;
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
                FirstConveyor = LastConveyor = conveyor;
                NbConveyor++;
            }

            public void AddConveyor(Conveyor newConveyor)
            {
                if (LastConveyor.IsAdjacent(newConveyor.Origin))
                {
                    Debug.Log("add at end of belt");

                    newConveyor.previousConveyor = LastConveyor;
                    LastConveyor.nextConveyor = newConveyor;

                    LastConveyor.UpdateRotation();
                    newConveyor.UpdateRotation();

                    LastConveyor = newConveyor;
                    newConveyor.belt = this;
                }
                if (FirstConveyor.IsAdjacent(newConveyor.Origin))
                {
                    Debug.Log("add at start of belt");

                    newConveyor.nextConveyor = FirstConveyor;
                    FirstConveyor.previousConveyor = newConveyor;

                    FirstConveyor.UpdateRotation();
                    newConveyor.UpdateRotation();
                    newConveyor.belt ??= this;

                    if (!newConveyor.IsLastConveyor) FirstConveyor = newConveyor;
                }
                else return;

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

                mergingConveyor.UpdateRotation();
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