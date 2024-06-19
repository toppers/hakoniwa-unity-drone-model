@echo off
setlocal

echo Copying drone_config.json to plugin-srcs
copy /Y drone\drone_config.json plugin\plugin-srcs\

echo Copying hakoniwa_path.json to plugin-srcs
copy /Y drone\hakoniwa_path.json plugin\plugin-srcs\

echo Copying Assets to plugin-srcs\Assets
xcopy /E /I /Y drone\Assets\* plugin\plugin-srcs\Assets\

echo Copying hakoniwa-base to hakoniwa-base
xcopy /E /I /Y drone\hakoniwa-base\* hakoniwa-base\

cd plugin

echo Running install.bat
set CALLER=1
call install.bat
set CALLER=

cd ..

echo Copying ros_types to plugin-srcs\ros_types
xcopy /E /I /Y drone\ros_types\* plugin\plugin-srcs\ros_types\

echo Process completed successfully.
pause
