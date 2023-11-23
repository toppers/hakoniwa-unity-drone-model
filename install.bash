#!/bin/bash

cp drone/hakoniwa_path.json plugin/plugin-srcs/
cp -rp drone/Assets/* plugin/plugin-srcs/Assets/
cp -rp drone/hakoniwa-base/* hakoniwa-base/

cd plugin

bash install.bash

cd ..
cp -rp drone/ros_types/* plugin/plugin-srcs/ros_types/
