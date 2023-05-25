#!/bin/bash

#Scene
cp -rp plugin/plugin-srcs/Assets/Scenes/Drone* drone/Assets/Scenes/

#Scripts
cp -rp plugin/plugin-srcs/Assets/Scripts/PluggableAsset/Assets/Robot/Drone/* \
    drone/Assets/Scripts/PluggableAsset/Assets/Robot/Drone/

#Prefab
cp -rp plugin/plugin-srcs/Assets/Prefab/Robots/Drone* \
    drone/Assets/Prefab/Robots/
