Feature: Deposits from bank
	In order to have funds for mobile payments
	As a mobile bank user
	I want to deposit money to my mobile account from bank

Scenario: Receive payment from bank
	Given following users are registered
	| Phone number | Balance |
	| 92748326     | 100     |
	When bank receives payment
	| From account | Amount | Payment message |
	| 395740384736 | 500 | 4792748326 |
	Then following SMS should be sent
	| Phone number | Message |
	| 92748326 | You received 500 from bank account 395740384736. Your new balance is 600. Thank you for using InMemory Bank. |

