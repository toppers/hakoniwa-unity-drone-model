#!/bin/bash

#Scene
cp -rp plugin/plugin-srcs/Assets/Scenes/Drone* drone/Assets/Scenes/
cp plugin/plugin-srcs/Assets/Scenes/Hakoniwa.unity* drone/Assets/Scenes/
cp plugin/plugin-srcs/Assets/Scenes/MultiDrones.unity* drone/Assets/Scenes/
cp plugin/plugin-srcs/Assets/Scenes/GyroDemo.unity* drone/Assets/Scenes/

#Scripts
cp -rp plugin/plugin-srcs/Assets/Scripts/PluggableAsset/Assets/Robot/Drone/* \
    drone/Assets/Scripts/PluggableAsset/Assets/Robot/Drone/
cp -rp plugin/plugin-srcs/Assets/Scripts/PluggableAsset/Assets/Environment/* \
    drone/Assets/Scripts/PluggableAsset/Assets/Environment/

#Prefab
cp -rp plugin/plugin-srcs/Assets/Prefab/Robots/Drone* \
    drone/Assets/Prefab/Robots/
cp -rp plugin/plugin-srcs/Assets/Prefab/Robots/Env* \
    drone/Assets/Prefab/Robots/

#Model
cp -rp plugin/plugin-srcs/Assets/Model/Hakoniwa/Robots/Drone/* \
    drone/Assets/Model/Hakoniwa/Robots/Drone/


#hakoniwa-base
cp hakoniwa-base/workspace/dev/ai/sample_drone.py drone/hakoniwa-base/workspace/dev/ai/
cp hakoniwa-base/workspace/dev/ai/drone_sensor.py drone/hakoniwa-base/workspace/dev/ai/
cp hakoniwa-base/workspace/runtime/asset_def.txt drone/hakoniwa-base/workspace/runtime/
