Feature: Withdraw to bank
	In order to use funds collected from mobile payments
	As a mobile bank user
	I want to withdraw money from my mobile account to bank

Scenario: Transfer funds to bank
	Given following users are registered
	| Phone number | Balance |
	| 92748326     | 100     |
	And payment fee is as follows
	| Payment type   | Payer fee | Collector fee |
	| BankWithdrawal | 5         | 0             |
	When user sends SMS
	| Phone number | Message              |
	| 92748326     | BANK 50 395740384736 |
	Then following SMS should be sent
	| Phone number | Message                                                                                                     |
	| 92748326     | You transferred 50 to bank account 395740384736. Your new balance is 45. Thank you for using InMemory Bank. |
