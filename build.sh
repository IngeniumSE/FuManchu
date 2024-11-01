#!/bin/bash

# Install dotnet tools
cd ./build
dotnet tool restore

# Run our build app
cd ./apps/Build
dotnet run $@
