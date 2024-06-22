#!/bin/bash

OPT_WIN=$1

OS_TYPE=`uname`
if [ $OS_TYPE = "Darwin" ]
then
	if [ ! -f /usr/local/lib/hakoniwa/libshakoc.dylib ]
	then
		echo "ERROR:  /usr/local/lib/hakoniwa/libshakoc.dylib is not installed"
		exit 1
	fi
fi
TB3_VERSION=v2.0.0
wget https://github.com/toppers/hakoniwa-unity-tb3model/releases/download/${TB3_VERSION}/tb3_models.zip
unzip tb3_models.zip
bash -x install_tb3.bash
rm -f tb3_models.zip

cp drone/drone_config.json plugin/plugin-srcs/
cp drone/hakoniwa_path.json plugin/plugin-srcs/
cp -rp drone/Assets/* plugin/plugin-srcs/Assets/

cd plugin

bash -x install.bash ${OPT_WIN}

cd ..
cp -rp drone/ros_types/* plugin/plugin-srcs/ros_types/

if [ $OS_TYPE = "Darwin" ]
then
	cp /usr/local/lib/hakoniwa/libshakoc.dylib plugin/plugin-srcs/Assets/Plugin/Libs/libshakoc.dylib
fi

