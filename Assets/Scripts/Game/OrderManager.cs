using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public List<DrinkData> availableDrinks = new List<DrinkData>();
    public float orderDecisionTime = 3f;

    private Dictionary<Customer, float> customerTimers = new Dictionary<Customer, float>();

    private void Awake()
    {
        EnsureDefaultDrinkCatalog();
    }

    private void Update()
    {
        if (availableDrinks.Count == 0)
        {
            EnsureDefaultDrinkCatalog();
        }

        List<Customer> keys = new List<Customer>(customerTimers.Keys);
        foreach (var customer in keys)
        {
            if (customer == null || customer.desiredDrink != null)
            {
                customerTimers.Remove(customer);
                continue;
            }

            if (customer.currentState == Customer.CustomerState.Waiting)
            {
                customerTimers[customer] += Time.deltaTime;
                if (customerTimers[customer] >= orderDecisionTime)
                {
                    DrinkData randomDrink = availableDrinks[UnityEngine.Random.Range(0, availableDrinks.Count)];
                    customer.PlaceOrder(randomDrink);
                    if (GameManager.Instance != null)
                    {
                        var order = new Order(customer, randomDrink);
                        GameManager.Instance.activeOrders.Add(order);

                        if (UIManager.Instance != null)
                        {
                            UIManager.Instance.ShowOrderNotification(order);
                        }
                    }
                    customerTimers.Remove(customer);
                }
            }
        }
    }

    public void RegisterCustomer(Customer customer)
    {
        if (customer == null)
            return;

        if (!customerTimers.ContainsKey(customer))
        {
            customerTimers.Add(customer, 0f);
        }
    }

    private void EnsureDefaultDrinkCatalog()
    {
        if (availableDrinks.Count > 0)
            return;

        var catalog = new[]
        {
            CreateDrink("latte", "Café Latte", "Warm and smooth.", 7f),
            CreateDrink("mocha", "Mocha", "Chocolate cream blend.", 8f),
            CreateDrink("soda", "Sparkling Soda", "Crisp and refreshing.", 6f),
            CreateDrink("whiskey", "Whiskey Sour", "Classic and strong.", 12f),
            CreateDrink("beer", "House Beer", "Easy and cold.", 9f),
        };

        availableDrinks.AddRange(catalog);
    }

    private DrinkData CreateDrink(string id, string displayName, string description, float price)
    {
        var drink = ScriptableObject.CreateInstance<DrinkData>();
        drink.drinkId = id;
        drink.displayName = displayName;
        drink.description = description;
        drink.price = price;
        return drink;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
