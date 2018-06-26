#!/bin/bash

APP_NAME="TFA-Bot.exe"                                                                                                                                                                                                                                                         
APP_DIR="/app/TFA-Bot/TFA-Bot/bin/Release" 
BUILD_DIR="/app/TFA-Bot"

[ -z "$BOTURL" ] && echo "BOTURL Google Spreadsheet URL missing" && exit 1;

function build
{
    cd $BUILD_DIR
    git pull
    mono ../nuget.exe restore TFA-Bot.sln    
    msbuild -property:Configuration=Release TFA-Bot.sln
}


exitcode=-1
until [ $exitcode -eq 0 ]
do
        startdate="$(date +%s)"
        cd $APP_DIR
        mono $APP_NAME --version=$(git describe --long) --versiondate=$(git log -1 --format=%cd)
        exitcode=$?
        enddate="$(date +%s)"
        
        echo "EXIT CODE = $exitcode"
        
        elapsed_seconds="$(expr $enddate - $startdate)"
        echo "Elapsed seconds $elapsed_seconds"
        
        if [ $exitcode -eq 2 ] #Restart
        then
          echo "RESTART"
        elif [ $exitcode -eq 4 ] #Previous version
        then
          echo "PREVIOUS VERSION"
          cp -fv $APP_NAME_previous $APP_NAME
        elif [ $exitcode -eq 3 ] #Update
        then
          echo "SOFTWARE UPDATE"
          cp -fv $APP_NAME $APP_NAME_previous
          build
        elif [ $exitcode -eq 0 ] #Shutdown
        then
          echo "SHUTDOWN"
        fi
        
        if [ $elapsed_seconds -lt 30 ]  #been running for less than 30 seconds
        then
                sleep 10  # delay to protect against eating the CPU resourses with infinate loop
        fi

done
echo "BASH: terminate $exitcode"
