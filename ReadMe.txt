Here is my Vending Machine app. It uses C# 10, .NET 6 with Blazor

I wanted to show what could be possible if an official app would be written (and expanded), so therefore I added extra features.
For example:
- The machine can use a different currency than EUR
- The quantity of product stacks can be highered / lowered
- The vending machine has 2 different versions (at random): a blue and a red one
- Resources are used to support different languages
- TypeScript and JsInterop is used so JavaScript can play different sounds
- The views are made responsive
- The state of the view is shared between multiple clients
- The component's logic is placed in a code-behind

If I would have more time, I would have implemented:
- Made the property ProductStacksByLocation on the class ProductStack immutable instead of an array
- Create a class that can handle calculating with nullable decimals as amount (where NULL means infinite)
- Coin drag-and-drop functionality and show the machine money on the machine itself

VendingMachine.App.StartUp.cs is a good place to start reading the code.

Usage:
Click on the Inserted Coins wallet to release your coins
Click on coins in the User's wallet to put them in the machine
A lot can be configured in VendingMachine.App.Startup.ConfigureServices