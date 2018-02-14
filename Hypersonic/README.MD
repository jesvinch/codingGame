# Coding Game
	* This solution represents solutions for different puzzles from codingame.com.
	* https://www.codingame.com

## HyperSonic
	* https://www.codingame.com/ide/puzzle/hypersonic
	* In progress
	* Current league - WoodLeague 2

### To do next
	* Score cells based on items that can be collected.
	* Find the best cell within certain range rather than within current row. 

### Current logic
	1. Score cells in the current row based on how many box can be blown up by placing bomb in the cell.
	2. Go to the next bets cell in row, until there are no more boxes in that row.
	3. If no boxes in current row, go to next row and repeat steps 1 & 2.
	4. Repeat till there are no more boxes.