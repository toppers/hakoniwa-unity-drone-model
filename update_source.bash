#!/bin/bash

#Scene
cp -rp plugin/plugin-srcs/Assets/Scenes/Drone* drone/Assets/Scenes/

#Scripts
cp plugin/plugin-srcs/Assets/Scripts/PluggableAsset/Assets/Robot/Drone/Hobber* \
    drone/Assets/Scripts/PluggableAsset/Assets/Robot/Drone/

#Prefab
cp plugin/plugin-srcs/Assets/Prefab/Robots/Drone.prefab* \
    drone/Assets/Prefab/Robots/