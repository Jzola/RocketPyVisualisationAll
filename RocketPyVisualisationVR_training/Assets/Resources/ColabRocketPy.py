{"nbformat":4,"nbformat_minor":0,"metadata":{"colab":{"provenance":[],"collapsed_sections":[],"authorship_tag":"ABX9TyN/1Fl4EBQ5y3BKUoYPw02/"},"kernelspec":{"name":"python3","display_name":"Python 3"},"language_info":{"name":"python"}},"cells":[{"cell_type":"markdown","source":[],"metadata":{"id":"HyYbtbbSZplH"}},{"cell_type":"code","execution_count":1,"metadata":{"colab":{"base_uri":"https://localhost:8080/"},"id":"YJathMYapB9G","executionInfo":{"status":"ok","timestamp":1666403414921,"user_tz":-630,"elapsed":22044,"user":{"displayName":"Luke G","userId":"17914055099655304430"}},"outputId":"1be0edc7-a6aa-490c-ad26-064ce7df6eab"},"outputs":[{"output_type":"stream","name":"stdout","text":["Mounted at /content/drive\n"]}],"source":["from google.colab import drive\n","drive.mount('/content/drive')"]},{"cell_type":"code","source":["%cd /content/drive/My\\ Drive/Rocket/RocketPy\n","\n","# List files to make sure we're in the expected directory.\n","# Your output will look different, showing your own Drive files here.\n","!ls"],"metadata":{"colab":{"base_uri":"https://localhost:8080/"},"id":"CdhDSyLmyJZW","executionInfo":{"status":"ok","timestamp":1666403417717,"user_tz":-630,"elapsed":900,"user":{"displayName":"Luke G","userId":"17914055099655304430"}},"outputId":"99077fd3-4394-44a0-b2ea-77adf1446e54"},"execution_count":2,"outputs":[{"output_type":"stream","name":"stdout","text":["/content/drive/My Drive/Rocket/RocketPy\n","CITATION.cff\t    CONTRIBUTING.md  LICENSE\trequirements_test.txt  setup.py\n","CODE_OF_CONDUCT.md  data\t     Makefile\trequirements.txt       tests\n","CODEOWNERS\t    docs\t     README.md\trocketpy\n"]}]},{"cell_type":"code","source":["#!pip install --upgrade --force-reinstall netCDF4\n","\n","!pip install numpy==1.21\n","#!pip install scipy==1.7.1\n","!pip install matplotlib>=3.0\n","#!pip install netCDF4==1.6.0\n","!pip install \"netCDF4<1.6.0\"\n","!pip install requests\n","!pip install pytz\n","!pip install simplekml\n","\n","\n","from rocketpy import Environment, SolidMotor, Rocket, Flight, Function\n","from time import process_time, perf_counter, time\n","from IPython.display import display\n","import datetime\n","#import numpy as np\n","import os\n","\n","\n","# Uncomment if crashing during weather api\n","import subprocess\n","import sys\n","#subprocess.check_call([sys.executable, \"-m\", \"pip\", \"install\", r\"netCDF4<1.6.0\"\"\"])\n","\n","%cd /content/drive/My\\ Drive/Rocket\n","!ls"],"metadata":{"colab":{"base_uri":"https://localhost:8080/"},"id":"oshjCNnqy7Et","outputId":"f4f51649-024f-41ed-9387-40e569ba3866","executionInfo":{"status":"ok","timestamp":1666403556270,"user_tz":-630,"elapsed":26371,"user":{"displayName":"Luke G","userId":"17914055099655304430"}}},"execution_count":4,"outputs":[{"output_type":"stream","name":"stdout","text":["Looking in indexes: https://pypi.org/simple, https://us-python.pkg.dev/colab-wheels/public/simple/\n","Requirement already satisfied: numpy==1.21 in /usr/local/lib/python3.7/dist-packages (1.21.0)\n","Looking in indexes: https://pypi.org/simple, https://us-python.pkg.dev/colab-wheels/public/simple/\n","Requirement already satisfied: netCDF4<1.6.0 in /usr/local/lib/python3.7/dist-packages (1.5.8)\n","Requirement already satisfied: numpy>=1.9 in /usr/local/lib/python3.7/dist-packages (from netCDF4<1.6.0) (1.21.0)\n","Requirement already satisfied: cftime in /usr/local/lib/python3.7/dist-packages (from netCDF4<1.6.0) (1.6.2)\n","Looking in indexes: https://pypi.org/simple, https://us-python.pkg.dev/colab-wheels/public/simple/\n","Requirement already satisfied: requests in /usr/local/lib/python3.7/dist-packages (2.23.0)\n","Requirement already satisfied: certifi>=2017.4.17 in /usr/local/lib/python3.7/dist-packages (from requests) (2022.9.24)\n","Requirement already satisfied: idna<3,>=2.5 in /usr/local/lib/python3.7/dist-packages (from requests) (2.10)\n","Requirement already satisfied: chardet<4,>=3.0.2 in /usr/local/lib/python3.7/dist-packages (from requests) (3.0.4)\n","Requirement already satisfied: urllib3!=1.25.0,!=1.25.1,<1.26,>=1.21.1 in /usr/local/lib/python3.7/dist-packages (from requests) (1.24.3)\n","Looking in indexes: https://pypi.org/simple, https://us-python.pkg.dev/colab-wheels/public/simple/\n","Requirement already satisfied: pytz in /usr/local/lib/python3.7/dist-packages (2022.4)\n","Looking in indexes: https://pypi.org/simple, https://us-python.pkg.dev/colab-wheels/public/simple/\n","Collecting simplekml\n","  Downloading simplekml-1.3.6.tar.gz (52 kB)\n","\u001b[K     |████████████████████████████████| 52 kB 1.2 MB/s \n","\u001b[?25hBuilding wheels for collected packages: simplekml\n","  Building wheel for simplekml (setup.py) ... \u001b[?25l\u001b[?25hdone\n","  Created wheel for simplekml: filename=simplekml-1.3.6-py3-none-any.whl size=65876 sha256=ea8040173c08bdd40b3148c3f394b9ac8aa3aafcb34f011cff4243e6fc78ce45\n","  Stored in directory: /root/.cache/pip/wheels/5c/ec/e6/10af1a1fb29ffca95151d4c886d6e06fc309c68f46519892de\n","Successfully built simplekml\n","Installing collected packages: simplekml\n","Successfully installed simplekml-1.3.6\n","/content/drive/My Drive/Rocket\n","'=3.0'\t\t\t      'multiple sims.py'       rocktest.py\n"," ColabRocketPy.py\t       RocketPy\t\t       searchd.py\n"," dispersion_analysis_inputs    rocketSimulation.py     simulation_files\n"," dispersion_analysis_outputs  'rocktest ORIGINAL.py'\n"]}]},{"cell_type":"code","source":["#!/usr/bin/env python3\n","# -*- coding: utf-8 -*-\n","\"\"\"\n","Created on Sun Mar 13 14:46:22 2022\n","\n","@author: lukegerschwitz\n","\"\"\"\n","\n","#from rocketpy import Environment, SolidMotor, Rocket, Flight, Function\n","#from time import process_time, perf_counter, time\n","#from IPython.display import display\n","#import datetime\n","#import numpy as np\n","#import os\n","\n","\n","# Uncomment if crashing during weather api\n","#import subprocess\n","#import sys\n","#subprocess.check_call([sys.executable, \"-m\", \"pip\", \"install\", r\"netCDF4<1.6.0\"\"\"])\n","\n","\n","# Switch to current directory\n","#abspath = os.path.abspath(__file__)\n","#dname = os.path.dirname(abspath)\n","#os.chdir(dname)\n","\n","####################################################################################\n","############################ Exporting Simulations #################################\n","####################################################################################\n","\n","\n","# Appends a current simulations input variables to a csv file.\n","def export_input_variables(flight_settings, file, head):\n","    \n","    inputs = \"\"\n","    first = True\n","    \n","    if head == True:\n","        for key in flight_settings:\n","            if first == True:\n","                inputs = str(key)\n","                first = False\n","            else:\n","                inputs += \",\" + str(key)\n","    first = True\n","    \n","    for key in flight_settings:\n","        if first == True:\n","            inputs += \"\\n\" + str(flight_settings[key])\n","            first = False\n","        else:\n","            inputs += \",\" + str(flight_settings[key])  \n","    file.write(inputs)\n","\n","def export_flight_error(flight_setting):\n","    num=1\n","    #dispersion_error_file.write(str(flight_setting) + \"\\n\")\n","\n","\n","\n","####################################################################################\n","############################ Create Files/Set Number of Sims #######################\n","####################################################################################\n","\n","# Basic analysis info\n","filename = \"dispersion_analysis_outputs/\"\n","\n","# # Create data files for inputs, outputs and error logging\n","# dispersion_error_file = open(str(filename) + \"disp_errors.txt\", \"w\")\n","# dispersion_input_file = open(str(filename) + \"disp_inputs.txt\", \"w\")\n","# dispersion_output_file = open(str(filename) + \"disp_outputs.txt\", \"w\")\n","\n","####################################################################################\n","############################ Simulation Settings ###################################\n","####################################################################################\n","\n","\n","iterationsPerMotor = 6\n","step = 1\n","startMass = 20\n","idCount = 1\n","\n","# Hypertek motor does not work with two parachutes\n","motors = [\"Cesaroni_M1300.eng\", \"Cesaroni_M1400.eng\", \n","          \"Cesaroni_M1540.eng\", \"Cesaroni_M1670.eng\", \"Cesaroni_M3100.eng\"]#, \"Cesaroni_7450M2505-P.eng\", # \"Hypertek_J115.eng\"]\n","\n","# Environmental data \n","Env = Environment(\n","    railLength=5.2,\n","    latitude=32.990254,\n","    longitude=-106.974998,\n","    elevation=1400\n",")\n","\n","# Set the date and time\n","tomorrow = datetime.date.today() + datetime.timedelta(days=1)\n","Env.setDate((tomorrow.year, tomorrow.month, tomorrow.day, 12))  # Hour given in UTC time\n","# Retrieve forecast data from GFS\n","# https://www.emc.ncep.noaa.gov/emc/pages/numerical_forecast_systems/gfs.php\n","Env.setAtmosphericModel(type=\"Forecast\", file=\"GFS\")\n","\n","\n","# Loop each motor \n","for motor in motors:\n","\n","    # Loop each mass for each motor\n","    for iter in range(1, iterationsPerMotor+1):\n","        \n","        # Basic analysis info\n","        filename = \"dispersion_analysis_outputs/\"\n","        #number_of_simulations = 1\n","        \n","        # Create data files for inputs, outputs and error logging\n","        #dispersion_error_file = open(str(filename) + \"disp_errors.txt\", \"w\")\n","        dispersion_input_file = open(str(filename) + \"all_inputs.csv\", \"a\")\n","        #dispersion_output_file = open(str(filename) + \"disp_outputs.txt\", \"w\")\n","    \n","        \n","        # Rocket Motor\n","        Pro75M1670 = SolidMotor(\n","            thrustSource=\"RocketPy/data/motors/\"+motor,\n","            burnOut=3.9,\n","            grainNumber=5,\n","            grainSeparation=5 / 1000,\n","            grainDensity=1815,\n","            grainOuterRadius=33 / 1000,\n","            grainInitialInnerRadius=15 / 1000,\n","            grainInitialHeight=120 / 1000,\n","            nozzleRadius=33 / 1000,\n","            throatRadius=11 / 1000,\n","            interpolationMethod=\"linear\",\n","        )\n","        \n","        # Rocket\n","        Calisto = Rocket(\n","            motor=Pro75M1670,\n","            radius=127 / 2000,\n","            mass=(startMass+(step*iter)),\n","            inertiaI=6.60,\n","            inertiaZ=0.0351,\n","            distanceRocketNozzle=-1.255,\n","            distanceRocketPropellant=-0.85704,\n","            powerOffDrag=\"RocketPy/data/calisto/powerOffDragCurve.csv\",\n","            powerOnDrag=\"RocketPy/data/calisto/powerOnDragCurve.csv\",\n","        )\n","        Calisto.setRailButtons([0.2, -0.5])\n","        \n","        # Aerodynamic Forces\n","        noseLen = 0.55829\n","        noseDisToCM = 0.71971\n","        NoseCone = Calisto.addNose(length=noseLen, kind=\"vonKarman\", distanceToCM=noseDisToCM)\n","        fSpan = 0.100\n","        fRootChord = 0.120\n","        fTipChord = 0.040\n","        fDistanceToCM=-1.04956\n","        FinSet = Calisto.addFins(\n","            4, span=fSpan, rootChord=fRootChord, tipChord=fTipChord, distanceToCM=fDistanceToCM\n","        )\n","        Tail = Calisto.addTail(\n","            topRadius=0.0635, bottomRadius=0.0435, length=0.060, distanceToCM=-1.194656\n","        )\n","        \n","        # Parachute Code\n","        def drogueTrigger(p, y):\n","            # p = pressure\n","            # y = [x, y, z, vx, vy, vz, e0, e1, e2, e3, w1, w2, w3]\n","            # activate drogue when vz < 0 m/s.\n","            return True if y[5] < 0 else False\n","        \n","        def mainTrigger(p, y):\n","            # p = pressure\n","            # y = [x, y, z, vx, vy, vz, e0, e1, e2, e3, w1, w2, w3]\n","            # activate main when vz < 0 m/s and z < 800 m + 1471m.\n","            return True if y[5] < 0 and y[2] < (800 + 1471) else False # 1471 for surface elevation of launch site\n","        \n","            # Remove extra parachutes just in case\n","            Calisto.parachutes.remove(Drogue)\n","            Calisto.parachutes.remove(Main)\n","        \n","        Main = Calisto.addParachute(\n","            \"Main\",\n","            CdS=10.0,\n","            trigger=mainTrigger,\n","            samplingRate=105,\n","            lag=1.5,\n","            noise=(0, 8.3, 0.5),\n","        )\n","\n","        Drogue = Calisto.addParachute(\n","            \"Drogue\",\n","            CdS=1.0,\n","            trigger=drogueTrigger,\n","            samplingRate=105,\n","            lag=1.5,\n","            noise=(0, 8.3, 0.5),\n","        )\n","        \n","     \n","        ####################################################################################\n","        ############################# Run Simulation #######################################\n","        ####################################################################################\n","        \n","        \n","        out = display(\"Commencing Simulation \" + str(idCount))\n","        \n","        # Simulate flight\n","        try:\n","            flightInclination = 80\n","            flightHeading = 90\n","            test_flight = Flight(rocket=Calisto, environment=Env, inclination=flightInclination, heading=flightHeading)\n","            \n","            # Custom export function from GitHub\n","            test_flight.exportData(\"simulation_files/\" + str(idCount) + \".csv\",\n","                                    \"x\",\n","                                    \"y\",\n","                                    \"z\",\n","                                    \"vx\",\n","                                    \"vy\",\n","                                    \"vz\",\n","                                    \"e0\",\n","                                    \"e1\",\n","                                    \"e2\",\n","                                    \"e3\",\n","                                    \"w1\",\n","                                    \"w2\",\n","                                    \"w3\",\n","                                    timeStep=0.6)\n","\n","            ####################################################################################\n","            ############################# Prep input variables for export ######################\n","            ####################################################################################\n","            \n","            analysis_parameters = {\n","                # id to link input data with trajectory data\n","                \"id\": idCount,\n","                # Mass Details\n","                \"rocket mass (kg)\": (Calisto.mass),  # Rocket's dry mass (kg) and its uncertainty (standard deviation)\n","                \"time (s)\": test_flight.tFinal, # Flight duration in seconds\n","                \"motor type\": motor.split(\".\", 1)[0],\n","                # Propulsion Details - run help(SolidMotor) for more information\n","                \"impulse (N*s)\": (Calisto.motor.totalImpulse),  # Motor total impulse (N*s)\n","                \"burn out (s)\": (Calisto.motor.burnOutTime),  # Motor burn out time (s)\n","                \"nozzle radius (m)\": (Calisto.motor.nozzleRadius),  # Motor's nozzle radius (m)\n","                \"throat radius (m)\": (Calisto.motor.throatRadius),  # Motor's nozzle throat radius (m)\n","                # Aerodynamic Details - run help(Rocket) for more information\n","                \"radius (kg*m^2)\": (Calisto.radius),  # Rocket's radius (kg*m^2)\n","                \"distance rocket nozzle (m)\": (Calisto.distanceRocketNozzle),  # Distance between rocket's center of dry mass and nozzle exit plane (m) (negative)\n","                \"distance rocket propellant (m)\": (Calisto.distanceRocketPropellant),  # Distance between rocket's center of dry mass and and center of propellant mass (m) (negative)\n","                \"nose length (m)\": noseLen,  # Rocket's nose cone length (m)\n","                \"nose distance to cm (m)\": noseDisToCM,  # Axial distance between rocket's center of dry mass and nearest point in its nose cone (m)\n","                \"fin span (m)\": fSpan,  # Fin span (m)\n","                \"fin distance to cm (m)\": fDistanceToCM,  # Axial distance between rocket's center of dry mass and nearest point in its fin (m)\n","                # Launch and Environment Details - run help(Environment) and help(Flight) for more information\n","                \"rail length (m)\": Env.rL,  # Launch rail length (m)\n","                \"latitude\": Env.lat, # Latitude of launch site\n","                \"longitude\": Env.lon, # Longitude of launch site\n","\n","                # Start time for each stage of flight\n","                \"Phase 1 - Rail Launch\": test_flight.flightPhases.list[0].t,\n","                \"Phase 2 - Powered Flight\": test_flight.flightPhases.list[1].t,\n","                \"Apogee\": test_flight.apogeeTime,     # Placing this in between for ease of use in Unity. Not counting\n","                                                          # it as its own phase, but it occurs between phases 2 & 3.\n","                \"Phase 3 - Drogue Parachute Deployment\": test_flight.flightPhases.list[2].t,\n","                \"Phase 4 - Descent Under Drogue Parachute\": test_flight.flightPhases.list[3].t,\n","                \"Phase 5 - Main Parachute Deployment\": test_flight.flightPhases.list[4].t,\n","                \"Phase 6 - Descent Under Main Parachute\": test_flight.flightPhases.list[5].t,\n","                \"Phase 7 - Impact\": test_flight.flightPhases.list[6].t,\n","                \"No. Stages\": len(test_flight.flightPhases.list),\n","\n","                # Apogee & Impact Details\n","                \"apogee X\": test_flight.apogeeX,\n","                \"apogee Y\": test_flight.apogeeY,\n","                \"apogee Z\": test_flight.apogee, # i.e. altitude\n","                \"x impact\": test_flight.xImpact,\n","                \"y impact\": test_flight.yImpact,\n","                \"impact velocity\": test_flight.impactVelocity,\n","            }\n","            \n","            # Append the input data to the all_inputs.csv file. If this is the first simulation, add the csv headings\n","            include_heading= False\n","            if idCount == 1:\n","                include_heading = True\n","            export_input_variables(analysis_parameters, dispersion_input_file, include_heading)\n","    \n","            #export_flight_data(analysis_parameters, test_flight, process_time() - start_time)    # Basic Export function\n","            \n","    \n","        except Exception as E:\n","            print(E)\n","            export_flight_error(analysis_parameters)\n","        \n","        # Analysing Flight Results\n","        #test_flight.allInfo()\n","        dispersion_input_file.close()\n","        idCount += 1\n","####################################################################################\n","############################# Cleanup ##############################################\n","####################################################################################\n","\n","\n","## Print and save total time\n","#out.update(final_string)\n","#dispersion_input_file.write(final_string + \"\\n\")\n","#dispersion_output_file.write(final_string + \"\\n\")\n","#dispersion_error_file.write(final_string + \"\\n\")\n","\n","# Close files\n","#dispersion_input_file.close()\n","#dispersion_output_file.close()\n","#dispersion_error_file.close()\n","\n","print(\"Simulations Completed\")"],"metadata":{"colab":{"base_uri":"https://localhost:8080/","height":586},"id":"BLhv01G3qwYB","executionInfo":{"status":"ok","timestamp":1666403912861,"user_tz":-630,"elapsed":93868,"user":{"displayName":"Luke G","userId":"17914055099655304430"}},"outputId":"215d6448-fbf2-4064-8d40-5a9dde426403"},"execution_count":6,"outputs":[{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 1'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 2'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 3'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 4'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 5'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 6'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 7'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 8'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 9'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 10'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 11'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 12'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 13'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 14'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 15'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 16'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 17'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 18'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 19'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 20'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 21'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 22'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 23'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 24'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 25'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 26'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 27'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 28'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 29'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"display_data","data":{"text/plain":["'Commencing Simulation 30'"],"application/vnd.google.colaboratory.intrinsic+json":{"type":"string"}},"metadata":{}},{"output_type":"stream","name":"stdout","text":["Simulations Completed\n"]}]}]}