'''
Project Title: Assignment 8 - Agario Game 

Authors: Draeden Jensen & Derek Kober
Date: 4/13/2023
Course: CS 3500, University of Utah, School of Computing

GitHub Repository: https://github.com/uofu-cs3500-spring23/assignment8agario-draeden.git
Completion Date: 4/13/2023
'''

# User Interface and Game Design Decisions

	Implemented a simple, clean user interface for the game with a connection screen and a game screen.
	Utilized the Drawable class to draw game objects on the canvas, adjusting object positions and sizes based on the
	zoom level. Created a separate thread for the game timer to update the GUI at 30 FPS.
	Locked access to shared resources like World.Foods and World.Players collections for thread safety.

	The assignment specs say to use spacebar as the split button, but we decided to leave this funcionality as being
	triggered by clicking. Maui doesn't have keyboard listening features built-in, so using the spacebar would've 
	required complicated workarounds. Even if we had the time to do that, it seemed like any workaround would result
	in a worse play experience than simple mouse funcionality, so we felt this was adequate.

# Partnership Information

	Draeden Jensen: Implemented the MainPage class, handling user input and game events.
	Derek Kober: Implemented the Drawable class, responsible for drawing game objects on the canvas.

	Both partners contributed to pair programming sessions and code reviews.

# Branching

	Some branches were created during the development of this project, but was coordinated such that no merging conflicts
	occured during pushed. Primarily work was completed via pair programming and direct collaboration.

# Testing

	We performed manual testing to ensure the functionality of the game client, such as connecting to the server, 
	moving  the player, and eating food. 

	We also tested edge cases, like invalid server addresses and disconnections.
	
	We didn't use unit tests for this assignment, but we ensured the code was "correct" by verifying the expected 
	behavior during gameplay and checking for any errors or issues.

# Time Tracking (Personal Software Practice)
	
	Initial time estimate: 25 hours

	Total time spent: 19 hours

	Time spent working together: 15 hours

	Time spent working individually: 4 hours (each)

# Reflection:

	Our initial time estimate was lower than the actual time spent on the project. This discrepancy may be due to the 
	time spent learning new tools and techniques, as well as debugging issues that arose during development. Overall, our
	estimates are improving as we gain more experience and understanding of our abilities. In the future, we will account
	for extra time spent on learning and debugging when making our estimates.

# References:

	https://learn.microsoft.com/en-us/dotnet/maui/user-interface/graphics/colors?view=net-maui-7.0
	https://stackoverflow.com/questions/71311565/system-drawing-vs-maui-graphics-from-argb-gives-different-output
	https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.igraphicsview.invalidate?view=net-maui-7.0#microsoft-maui-igraphicsview-invalidate
	https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/gestures/pointer?view=net-maui-7.0
	https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.graphics.point.deconstruct?view=net-maui-6.0#microsoft-maui-graphics-point-deconstruct(system-double@-system-double@)
