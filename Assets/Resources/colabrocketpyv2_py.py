# -*- coding: utf-8 -*-
"""ColabRocketPyV2.py

Automatically generated by Colaboratory.

Original file is located at
    https://colab.research.google.com/drive/1PnZ-QKRtHjAUOZz4Xil0JY0bm9HZzIhw

Access to google drive will need to be permitted.
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
import os


# Uncomment if crashing during weather api
import subprocess
import sys
#subprocess.check_call([sys.executable, "-m", "pip", "install", r"netCDF4<1.6.0"""])

# %cd /content/drive/My\ Drive/Rocket
!ls

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
filename = "dispersion_analysis_outputs/"

# # Create data files for inputs, outputs and error logging
# dispersion_error_file = open(str(filename) + "disp_errors.txt", "w")
# dispersion_input_file = open(str(filename) + "disp_inputs.txt", "w")
# dispersion_output_file = open(str(filename) + "disp_outputs.txt", "w")

####################################################################################
############################ Simulation Settings ###################################
####################################################################################


iterationsPerMotor = 6
step = 1
startMass = 50 #changed this line for more mass analysis (Original value 20)
idCount = 1

# Hypertek motor does not work with two parachutes
motors = ["Cesaroni_M1300.eng", "Cesaroni_M1400.eng", 
          "Cesaroni_M1540.eng", "Cesaroni_M1670.eng", "Cesaroni_M3100.eng"]#, "Cesaroni_7450M2505-P.eng", # "Hypertek_J115.eng"]

# Environmental data 
Env = Environment(
    railLength=5.2,
    latitude=32.990254,
    longitude=-106.974998,
    elevation=1400
)

# Set the date and time
tomorrow = datetime.date.today() + datetime.timedelta(days=1)
Env.setDate((tomorrow.year, tomorrow.month, tomorrow.day, 12))  # Hour given in UTC time
# Retrieve forecast data from GFS
# https://www.emc.ncep.noaa.gov/emc/pages/numerical_forecast_systems/gfs.php
Env.setAtmosphericModel(type="Forecast", file="GFS")


# Loop each motor 
for motor in motors:

    # Loop each mass for each motor
    for iter in range(1, iterationsPerMotor+1):
        
        # Basic analysis info
        filename = "dispersion_analysis_outputs/" #change the folder for future code
        #number_of_simulations = 1
        
        # Create data files for inputs, outputs and error logging
        #dispersion_error_file = open(str(filename) + "disp_errors.txt", "w")
        dispersion_input_file = open(str(filename) + "all_inputs.csv", "a")
        #dispersion_output_file = open(str(filename) + "disp_outputs.txt", "w")
    
        
        # Rocket Motor
        Pro75M1670 = SolidMotor(
            thrustSource="RocketPy/data/motors/"+motor,
            burnOut=3.9,
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
        
        # Rocket
        Calisto = Rocket(
            motor=Pro75M1670,
            radius=127 / 2000,
            mass=(startMass+(step*iter)),  #this line changes the mass
            inertiaI=6.60,
            inertiaZ=0.0351,
            distanceRocketNozzle=-1.255,
            distanceRocketPropellant=-0.85704,
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
            test_flight = Flight(rocket=Calisto, environment=Env, inclination=flightInclination, heading=flightHeading)
            
            # Custom export function from GitHub
            test_flight.exportData("simulation_files/" + str(idCount) + ".csv",
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
            
            # Append the input data to the all_inputs.csv file. If this is the first simulation, add the csv headings
            include_heading= False
            if idCount == 1:
                include_heading = True
            export_input_variables(analysis_parameters, dispersion_input_file, include_heading)
    
            #export_flight_data(analysis_parameters, test_flight, process_time() - start_time)    # Basic Export function
            
    
        except Exception as E:
            print(E)
            export_flight_error(analysis_parameters)
        
        # Analysing Flight Results
        #test_flight.allInfo()
        dispersion_input_file.close()
        idCount += 1
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