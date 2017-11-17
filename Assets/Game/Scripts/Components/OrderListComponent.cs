using Entitas;
using UnityEngine;
using System.Collections.Generic;

namespace MergeWar.Game.Components
{
    [Game]
    public class OrderListComponent : IComponent
    {
        public List<Order> orderList;

        public void AddOrder(string id, int amount = 1)
        {
            foreach(var ingredient in orderList)
                if (ingredient.id.Equals(id))
                {
                    ingredient.amount += amount;
                    ingredient.amount = Mathf.Clamp(ingredient.amount, 0, ingredient.target);
                    break;
                }

            orderList.Add(new Order(id, amount));
        }

        public void RemoveOrder(string id)
        {
            orderList.RemoveAll(i => i.id.Equals(id));
        }

        public bool HasOrder(string id)
        {
            return orderList.Find(o => o.id.Equals(id)) != null;
        }

        public bool HasOrderAmount(string id, int amount)
        {
            if (HasOrder(id))
            {
                var ingredient = orderList.Find(i => i.id.Equals(id));
                return ingredient.amount >= amount;
            }
            return false;
        }

        public int OrderAmount(string id)
        {
            return HasOrder(id) ? orderList.Find(i => i.id.Equals(id)).amount : 0;
        }

        public bool Filled()
        {
            return orderList.Find(i => i.amount < i.target) == null;
        }

        [System.Serializable]
        public class Order
        {
            public string id;
            public int amount;
            public int target;

            public Order() { }

            public Order(string id)
            {
                this.id = id;
                this.amount = 0;
            }

            public Order(string id, int amount)
            {
                this.id = id;
                this.amount = amount;
            }

            public Order(string id, int amount, int target)
            {
                this.id = id;
                this.amount = amount;
                this.target = target;
            }
        }
    }
}