using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

namespace PosAppV1
{
    class Pos
    {
        private Dictionary<int, Product> dictionaryProducts = new Dictionary<int, Product>();
        private Dictionary<int, Inventory> dictionaryInventory = new Dictionary<int, Inventory>();
        private string _inputErrorMessage;
        private string _inventoryTableHeader;
        private string _productTableHeader;
        private string _tableLine;

        private enum Action
        {
            IsAdmin,
            IsCustomer,
            start,
            Exit
        }

        public void StartApp()
        {
            InIt();
            AdminOrCustomer();

        }

        //admin or customer Action 
        private void AdminOrCustomer()
        {

            var action = (Action) TakeUserInput("Enter '0' For Admin '1' For Customer ", _inputErrorMessage);

            if (action.Equals(Action.IsAdmin))
                this.GoForAdmin();
            if (action.Equals(Action.IsCustomer))
                this.GoForCustomer();

        }

        private void GoForAdmin()
        {

            var option =
                (Action)
                TakeUserInput("Enter 0 for Add Product 1 for view  all product  2 for start ",
                    _inputErrorMessage);

            if (Action.IsAdmin.Equals(option))
            {

                AddProduct();
            }
            if (Action.IsCustomer.Equals(option))
            {

                this.GetAllProduct();
            }
            if (Action.start.Equals(option))
            {

                this.AdminOrCustomer();
            }
            else
            {

                this.GoForAdmin();

            }

        }

        private void GoForCustomer()
        {

            this.GetAllProduct();
            a:
            var option = (Action) TakeUserInput("Enter 1 for buy ", _inputErrorMessage);
            
            if (option.Equals(Action.IsCustomer))
            {

                var id = TakeUserInput("Enter Product ID \n ", _inputErrorMessage);

                if (IsProductIdExist(id))
                {
                    var product = this.GetProductById(id);
                    b:
                    var qty = TakeUserInput("Enter  Availbale Quantity", _inputErrorMessage);
                    //check this quantity is available
                    if (product.Quantity >= qty)
                    {
                        //go for transaction
                        this.Transaction(product, qty);
                    }
                    else
                    {
                        this.ErrorMassage("Quantity is not Available try again \n");

                        goto b;
                    }

                }

                else
                {
                    this.ErrorMassage("No Product Found For this id \n");
                    goto a;
                }

            }

            else
            {
                this.ErrorMassage(" !!Wrong input!!\n");
                goto a;
            }

        }


        private Product GetProductById(int id)
        {
            var product = dictionaryProducts[id];
            return product;

        }

        private Inventory GetInventoryById(int id)
        {
            var inventory = dictionaryInventory[id];
            return inventory;

        }

        private bool IsProductIdExist(int id)
        {
            if (dictionaryProducts.ContainsKey(id))
            {
                return true;
            }
            return false;

        }

        private bool IsInventoryIdExist(int id)
        {
            if (dictionaryInventory.ContainsKey(id))
            {
                return true;
            }
            return false;

        }


        private void Transaction(Product product, int quantity)
        {
            // int updateQuantity;
            var totalPrice = product.ProductPrice * quantity; //not use
            //set current quantity of the product
            var updateQuantity = product.Quantity - quantity;
            product.Quantity = updateQuantity;
            //update product/quantity
            this.UpdateProduct(product, quantity);
            this.AddOrUpdateEnventoryEntry(product.Id, quantity);
            var option = ConfirmForCheckoutOrBuy();
            if (option)
            {

                this.GoForCustomer();

            }
            else
            {
                this.GetAllInventory();

            }

        }

        private bool ConfirmForCheckoutOrBuy()
        {
            var hints = "Enter 0 for buy ,  1 for checkout";
            var input = TakeUserInput(hints, _inputErrorMessage);

            if (input.Equals(0))
            {
                return true;
            }
            if (input.Equals(1))
            {
                return false;
            }
            return ConfirmForCheckoutOrBuy();

        }

        private bool UpdateProduct(Product product, int quantity)
        {
            // if (dictionaryProducts.ContainsKey(id,product.Id))
            dictionaryProducts[product.Id] = product;
            return true;
        }

        private bool AddProduct()
        {

            var id = TakeUserInput("Enter Product ID= ", _inputErrorMessage);
            if (!IsProductIdExist(id))
            {
                var product = new Product();
                product.Id = id;
                var hints = "Enter Product Product title= ";
                product.ProductName = TakeInputString(hints);
                hints = "Enter Price ";
                product.ProductPrice = TakeUserInput(hints, _inputErrorMessage);
                product.Quantity = TakeUserInput("Enter qty", _inputErrorMessage);
                dictionaryProducts.Add(product.Id, product);
                MessageDisplay("successfully add an item\n");
                return true;

            }
            else
            {
                ErrorMassage("Error Found Inter Valid Id\n");
                return AddProduct();
            }

        }


        private void AddOrUpdateEnventoryEntry(int id, int quantity)
        {
            // if (dictionaryProducts.ContainsKey(id,product.Id))
            if (IsInventoryIdExist(id))
            {
                //true
                MessageDisplay("Update Enventory\n");
                var inventory = this.GetInventoryById(id);
                inventory.Quantity = inventory.Quantity + quantity;
                dictionaryProducts[inventory.Id] = inventory;

            }
            else
            {
                var product = GetProductById(id);

                MessageDisplay("Add New  Enventory\n");
                //dictionaryProducts.Add(inventory.Id, inventory);

                dictionaryInventory.Add(key: product.Id,
                    value:
                    new Inventory()
                    {
                        Id = product.Id,
                        ProductName = product.ProductName,
                        ProductPrice = product.ProductPrice,
                        Quantity = quantity
                    });

            }

        }


        private void MessageDisplay(string msg)
        {
            System.Console.Write(msg);
        }

        private void ErrorMassage(string value = "")
        {
            this.MessageDisplay(" Invalid Request Fount  " + value);
        }

        private string TakeInputString(string hints = null)
        {
            MessageDisplay(hints);
            return Console.ReadLine();
        }


        private int TakeUserInput(string inputPrompt = null, string errorMessage = null)
        {
            Console.WriteLine(inputPrompt);
            var input = Console.ReadLine();
            try
            {
                return Convert.ToInt32(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine(errorMessage);
                return TakeUserInput(inputPrompt, errorMessage);
            }
        }


        private void GetAllProduct()
        {
            MessageDisplay(_inventoryTableHeader);
            // foreach (var productKeyValuePair in dictionaryProducts)
            foreach (KeyValuePair<int, Product> productKeyValuePair in dictionaryProducts)
            {

                var prod = productKeyValuePair.Value;
                MessageDisplay(_tableLine);

                Console.WriteLine(" \t|{0}|\t|{1}|\t|{2}|\t|{3}|", prod.Id, prod.ProductName, prod.ProductPrice,
                    prod.Quantity);
            }
            MessageDisplay(_tableLine);
        }


        private void GetAllInventory()
        {
            MessageDisplay(_productTableHeader);
            foreach (KeyValuePair<int, Inventory> inventoryKeyValuePair in dictionaryInventory)
            {

                var inventory = inventoryKeyValuePair.Value;
                if (!inventory.Quantity.Equals(0))
                {
                    MessageDisplay(_tableLine);

                    Console.WriteLine(" \t|{0}|\t|{1}|\t|{2}|\t|{3}|\t|{4}|", inventory.Id, inventory.ProductName,
                        inventory.ProductPrice, inventory.Quantity, inventory.Quantity * inventory.ProductPrice);
                }
            }
            MessageDisplay(_tableLine);
        }


        private void InIt()
        {

            var product1 = new Product() {Id = 3, ProductName = "Shirt", ProductPrice = 1000, Quantity = 5};
            var product2 = new Product() {Id = 1, ProductName = "Pent", ProductPrice = 400, Quantity = 10};
            var product3 = new Product() {Id = 2, ProductName = "Soap", ProductPrice = 300, Quantity = 15};
            var product4 = new Product() {Id = 104, ProductName = "Socks", ProductPrice = 394, Quantity = 12};
            var product5 = new Product() {Id = 20, ProductName = "Jens", ProductPrice = 404, Quantity = 19};

            dictionaryProducts.Add(key: product1.Id, value: product1);
            dictionaryProducts.Add(key: product2.Id, value: product2);
            dictionaryProducts.Add(key: product3.Id, value: product3);
            dictionaryProducts.Add(key: product4.Id, value: product4);
            dictionaryProducts.Add(key: product5.Id, value: product5);

            //varibale initialize
            _inputErrorMessage = "Enter An Intager Number ";
            _inventoryTableHeader = "\t" + "ID" + "\t" + "Title" + "\t" + "Price" + "\t" + "Qunatity" + "\t" + "\n";
            _productTableHeader = "\t" + "ID" + "\t" + "Title" + "\t" + "Price" + "\t" + "Qunatity " + "Total" + "\t" +
                                  "\n";
            _tableLine = "--------------------------------------------------------------\n";

        }


    }
}