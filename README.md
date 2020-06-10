# crypto-transaction-branch-tracker
Crypto Transaction Branch Tracker(CTBT) is a program that will allow you to easily keep track of your cryptocurrency investments by allowing you to create branches.

A branch is a series of investments/payments that are made, up until the point you have sold. By doing this, you can quickly see exactly how much money you have invested in a cryptocurrency in your current branch. This means that CTBT allows you to, at a glance, see if selling right now would generate a profit or not.

# How CTBT stores your data
There are no logins, and nothing is stored on any kind of servers. When starting up, you'll be greated with a blank slate, with options to start new branches and add some transactions.

The main data wrap is stored at the executing directory within a JSON file. The specific data that is written to each branch and transaction is encoded in Base64 format. This just means that the application doesn't even need an internet connection for it to function, and you can be reassured that nothing is ever stored anywhere but your own device.
