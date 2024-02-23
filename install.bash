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

cp drone/hakoniwa_path.json plugin/plugin-srcs/
cp -rp drone/Assets/* plugin/plugin-srcs/Assets/
cp -rp drone/hakoniwa-base/* hakoniwa-base/

cd plugin

bash -x install.bash ${OPT_WIN}

cd ..
cp -rp drone/ros_types/* plugin/plugin-srcs/ros_types/

if [ $OS_TYPE = "Darwin" ]
then
	cp /usr/local/lib/hakoniwa/libshakoc.dylib plugin/plugin-srcs/Assets/Plugin/Libs/libshakoc.dylib
fi

