English ｜ [日本語](README-ja.md)

# 🚨【Important Notice】🚨  
This repository was originally developed as the Unity-based visualization model for the Hakoniwa Drone Simulator.  
However, development has now **fully transitioned** to its successor:  
[`hakoniwa-unity-drone`](https://github.com/hakoniwalab/hakoniwa-unity-drone).

This repository is preserved **for archival purposes only**, and **no further updates or support will be provided**.  
For all new projects or references, please use the following repository:

👉 https://github.com/hakoniwalab/hakoniwa-unity-drone

----

This repository contains a drone simulator created with Unity that is compatible with PX4.

![image](images/tojinbo.png)

The simulator can be used for real-time drone flight simulation and testing flight control algorithms.

For those who want to use the Hakoniwa Drone Simulator on native Windows, please refer to [this](README-win.md).

# Table of Contents

- [Hakoniwa Drone Model](#hakoniwa-drone-model)
- [Tojinbo 3D Model](#tojinbo-3d-model)
- [Integration with PX4](#integration-with-px4)
- [Supported Environments](#supported-environments)
  - [Mac Environment](#mac-environment)
  - [Windows Environment](#windows-environment)
- [Usage Instructions](#usage-instructions)
  - [Using the Unity Application](#using-the-unity-application)
  - [Using the Unity Editor](#using-the-unity-editor)
- [Community and Support](#community-and-support)
- [Repository Contents and License](#repository-contents-and-license)
- [Contribution Guidelines](#contribution-guidelines)

# Hakoniwa Drone Model

The drone model created in Unity is an original Hakoniwa model and is a quadcopter type (as shown below).

![image](https://github.com/toppers/hakoniwa-unity-drone-model/assets/164193/201fce8f-5f8c-43de-9d19-26870348624f)

Unity assets of the Hakoniwa Drone Model H1 are based on data provided by Mr. Hodaka Nakamura of Ida B. Wells-Barnett High School. We want to express our deepest gratitude for his awesome contribution :D

# Tojinbo 3D Model

The Tojinbo 3D model was created with the generous support of the following individuals:

- **Mr. Ueno from Virtual Sky**  
  - Provided 360-degree drone footage of the Tojinbo landscape.  
  - For more details, please visit the [Virtual Sky official website](https://www.vsky2019.com/).

- **Mr. Komori from Komori Laboratory**  
  - Converted the captured data into a 3D model.  
  - For more details, please visit the [Komori Laboratory official website](https://komori-lab.com/).

This model could not have been completed without their invaluable assistance. We deeply appreciate their contributions to Hakoniwa's OSS initiatives.

For instructions on integrating the Tojinbo 3D model into the Hakoniwa Drone Simulator, please refer to [this guide](README-tojinbo.md).


# Integration with PX4

To integrate this drone simulator with PX4, the hakoniwa-px4sim repository is required. For detailed instructions on how to integrate with PX4, please read the documentation at the following link:

https://github.com/toppers/hakoniwa-px4sim

# Supported Environments

- [X] Intel-based Mac
- [X] Arm-based Mac
- [X] Ubuntu
- [X] Windows 10/11

# Usage Instructions

The Hakoniwa Drone Model for Unity can be used in two ways:

1. Using the Unity Application
2. Using the Unity Editor

For the first method, there is no setup hassle as you use a pre-built application.

For the second method, you will use the Unity Editor, which requires some setup.

## Using the Unity Application

### Required Environment for Using the Unity Application

No specific Unity environment is required.

### Installation Instructions for Using the Unity Application

Clone the repository as follows:

```
git clone --recursive https://github.com/toppers/hakoniwa-unity-drone-model.git
```

Once the cloning is complete, navigate to the directory as follows:

```
cd hakoniwa-unity-drone-model/
```

Download the application suitable for your environment from the following site:

https://github.com/toppers/hakoniwa-unity-drone-model/releases

Unzip the downloaded file under the `hakoniwa-unity-drone-model` directory.

The directory structure will be as follows:

```
hakoniwa-unity-drone-model/DroneApp<OS>
```

### Launching the Unity Application

Run the following command directly under the `hakoniwa-unity-drone-model` directory:

```
bash ./plugin/activate_app.bash DroneApp<OS>
```

For the Windows native app, double-click `DroneWinNative/model.exe`.

If successful, the Unity application will launch.

## Using the Unity Editor

### Required Environment for Using the Unity Editor

- Homebrew (Mac only)
- Unity Hub
- Unity (compatible with your CPU architecture)
  - Version 2022.3.5f1 or later

### Installation Instructions for Using the Unity Editor

Clone the repository as follows:

```
git clone --recursive https://github.com/toppers/hakoniwa-unity-drone-model.git
```

Once the cloning is complete, navigate to the directory as follows:

```
cd hakoniwa-unity-drone-model/
```

Then, install the necessary Unity modules.

For MacOS/Ubuntu and WSL2:

```
bash install.bash 
```

For Windows:

```
bash install.bash win
```

### Launching Unity

Open the project in Unity Hub.

Note: Ensure that the Unity Editor version matches your CPU architecture.

Target folder: `hakoniwa-unity-drone-model\plugin\plugin-srcs`

If a message about "Opening Project in Non-Matching Editor Installation" appears, click "Continue."

In the following dialog, click `Continue`.

![image](https://github.com/toppers/hakoniwa-unity-drone-model/assets/164193/e1fbc477-4edc-4e39-ab15-ccd6f0707f33)

Next, ignore the following dialog.

![image](https://github.com/toppers/hakoniwa-unity-drone-model/assets/164193/7c03ae41-f988-44cb-9ac1-2263507d254d)

If successful, you will see this.

![image](https://github.com/toppers/hakoniwa-unity-drone-model/assets/164193/50398cfa-f6fc-4eef-9679-5442bbd9de76)

Upon initial launch, there may be many errors in the console due to the following reasons. Please follow the links to address them sequentially:

* [Missing Newtonsoft.Json](https://github.com/toppers/hakoniwa-document/tree/main/troubleshooting/unity#unity%E8%B5%B7%E5%8B%95%E6%99%82%E3%81%ABnewtonsoftjson%E3%81%8C%E3%81%AA%E3%81%84%E3%81%A8%E3%81%84%E3%81%86%E3%82%A8%E3%83%A9%E3%83%BC%E3%81%8C%E5%87%BA%E3%82%8B)
* [Error with gRPC library usage](https://github.com/toppers/hakoniwa-document/blob/main/troubleshooting/unity/README.md#grpc-%E3%81%AE%E3%83%A9%E3%82%A4%E3%83%96%E3%83%A9%E3%83%AA%E5%88%A9%E7%94%A8%E7%AE%87%E6%89%80%E3%81%8C%E3%82%A8%E3%83%A9%E3%83%BC%E5%87%BA%E5%8A%9B%E3%81%97%E3%81%A6%E3%81%84%E3%82%8B) (Mac or Linux)

### Preparing the Simulation Environment

Double-click the Unity scene (`Assets/Scenes/ApiDemo`) as shown below.

![image](images/ProjectApiDemoScene.png)

#### Generating Configuration Files

Click `Window` -> `Hakoniwa` -> `Generate`.

![Screenshot](https://github.com/toppers/hakoniwa-unity-picomodel/assets/164193/85ab96b7-fd8b-4547-a4a3-c386d0a35813)

If successful, you will see JSON logs in the console without any error logs.

![Screenshot](https://github.com/toppers/hakoniwa-unity-picomodel/assets/164193/6fa55a56-1693-4728-b0ef-091e10fb4b22)

For Windows, this operation must be repeated after every machine reboot.

#### Configuration Files

The generated configuration files will be located directly under `hakoniwa-unity-drone-model\plugin\plugin-srcs`.

* [core_config.json](https://github.com/toppers/hakoniwa-document/blob/main/architecture/assets/README-unity.md#%E7%AE%B1%E5%BA%AD%E3%82%B3%E3%83%B3%E3%83%95%E3%82%A3%E3%82%B0%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB%E5%85%A5%E5%8A%9Bcore_configjson)
* custom.json

The output of custom.json is as follows:

```json
{
  "robots": [
    {
      "name": "DroneTransporter",
      "rpc_pdu_readers": [],
      "rpc_pdu_writers": [],
      "shm_pdu_readers": [
        {
          "type": "hako_mavlink_msgs/HakoHilActuatorControls",
          "org_name": "drone_motor",
          "name": "DroneTransporter_drone_motor",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduReader",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduReaderConverter",
          "channel_id": 0,
          "pdu_size": 112,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "geometry_msgs/Twist",
          "org_name": "drone_pos",
          "name": "DroneTransporter_drone_pos",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduReader",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduReaderConverter",
          "channel_id": 1,
          "pdu_size":

 72,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/ManualPosAttControl",
          "org_name": "drone_manual_pos_att_control",
          "name": "DroneTransporter_drone_manual_pos_att_control",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 3,
          "pdu_size": 80,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/HakoDroneCmdTakeoff",
          "org_name": "drone_cmd_takeoff",
          "name": "DroneTransporter_drone_cmd_takeoff",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 5,
          "pdu_size": 64,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/HakoDroneCmdMove",
          "org_name": "drone_cmd_move",
          "name": "DroneTransporter_drone_cmd_move",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 6,
          "pdu_size": 80,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/HakoDroneCmdLand",
          "org_name": "drone_cmd_land",
          "name": "DroneTransporter_drone_cmd_land",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 7,
          "pdu_size": 64,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/GameControllerOperation",
          "org_name": "hako_cmd_game",
          "name": "DroneTransporter_hako_cmd_game",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 8,
          "pdu_size": 136,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/HakoCmdCamera",
          "org_name": "hako_cmd_camera",
          "name": "DroneTransporter_hako_cmd_camera",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 9,
          "pdu_size": 44,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/HakoCmdCameraMove",
          "org_name": "hako_cmd_camera_move",
          "name": "DroneTransporter_hako_cmd_camera_move",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 11,
          "pdu_size": 64,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/HakoCmdMagnetHolder",
          "org_name": "hako_cmd_magnet_holder",
          "name": "DroneTransporter_hako_cmd_magnet_holder",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 13,
          "pdu_size": 40,
          "write_cycle": 1,
          "method_type": "SHM"
        }
      ],
      "shm_pdu_writers": [
        {
          "type": "hako_msgs/Collision",
          "org_name": "drone_collision",
          "name": "DroneTransporter_drone_collision",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 2,
          "pdu_size": 304,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/Disturbance",
          "org_name": "drone_disturbance",
          "name": "DroneTransporter_drone_disturbance",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 4,
          "pdu_size": 32,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/HakoCameraData",
          "org_name": "hako_camera_data",
          "name": "DroneTransporter_hako_camera_data",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 10,
          "pdu_size": 102968,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/HakoCameraInfo",
          "org_name": "hako_cmd_camera_info",
          "name": "DroneTransporter_hako_cmd_camera_info",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 12,
          "pdu_size": 56,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "hako_msgs/HakoStatusMagnetHolder",
          "org_name": "hako_status_magnet_holder",
          "name": "DroneTransporter_hako_status_magnet_holder",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 14,
          "pdu_size": 32,
          "write_cycle": 1,
          "method_type": "SHM"
        },
        {
          "type": "sensor_msgs/PointCloud2",
          "org_name": "lidar_points",
          "name": "DroneTransporter_lidar_points",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 15,
          "pdu_size": 177400,
          "write_cycle": 5,
          "method_type": "SHM"
        },
        {
          "type": "geometry_msgs/Twist",
          "org_name": "lidar_pos",
          "name": "DroneTransporter_lidar_pos",
          "class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriter",
          "conv_class_name": "Hakoniwa.PluggableAsset.Communication.Pdu.Raw.RawPduWriterConverter",
          "channel_id": 16,
          "pdu_size": 72,
          "write_cycle": 5,
          "method_type": "SHM"
        }
      ]
    }
  ]
}
```

# Community and Support

Questions and discussions about this project are conducted on the [Hakoniwa Community Forum](https://github.com/toppers/hakoniwa/discussions). Here, you can resolve doubts about the project, share ideas, and provide feedback. The latest information and updates about the project are also shared here.

If you have questions, suggestions, or want to discuss issues faced by other users, feel free to post [here](https://github.com/toppers/hakoniwa/discussions).

# Repository Contents and License

Regarding the content

 of this repository, if the license is specified in each file, follow that license. For content that is not explicitly mentioned, it is published under the [TOPPERS License](https://www.toppers.jp/license.html).

The TOPPERS License is an open-source license for projects, outlining conditions for software use, modification, and distribution. For details, refer to the link above.

# Contribution Guidelines

Thank you for your interest in contributing to this project. Various forms of contributions are welcome. Below are guidelines on how to contribute to the project.

## Reporting Issues

- Report bugs and propose new features through GitHub Issues.
- Before creating an issue, check if a similar issue already exists.
- Provide as much information as possible when creating an issue. This includes reproduction steps, expected behavior, actual behavior, and the environment used.

## Pull Requests

- Contribute code changes such as feature additions and bug fixes through pull requests.
- For major changes or new features, it is recommended to discuss them in a related issue beforehand.
- Ensure consistent coding style and conventions by following the existing code style.

## Communication

- Discuss and ask questions about the project in [Discussions](https://github.com/toppers/hakoniwa/discussions).
- Communicate respectfully with other contributors.

## Other Contributions

- Improvements to documentation and translations, among other non-code contributions, are also welcome.
