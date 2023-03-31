# Rocket Trajectory Visualisation Tool (R2V2)
The Rocket Trajectory Visualisation Tool is a Unity project to visualise rocket simulation data. It applies the model of the [Ladders of Abstraction](http://worrydream.com/LadderOfAbstraction/) to visualise trajectories of rocket launches whilst introducing interactivity in moving between different levels of data abstraction. Along with the trajectory visualisation is the visualisation of the abstracted variables(s) as a 2D/3D bar chart. Linked highlighting between the two visualisations then allows researchers to observe correlations between input values and the impacts that changing them have on flight trajectories.

The project has been developed based on the rocket simulation data generated from the open-source project RocketPy. RocketPy simulates realistic trajectories of launches based on input environmental, launch and rocket variables. The RocketPy source code has then been refactored to output the results of the simulations into CSV files which are then fed into Unity and visualised using IATK (Immersive Analytics Toolkit) and the 3D Interactive Bar Chart package.

# Getting Started
## Requirements
 - Project pulled from GitHub 
 - Unity 2020.3.32f 
 - OpenXR compatible headset, including Oculus Quest, Oculus Quest 2, Microsoft HoloLens,  and Valve Index

## Setting up the Project
1.	Connect and setup your VR headset according to manufacturer’s instructions. 
2.	Open the project folder and navigate to Swordfish_IT_Project -> Assets -> Swordfish -> Main.unity
3.	Play the scene. The project should automatically detect and run the project on the VR headset. 

## Generating new datasets from the python file
1.      Login to Google Colab with a google account
2.      Click "Upload" on the pop-up window
3.      Drop a copy of the python file "ColabRocketPyV2_1.py", which can be obtained in this repository
4.      Ensure the source code from the RocketPy GitHub is added to the Google Drive folder from https://github.com/RocketPy-Team/RocketPy/releases. You will need version 11 to ensure compatibility.
5.      Rename the source code folder to Rocket
6.      Follow the instructions within the Colab file, running the cells one by one (Some cells can be skipped and are commented accordingly)
7.      The first cell requires access permission from Google Drive. Grant access. 
8.      After the simulations have run, the folder with the data should exist within Google Drive.
	

## Controller bindings
 - Trigger button = selection  
	 - To show display for data point in trajectory visualisation
	 - To show display for bar in bar chart
	 - UI components
	 - Grabbing the bar chart 
 - Primary button (A/X button) = select a data point to select its trajectory as the trajectory to play the animation over 
 - Left thumbstick/ touchpad = movement  
 - Right thumbstick = Rotation
 - Right grip button = activate pointer to interact with UI components

## Original Team
- Luke Gerschwitz (luke.gerschwitz@gmail.com)
- Elyssa Yeo (elyssayeo16@gmail.com)

## New Team
 - David Galbory (galdp004@mymail.unisa.edu.au)
 - Heather Leyssenaar (leyhm006@mymail.unisa.edu.au)
