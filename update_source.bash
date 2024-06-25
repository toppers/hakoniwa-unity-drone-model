#!/bin/bash

#Scene
cp plugin/plugin-srcs/Assets/Scenes/Hakoniwa.unity* drone/Assets/Scenes/
cp plugin/plugin-srcs/Assets/Scenes/MultiDrones.unity* drone/Assets/Scenes/
cp plugin/plugin-srcs/Assets/Scenes/GyroDemo.unity* drone/Assets/Scenes/
cp plugin/plugin-srcs/Assets/Scenes/ApiDemo.unity* drone/Assets/Scenes/
cp plugin/plugin-srcs/Assets/Scenes/DigitalTwin.unity* drone/Assets/Scenes/
cp plugin/plugin-srcs/Assets/Scenes/DigitalTwin/* drone/Assets/Scenes/DigitalTwin/

#Scripts
cp -rp plugin/plugin-srcs/Assets/Scripts/PluggableAsset/Assets/Robot/Drone/* \
    drone/Assets/Scripts/PluggableAsset/Assets/Robot/Drone/
cp -rp plugin/plugin-srcs/Assets/Scripts/PluggableAsset/Assets/Environment/* \
    drone/Assets/Scripts/PluggableAsset/Assets/Environment/
cp -rp plugin/plugin-srcs/Assets/Plugin/src/Unity/AR/Assets/VirtualPlayerSignalController.cs* \
    drone/Assets/Plugin/src/Unity/AR/Assets/

#Prefab
cp -rp plugin/plugin-srcs/Assets/Prefab/Robots/Drone* \
    drone/Assets/Prefab/Robots/
cp -rp plugin/plugin-srcs/Assets/Prefab/Robots/Baggage* \
    drone/Assets/Prefab/Robots/
cp -rp plugin/plugin-srcs/Assets/Prefab/Robots/Env* \
    drone/Assets/Prefab/Robots/
cp -rp plugin/plugin-srcs/Assets/Prefab/Robots/Transporter* \
    drone/Assets/Prefab/Robots/
cp -rp plugin/plugin-srcs/Assets/Prefab/Robots/LiDAR* \
    drone/Assets/Prefab/Robots/
cp -rp plugin/plugin-srcs/Assets/Prefab/GUI/Drone* \
    drone/Assets/Prefab/GUI/
cp -rp plugin/plugin-srcs/Assets/Prefab/Robots/AR/Drone/* \
    drone/Assets/Prefab/Robots/AR/Drone/
cp -rp plugin/plugin-srcs/Assets/Prefab/Robots/TB3RoboAvatar* \
    drone/Assets/Prefab/Robots/
cp -rp plugin/plugin-srcs/Assets/Prefab/Robots/AR/VirtualSignalPlayer.prefab* \
    drone/Assets/Prefab/Robots/AR/

#Model
cp -rp plugin/plugin-srcs/Assets/Model/Hakoniwa/Robots/Drone/* \
    drone/Assets/Model/Hakoniwa/Robots/Drone/

#drone config
cp plugin/plugin-srcs/drone_config.json drone/
