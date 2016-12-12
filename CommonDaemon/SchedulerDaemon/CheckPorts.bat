REM this code to check if the Ports have been holded.

netstat -ano | findstr ":9200" 
netstat -ano | findstr ":5601" 

echo "please kill the pid by: tskill"