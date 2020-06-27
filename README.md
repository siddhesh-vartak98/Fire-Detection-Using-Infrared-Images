# Fire-Detection-Using-Infrared-Images
## How to start: open file "cam_aforge1.csproj" in visual studio. 
### Note: File to be excreted.(aforge.imaafing->aforge->imaging->filter.zip)

The project is fire detection using the infrared technologies. We use  aforge library of image processing technology . The software application is getting the input video and then segment into images. After that these images then filter, colours detection of fire on infrared based include flame of fire detect , motion detection gets high accuracy to detect whether the fire is present or not. In both method if fire detected then an alert message and sound is generated. 

The project has two input method through GUI. 
1. Real time (camera)
 - The real time system , extracts frames from camera or video recording device in every 2 seconds, it provides continuous monitoring. And send to system. 
 -Note : If you have to connect external camera device then Aforge library code has to install. 
 2. Store ir video 
  - we have given input IR video but it must be in .wmv format. 
 Future Scope: GSM can be conneted. 
