// Services/CartService.cs
using FoodShopBlazor.Models;
using System.Collections.ObjectModel;

namespace FoodShopBlazor.Services
{
    public class CartService
    {
        private List<CartItem> _cartItems = new();

        public IReadOnlyCollection<CartItem> CartItems => new ReadOnlyCollection<CartItem>(_cartItems);

        public event Action? OnCartChanged;

        public void AddToCart(Food food, int quantity)
        {
            var existingItem = _cartItems.FirstOrDefault(i => i.Food.Id == food.Id);

            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
            }
            else
            {
                _cartItems.Add(new CartItem { Food = food, Quantity = quantity });
            }

            NotifyCartChanged();
        }

        public void RemoveFromCart(int foodId)
        {
            _cartItems.RemoveAll(i => i.Food.Id == foodId);
            NotifyCartChanged();
        }

        public void UpdateQuantity(int foodId, int quantity)
        {
            var item = _cartItems.FirstOrDefault(i => i.Food.Id == foodId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    _cartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                NotifyCartChanged();
            }
        }

        public void ClearCart()
        {
            _cartItems.Clear();
            NotifyCartChanged();
        }

        public decimal GetTotal()
        {
            return _cartItems.Sum(item => item.Food.Price * item.Quantity);
        }

        private void NotifyCartChanged() => OnCartChanged?.Invoke();

        public class CartItem
        {
            public Food Food { get; set; } = null!;
            public int Quantity { get; set; }
        }
    }
}
