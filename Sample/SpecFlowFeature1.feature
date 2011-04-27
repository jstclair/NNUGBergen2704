Feature: Addition
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Scenario Outline: Add two numbers
	Given I have entered <number1> into the calculator
	And I have entered <number2> into the calculator
	When I press add
	Then the result should be <sum> on the screen
	
	Examples: 
	| number1 | number2 | sum |
	| 50      | 70      | 120 |
	| 30      | 50      | 80  |
	| 10      | 100     | 110 |
	| 0       | 0       | 0   |