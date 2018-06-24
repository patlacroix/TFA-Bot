#!/bin/bash

APP_NAME="TFA-Bot.exe"                                                                                                                                                                                                                                                         
APP_DIR="\app\TFA-Bot\TFA-Bot\bin\Release" 
BUILD_DIR="\app\TFA-Bot"
MONOPATH=/usr/bin/mono


build()
{
    cd $BUILD_DIR
    git pull
    xbuild //property:Configuration=Release TFA-Bot.sln
}


exitcode=0
until [ $exitcode -eq 9 ]
do
        startdate="$(date +%s)"
        cd $APP_DIR
        $MONOPATH $APP_NAME
        exitcode=$?
        enddate="$(date +%s)"
        
        echo "EXIT CODE = $exitcode"
        
        elapsed_seconds="$(expr $enddate - $startdate)"
        echo "Elapsed seconds $elapsed_seconds"
        
        if [ $exitcode -eq 6 ] #Restart
        then
          echo "RESTART"
        elif [ $exitcode -eq 7 ] #Previous version
        then
          echo "PREVIOUS VERSION"
          cp -fv $APP_NAME_previous $APP_NAME
        elif [ $exitcode -eq 8 ] #Update
        then
          echo "SOFTWARE UPDATE"
          cp -fv $APP_NAME $APP_NAME_previous
          build()
        elif [ $exitcode -eq 9 ] #Shutdown
        then
          echo "SHUTDOWN"
        fi
        
        if [ $elapsed_seconds -lt 30 ]  #been running for less than 30 seconds
        then
                sleep 5  # delay to protect against eating the CPU resourses with infinate loop
        fi

        
done
echo "BASH: terminate $exitcode"
