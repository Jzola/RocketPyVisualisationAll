# -*- coding: utf-8 -*-
"""ColabRocketPyV2_1.py

Automatically generated by Colaboratory.

Original file is located at
    https://colab.research.google.com/drive/10CN0aYOMJ35DZbWgeCJT_Hpd26CJ1sb5

V2.0 created 19/03/23 by Heather Leyssenaar: leyhm006

V2.01 includes user menu options to improve the usability and allow a choice of rocket characteristic to change, how many simulations will be run, and how much the characteristic will change for each simulation.
The new version also offers new environment variables to change (temp and wind speed)

HOW TO RUN THE FILE:
1. Click on each cell in order, and press the 'play' symbol to the left. (It will show when you hover over the '[ ]' symbol.)
2. The first cell will require you to permit access to google drive.
3. Some cells will require some input from the user. Type the input, then press 'Enter'. If you make a mistake, just re-run the cell.
4. You will require the Rocket folder in your google drive. (More instructions to follow.) If you do not have these, they can be obtained from https://github.com/RocketPy-Team/RocketPy/releases - You will need version 11
"""

from google.colab import drive
drive.mount('/content/drive')

# Commented out IPython magic to ensure Python compatibility.
# %cd /content/drive/My\ Drive/Rocket/RocketPy

# List files to make sure we're in the expected directory.
# Your output will look different, showing your own Drive files here.
!ls

# Commented out IPython magic to ensure Python compatibility.
#!pip install --upgrade --force-reinstall netCDF4

!pip install numpy==1.21
#!pip install scipy==1.7.1
!pip install matplotlib>=3.0
#!pip install netCDF4==1.6.0
!pip install "netCDF4<1.6.0"
!pip install requests
!pip install pytz
!pip install simplekml


from rocketpy import Environment, SolidMotor, Rocket, Flight, Function
from time import process_time, perf_counter, time
from IPython.display import display
import datetime
#import numpy as np
#import os


# Uncomment if crashing during weather api
import subprocess
import sys
#subprocess.check_call([sys.executable, "-m", "pip", "install", r"netCDF4<1.6.0"""])

# %cd /content/drive/My\ Drive/Rocket
!ls

#Added in V2 to aid in generating folders in google drive
#Run this cell every time you wish to run a simulation, to set up the new folder for data
import os

folderName = input("Please enter a (unique) folder name to generate your new data :")

filename = "inputData/"
path = folderName +"/"+ filename

if not os.path.isdir(path):
  os.makedirs(path)
  print(f"You will find input data in {path}")


else:
  print("The folder already existed. Please re-run the cell, either entering a new folder name, or deleting the original folder first.")

#Added in V2 
#Default values for the variables that the user will be able to change

#RERUN this cell every time, to reset the modifiers to zero.
step = 1 
startBurnOut = 3.6
startMass = 20 
startTemperature = 300
startWindSpeedEast = 0
endWindSpeedEast = 10
startWindSpeedNorth = -2
endWindSpeedNorth = 0
          
startRocketNozzle =  -1.255 
startRocketPropellant = -0.85704


#modifiers that will determine if the variable is the focus of the simulations.
massModifier = 1 #by default mass is the changed variable to simulate. 
burnoutModifier= 0
tempModifier = 0
windSpeedModifier = 0
rocketNozzleModifier = 0
rocketPropellantModifier = 0


#V3 optional try to get another level of abstraction with another loop of code.

#Set up whether all 6 rocket motors are used (per set of simulations)
print("The default simulations are set up to cycle through 6 different motor types, with changes in the chosen variable (eg. mass of rocket) simulated for each motor.")
motorChoice = input("Would you like to use only one motor? Y/N \n (this will improve performance if you want to run a greater number of simulations on your chosen rocket characteristic ) ")

#Added to version 2
#Optional: run this cell if you need to convert temperature to kelvin. Note that 300 Kelvins is roughly equivalent to 26 degrees. 
Celsius = int(input("Please enter a temperature in Celsius: "))

Kelvin = int(Celsius + 273.15)
print(str(Kelvin))
print("You can rerun the cell if you would like to check a different value.")

#V2 added a user menu to choose the rocket characteristic to focus on during run time
#get user choice for type of data to analyse
print(f"Which variable would you like to adjust:\n 1. Mass of the rocket (default weight is {str(startMass)})") 
print(f" 2: Temperature in Kelvins (default temp is {str(startTemperature)})")
print(f" 3: Change to the wind speed (default values east: start {str(startWindSpeedEast)}, end {str(endWindSpeedEast)})")
print(f" (default values north: start {str(startWindSpeedNorth)}, end {str(endWindSpeedNorth)})")
print(f" 4: Change the distance of Rocket Nozzle (default value {str(startRocketNozzle)})")
print(f" 5: Change the distance of the rocket propellant (default value {str(startRocketPropellant)})")
print(f" 6: Change the value of the burn out time in seconds (default value {str(startBurnOut)})")
varChoice = input()
#if windspeed, get all values. Note that a negative value blows from opposite direction

#V2_1 added more wind modifiers, and variables to adjust the windspeed during different simulations
#Run this cell every time.
windNorthModifier = 0
windEastModifier = 0
eastWindStep = 0
northWindStep = 0
windModifier = 0

#V2_1 added more menu options (temperature and wind speed)
startVal = 0
step = 0
#Reset the mass modifier in case the user has picked another option
massModifier = 0

if(varChoice.lower() in ["1", "mass"]):
 
    massModifier = 1
    startVal = input("What starting weight would you like to use?")
    if startVal.isdigit():
      startMass = int(startVal)
    else:
      print("You have entered an invalid value, please rerun the cell.")
elif(varChoice.lower() in ["2", "temperature"]):
    tempModifier = 1
    startVal = input("What starting temperature would you like to use (in Celsius)?")
    if startVal.isdigit():
      startTemperature = int(startVal) + 273.15
    else:
      print("You have entered an invalid value, please rerun the cell.")    
elif(varChoice.lower() in ["3", "wind", "wind speed"]):
    #ensure that only the wind value changes for every loop
    windModifier = 1 #only used to help avoid errors in the step value
    print(f"Current Wind speeds East: start {str(startWindSpeedEast)}, end {str(endWindSpeedEast)}")
    print(f" Current Wind speeds North: start {str(startWindSpeedNorth)}, end {str(endWindSpeedNorth)}")
    startVal = input("What starting wind speed (east) in meters/sec would you like to use at 0m? (use a negative number for a westerly wind) ")
    if startVal.lstrip('-').isdigit():
      startWindSpeedEast = int(startVal)
    else:
      print("You have entered an invalid value, please rerun the cell.")  

    startVal = input("What starting wind speed (north) in meters/sec would you like to use at 0m? (use a negative number for a southerly wind) ")
    if startVal.lstrip('-').isdigit():
      startWindSpeedNorth = int(startVal)
    else:
      print("You have entered an invalid value, please rerun the cell.")

    startVal = input("What wind speed (east) in meters/sec would you like to use at 1km? (use a negative number for a westerly wind) ")
    if startVal.lstrip('-').isdigit():
      endWindSpeedEast = int(startVal)
    else:
      print("You have entered an invalid value, please rerun the cell.")  
    startVal = input("What ending wind speed (north) in meters/sec would you like to use? (use a negative number for a southerly wind) ")
    if startVal.lstrip('-').isdigit():
      endWindSpeedNorth = int(startVal)
    else:
      print("You have entered an invalid value, please rerun the cell.")

    step = input("How much do you want the east wind value to change? (Type '0' if only simulating North wind changes, or use a negative number for westerly changes)\n Note: both start and end values will change by this amount\n")  
    if step.lstrip('-').isdigit(): 
      if step != "0":
        eastWindStep = int(step)

        windEastModifier = 1
    else: 
      print("You have entered an invalid value, please rerun the cell.")    

    step = input("How much do you want the north wind value to change? (Type '0' if only simulating East wind changes, or a use negative number for south changes)\n Both start and end values will change by this amount \n")  
    if step.lstrip('-').isdigit(): 
      if step != "0":
        northWindStep = int(step)

        windNorthModifier = 1
    else: 
      print("You have entered an invalid value, please rerun the cell.")

elif varChoice.lower() in ["4", "nozzle"]:
  rocketNozzleModifier = 1
  print("The rocket nozzle is the distance between rocket’s center of mass, without propellant, to the exit face of the nozzle, in meters")
  startVal = input("What starting value should be used? ")
  if startVal.replace('.', '').replace('-', '').isdigit():
      startRocketNozzle = float(startVal)
      print(startRocketNozzle)
  else:
      print("You have entered an invalid value, please rerun the cell.")


elif varChoice.lower() in ["5", "propellant"]:
  rocketPropellantModifier = 1

  print("Rocket Propellant: Distance between rocket’s center of mass, without propellant,") 
  print("to the motor reference point, which for solid and hybrid motors is the center of mass of solid propellant, in meters")
  startVal = input("What starting value should be used? ")
  if startVal.replace('.', '').replace('-', '').isdigit():
      startRocketPropellant = float(startVal)
  else:
      print("You have entered an invalid value, please rerun the cell.")
elif varChoice.lower() in ["6", "burnout", "burn out", "burn out time"]:
  burnoutModifier = 1
  startVal = input("What starting value (in seconds) should be used? ")
  if startVal.replace('.', '').replace('-', '').isdigit():
      startBurnOut = float(startVal)
      #print(startBurnOut)
  else:
      print("You have entered an invalid value, please rerun the cell.")
else:
  print("Your menu option was not recognised. Please rerun the user choice of rocket characteristic cell,") 

if windModifier != 1:
  step = float(input(f"What value would you like to increase the rocket characteristic by for each simulation?\n (Starting at {str(startVal)})\n ")) #call in the value they have already used to make their decision easier
else:
  #reset the step value so that only wind changes are in effect.
  step = 0

#V2_1 Debug lines to ensure code running correctly without having to generate the full simulation

print(windEastModifier)
print(windNorthModifier)
print(eastWindStep)
print(northWindStep)
print(startRocketNozzle)
print(startRocketPropellant)
print(startBurnOut)

#V2_1 debug line to check source of issue when using 'step' value after user input
print(type(step))

#V2 allow the user to choose the number of simulations

iterations = int(input("How many simulations / variations in data would you like to run? \n (if using multiple motors, remember the default is 6 per motor, resulting in a total of 30 simulations)\n WARNING - Excessive numbers will extend runtime of the program: "))

#V2_1 (Optional cell)- TEST CODE FOR WIND SPEED CHECKS (to ensure code runs as expected without having to generate the full simulations)
for iter in range(0, iterations):
  print("--\nEast")
  print(startWindSpeedEast + (iter * eastWindStep * windEastModifier))
  print(endWindSpeedEast + (iter * eastWindStep * windEastModifier))
  print("--\nNorth")
  print(startWindSpeedNorth + (iter * northWindStep * windNorthModifier))
  print(endWindSpeedNorth + (iter * northWindStep * windNorthModifier))
  #print("-")

#Next ladder of abstraction level:
#(temperature and windSpeed), (mass and distanceRocketPropellant),  (mass and windSpeed)

#DEBUG line only
print(tempModifier)

#V2_1 DEBUG - Attempt to find out why the burnout is not changing
iter2 = 3
motor = "Cesaroni_M1300.eng"
Pro75M1670 = SolidMotor(
            #choose the motor from the available file
            thrustSource="RocketPy/data/motors/"+motor,
            burnOut = (startBurnOut + (burnoutModifier * step * iter2)),
            #burnOut= startBurnOut,
            grainNumber=5,
            grainSeparation=5 / 1000,
            grainDensity=1815,
            grainOuterRadius=33 / 1000,
            grainInitialInnerRadius=15 / 1000,
            grainInitialHeight=120 / 1000,
            nozzleRadius=33 / 1000,
            throatRadius=11 / 1000,
            interpolationMethod="linear",
        )

#Need to manually override the burnout calculation from the engineering file
Pro75M1670.burnOutTime = (startBurnOut + (burnoutModifier * step * iter2))
print(Pro75M1670.burnOutTime)

#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sun Mar 13 14:46:22 2022

@author: lukegerschwitz
"""

#from rocketpy import Environment, SolidMotor, Rocket, Flight, Function
#from time import process_time, perf_counter, time
#from IPython.display import display
#import datetime
#import numpy as np
#import os

# Uncomment if crashing during weather api
#import subprocess
#import sys
#subprocess.check_call([sys.executable, "-m", "pip", "install", r"netCDF4<1.6.0"""])


# Switch to current directory
#abspath = os.path.abspath(__file__)
#dname = os.path.dirname(abspath)
#os.chdir(dname)

####################################################################################
############################ Exporting Simulations #################################
####################################################################################


# Appends a current simulations input variables to a csv file.
def export_input_variables(flight_settings, file, head):
    
    inputs = ""
    first = True
    
    if head == True:
        for key in flight_settings:
            if first == True:
                inputs = str(key)
                first = False
            else:
                inputs += "," + str(key)
    first = True
    
    for key in flight_settings:
        if first == True:
            inputs += "\n" + str(flight_settings[key])
            first = False
        else:
            inputs += "," + str(flight_settings[key])  
    file.write(inputs)

def export_flight_error(flight_setting):
    num=1
    #dispersion_error_file.write(str(flight_setting) + "\n")

####################################################################################
############################ Create Files/Set Number of Sims #######################
####################################################################################

# Basic analysis info 

# # Create data files for inputs, outputs and error logging
# dispersion_error_file = open(str(filename) + "disp_errors.txt", "w")
# dispersion_input_file = open(str(filename) + "disp_inputs.txt", "w")
# dispersion_output_file = open(str(filename) + "disp_outputs.txt", "w")

####################################################################################
############################ Simulation Settings ###################################
####################################################################################

#V2 update this to use input iterations
iterationsPerMotor = iterations


#startMass = 20 #changed this line to 50 for more mass analysis (Original value 20)
idCount = 1


# Hypertek motor does not work with two parachutes

if motorChoice.lower() in ["y", "yes"]:
  motors = ["Cesaroni_M1300.eng"]  
else:
  
  motors = ["Cesaroni_M1300.eng", "Cesaroni_M1400.eng", 
          "Cesaroni_M1540.eng", "Cesaroni_M1670.eng", "Cesaroni_M3100.eng"]#, "Cesaroni_7450M2505-P.eng", # "Hypertek_J115.eng"]

# Environmental data 
Env = Environment(
    railLength=5.2,
    latitude=32.990254,
    longitude=-106.974998,
    elevation=1400
)

#V2 Forecast data is no longer used
# Set the date and time
#tomorrow = datetime.date.today() + datetime.timedelta(days=1)
#Env.setDate((tomorrow.year, tomorrow.month, tomorrow.day, 12))  # Hour given in UTC time
# Retrieve forecast data from GFS
# https://www.emc.ncep.noaa.gov/emc/pages/numerical_forecast_systems/gfs.php
#Env.setAtmosphericModel(type="Forecast", file="GFS")

"""
Change to Environment model added by Heather Leyssenaar 
"""
EnvCA = Environment(railLength=5,
                     latitude=32.990254,
                    longitude=-106.974998,
                    elevation=1400)

#Example Environment
#Wind U is wind from east, wind_v is wind from north. A negative number indicates the wind is coming from opposite direction
#Default value has easterly wind at 5m/s at 0 distance, and 10m/s at  1km. wind_u=[(0, 5), (1000, 10)], 
#and a southerly wind of 2m/s at 0 distance, changing to 3m/s from north at 500m, then 8m/s at 1.6km 
EnvCA.setAtmosphericModel(
    type="CustomAtmosphere",
    pressure=None, # standard atmospheric pressure is 101325 Pa. This is the default.
    temperature=startTemperature, #temperature is in Kelvins, default 300
    wind_u=[(0, 5), (1000,10)],
    wind_v=[(0, -2), (500, 3), (1600, 8)],
)



# Loop each motor 
for motor in motors:

    # Loop each mass for each motor #TODO - update 
    for iter in range(0, iterationsPerMotor):
        
        
        #basic analysis
        #number_of_simulations = 1
        
        # Create data files for inputs, outputs and error logging (not used as of Version 2)
        #dispersion_error_file = open(str(filename) + "disp_errors.txt", "w")
        #dispersion_input_file = open(str(filename) + "all_inputs.csv", "a")
        #dispersion_input_file = open(str(path) + "all_inputs.csv", "a")
        dispersion_input_file2 = open(str(path) + "all_inputs.csv", "a") #New inputs added for version 2
        #dispersion_output_file = open(str(filename) + "disp_outputs.txt", "w")
        
        #v2 User changes to the atmospheric model go here
        #Wind U is wind from east, wind_v is wind from north. A negative number indicates the wind is coming from opposite direction
        #Default value has easterly wind at 5m/s at 0 distance, and 10m/s at  1km. wind_u=[(0, 5), (1000, 10)], and a southerly wind of 2m/s at 0 distance 
        input_temp = startTemperature + (tempModifier * step * iter) 
        east_wind = (startWindSpeedEast + (iter * eastWindStep * windEastModifier),(endWindSpeedEast + (iter * eastWindStep * windEastModifier)) )
        north_wind = ((startWindSpeedNorth + (iter * northWindStep * windNorthModifier)) ,  (endWindSpeedNorth + (iter * northWindStep * windNorthModifier)))
        EnvCA.setAtmosphericModel(
          type="CustomAtmosphere",
          pressure=None, # standard atmospheric pressure is 101325 Pa
          temperature=startTemperature + (tempModifier * step * iter), #temperature is in Kelvins
          wind_u=[(0, (startWindSpeedEast + (iter * eastWindStep * windEastModifier))), (1000, (endWindSpeedEast + (iter * eastWindStep * windEastModifier)))],
          wind_v=[(0, (startWindSpeedNorth + (iter * northWindStep * windNorthModifier))), (1000, (endWindSpeedNorth + (iter * northWindStep * windNorthModifier)))],
        )
       
        input_burnOut = (startBurnOut + (burnoutModifier * step * iter))
        
        # Rocket Motor
        Pro75M1670 = SolidMotor(
            #choose the motor from the available file
            thrustSource="RocketPy/data/motors/"+motor,
            burnOut = (startBurnOut + (burnoutModifier * step * iter)),
            #burnOut= startBurnOut,
            grainNumber=5,
            grainSeparation=5 / 1000,
            grainDensity=1815,
            grainOuterRadius=33 / 1000,
            grainInitialInnerRadius=15 / 1000,
            grainInitialHeight=120 / 1000,
            nozzleRadius=33 / 1000,
            throatRadius=11 / 1000,
            interpolationMethod="linear",
        )
        #V2_1 code added to override the burnout changes that happen during motor initialization
        Pro75M1670.burnOutTime = (startBurnOut + (burnoutModifier * step * iter))
        #V2_1 values to be added to the input csv file
        input_Rocket_Nozzle = startRocketNozzle + (iter * rocketNozzleModifier * step)
        input_Rocket_Propellant = startRocketPropellant + (iter * rocketPropellantModifier * step)
        # Rocket
        Calisto = Rocket(
            motor=Pro75M1670,
            radius=127 / 2000,
            mass=(startMass+(massModifier* step*iter)),  
            #mass = startMass,
            inertiaI=6.60,
            inertiaZ=0.0351,
            distanceRocketNozzle=startRocketNozzle + (iter * rocketNozzleModifier * step), 
            distanceRocketPropellant=startRocketPropellant + (iter * rocketPropellantModifier * step), 
            powerOffDrag="RocketPy/data/calisto/powerOffDragCurve.csv",
            powerOnDrag="RocketPy/data/calisto/powerOnDragCurve.csv",
        )
        Calisto.setRailButtons([0.2, -0.5])
        
        # Aerodynamic Forces
        noseLen = 0.55829
        noseDisToCM = 0.71971
        NoseCone = Calisto.addNose(length=noseLen, kind="vonKarman", distanceToCM=noseDisToCM)
        fSpan = 0.100
        fRootChord = 0.120
        fTipChord = 0.040
        fDistanceToCM=-1.04956
        FinSet = Calisto.addFins(
            4, span=fSpan, rootChord=fRootChord, tipChord=fTipChord, distanceToCM=fDistanceToCM
        )
        Tail = Calisto.addTail(
            topRadius=0.0635, bottomRadius=0.0435, length=0.060, distanceToCM=-1.194656
        )
        
        # Parachute Code
        def drogueTrigger(p, y):
            # p = pressure
            # y = [x, y, z, vx, vy, vz, e0, e1, e2, e3, w1, w2, w3]
            # activate drogue when vz < 0 m/s.
            return True if y[5] < 0 else False
        
        def mainTrigger(p, y):
            # p = pressure
            # y = [x, y, z, vx, vy, vz, e0, e1, e2, e3, w1, w2, w3]
            # activate main when vz < 0 m/s and z < 800 m + 1471m.
            return True if y[5] < 0 and y[2] < (800 + 1471) else False # 1471 for surface elevation of launch site
        
            # Remove extra parachutes just in case
            Calisto.parachutes.remove(Drogue)
            Calisto.parachutes.remove(Main)
        
        Main = Calisto.addParachute(
            "Main",
            CdS=10.0,
            trigger=mainTrigger,
            samplingRate=105,
            lag=1.5,
            noise=(0, 8.3, 0.5),
        )

        Drogue = Calisto.addParachute(
            "Drogue",
            CdS=1.0,
            trigger=drogueTrigger,
            samplingRate=105,
            lag=1.5,
            noise=(0, 8.3, 0.5),
        )
        
     
        ####################################################################################
        ############################# Run Simulation #######################################
        ####################################################################################
        
        
        out = display("Commencing Simulation " + str(idCount))
        
        # Simulate flight
        try:

            flightInclination = 80
            flightHeading = 90
            #set up the flight with all pre set values
            #start extra sims #TODO run extra environments automatically
            environmentID = 1
            #for envirs in envs:
            #generate the flight data
            #test_flight = Flight(rocket=Calisto, environment=Env, inclination=flightInclination, heading=flightHeading)
            test_flight = Flight(rocket=Calisto, environment=EnvCA, inclination=flightInclination, heading=flightHeading)
            
            if not os.path.isdir(folderName+ "Env" + str(environmentID)+"/"):
              #os.makedirs(folderName+ "Env" + str(environmentID)+"/")
              pass # we are unlikely to use this, unless we get to third ladder of abstraction 

            # Custom export function from GitHub #change to use env number
            #TODO fix folder layout.
            #test_flight.exportData(newFolderName+ "Env" + str(environmentID)+"/" + str(idCount) + ".csv",
            test_flight.exportData(folderName+ "/" +str(idCount) + ".csv",
                                    "x",
                                    "y",
                                    "z",
                                    "vx",
                                    "vy",
                                    "vz",
                                    "e0",
                                    "e1",
                                    "e2",
                                    "e3",
                                    "w1",
                                    "w2",
                                    "w3",
                                   "ax",
                                   "ay",
                                   "az",
                                   "alpha1",
                                   "alpha2",
                                   "alpha3",
                                   "R1",
                                   "R2",
                                   "R3",
                                   "M1", 
                                   "M2", 
                                   "M3",
                                   "pressure",
                                   "density", 
                                   "dynamicViscosity", 
                                   "speedOfSound", 
                                   "windVelocityX", 
                                   "windVelocityY",
                                   "speed", 
                                   "acceleration", 
                                   #"maxAcceleration",
                                   #"maxAccelerationTime", 
                                   "horizontalSpeed", 
                                   "pathAngle",
                                   #"drift", 
                                   #"bearing", 
                                   "latitude", 
                                   "longitude", 
                                   #"maxSpeed", 
                                   "horizontalSpeed", 
                                   "phi",
                                   "dynamicPressure", 
                                   "aerodynamicLift", 
                                   "aerodynamicDrag",
                                   "rotationalEnergy",
                                   "kineticEnergy", 
                                   "potentialEnergy", 
                                   "totalEnergy",
                                   "thrustPower", 
                                   "dragPower", 
                                   #"angleOfAttack", 
                                    timeStep=0.6)

            ####################################################################################
            ############################# Prep input variables for export ######################
            ####################################################################################
            
            analysis_parameters = {
                # id to link input data with trajectory data
                "id": idCount,
                # Mass Details
                "rocket mass (kg)": (Calisto.mass),  # Rocket's dry mass (kg) and its uncertainty (standard deviation)
                "time (s)": test_flight.tFinal, # Flight duration in seconds
                "motor type": motor.split(".", 1)[0],
                # Propulsion Details - run help(SolidMotor) for more information
                "impulse (N*s)": (Calisto.motor.totalImpulse),  # Motor total impulse (N*s)
                "burn out (s)": (Calisto.motor.burnOutTime),  # Motor burn out time (s)
                "nozzle radius (m)": (Calisto.motor.nozzleRadius),  # Motor's nozzle radius (m)
                "throat radius (m)": (Calisto.motor.throatRadius),  # Motor's nozzle throat radius (m)
                # Aerodynamic Details - run help(Rocket) for more information
                "radius (kg*m^2)": (Calisto.radius),  # Rocket's radius (kg*m^2)
                "distance rocket nozzle (m)": (Calisto.distanceRocketNozzle),  # Distance between rocket's center of dry mass and nozzle exit plane (m) (negative)
                "distance rocket propellant (m)": (Calisto.distanceRocketPropellant),  # Distance between rocket's center of dry mass and and center of propellant mass (m) (negative)
                "nose length (m)": noseLen,  # Rocket's nose cone length (m)
                "nose distance to cm (m)": noseDisToCM,  # Axial distance between rocket's center of dry mass and nearest point in its nose cone (m)
                "fin span (m)": fSpan,  # Fin span (m)
                "fin distance to cm (m)": fDistanceToCM,  # Axial distance between rocket's center of dry mass and nearest point in its fin (m)
                # Launch and Environment Details - run help(Environment) and help(Flight) for more information
                "rail length (m)": Env.rL,  # Launch rail length (m)
                "latitude": Env.lat, # Latitude of launch site
                "longitude": Env.lon, # Longitude of launch site
                # Start time for each stage of flight
                "Phase 1 - Rail Launch": test_flight.flightPhases.list[0].t,
                "Phase 2 - Powered Flight": test_flight.flightPhases.list[1].t,
                "Apogee": test_flight.apogeeTime,     # Placing this in between for ease of use in Unity. Not counting
                                                          # it as its own phase, but it occurs between phases 2 & 3.
                "Phase 3 - Drogue Parachute Deployment": test_flight.flightPhases.list[2].t,
                "Phase 4 - Descent Under Drogue Parachute": test_flight.flightPhases.list[3].t,
                "Phase 5 - Main Parachute Deployment": test_flight.flightPhases.list[4].t,
                "Phase 6 - Descent Under Main Parachute": test_flight.flightPhases.list[5].t,
                "Phase 7 - Impact": test_flight.flightPhases.list[6].t,
                "No. Stages": len(test_flight.flightPhases.list),

                # Apogee & Impact Details
                "apogee X": test_flight.apogeeX,
                "apogee Y": test_flight.apogeeY,
                "apogee Z": test_flight.apogee, # i.e. altitude
                "x impact": test_flight.xImpact,
                "y impact": test_flight.yImpact,
                "impact velocity": test_flight.impactVelocity,


            }
            analysis_parameters2 = {
                # id to link input data with trajectory data
                "id": idCount,
                # Mass Details
                "rocket mass (kg)": (Calisto.mass),  # Rocket's dry mass (kg) and its uncertainty (standard deviation)
                "time (s)": test_flight.tFinal, # Flight duration in seconds
                "motor type": motor.split(".", 1)[0],
                # Propulsion Details - run help(SolidMotor) for more information
                "impulse (N*s)": (Calisto.motor.totalImpulse),  # Motor total impulse (N*s)
                "burn out (s)": (Calisto.motor.burnOutTime),  # Motor burn out time (s)
                "nozzle radius (m)": (Calisto.motor.nozzleRadius),  # Motor's nozzle radius (m)
                "throat radius (m)": (Calisto.motor.throatRadius),  # Motor's nozzle throat radius (m)
                # Aerodynamic Details - run help(Rocket) for more information
                "radius (kg*m^2)": (Calisto.radius),  # Rocket's radius (kg*m^2)
                "distance rocket nozzle (m)": (Calisto.distanceRocketNozzle),  # Distance between rocket's center of dry mass and nozzle exit plane (m) (negative)
                "distance rocket propellant (m)": (Calisto.distanceRocketPropellant),  # Distance between rocket's center of dry mass and and center of propellant mass (m) (negative)
                "nose length (m)": noseLen,  # Rocket's nose cone length (m)
                "nose distance to cm (m)": noseDisToCM,  # Axial distance between rocket's center of dry mass and nearest point in its nose cone (m)
                "fin span (m)": fSpan,  # Fin span (m)
                "fin distance to cm (m)": fDistanceToCM,  # Axial distance between rocket's center of dry mass and nearest point in its fin (m)
                # Launch and Environment Details - run help(Environment) and help(Flight) for more information
                "rail length (m)": Env.rL,  # Launch rail length (m)
                "latitude": Env.lat, # Latitude of launch site
                "longitude": Env.lon, # Longitude of launch site
                #add more environment data here
                

                # Start time for each stage of flight
                "Phase 1 - Rail Launch": test_flight.flightPhases.list[0].t,
                "Phase 2 - Powered Flight": test_flight.flightPhases.list[1].t,
                "Apogee": test_flight.apogeeTime,     # Placing this in between for ease of use in Unity. Not counting
                                                          # it as its own phase, but it occurs between phases 2 & 3.
                "Phase 3 - Drogue Parachute Deployment": test_flight.flightPhases.list[2].t,
                "Phase 4 - Descent Under Drogue Parachute": test_flight.flightPhases.list[3].t,
                "Phase 5 - Main Parachute Deployment": test_flight.flightPhases.list[4].t,
                "Phase 6 - Descent Under Main Parachute": test_flight.flightPhases.list[5].t,
                "Phase 7 - Impact": test_flight.flightPhases.list[6].t,
                "No. Stages": len(test_flight.flightPhases.list),

                # Apogee & Impact Details
                "apogee X": test_flight.apogeeX,
                "apogee Y": test_flight.apogeeY,
                "apogee Z": test_flight.apogee, # i.e. altitude
                "x impact": test_flight.xImpact,
                "y impact": test_flight.yImpact,
                "impact velocity": test_flight.impactVelocity,
                #Variables added for V2_1
                "temperature": input_temp,
                "E wind start": east_wind[0],
                "E wind end": east_wind[1],
                "N wind start": north_wind[0],
                "N wind end": north_wind[1],
                "Burn out time": input_burnOut,
                "Nozzle Distance": input_Rocket_Nozzle,
                "Propellant": input_Rocket_Propellant
               # "wind direction": Env.windDirection[0], #return as a function

                
            }
            environmentID += 1
              #end the extra environment sims
              # Append the input data to the all_inputs.csv file. If this is the first simulation, add the csv headings
            include_heading= False
            if idCount == 1:
                include_heading = True
            #export_input_variables(analysis_parameters, dispersion_input_file, include_heading)
            export_input_variables(analysis_parameters2, dispersion_input_file2, include_heading) 



              #export_flight_data(analysis_parameters, test_flight, process_time() - start_time)    # Basic Export function
            
              
    
        except Exception as E:
            print(E)
            export_flight_error(analysis_parameters)
        
        # Analysing Flight Results
        #test_flight.allInfo()
        #dispersion_input_file.close()
        dispersion_input_file2.close()
        idCount += 1
# end of the motor for loop

####################################################################################
############################# Cleanup ##############################################
####################################################################################


## Print and save total time
#out.update(final_string)
#dispersion_input_file.write(final_string + "\n")
#dispersion_output_file.write(final_string + "\n")
#dispersion_error_file.write(final_string + "\n")

# Close files
#dispersion_input_file.close()
#dispersion_output_file.close()
#dispersion_error_file.close()

print("Simulations Completed")