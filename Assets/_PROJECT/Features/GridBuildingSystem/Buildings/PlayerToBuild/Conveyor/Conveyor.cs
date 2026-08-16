using ArTiX.Effects.Tween;
using ArTiX.Inventory;
using ArTiX.Utils;
using ArTiX.Utils.TickSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public class Conveyor : PlacedObject, IItemHolder
    {
        private class ConveyorBelt
        {
            public Conveyor LastConveyor
            {
                get;
                private set;
            }
            public Conveyor FirstConveyor
            {
                get;
                private set;
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
                    newConveyor.previousConveyor = LastConveyor;
                    LastConveyor.nextConveyor = newConveyor;

                    LastConveyor.UpdateRotation();
                    newConveyor.UpdateRotation();

                    LastConveyor = newConveyor;
                    newConveyor.belt = this;
                }
                if (FirstConveyor.IsAdjacent(newConveyor.Origin) && NbConveyor > 1)
                {
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
                    LastConveyor.nextConveyor = mergedBelt.FirstConveyor;
                    LastConveyor.UpdateRotation();
                    LastConveyor = mergedBelt.LastConveyor;
                    Conveyor conv = LastConveyor;
                    while (conv.belt != this)
                    {
                        conv.belt = this;
                        conv = conv.previousConveyor;
                    }
                }
                void MergeFromFirst(in ConveyorBelt mergedBelt)
                {
                    mergedBelt.LastConveyor.nextConveyor = FirstConveyor;
                    FirstConveyor.previousConveyor = mergedBelt.LastConveyor;
                    FirstConveyor.UpdateRotation();
                    FirstConveyor = mergedBelt.FirstConveyor;
                    Conveyor conv = FirstConveyor;
                    while (conv.belt != this)
                    {
                        conv.belt = this;
                        conv = conv.nextConveyor;
                    }
                }

                if (LastConveyor.IsAdjacent(mergedBelt.FirstConveyor.Origin)) 
                    MergeFromLast(mergedBelt);
                else if (LastConveyor.IsAdjacent(mergedBelt.LastConveyor.Origin))
                {
                    mergedBelt.Reverse(); 
                    MergeFromLast(mergedBelt);
                }
                else if (FirstConveyor.IsAdjacent(mergedBelt.LastConveyor.Origin)) 
                    MergeFromFirst(mergedBelt);
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

        #region Variables & Properties

        [SerializeField] private Transform itemPos;

        private ConveyorBelt belt;
        private Conveyor previousConveyor;
        private Conveyor nextConveyor;

        private bool IsLastConveyor => belt.LastConveyor == this;
        private bool IsFirstConveyor => belt.FirstConveyor == this;

        // The base direction of the conveyor is right, so right is considered to be front.
        private Vector2Int FrontCell => Origin + Vector2Int.right.Rotate(transform.eulerAngles.y);
        private Vector2Int BackCell => Origin + Vector2Int.left.Rotate(transform.eulerAngles.y);

        private Transform worldItem;
        private MasterTween.Tween tween;
        private Transform WorldItem
        {
            set
            {
                worldItem = value;
                tween?.Kill();
                tween = MasterTween.Create();
                worldItem.transform.TweenPosition(itemPos.position,
                    animParams: new MasterTween.AnimParams(
                        duration: 0.5f,
                        transition: MasterTween.ETransition.SmoothStop5),
                    tween: tween);
            }
        }
        private ItemSO item;
        public ItemSO Item => item;

        #endregion

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
                case 3:
                case 4:
                    // 1. Decide which belt the other will be merged into based on rotation
                    adjacentConveyor = null;

                    if (grid.GetObjectOnCell(FrontCell) is Conveyor)
                        adjacentConveyor = grid.GetObjectOnCell(FrontCell) as Conveyor;
                    else if (grid.GetObjectOnCell(BackCell) is Conveyor)
                        adjacentConveyor = grid.GetObjectOnCell(BackCell) as Conveyor;
                    else
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
                    }

                    adjacentConveyor.belt.AddConveyor(this);
                    adjacentBeltList.Remove(adjacentConveyor.belt);

                    // Call the merge func
                    adjacentConveyor = null;
                    if (IsLastConveyor && grid.GetObjectOnCell(FrontCell) is Conveyor)
                        adjacentConveyor = grid.GetObjectOnCell(FrontCell) as Conveyor;
                    else if (IsFirstConveyor && grid.GetObjectOnCell(BackCell) is Conveyor)
                        adjacentConveyor = grid.GetObjectOnCell(BackCell) as Conveyor;
                    else
                    {
                        foreach (ConveyorBelt conveyorBelt in adjacentBeltList)
                        {
                            if (conveyorBelt.LastConveyor.FrontCell == Origin)
                            {
                                adjacentConveyor = conveyorBelt.LastConveyor;
                                break;
                            }
                            else if (conveyorBelt.FirstConveyor.BackCell == Origin)
                            {
                                adjacentConveyor = conveyorBelt.FirstConveyor;
                                break;
                            }
                        }
                    }

                    belt.MergeBelt(adjacentConveyor != null ? adjacentConveyor.belt : adjacentBeltList[0]);
                    UpdateRotation();
                    break;
            }

            if (belt == null)
                Debug.LogError("I have no belt");
        }

        #region Conveyor

        private void MoveItem()
        {
            if (item == null || nextConveyor.item != null) return;

            nextConveyor.WorldItem = worldItem;
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

        #endregion

        #region Item Holder

        public ItemSO GiveItem()
        {
            ItemSO item = this.item;
            this.item = null;
            Destroy(worldItem.gameObject);
            tween?.Kill();
            return item;
        }
        public bool CanGiveItem() => item != null;

        public void TakeItem(ItemSO item) 
        {
            this.item = item;
            worldItem = Instantiate(item.Prefab, itemPos.position, Quaternion.identity);
        }
        public bool CanTakeItem(ItemSO item) => this.item == null;

        #endregion
    }
}