#!/bin/bash

dotnet build;
if [ $? -ne 0 ]
then
    exit;
fi

cd ../app;
dotnet build;
if [ $? -ne 0 ]
then
    exit;
fi

dotnet run &
thread=$!
sleep 3;

cd ../SE2E;
dotnet test;

kill $!
