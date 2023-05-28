#!/usr/bin/python
# -*- coding: utf-8 -*-
import json
import sys
from hako_binary import offset_map
from hako_binary import binary_writer
from hako_binary import binary_reader
import qtable_model2
import hako_env
import hako
import time
import signal
from types import MethodType
import hako_robomodel_any
import struct

import drone_sensor
from drone_angle_control import PIDController

# パラメータ設定
kp = 0.01  # 比例ゲイン
ki = 0.001  # 積分ゲイン
kd = 2.0  # 微分ゲイン
dt = 0.02 # 単位時間[20msec]
controller = PIDController(kp, ki, kd) 

def handler(signum, frame):
  print(f'SIGNAL(signum={signum})')
  sys.exit(0)
  
print("START DRONE TEST")

# signal.SIGALRMのシグナルハンドラを登録
signal.signal(signal.SIGINT, handler)

#create hakoniwa env
env = hako_env.make("Drone", "any", "dev/ai/custom.json")
print("WAIT START:")
env.hako.wait_event(hako.HakoEvent['START'])
print("WAIT RUNNING:")
env.hako.wait_state(hako.HakoState['RUNNING'])

print("GO:")

#do simulation
def delta_usec():
  return 20000

robo = env.robo()
robo.delta_usec = delta_usec

for episode in range(1):
  total_time = 0
  done = False
  state = 0
  total_reward = 0

  while not done and total_time < 4000:
    
    value=0.0
    # input command
    if total_time % 10 == 0:
      f = open('dev/ai/cmd.txt', 'r')
      value = float(f.readlines()[0])
      f.close()
    
    sensors = env.hako.execute()

    #imu
    imu = robo.get_state("imu", sensors)
    imu_orientation = imu['orientation']
    current_angles = drone_sensor.quaternion_to_euler_xyz(imu_orientation)
    imu_angular_velocity = imu['angular_velocity']
    print("or=" + str(current_angles))
    print("vl=" + str(imu_angular_velocity))
    target_angles = [ value, 0.0, 0.0 ]
    print("tr=" + str(target_angles))

    commanded_control_signal = controller.control(target_angles, current_angles, dt)
    print("cr=" + str(commanded_control_signal))

    #motor control
    motor = robo.get_action('hobber_control')
    motor['linear']['x'] =  0.000 * commanded_control_signal[0]
    motor['linear']['y'] =  0.0
    motor['linear']['z'] = -0.010 * commanded_control_signal[1]
    motor['angular']['x'] = 0.0
    motor['angular']['y'] = 0.0
    motor['angular']['z'] = 0.0
    
    for channel_id in robo.actions:
      robo.hako.write_pdu(channel_id, robo.actions[channel_id])
    
    total_time = total_time + 1

  env.reset()


print("END")
env.reset()
sys.exit(0)

