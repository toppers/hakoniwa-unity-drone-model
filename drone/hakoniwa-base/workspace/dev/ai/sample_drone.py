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
  #100secs
  while not done and total_time < 4000:
    
    # input command
    if total_time % 100 == 0:
      f = open('dev/ai/cmd.txt', 'r')
      value = f.readlines()
      f.close()
    
    sensors = env.hako.execute()

    #laser scan
    scan = robo.get_state("scan", sensors)
    scan_ranges = scan['ranges']
    scan_min = min(min(scan_ranges[0:15]), min(scan_ranges[345:359]))
    #print("scan=" + str(scan_min))

    #camera sensor
    if total_time % 100 == 0:
      img = robo.get_state("camera_image_jpg", sensors)
      file_data = img['data__raw']
      #file_data = struct.pack('B' * len(image_data), *image_data)
      with open("camera-01.jpg" , 'bw') as f:
          f.write(file_data)

    #motor control
    motor = robo.get_action('hobber_control')
    motor['linear']['x'] = float(value[0])
    motor['linear']['y'] = 0.0
    motor['linear']['z'] = 0.0
    motor['angular']['x'] = 0.0
    motor['angular']['y'] = 0.0
    motor['angular']['z'] = 0.0
    #if (scan_min >= 0.2):
      #motor['linear']['x'] = float(value[0])
      #motor['angular']['x'] = 0.0
    #else:
      #motor['linear']['x'] = 0.0
      #motor['angular']['x'] = -2.0
    
    for channel_id in robo.actions:
      robo.hako.write_pdu(channel_id, robo.actions[channel_id])
    
    total_time = total_time + 1

  env.reset()


print("END")
env.reset()
sys.exit(0)

