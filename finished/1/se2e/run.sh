#!/bin/bash

# Build the test project
dotnet build;
if [ $? -ne 0 ]
then
    exit;
fi

pushd .;
cd ../api;
dotnet build;
if [ $? -ne 0 ]
then
    popd;
    exit;
fi

dotnet run &
thread=$!
sleep 3;

popd;
dotnet test;

kill $!;
