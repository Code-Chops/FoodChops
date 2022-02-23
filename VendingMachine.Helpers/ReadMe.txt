The helpers being defined here are made by me and another programmer in our spare time. Unfortunately it has not been converted to support nullable reference types.

Amounts:
It gives a good and solid base to use currencies/amounts in your application. It also has strict rules on if an amount should be negative/positive or both.
This creates a safe environment in which an amount (in for example a wallet) cannot become negative.
This namespace has a lot of implicit and explicit operators so conversion is done easily.

Services:
The service dictionary defines a solid way to dynamically and automatically find implementations of a base class (using the represented value).
It diminishes the need for extending and editing switches throughout the code.