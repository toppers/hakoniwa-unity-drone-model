#!/bin/bash

cp -rp drone/Assets/* plugin/plugin-srcs/Assets/
cp -rp drone/hakoniwa-base/* hakoniwa-base/

cd plugin

bash install.bash
